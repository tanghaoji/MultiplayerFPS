using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    public Camera fpsCam;
    public GameObject bullet;

    // Gun animation
    public Animation gun;
    public AnimationClip shoot;

    public int damage = 30;
    public int range = 10000;

    void Update()
    {
        // Mouse left click
        if (Input.GetMouseButton(0))
        {
            fireShot();
        }
    }

    public void fireShot()
    {
        // Don't shoot bullets while the gun is animating
        if (gun.IsPlaying(shoot.name)) return;

        gun.CrossFade(shoot.name);

        RaycastHit hit;
        Ray ray = fpsCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hit, range))
        {
            // Shoot a bullet at the aiming point
            GameObject par = PhotonNetwork.Instantiate(bullet.name, hit.point, hit.transform.rotation, 0) as GameObject;

            // TODO: make it PhotonNetwork.Destroy
            Destroy(par, 0.2f);

            if (hit.transform.tag == "Player")
            {
                hit.transform.GetComponent<PhotonView>().RPC("applyDamage", PhotonTargets.All, damage);
                Debug.Log("hit target!");
            }
        }
    }
}
