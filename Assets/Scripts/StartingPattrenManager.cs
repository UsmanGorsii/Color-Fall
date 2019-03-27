using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPattrenManager : MonoBehaviour {

	public Vector2[] lastPlaformPositions;
	public PlatformGenerator platformGenerator;
	void Start () {
        int selectedIndex = Random.Range(0, 4);
		transform.GetChild(selectedIndex).gameObject.SetActive(true);
		platformGenerator.LNP = platformGenerator.LPP = lastPlaformPositions[selectedIndex];
		Invoke("StartGame", .1f);
	}

	void StartGame()
	{
		platformGenerator.startGame = true;
	}
}