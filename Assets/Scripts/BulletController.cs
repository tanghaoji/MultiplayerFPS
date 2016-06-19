using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    public float parDestroyDelay = 0.2f;

    // Use this for initialization
    void Start () {
        Destroy(this.gameObject, parDestroyDelay);
	}
	
}
