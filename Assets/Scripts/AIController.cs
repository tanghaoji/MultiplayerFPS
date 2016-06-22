using UnityEngine;
using System.Collections;

/// <summary>
/// A class to control the bot's behaviour
/// </summary>
public class AIController : MonoBehaviour {

    public NavMeshAgent nma;
    public GameObject me;
    public GameObject ragDoll;

    public string botName;
    public int health = 100;
    public int damage = 10;
    public int maxDamage = 20;

    private GameObject[] targets;

    // TODO: remove this temp var
    private string dmgFrom = "";

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
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.GetComponent<PhotonView>().RPC("applyDamage", PhotonTargets.All, Random.Range(damage, maxDamage), botName);
        }
    }

    public void setTarget()
    {
        if (targets.Length < 1) return;
        int targetIdx = Random.Range(0, targets.Length);
        nma.SetDestination(targets[targetIdx].transform.position);
    }

    private void die()
    {
        Destroy(me);
        GameObject doll = Instantiate(ragDoll, transform.position, transform.rotation) as GameObject;
        Destroy(doll, 6f);

        // spawn AI per client, so if there are two clients, it will spawn two AIs per time
        GameObject.Find("_ROOM").GetComponent<RoomManager>().spawnAI();
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
