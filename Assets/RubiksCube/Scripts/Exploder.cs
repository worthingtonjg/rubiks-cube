using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : MonoBehaviour {
	GameObject[] cubies;
	GameObject[] mirrors;
	List<Vector3> originalTransforms;
	List<Quaternion> originalRotation;
	GameObject core;
	bool reassembling;

	[SerializeField] float power = 100f;
	[SerializeField] float returnSpeed = 50f;
	[SerializeField] float rotationalSpeed = 100f;
	[SerializeField] float exlposionLength = .8f;

	// Use this for initialization
	void Start () {
		mirrors = GameObject.FindGameObjectsWithTag("mirrors");
		cubies = GameObject.FindGameObjectsWithTag("cubie");
		core = GameObject.Find("core");
		
		SaveOriginals();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(reassembling)
		{
			bool complete = true;
			for(int i = 0; i < cubies.Length; i++)
			{
				cubies[i].transform.position = Vector3.MoveTowards(cubies[i].transform.position, originalTransforms[i], Time.deltaTime * returnSpeed);
				cubies[i].transform.rotation = Quaternion.RotateTowards(cubies[i].transform.rotation, Quaternion.identity, Time.deltaTime * rotationalSpeed);

				var distanceRemaining = Vector3.Distance(cubies[i].transform.position, originalTransforms[i]); 
				float angleRemaining = Quaternion.Angle(cubies[i].transform.rotation, Quaternion.identity);
				if(distanceRemaining > .0001f || angleRemaining > .0001f)
				{
					complete = false;
				}
			}

			reassembling = !complete;

			if(!reassembling)
			{
				core.SetActive(true);
				foreach(var mirror in mirrors)
				{
					mirror.SetActive(true);
				}
			}
		}
	}

	public void Explode()
	{
		if(reassembling) return;

		foreach(var cubie in cubies)
		{
			var rb = cubie.AddComponent<Rigidbody>();
			rb.useGravity = false;
			rb.AddForceAtPosition(rb.transform.position * power, rb.transform.position, ForceMode.Impulse);
		}

		foreach(var mirror in mirrors)
		{
			mirror.SetActive(false);
		}

		core.SetActive(false);

		StartCoroutine(Reassemble());
	}

	void SaveOriginals()
	{
		originalTransforms = new List<Vector3>();
		foreach(var cubie in cubies)
		{
			originalTransforms.Add(new Vector3(cubie.transform.position.x, cubie.transform.position.y, cubie.transform.position.z));
		}
	}

	IEnumerator Reassemble()
	{
		yield return new WaitForSeconds(exlposionLength);

		foreach(var cubie in cubies)
		{
			var rb = cubie.GetComponent<Rigidbody>();
			Destroy(rb);
		}

		reassembling = true;
	}
}
