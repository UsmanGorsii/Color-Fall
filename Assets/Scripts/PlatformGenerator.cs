using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public static PlatformGenerator Instance;
    [System.Serializable]
    public class TilesSettings
    {
        public CustomColor[] unitTileTypes;
    }
    public TilesSettings tilesSettings;

    [System.Serializable]
    public class PlatformSettings
    {
        public GameObject platform;
        public Transform platformGenerationPoint;
        public float platformScale;
        public Transform platformBucket;

        [Tooltip("Index of selected pooled platform")]
        public int platformIndex;
        public int pooledPlatformsCount;

        [Header("Size Settings")]
        [Tooltip("Maximum number of unit tiles used in one platform")]
        [Range(5, 10)] public int platformMaxSize;
        [Tooltip("Minimum number of unit tiles used in one platform")]
        [Range(1, 4)] public int platformMinSize;

        [Header("Height Settings")]
        public int minHeight;
        public int maxHeight;
        public int maxHeightChange;

        [Header("Distance Settings")]
        public float minDistance;
        public float maxDistance;
        public float maxDistanceChange;

        [Header("")]
        [Tooltip("Number of Player Color Platform(s) Generated")]
        public int NPCPG;
        [Tooltip("Minimum plaforms to generate color changer pickup")]
        public int minPlaforms;
        [Tooltip("Maximum plaforms to generate color changer pickup")]
        public int maxPlaforms;
        [Tooltip("Generate color changer after")]
        public int GCCA;
        public GameObject colorChanger;

        public int GetPlatformSize()
        {
            return Random.Range(platformMinSize, platformMaxSize);
        }

        public int GetPlatformHeight(int prevHeight)
        {
            return Random.Range
            (
                prevHeight - maxHeightChange < minHeight ? minHeight : prevHeight - maxHeightChange,
                prevHeight + maxHeightChange > maxHeight ? maxHeight : prevHeight + maxHeightChange
            );
        }

        public void PoolPlatforms()
        {
            for (int i = 0; i < pooledPlatformsCount; i++)
            {
                Instantiate(platform, Vector3.zero, Quaternion.identity).transform.parent = platformBucket;
            }
        }
    }
    [Space]
    public PlatformSettings platformSettings;
    
    [Space]
    public bool testGeneratePlatform;

    #region Variables
    ///<summary>Last Player Platform X (of player's color)</summary>
    public Vector2 LPP;
    ///<summary>Last Normal Platform X,Y (of other colors)</summary>
    public Vector2 LNP;
    ///<summary>Generation point of new platform in X axis</summary>
    float GP = 0;
    ///<summary>Generate Required Plaform (between the span when GP has passed the limit to create atleast one Player color platform)</summary>
    bool GRP;
    ///<summary>Create player color platform</summary>
    bool CPCP;
    float distance;
    int platformSize = 0;
    public bool startGame;
    public PlayerController playerController;
    int selectedColorIndex;
    #endregion
	CustomColor prevColor;
    #region Debuging variables
    public GameObject LPP_G, LNP_G;
    public float gameSpeed = 1;
    public bool changeGameSpeed;
    #endregion
    void Start()
    {
        Instance = this;
		prevColor = new CustomColor ("White", Color.white);
        platformSettings.PoolPlatforms();
    }

    void Update()
    {
        if (testGeneratePlatform)
        {
            testGeneratePlatform = false;
            GenerateTile(new CustomColor("White", Color.white), platformSettings.GetPlatformSize());
        }
        // LPP_G.transform.position = new Vector3(LPP.x, LPP_G.transform.position.y, 1);
        // LNP_G.transform.position = new Vector3(LNP.x, LNP.y, 1);
        

        if (changeGameSpeed)
        {
            changeGameSpeed = false;
            Time.timeScale = gameSpeed;
        }

        if (!startGame)
            return;

        GP = platformSettings.platformGenerationPoint.position.x;
        // Debug.Log(GP);

        distance = -LPP.x +GP;

        // If GP is in between max and min distance from LPP
        if (platformSettings.maxDistance > distance && distance > platformSettings.minDistance)
        {
            GRP = true;
            // Generating random size plaform in given limits
            platformSize = Random.Range(platformSettings.platformMinSize, platformSettings.platformMaxSize);

            // print("Distance(LPP, GP): " + distance);
            // print("Distance(GP, LNP): " + Distance(GP, LNP.x));
            // print("platformSize: " + platformSize);            
            // print(platformSize * platformSettings.platformScale + " < " + (platformSettings.maxDistance - GP + LPP.x));

            if (platformSize * platformSettings.platformScale <= platformSettings.maxDistance - (GP - LPP.x))
            {
                if (GP - LNP.x >= platformSettings.minDistance)
                {
                    selectedColorIndex = Random.Range(0, tilesSettings.unitTileTypes.Length - 1);

                    if (tilesSettings.unitTileTypes[selectedColorIndex].name.Equals(playerController.playerColor.name))
                    {
                        // print("Player color platform generated.");
                        CPCP = false;
                        Vector3 plafotmPosition = GenerateTile
                        (
                            playerController.playerColor,
                            platformSize
                        ).position;
                        LNP.x = LPP.x = plafotmPosition.x + platformSize * platformSettings.platformScale;
                        LNP.y = LPP.y = plafotmPosition.y;
                        platformSettings.NPCPG++;
                    }
                    else
                    {
                        CPCP = true;
                        Vector3 plafotmPosition = GenerateTile
                        (
                            tilesSettings.unitTileTypes[selectedColorIndex],
                            platformSize
                        ).position;
                        LNP.x = plafotmPosition.x + platformSize * platformSettings.platformScale;
                        LNP.y = plafotmPosition.y;
                    }
                }
            }
            //  print("CPCP: " + CPCP);
        }
        else if (GRP)
        {
            GRP = false;
            if (true)
            {
                platformSize = Random.Range(platformSettings.platformMinSize, platformSettings.platformMaxSize);

                Vector3 plafotmPosition = GenerateTile
                (
                    playerController.playerColor,
                    platformSize,
                    platformSettings.minDistance - platformSettings.platformGenerationPoint.position.x + LNP.x
                ).position;

                LNP.x = LPP.x = plafotmPosition.x + platformSize * platformSettings.platformScale;
                LNP.y = LPP.y = plafotmPosition.y;
                platformSettings.NPCPG++;
                // print("LPP: " + LPP);
            }
            else
            {
                platformSize = platformSettings.GetPlatformSize();
                int selectedColorIndex;
                // Try to select color until it is different from player color
                do
                {
                    selectedColorIndex = Random.Range(0, tilesSettings.unitTileTypes.Length - 1);
                } while (tilesSettings.unitTileTypes[selectedColorIndex].Equals(playerController.playerColor));


                LNP.x = GenerateTile
                (
                    tilesSettings.unitTileTypes[selectedColorIndex],
                    platformSize,
                    platformSettings.minDistance - platformSettings.platformGenerationPoint.position.x + LNP.x
                ).position.x + platformSize * platformSettings.platformScale;

                // print("LNP: " + LNP);
            }
        }
        // print("--------------------------------------------");
    }

    public Transform GenerateTile(CustomColor customColor, int platformSize, float x = 0)
    {
        int platformHeight = platformSettings.GetPlatformHeight((int)LNP.y);

        Transform platform = platformSettings.platformBucket.GetChild(platformSettings.platformIndex % platformSettings.pooledPlatformsCount);

        platform.position = new Vector3
        (
            platformSettings.platformGenerationPoint.position.x + x,
            platformHeight,
            0
        );
        platform.parent = platformSettings.platformBucket;
        platform.localScale = new Vector3(platformSettings.platformScale, platformSettings.platformScale, 1);
        
        platformSettings.colorChanger.GetComponent<ColorChanger>().ClearColorChanger(platformSettings.platformIndex% platformSettings.pooledPlatformsCount);
        for (int i = 0; i < 10; i++)
        {
            if (i < platformSize)
            {
                platform.GetChild(i).gameObject.SetActive(true);
                platform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = customColor.color;
                platform.GetChild(i).gameObject.tag = customColor.name;
            }
            else if (platform.GetChild(i).gameObject.activeInHierarchy)
                platform.GetChild(i).gameObject.SetActive(false);
            else break;
        }

        // print("Platform: " + customColor.name);
        GenerateColorChangerRange(false);
        // Checking is it center of the new platform and it is right time to generate

		if (playerController.playerColor.Equals (customColor) && platformSettings.NPCPG >= platformSettings.GCCA) {
			GenerateColorChanger (prevColor, platform.GetChild (platformSize / 2).gameObject.transform);
			platformSettings.colorChanger.GetComponent<ColorChanger> ().platformIndex = platformSettings.platformIndex % platformSettings.pooledPlatformsCount;
		} else if(!playerController.playerColor.Equals (customColor)) {
			prevColor = customColor;
		}
        platformSettings.platformIndex++;
        return platform.transform;
    }
    
    public void GenerateColorChanger(CustomColor customColor, Transform parent)
    {
        // print("Generated: " + platformSize);
        platformSettings.colorChanger.SetActive(true);
        platformSettings.colorChanger.GetComponent<ColorChanger>().ChangerColor(customColor);
        platformSettings.colorChanger.transform.parent = parent;
        platformSettings.colorChanger.transform.localPosition = platformSize % 2 == 0 ? new Vector3(-.5f, 2, 0) : new Vector3(0, 2, 0);
        platformSettings.NPCPG = 0;
        GenerateColorChangerRange(true);
    }

    public void GenerateColorChangerRange(bool isRenew)
    {
        if (isRenew || platformSettings.GCCA == 0)
            platformSettings.GCCA = Random.Range(platformSettings.minPlaforms, platformSettings.maxPlaforms);
    }
}
