using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
	public PlayerController playerController;
    public CustomColor color;

	public int platformIndex;
	public float starEffectTime;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangerColor(CustomColor customColor)
    {
		print("Color: " + customColor.name);
        color.color = customColor.color;
        color.name = customColor.name;

        spriteRenderer.color = color.color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
			print("Player Hit");
            playerController.ChangeColor(color);
			playerController.ActivateStarEffect(starEffectTime, color.color);
			gameObject.SetActive(false);
        }
    }

	public void ClearColorChanger(int platformIndex)
	{
		if(this.platformIndex == platformIndex)
			gameObject.SetActive(false);
	}
}
