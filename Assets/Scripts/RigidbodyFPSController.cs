using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

/**
 * This class contains player movement and health logic
 */
public class RigidbodyFPSController : MonoBehaviour
{

    public float speed = 10.0f;
    public float gravity = 9.8f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    private bool grounded = false;

    public int health;
    public int maxHealth = 100;

    public GameObject fpsCam;
    public GameObject me;
    public GameObject graphics;
    public PhotonView playerStatusReceiver;

    public AudioSource shootSound;

    // TODO: put the score menu in a better place
    public bool isPause = false;

    void Awake()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().useGravity = false;
        health = maxHealth;
        playerStatusReceiver.RPC("updateName", PhotonTargets.AllBuffered, PhotonNetwork.playerName);
        playerStatusReceiver.RPC("updateHP", PhotonTargets.AllBuffered, health, maxHealth);
    }

    void FixedUpdate()
    {
        if (grounded)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = GetComponent<Rigidbody>().velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump
            if (canJump && Input.GetButton("Jump"))
            {
                GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }
        }

        // We apply gravity manually for more tuning control
        GetComponent<Rigidbody>().AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));

        grounded = false;

        if (health <= 0)
        {
            GetComponent<PhotonView>().RPC("die", PhotonTargets.AllBuffered, null);
        }

        // Hold TAB will pause the game
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isPause = true;
        } 
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            isPause = false;
        }
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    /*
     * Renders current player's hp and score
     */
    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 100, 30), "HP | " + health + "/" + maxHealth);
        GUI.Box(new Rect(10, 45, 100, 30), "Score | " + PhotonNetwork.player.GetScore());

        // Renders a score menu
        if (isPause)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 250, 500, 500));
            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                GUILayout.Box(player.name + " | " + player.GetScore());
            }
            GUILayout.EndArea();
        }
    }

    [PunRPC]
    public void applyDamage(int dmg)
    {
        health -= dmg;
        playerStatusReceiver.RPC("updateHP", PhotonTargets.AllBuffered, health, maxHealth);
    }

    [PunRPC]
    public void die()
    {
        PhotonNetwork.Destroy(me);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    [PunRPC]
    public void playSound()
    {
        shootSound.Play();
    }

}
