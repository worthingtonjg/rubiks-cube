using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeShuffler : MonoBehaviour 
{
	private int timesToRotate;
	private int timesRotated;
	private int? lastSlice = null;

	// Use this for initialization
	void Start () 
	{
		CubeController.Instance.RotationComplete += OnRotationComplete;
	}

	public void StartShuffle()
	{
		if(CubeController.Instance.gameState == EnumGameState.shuffling) return;

		timesRotated = 1;
		timesToRotate = UnityEngine.Random.Range(25, 50);

		CubeController.Instance.gameState = EnumGameState.shuffling;
        
		Shuffle();
	}

    private void OnRotationComplete()
    {
		if(CubeController.Instance.gameState != EnumGameState.shuffling) return;

		++timesRotated;

		if(timesRotated <= timesToRotate)
		{
			Shuffle();
		}
		else
		{
			CubeController.Instance.gameState = EnumGameState.playing;
			CubeController.Instance.ResetHistory();
		}
    }

	private void Shuffle()
	{
		EnumAxis axis = (EnumAxis)UnityEngine.Random.Range(0, 3);

		int slice = UnityEngine.Random.Range(0, 3);
		while(slice == lastSlice)
		{
			slice = UnityEngine.Random.Range(0, 3);
		}

		lastSlice = slice;

		EnumDirection direction = (EnumDirection)UnityEngine.Random.Range(0, 2);

		CubeController.Instance.StartRotation(axis, EnumAnimType.slice, direction, slice, 1200f);
	}
}
