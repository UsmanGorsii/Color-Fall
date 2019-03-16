using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPoint : MonoBehaviour {

    public GameObject player;
    public Text readyText;
    public float readyTextBlinkSpeed;
    private ScoreManager scoreManager;
	public int countDown;
    // Use this for initialization
    void Start () {
		countDown = 4;
		//readyText.text = "Starting in " + countDown;
        scoreManager = FindObjectOfType<ScoreManager>();
        StartCoroutine(GameStartWaiting());
    }

    // Update is called once per frame
    void Update() {
        //readyText.color = new Color(readyText.color.r, readyText.color.g, readyText.color.b, Mathf.Round(Mathf.PingPong(Time.time * readyTextBlinkSpeed, 1.0f)));
    }

    IEnumerator GameStartWaiting() {
		for (int i = 0; i < countDown; i++) {
			readyText.text = "Starting in "+(countDown-(i+1));
			yield return new WaitForSeconds(0.5f);
			readyText.text = "";
			yield return new WaitForSeconds(0.5f);
		}

		readyText.text = "Go!!!";
		yield return new WaitForSeconds(0.5f);
        readyText.enabled = false;
        player.SetActive(true);
		if(scoreManager)
		scoreManager.scoreIncreasing = true;
    }
}