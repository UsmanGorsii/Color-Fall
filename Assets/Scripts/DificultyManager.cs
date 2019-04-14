using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DificultyManager : MonoBehaviour {

	public static DificultyManager Instance;

	public PlayerController playerController;
	public PlatformGenerator platformGenerator;
	public ScoreManager scoreManager;
	[System.Serializable]
	public class DificultyLevel
	{
		
	}

	[System.Serializable]
	public class DifficultySettings
	{
		[Header("Player Difficulty Increments")]
		public float movementSpeedIncrement;
		
		[Header("Plaform Difficulty Increments")]
		public int tileDistanceIncrement;
		public int colorChangerMaxDistanceIncrement;
		public int colorChangerMinDistanceIncrement;

		public int maxPlatformSizeDecrement;
		public float minPlatformSizeDecrement;
		public float minPlatformSizeF;

		public int[] scoreToIncreaseDifficulty;
		public int difficultyLevel = 0;
	}
	public DifficultySettings difficultySettings;
	// Use this for initialization
	void Start () {
		Instance = this;
		difficultySettings.minPlatformSizeF = platformGenerator.platformSettings.platformMinSize;
	}
	
	void Update () {

		if(difficultySettings.difficultyLevel < difficultySettings.scoreToIncreaseDifficulty.Length && scoreManager.scoreCounts >= difficultySettings.scoreToIncreaseDifficulty[difficultySettings.difficultyLevel])
		{
			difficultySettings.difficultyLevel++;
			print("Difficulty Level: " + difficultySettings.difficultyLevel);

			playerController.movementSettings.moveSpeed += difficultySettings.movementSpeedIncrement;
			platformGenerator.platformSettings.maxDistance += difficultySettings.tileDistanceIncrement;
			platformGenerator.platformSettings.minPlaforms += difficultySettings.colorChangerMinDistanceIncrement;
			platformGenerator.platformSettings.maxPlaforms += difficultySettings.colorChangerMaxDistanceIncrement;
			platformGenerator.platformSettings.platformMaxSize -= difficultySettings.maxPlatformSizeDecrement;
			difficultySettings.minPlatformSizeF -= difficultySettings.minPlatformSizeDecrement;
			platformGenerator.platformSettings.platformMinSize = (int)difficultySettings.minPlatformSizeF;

		}
	}
}
