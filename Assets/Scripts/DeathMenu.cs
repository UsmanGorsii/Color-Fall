using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour {

    public Text text;
    public string mainSceneName;

    public virtual void Retry() {
        text.text = "Color Fatigue!";
        FindObjectOfType<GameManager>().ResetGame();
    }

    public virtual void QuitToMainMenu() {
        SceneManager.LoadScene(mainSceneName);
    }

}
