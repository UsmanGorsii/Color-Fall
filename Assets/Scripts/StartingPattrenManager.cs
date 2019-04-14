using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPattrenManager : MonoBehaviour
{
    public PlatformGenerator platformGenerator;
	public GameObject[] gameObjects;

    void Start()
    {
		StartGame();
    }

	public void SetPlatformColor(CustomColor customColor)
	{
		for(int i = 0 ; i < gameObjects.Length; i++)
		{
			for(int j = 0 ; j < gameObjects[i].transform.childCount; j++)
			{
				gameObjects[i].transform.GetChild(j).tag = customColor.name;
				gameObjects[i].transform.GetChild(j).GetComponent<SpriteRenderer>().color = customColor.color;
			}
		}	
	}

    void StartGame()
    {
        platformGenerator.startGame = true;
    }
}