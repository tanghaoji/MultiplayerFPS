using UnityEngine;
using System.Collections;

/**
 * This class contains weapon logic and weapon animations
 **/
public class Weapon : MonoBehaviour {

    public Camera fpsCam;
    public GameObject bullet;

    // Gun animation
    public Animation gun;
    public AnimationClip shoot;
    public AnimationClip reload;

    public int damage = 30;
    public int range = 10000;
    public int ammo = 17;
    public int clipSize = 17;
    // TODO: decide if we want limited ammos
    public int ammoAvailable = 1000;

    void Update()
    {
        // Mouse left click
        if (Input.GetMouseButton(0))
        {
            fireShot();
        }

        // Keyboard R
        if (Input.GetKeyDown(KeyCode.R))
        {
            reloadAmmo();
        }
    }

    public void fireShot()
    {
        // Don't shoot bullets while the gun is animating or there's no ammo
        if (gun.IsPlaying(shoot.name) || gun.IsPlaying(reload.name)  || ammo <= 0) return;

        gun.CrossFade(shoot.name);
        ammo--;

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

    public void reloadAmmo()
    {
        if (ammoAvailable <= 0) return;
        gun.CrossFade(reload.name);
        ammo = clipSize;
        ammoAvailable -= clipSize;
    }

    void OnGUI()
    {
        GUI.Box(new Rect(120, 10, 120, 30), "Ammo | " + ammo + "/" + ammoAvailable);
    }

}
