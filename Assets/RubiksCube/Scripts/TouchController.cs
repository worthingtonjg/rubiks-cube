using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TouchController : MonoBehaviour 
{
	private List<GameObject> touchList;

	void Start()
	{
		touchList = new List<GameObject>();

	}

	void Update () {
		if(Input.GetMouseButton(0))
		{
			DetectTouch();
		}

		if(Input.GetMouseButtonUp(0))
		{
			ProcessTouch();
		}
	}

	private void DetectTouch()
	{
		RaycastHit hit = new RaycastHit();

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		var facesLayer = 1 << 10;
		if (Physics.Raycast(ray, out hit, 1000f, facesLayer)) 
		{
			var face = hit.transform.gameObject;
			
			if(touchList.FirstOrDefault(t => t.GetInstanceID() == face.GetInstanceID()) == null)
			{
				touchList.Add(face);
			}
		}
	}

	private void ProcessTouch()
	{
		// did not touch enough faces
		if(touchList.Count <= 1) 
		{
			touchList.Clear();
			print("did not touch enough faces");
			return;
		}

		var axisTouched = touchList[0].name.Substring(4,1);

		List<GameObject> facesOnSameAxis = touchList.Where(f => f.name.Contains(axisTouched)).ToList();

		// Did not touch enough faces on same axis
		if(facesOnSameAxis.Count == 1) 
		{
			touchList.Clear();
			print("Did not touch enough faces on same axis");
			return;
		}

		var firstFace = facesOnSameAxis.FirstOrDefault();
		var firstFaceRow = int.Parse(firstFace.name.Substring(5,1));
		var firstFaceCol = int.Parse(firstFace.name.Substring(6,1));

		var lastFace = facesOnSameAxis.LastOrDefault();
		var lastFaceRow = int.Parse(lastFace.name.Substring(5,1));
		var lastFaceCol = int.Parse(lastFace.name.Substring(6,1));

		// Can't determine slice touched
		if(firstFaceCol != lastFaceCol && firstFaceRow != lastFaceRow) 
		{
			touchList.Clear();
			print("Cannot determine slice touched");
			return;
		}

		EnumAxis axis = EnumAxis.z; // remove default;
		int slice = 0;
		EnumDirection direction = EnumDirection.positive;
		EnumAnimType animType = EnumAnimType.slice;

		switch(axisTouched)
		{
			case "X" :
				if(firstFaceRow == lastFaceRow)
				{
					axis = EnumAxis.y;
					slice = firstFaceRow;
					direction = firstFaceCol > lastFaceCol ? EnumDirection.negative : EnumDirection.positive;
					animType = firstFaceRow == 1 ? EnumAnimType.cube : EnumAnimType.slice;
				}
				else
				{
					axis = EnumAxis.z;
					slice = firstFaceCol;
					direction = firstFaceRow > lastFaceRow ? EnumDirection.negative : EnumDirection.positive;
					animType = firstFaceCol == 1 ? EnumAnimType.cube : EnumAnimType.slice;
				}
				break;
			case "Y" :
				if(firstFaceRow == lastFaceRow)
				{
					axis = EnumAxis.x;
					slice = firstFaceRow;
					direction = firstFaceCol > lastFaceCol ? EnumDirection.negative : EnumDirection.positive;
					animType = firstFaceRow == 1 ? EnumAnimType.cube : EnumAnimType.slice;
				}
				else
				{
					axis = EnumAxis.z;
					slice = firstFaceCol;
					direction = firstFaceRow > lastFaceRow ? EnumDirection.positive : EnumDirection.negative;
					animType = firstFaceCol == 1 ? EnumAnimType.cube : EnumAnimType.slice;
				}
				break;
			case "Z" :
				if(firstFaceRow == lastFaceRow)
				{
					axis = EnumAxis.y;
					slice = firstFaceRow;
					direction = firstFaceCol > lastFaceCol ? EnumDirection.positive : EnumDirection.negative;
					animType = firstFaceRow == 1 ? EnumAnimType.cube : EnumAnimType.slice;
				}
				else
				{
					axis = EnumAxis.x;
					slice = firstFaceCol;
					direction = firstFaceRow > lastFaceRow ? EnumDirection.positive : EnumDirection.negative;
					animType = firstFaceCol == 1 ? EnumAnimType.cube : EnumAnimType.slice;
				}
				break;
		}

		touchList.Clear();
		print("rotating");
		CubeController.Instance.StartRotation(axis, animType, direction, slice);
	}
}
