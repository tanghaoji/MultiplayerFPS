using UnityEngine;
using System.Collections;

/// <summary>
/// A class to control the bot's behaviour
/// </summary>
public class AIController : MonoBehaviour {

    public NavMeshAgent nma;
    public GameObject me;
    public GameObject ragDoll;

    public GameObject bulletPref;
    public Transform muzzle;
    public float bulletSpeed = 6000;
    public float fireRate = 1f;

    public float changeTargetRate = 10f;

    public string botName;
    public int health = 100;
    public int damage = 10;
    public int maxDamage = 20;

    // tp animations
    public TpAnimationManager tpAnimationManager;
    public AnimationClip tpIdle;
    public AnimationClip tpRun;
    public AnimationClip tpRunShoot;
    // TODO: add more tp animations here...

    // for testing purpose
    public bool enableDmg = true;
    public bool destroyRagDoll = true;

    private GameObject[] targets;

    // TODO: remove this temp var
    private string dmgFrom = "";

    private bool canChangeTarget = true;
    private bool isPlayingShoot = false;

    private int targetIdx = 0;

    void Awake()
    {
        string name_prefix = "BOT ";
        botName = name_prefix + Random.Range(0, 20);
    }

    void Update()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
        if (targets.Length > 0)
        {
            setTarget();
        }

        if (health <= 0)
        {
            die();
        }

        // Play movement animation
        if (!isPlayingShoot)
        {
            tpAnimationManager.playAnimation(tpRun);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.GetComponent<PhotonView>().RPC("applyDamage", PhotonTargets.All, getDamage(), botName);
        }
    }

    void OnTriggerStay(Collider collision)
    {
        if (!isPlayingShoot && collision.transform.tag == "Player")
        {
            StartCoroutine(shoot());
        }
    }

    IEnumerator shoot()
    {
        isPlayingShoot = true;
        Debug.Log("AI shooting");
        GameObject bullet = PhotonNetwork.Instantiate(bulletPref.name, muzzle.position, muzzle.rotation, 0) as GameObject;
        bullet.GetComponent<Bullet>().shooterName = botName;
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);

        tpAnimationManager.stopAnimation();
        tpAnimationManager.playAnimation(tpRunShoot);

        yield return new WaitForSeconds(fireRate);
        isPlayingShoot = false;
        StopCoroutine(shoot());
    }

    public void setTarget()
    {
        if (targets.Length >= 1)
        {
            if (canChangeTarget)
            {
                StartCoroutine(changeTarget());
            }

            // validate the tagetIdx
            if (targetIdx >= 0 && targetIdx < targets.Length)
            {
                nma.SetDestination(targets[this.targetIdx].transform.position);
            } else
            {
                // if the current target does not exist anymore, change target
                changeTarget();
            }
        }
    }

    /// <summary>
    /// Randomly changes the target
    /// </summary>
    /// <returns></returns>
    IEnumerator changeTarget()
    {
        canChangeTarget = false;
        if (targets.Length >= 1)
        {
            this.targetIdx = Random.Range(0, targets.Length);
        }
        yield return new WaitForSeconds(changeTargetRate);
        canChangeTarget = true;
    }

    /// <summary>
    /// A method to calculate the damage value to a target
    /// </summary>
    /// <returns></returns>
    private int getDamage()
    {
        if (enableDmg)
        {
            return Random.Range(damage, maxDamage);
        } else
        {
            return 0;
        }
    }

    private void die()
    {
        Destroy(me);
        GameObject doll = Instantiate(ragDoll, transform.position, transform.rotation) as GameObject;
        if (destroyRagDoll)
        {
            Destroy(doll, 6f);
        }
        
        // spawn AI per client, so if there are two clients, it will spawn two AIs per time
        GameObject.Find("_ROOM").GetComponent<RoomManager>().onAIDIe();
    }

    [PunRPC]
    public void applyDamageAI(int dmg, string fromPlayer)
    {
        dmgFrom = fromPlayer;
        health -= dmg;

        // TODO: this should not be called in the RPC, should be called in local client
        if (GetComponent<PhotonView>().isMine)
        {
            GameObject.Find("_NETWORK").GetComponent<FeedManager>().addDamageFeed(botName, dmgFrom, dmg);

            if (health <= 0)
            {
                GameObject.Find("_NETWORK").GetComponent<FeedManager>().addKillFeed(botName, dmgFrom);
            }
        }

    }

}
