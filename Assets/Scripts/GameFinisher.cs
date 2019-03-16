using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinisher : MonoBehaviour
{

    public PlayerController playerController;
    // Use this for initialization
    public GameObject[] effects;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            playerController.LevelComplete();
            GetComponent<AudioSource>().Play();
        }
    }

    IEnumerator Effects()
    {
        yield return new WaitForSeconds(.05f);
        effects[0].SetActive(true);
        yield return new WaitForSeconds(.05f);
        effects[1].SetActive(true);
        yield return new WaitForSeconds(.025f);
        effects[2].SetActive(true);
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 3; i++)
            effects[i].SetActive(false);
    }
}
