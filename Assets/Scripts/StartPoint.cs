using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPoint : MonoBehaviour
{
    public GameObject player;
    public Text displayText;
    public string[] textsToShow;
    public float[] timeBetweenTexts;
    private ScoreManager scoreManager;
    // Use this for initialization
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        for (int i = 0; i < textsToShow.Length; i++)
        {
            displayText.text = textsToShow[i];
            yield return new WaitForSeconds(timeBetweenTexts[i]/2);
            displayText.text = "";
            yield return new WaitForSeconds(timeBetweenTexts[i]/2);
        }
        
        displayText.enabled = false;
        player.SetActive(true);
        if (scoreManager)
            scoreManager.scoreIncreasing = true;
    }
}