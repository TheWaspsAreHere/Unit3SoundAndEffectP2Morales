using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour


{
    private Rigidbody playerRb;
    public float jumpForce;
    public float gravityModifier;
    public bool isOnGround = true;
    public bool gameOver;
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;
    public AudioClip jumpSound;
    public AudioClip crashSound;
    private AudioSource playerAudio;
    private Animator playerAnim;

    private int jumpCount = 0;
    private int maxJumps = 2;

    public bool startGame = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        Physics.gravity *= gravityModifier;
        playerAudio = GetComponent<AudioSource>();
    }
    IEnumerator WalkThenRun()
    {
        WalkIn();
        yield return new WaitForSeconds(1.5f);
        Run();
    }

    void WalkIn()
    {
        playerAnim.SetFloat("Speed_f", 0.4f);
    }

    void Run()
    {
        playerAnim.SetFloat("Speed_f", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps && !gameOver)
        {
            if (jumpCount > 0)
            {
                // reset the vertical velocity before the second jump to avoid it being too high
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
            }

            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            playerAnim.Play("Running_Jump", -1, 0f);
            dirtParticle.Stop();
            playerAudio.PlayOneShot(jumpSound, 1.0f);

            jumpCount++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && isOnGround && !gameOver) 
        {
            playerAnim.speed = 5.0f;
        }
        else if ( Input.GetKeyDown(KeyCode.LeftShift) && isOnGround && !gameOver)
        {
            playerAnim.speed = 1.0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0; // reset the jump count to 0 when 
            dirtParticle.Play();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over");
            gameOver = true;
            playerAnim.SetBool("Death_b", true);
            playerAnim.SetInteger("DeathType_int", 1);
            explosionParticle.Play();
            dirtParticle.Stop();
            playerAudio.PlayOneShot(crashSound, 1.0f);
        }
    }
}