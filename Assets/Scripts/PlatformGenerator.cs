using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [System.Serializable]
    public class CustomColor
    {
        public string name;
        public Color color;

        public CustomColor(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }

        public bool Equals(CustomColor customColor)
        {
            return name.Equals(customColor.name);
        }
    }
    [System.Serializable]
    public class TilesSettings
    {
        public CustomColor[] unitTileTypes;
        public CustomColor playerColor;
    }
    public TilesSettings tilesSettings;

    [System.Serializable]
    public class PlatformSettings
    {
        public GameObject platform;
        public Transform platformGenerationPoint;
        public float platformScale;
        public Transform platformBucket;
        // [Tooltip("Index of selected pooled platform")]
        int platformIndex;
        public int PlatformIndex
        {
            get { return platformIndex++ % pooledPlatformsCount; }
            set { platformIndex = value; }
        }
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
    #endregion

    public GameObject LPP_G, LNP_G;
    public float gameSpeed = 1;
    public bool changeGameSpeed;
    void Start()
    {
        // LPP = LNP = new Vector2(3, -1);
        platformSettings.PoolPlatforms();
    }

    void Update()
    {
        if (testGeneratePlatform)
        {
            testGeneratePlatform = false;
            GenerateTile(new CustomColor("White", Color.white), platformSettings.GetPlatformSize());
        }
        LPP_G.transform.position = new Vector3(LPP.x, LPP_G.transform.position.y, 1);
        LNP_G.transform.position = new Vector3(LNP.x, LNP.y, 1);
        if(!startGame)
            return;

        if(changeGameSpeed)
        {
            changeGameSpeed = false;
            Time.timeScale = gameSpeed;
        }

        GP = platformSettings.platformGenerationPoint.position.x;
        Debug.Log(GP);

        distance = Distance(-LPP.x, -GP);
        

        // If GP is in between max and min distance from LPP
        if (platformSettings.maxDistance > distance && distance > platformSettings.minDistance)
        {
            GRP = true;
            platformSize = platformSettings.GetPlatformSize();

            print("Distance(LPP, GP): " + distance);
            print("Distance(GP, LNP): " + Distance(GP, LNP.x));
            print("platformSize: " + platformSize);            
            print(platformSize * platformSettings.platformScale + " < " + (platformSettings.maxDistance - GP + LPP.x));
           

            if (platformSize * platformSettings.platformScale <= platformSettings.maxDistance - (GP - LPP.x))
            {
                if (Distance(GP, LNP.x) >= platformSettings.minDistance)
                {
                    int selectedColorIndex = Random.Range(0, tilesSettings.unitTileTypes.Length - 1);

                    if (tilesSettings.unitTileTypes[selectedColorIndex].name.Equals(tilesSettings.playerColor.name))
                    {
                        print("Player color platform generated.");
                        CPCP = false;
                        Vector3 plafotmPosition = GenerateTile
                        (
                            tilesSettings.playerColor,
                            platformSize
                        ).position;
                        LNP.x = LPP.x = plafotmPosition.x + platformSize * platformSettings.platformScale;
                        LNP.y = LPP.y = plafotmPosition.y;
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
             print("CPCP: " + CPCP);
        }
        else if (GRP)
        {
            GRP = false;
            if (true)
            {
                platformSize = platformSettings.GetPlatformSize();
                Vector3 plafotmPosition = GenerateTile
                (
                    tilesSettings.playerColor,
                    platformSize,
                    platformSettings.minDistance - platformSettings.platformGenerationPoint.position.x + LNP.x
                ).position;

                LNP.x = LPP.x = plafotmPosition.x + platformSize * platformSettings.platformScale;
                LNP.y = LPP.y = plafotmPosition.y;
                print("LPP: " + LPP);
            }
            else
            {
                platformSize = platformSettings.GetPlatformSize();
                int selectedColorIndex;
                // Try to select color until it is different from player color
                do
                {
                    selectedColorIndex = Random.Range(0, tilesSettings.unitTileTypes.Length - 1);
                } while (tilesSettings.unitTileTypes[selectedColorIndex].Equals(tilesSettings.playerColor));
                 

                LNP.x = GenerateTile
                (
                    tilesSettings.unitTileTypes[selectedColorIndex],
                    platformSize,
                    platformSettings.minDistance - platformSettings.platformGenerationPoint.position.x + LNP.x
                ).position.x + platformSize * platformSettings.platformScale;

                print("LNP: " + LNP);
            }
        }
        print("--------------------------------------------");
    }

    public Transform GenerateTile(CustomColor customColor, int platformSize, float x = 0)
    {
        int platformHeight = platformSettings.GetPlatformHeight((int)LNP.y);

        Transform platform = platformSettings.platformBucket.GetChild(platformSettings.PlatformIndex);

        platform.position = new Vector3
        (
            platformSettings.platformGenerationPoint.position.x + x,
            platformHeight,
            0
        );
        platform.parent = platformSettings.platformBucket;
        platform.localScale = new Vector3(platformSettings.platformScale, platformSettings.platformScale, 1);

        for (int i = 0; i < 10; i++)
        {
            if (i < platformSize)
            {
                platform.GetChild(i).gameObject.SetActive(true);
                platform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = customColor.color;
                platform.GetChild(i).gameObject.tag = customColor.name;
            }
            else if(platform.GetChild(i).gameObject.activeInHierarchy)
                platform.GetChild(i).gameObject.SetActive(false);
            else break;
        }

        return platform.transform;
    }

    float Distance(float from, float to)
    {
        return from - to;
    }
}