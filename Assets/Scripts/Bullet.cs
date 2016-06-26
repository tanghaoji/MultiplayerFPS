using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public int damage = 10;
    public int maxDamage = 23;
    public float parDestroyDelay = 0.7f;

    // Should be set right after instantiate
    public string shooterName = "";

    // For testing purpose
    public bool enableDmg = true;

    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, parDestroyDelay);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.GetComponent<PhotonView>().RPC("applyDamage", PhotonTargets.All, getDamage(), shooterName);
        }
        Destroy(this.gameObject);
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
        }
        else
        {
            return 0;
        }
    }

}
