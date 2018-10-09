using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WinController : MonoBehaviour 
{
	private GameObject[] cubies;
	
	void Start () 
	{
		CubeController.Instance.RotationComplete += OnRotationComplete;
	}

    private void OnRotationComplete()
    {
		if(CubeController.Instance.gameState == EnumGameState.playing)
		{
        	CheckForWin();
		}
    }

	private void CheckForWin()
    {
        List<Cubie> cubies = CubeController.Instance.GetCubies();

        Dictionary<string, string> colorToDirection = GetColorDirectionDictionary(cubies);

		// Check all the cubies and make sure they are in position
        bool win = true;
        foreach (var cubie in cubies)
        {
            if (cubie.CubieType == EnumCubieType.core) continue;

            if (!cubie.InPosition(colorToDirection))
            {
                win = false;
            }
        }

        if (win)
        {
            print("cube solved");
        }
    }

    private static Dictionary<string, string> GetColorDirectionDictionary(List<Cubie> cubies)
    {
        var centers = cubies.Where(c => c.CubieType == EnumCubieType.center).ToList();

        var colorToDirection = new Dictionary<string, string>();
        foreach (var center in centers)
        {
            string centerColor = center.GetColors().FirstOrDefault();
            string centerDirection = center.GetCenterDirection();
            colorToDirection[centerColor] = centerDirection;
        }

        return colorToDirection;
    }

}
