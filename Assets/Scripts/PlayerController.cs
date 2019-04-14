using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class JumpSettings
    {
        public float jumpForce;
        public float jumpTime;
        public float jumpTimeCounter;
        public int jumpsCount;
        public int maxJumps;
        public AudioSource jumpSound;
    }
    public JumpSettings jumpSettings;

    [System.Serializable]
    public class MovementSettings
    {
        public float moveSpeed;
        public float moveSpeedOrigin;
        public float speedMutiplier;
        public float speedIncreaseMilestone;
        public float speedIncreaseMilestoneOrigin;
        public float speedMilestoneCounts;
        public float speedMilestoneCountsOrigin;
    }
    public MovementSettings movementSettings;

    public Text text;
    
	public float minYBeforeDeath = -4.5f;


    // Ground checking properties

    public bool isOnGround;
	public bool rotate = false;
	private float rotateDirection = 1;
	public Transform playerSprite;

	public float speedRotate = 10;
    
	public LayerMask groundLayer;

    public Transform groundCheckPoint;
    public float groundCheckRadius;

    private Rigidbody2D myRigidbody;
    private Animator myAnimator;

    public GameManager gameManager;

    public AudioSource deathSound;

    public CustomColor playerColor;
    public bool isDead;
    public GameObject thinking;

    public bool starEffect;
    public ParticleSystem starEffectPS;
    public ScoreManager scoreManager;

    // Use this for initialization
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        this.GetComponent<CircleCollider2D>().enabled = true;
        movementSettings.speedMilestoneCounts = movementSettings.speedIncreaseMilestone;

        movementSettings.moveSpeedOrigin = movementSettings.moveSpeed;
        movementSettings.speedMilestoneCountsOrigin = movementSettings.speedMilestoneCounts;
        movementSettings.speedIncreaseMilestoneOrigin = movementSettings.speedIncreaseMilestone;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
			return;

		if (this.transform.position.y < minYBeforeDeath)
			ManageDeath ();
        // Detect the ground
        isOnGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        jumpSettings.jumpsCount = isOnGround ? 0 : jumpSettings.jumpsCount;

        if (isOnGround && isThoughtsVisible)
            HideThoughts();
        
        // Speed up if pass the speed milestone
        // if (transform.position.x > movementSettings.speedMilestoneCounts)
        // {
        //     movementSettings.speedMilestoneCounts += movementSettings.speedIncreaseMilestone;
        //     movementSettings.moveSpeed *= movementSettings.speedMutiplier;

        //     movementSettings.speedIncreaseMilestone += movementSettings.speedIncreaseMilestone * movementSettings.speedMutiplier;
        // }

        // Moving right
        myRigidbody.velocity = new Vector2(movementSettings.moveSpeed, myRigidbody.velocity.y);

        // Jump (Space and left key of mouse)
        // if space or mouse button is pressed
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            // If jump count is less then max number of jumps and jump Timer is not over
            if (jumpSettings.jumpsCount < jumpSettings.maxJumps && jumpSettings.jumpTimeCounter > 0)
            {
				rotateDirection = rotateDirection == 1 ? 0 : 1;
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpSettings.jumpForce);
				rotate = true;
                jumpSettings.jumpTimeCounter -= Time.deltaTime;
            }
        }



        // if (isActiveAndEnabled && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        // {
        //     jumpSound.Play();
        //     if (isOnGround)
        //     {
        //         myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
        //         stopJumpping = false;
        //     }
        //     else if (canDoubleJumping)
        //     {
        //         canDoubleJumping = false;
        //         jumpTimeCounter = jumpTime;
        //         myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
        //         stopJumpping = false;
        //     }
        // }

        // if(!isActiveAndEnabled && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        // {
        //     thinking.SetActive(true);
        //     thinking.GetComponent<SpriteRenderer>().color = playerColor.color;
        //     thinking.transform.GetChild(0).gameObject.SetActive(true);
        //     thinking.transform.GetChild(0).GetComponent<TextMesh>().color = playerColor.color;
        // }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            jumpSettings.jumpsCount++;
            jumpSettings.jumpTimeCounter = jumpSettings.jumpTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if(jumpSettings.jumpsCount >= jumpSettings.maxJumps)
                ShowThoughts();
            else
                jumpSettings.jumpSound.Play();
        }

        // if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && !stopJumpping)
        // {
        //     if (jumpTimeCounter > 0)
        //     {
        //         myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
        //         jumpTimeCounter -= Time.deltaTime;
        //     }
        // }

        // if (isOnGround)
        // {
        //     jumpTimeCounter = jumpTime;
        //     canDoubleJumping = true;
        // }

        // Setup animators
        myAnimator.SetFloat("Speed", myRigidbody.velocity.x);
        myAnimator.SetBool("IsOnGround", isOnGround);

		if (rotate) {
			if (rotateDirection == 1) {
				playerSprite.Rotate (0,0,speedRotate);
			} else {
				playerSprite.Rotate (0, -0,-speedRotate);
			}

			//speedRotate -= Time.deltaTime;
		}
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (starEffect)
            return;

		if (!other.gameObject.CompareTag (playerColor.name)) {
			ManageDeath ();
		} else {
			rotate = false;
			Debug.Log (transform.rotation.y);
			playerSprite.rotation = new Quaternion (0,0,0,0);
		}
    }

	void ManageDeath(){
		scoreManager.stopScore = true;
		isDead = true;
		deathSound.Play ();
		myRigidbody.velocity = new Vector2 (0, jumpSettings.jumpForce);
		this.GetComponent<CircleCollider2D> ().enabled = false;
		StartCoroutine (Death ());
	}
    IEnumerator Death()
    {
        yield return new WaitForSeconds(2);
        this.GetComponent<CircleCollider2D>().enabled = true;
        isDead = false;
        gameManager.RestartGame();
        movementSettings.moveSpeed = movementSettings.moveSpeedOrigin;
        movementSettings.speedMilestoneCounts = movementSettings.speedMilestoneCountsOrigin;
        movementSettings.speedIncreaseMilestone = movementSettings.speedIncreaseMilestoneOrigin;
    }
    public void LevelComplete()
    {
        text.text = "Congratulations! You have successfully completed demo version";
        // deathSound.Play();
        gameManager.RestartGame();
        movementSettings.moveSpeed = movementSettings.moveSpeedOrigin;
        movementSettings.speedMilestoneCounts = movementSettings.speedMilestoneCountsOrigin;
        movementSettings.speedIncreaseMilestone = movementSettings.speedIncreaseMilestoneOrigin;
    }

    public void ChangeColor(CustomColor customColor)
    {
        playerColor.name = customColor.name;
        playerColor.color = customColor.color;
		playerSprite.rotation = new Quaternion (0,0,0,0);

        //GetComponent<SpriteRenderer>().color = customColor.color;
		playerSprite.GetComponent<SpriteRenderer>().color = customColor.color;
    }

    #region Start Effect
    Coroutine startEffectE;
	public void ActivateStarEffect(float time, Color color)
    {
        starEffect = true;

        if (startEffectE == null)
			startEffectE = StartCoroutine(ActivateStarEffectE(time, color));
        else
        {
            StopCoroutine(startEffectE);
			startEffectE = StartCoroutine(ActivateStarEffectE(time, color));
        }
    }

	IEnumerator ActivateStarEffectE(float time, Color color)
    {
        starEffectPS.Play();
		ParticleSystem.MainModule mainSettings = starEffectPS.main;
		mainSettings.startColor = new ParticleSystem.MinMaxGradient (color);

        for (int i = 0; i < time; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                playerSprite.GetComponent<SpriteRenderer>().color = PlatformGenerator.Instance.tilesSettings.unitTileTypes[j % 7].color;
                yield return new WaitForSeconds(.05f);
            }
        }
        //GetComponent<SpriteRenderer>().color = playerColor.color;
		playerSprite.GetComponent<SpriteRenderer>().color = playerColor.color;
        starEffectPS.Stop();
        starEffect = false;
    }
    #endregion

    #region Thoughts
    Coroutine thoughts;
    public bool isThoughtsVisible;
    public float thoughtsDisplayTime;
    public void ShowThoughtsFor(float time)
    {
        if (thoughts == null)
            thoughts = StartCoroutine(ShowThoughtsForE(time));
        else
        {
            StopCoroutine(thoughts);
            thoughts = StartCoroutine(ShowThoughtsForE(time));
        }
    }

    IEnumerator ShowThoughtsForE(float time)
    {
        ShowThoughts();
        yield return new WaitForSeconds(time);
        HideThoughts();
    }

    void ShowThoughts()
    {
        if(isThoughtsVisible)
            return;

        isThoughtsVisible = true;
        thinking.SetActive(true);
        thinking.GetComponent<SpriteRenderer>().color = playerColor.color;
        thinking.transform.GetChild(0).gameObject.SetActive(true);
        thinking.transform.GetChild(0).GetComponent<TextMesh>().color = playerColor.color;
    }

    void HideThoughts()
    {
        if(!isThoughtsVisible)
            return;

        isThoughtsVisible =false;
        thinking.SetActive(false);
        thinking.transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion
}