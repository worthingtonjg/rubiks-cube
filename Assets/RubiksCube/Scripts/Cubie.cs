using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cubie : MonoBehaviour 
{
	[SerializeField] public EnumCubieType CubieType;
	
	private float rayLength = 2f;
	private Renderer r;
	private bool whiteFace;
	private bool yellowFace;
	private bool blueFace;
	private bool greenFace;
	private bool redFace;
	private bool orangeFace;

	private const string white = "white";
	private const string yellow = "yellow";
	private const string blue = "blue";
	private const string green = "green";
	private const string red = "red";
	private const string orange = "orange";

	void Start () {
		r = GetComponent<Renderer>();

		whiteFace = r.materials.Any(m =>  m.name.Contains(white));
		yellowFace = r.materials.Any(m =>  m.name.Contains(yellow));
		blueFace = r.materials.Any(m =>  m.name.Contains(blue));
		greenFace = r.materials.Any(m =>  m.name.Contains(green));
		redFace = r.materials.Any(m =>  m.name.Contains(red));
		orangeFace = r.materials.Any(m =>  m.name.Contains(orange));
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	public List<string> GetColors()
	{
		return r.materials
			.Where(m => !m.name.Contains("none"))
			.Where(m => !m.name.Contains("border"))
			.Select(m => m.name.Replace(" (Instance)","")).ToList();
	}

	public string GetCenterDirection()
	{
		if(CubieType != EnumCubieType.center)
		{
			throw new UnityException("This can only be called for center cubies");
		}

		string side = null;

		// Raycasts from center pieces should only get one side collider
		side = GetSide(Vector3.up) ?? side;
		side = GetSide(Vector3.down) ?? side;
		side = GetSide(Vector3.left) ?? side;
		side = GetSide(Vector3.right) ?? side;
		side = GetSide(Vector3.forward) ?? side;
		side = GetSide(Vector3.back) ?? side;

		return side;
	}

	public bool InPosition(Dictionary<string, string> colorToDirection)
    {
        bool orangeFaceCorrect = IsFaceCorrect(orangeFace, Vector3.forward, colorToDirection[orange]);
		bool redFaceCorrect = IsFaceCorrect(redFace, Vector3.back, colorToDirection[red]);
		bool whiteFaceCorrect = IsFaceCorrect(whiteFace, Vector3.up, colorToDirection[white]);
		bool yellowFaceCorrect = IsFaceCorrect(yellowFace, Vector3.down, colorToDirection[yellow]);
		bool blueFaceCorrect = IsFaceCorrect(blueFace, Vector3.left, colorToDirection[blue]);
		bool greenFaceCorrect = IsFaceCorrect(greenFace, Vector3.right, colorToDirection[green]);

		return orangeFaceCorrect && redFaceCorrect && whiteFaceCorrect && yellowFaceCorrect && blueFaceCorrect && greenFaceCorrect;
    }

    private bool IsFaceCorrect(bool checkFace, Vector3 direction, string directionName)
    {
		bool result = true;

        if (checkFace)
        {
            string colliderName = GetSide(direction);
			result = colliderName == directionName;
        }

        return result;
    }

	private string GetSide(Vector3 direction)
	{
		RaycastHit hit;
		Vector3 dir = transform.TransformDirection(direction) * rayLength;
		Debug.DrawRay(transform.position, dir, Color.green);
		
		var sidesLayer = 1 << 9;
		if(Physics.Raycast(new Ray(transform.position, dir), out hit, rayLength, sidesLayer))
		{
			return hit.collider.name;
		}

		return null;
	}
}
