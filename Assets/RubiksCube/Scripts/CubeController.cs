using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CubeController : MonoBehaviour 
{
	[SerializeField] EnumCubeAnimState state;
	[SerializeField] EnumAnimType animationType;
	[SerializeField] float rotationSpeed = 300f;
	[SerializeField] EnumAxis rotationAxis;
	[SerializeField] EnumDirection rotationDirection;
	[SerializeField] int slice;

	private List<Slice> xSlices;
	private List<Slice> ySlices;
	private List<Slice> zSlices;

	private Quaternion startingRotation;
	private Quaternion targetRotation;
	private Transform target;
	private GameObject[] cubies;

	private static CubeController _instance;

	public EnumGameState gameState = EnumGameState.playing;

    public static CubeController Instance { get { return _instance; } }

	public Action RotationComplete;

	private Stack<Move> History;

    void Awake()
    {
		

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

	// Use this for initialization
	void Start () 
	{
		History = new Stack<Move>();
		cubies = GameObject.FindGameObjectsWithTag("cubie");

		 state = EnumCubeAnimState.idle;
		 xSlices = new List<Slice> { new Slice(), new Slice(), new Slice() };
		 ySlices = new List<Slice> { new Slice(), new Slice(), new Slice() };
		 zSlices = new List<Slice> { new Slice(), new Slice(), new Slice() };
	}

	// Update is called once per frame
	void Update () 
	{
		switch(state)
		{
			case EnumCubeAnimState.animating:
				AnimateRotate();
				break;
			case EnumCubeAnimState.idle:
				break;
		}
	}

	public void StartRotation(EnumAxis axis, EnumAnimType animType, EnumDirection direction, int nslice = 0, float speed = 300f)
	{
		rotationAxis = axis;
		animationType = animType;
		rotationDirection = direction;
		slice = nslice;
		rotationSpeed = speed;

		if(gameState == EnumGameState.playing)
		{
			History.Push(new Move {
				Axis = axis,
				Type = animationType,
				Direction = rotationDirection,
				Slice = slice
			});
		}

		DoRotate();
	}

	public List<Cubie> GetCubies()
	{
		var result = new List<Cubie>();
		
		foreach(var cubie in cubies)
		{
			var script = cubie.GetComponent<Cubie>();
			result.Add(script);
		}

		return result;
	}

	public void Undo()
	{
		if(state == EnumCubeAnimState.animating) return;
		if(History.Count == 0) return;

		var move = History.Pop();

		rotationAxis = move.Axis;
		animationType = move.Type;
		rotationDirection = move.Direction == EnumDirection.positive ? EnumDirection.negative : EnumDirection.positive;
		slice = move.Slice.GetValueOrDefault();
		rotationSpeed = 300f;

		DoRotate();
	}

	public void ResetHistory()
	{
		History.Clear();
	}

	private void DoRotate()
    {
        if (state != EnumCubeAnimState.idle) return;

        state = EnumCubeAnimState.animating;
        int direction = rotationDirection == EnumDirection.positive ? 1 : -1;

		RebuildSlices();

        switch (animationType)
        {
            case EnumAnimType.cube:
				SetupCubeAnimation(direction);
                break;
            case EnumAnimType.slice:
				SetupSliceAnimation(direction);
                break;
        }
    }

    private void SetupCubeAnimation(int direction)
    {
        switch (rotationAxis)
        {
            case EnumAxis.x:
                startingRotation = transform.rotation;
                targetRotation = Quaternion.AngleAxis(90 * direction, Vector3.right) * transform.rotation;
                break;
            case EnumAxis.y:
                startingRotation = transform.rotation;
                targetRotation = Quaternion.AngleAxis(90 * direction, Vector3.up) * transform.rotation;
                break;
            case EnumAxis.z:
                startingRotation = transform.rotation;
                targetRotation = Quaternion.AngleAxis(90 * direction, Vector3.forward) * transform.rotation;
                break;
        }

		target = transform;
    }

	private void SetupSliceAnimation(int direction)
	{
		List<Slice> targetSlices = null;
		if(rotationAxis == EnumAxis.x)
		{
			targetSlices = xSlices;
		}
		else if(rotationAxis == EnumAxis.y)
		{
			targetSlices = ySlices;
		}
		else if(rotationAxis == EnumAxis.z)
		{
			targetSlices = zSlices;
		}

		var cubies = targetSlices[slice].Cubies;
		GameObject center = null;

		if(slice == 1) 
		{
			center = cubies.FirstOrDefault(c => c.name == "core");
		}
		else
		{
			center = cubies.FirstOrDefault(c => c.name == "center");
		}

		cubies.Remove(center);

		foreach(var cubie in cubies)
		{
			cubie.transform.parent = center.transform;
		}

		switch (rotationAxis)
        {
            case EnumAxis.x:
                startingRotation = center.transform.rotation;
                targetRotation = Quaternion.AngleAxis(90 * direction, Vector3.right) * center.transform.rotation;
                break;
            case EnumAxis.y:
                startingRotation = center.transform.rotation;
                targetRotation = Quaternion.AngleAxis(90 * direction, Vector3.up) * center.transform.rotation;
                break;
            case EnumAxis.z:
                startingRotation = center.transform.rotation;
                targetRotation = Quaternion.AngleAxis(90 * direction, Vector3.forward) * center.transform.rotation;
                break;
        }
		
		target = center.transform;
	}

    private void AnimateRotate()
    {
        target.rotation = Quaternion.RotateTowards(target.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (target.rotation == targetRotation)
        {
            state = EnumCubeAnimState.idle;

			if(RotationComplete != null)
			{
				RotationComplete.Invoke();
			}

			
        }
    }

    private void RebuildSlices()
	{
		foreach(var cubie in cubies)
		{
			if(animationType == EnumAnimType.cube)
			{
				cubie.transform.parent = transform;
			}
			else
			{
				cubie.transform.parent = transform;
			}

		}

		ySlices[0].Cubies = cubies.Where(c => Mathf.Round(c.transform.position.y) == 2).ToList();
		ySlices[1].Cubies = cubies.Where(c => Mathf.Round(c.transform.position.y) == 0).ToList();
		ySlices[2].Cubies = cubies.Where(c => Mathf.Round(c.transform.position.y) == -2).ToList();

		xSlices[0].Cubies = cubies.Where(c => Mathf.Round(c.transform.position.x) == -2).ToList();
		xSlices[1].Cubies = cubies.Where(c => Mathf.Round(c.transform.position.x) == 0).ToList();
		xSlices[2].Cubies = cubies.Where(c => Mathf.Round(c.transform.position.x) == 2).ToList();

		zSlices[0].Cubies = cubies.Where(c => Mathf.Round(c.transform.position.z) == -2).ToList();
		zSlices[1].Cubies = cubies.Where(c => Mathf.Round(c.transform.position.z) == 0).ToList();
		zSlices[2].Cubies = cubies.Where(c => Mathf.Round(c.transform.position.z) == 2).ToList();
	}
}
