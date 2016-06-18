using UnityEngine;
using System.Collections;

/**
 * This class contains weapon logic and weapon animations
 */
public class Weapon : MonoBehaviour {

    public Camera fpsCam;
    public GameObject bullet;
    public Texture scope;
    public AnimationManager fpAnimationManager;
    public TpAnimationManager tpAnimationManager;
    public PhotonView soundReceiver;

    // the array of objects to disable when aiming
    public GameObject[] partsToDisable;

    // fp animations
    //public Animation gun;
    public AnimationClip fpShoot;
    public AnimationClip fpReload;

    // tp animations
    // TODO: add more animations
    public AnimationClip tpShoot;
    public AnimationClip tpRunShoot;
    public AnimationClip tpReload;

    // Gun audio
    // Since the current Unity doesn't support storing AudioClip array in AudioSource, 
    // so we have to use hard coded index for each sound
    public int shootSoundIndex;

    public float recoilPower = 30;
    public int damage = 10;
    public int maxDamage = 30;
    public int range = 10000;
    public int ammo = 17;
    public int clipSize = 17;
    // TODO: decide if we want limited ammos
    public int ammoAvailable = 1000;

    public bool canAim = false;
    private bool isAiming = false;
    public float aimFOV = 20; // aim field of view
    public float regFOV = 60; // regular field of view

    // temp var to destroy when next gun shot
    private GameObject _par;

    void Update()
    {
        // Mouse left click
        if (Input.GetMouseButton(0))
        {
            fireShot();
        }

        // Mouse right click down
        if (Input.GetMouseButtonDown(1))
        {
            aim();
        }

        // Mouse right click up
        if (Input.GetMouseButtonUp(1))
        {
            aimOut();
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
        if (fpAnimationManager.isPlaying() || ammo <= 0) return;

        // render animation on local and network
        fpAnimationManager.playAnimation(fpShoot);
        tpAnimationManager.stopAnimation();
        tpAnimationManager.playAnimation(tpRunShoot);
        Debug.Log("Gun shot-------------------------------------------------------");

        // play gun shoot sound
        soundReceiver.RPC("playAudioClip", PhotonTargets.AllBuffered, shootSoundIndex);

        ammo--;

        // Recoil
        fpsCam.transform.Rotate(Vector3.right, -recoilPower * Time.deltaTime);

        RaycastHit hit;
        Ray ray = fpsCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2 - 25, 0));

        if (Physics.Raycast(ray, out hit, range))
        {
            // Temp fix: network destroy the last bullet on next shooting
            if (_par != null)
            {
                PhotonNetwork.Destroy(_par);
            }
            
            // Shoot a bullet at the aiming point
            GameObject par= PhotonNetwork.Instantiate(bullet.name, hit.point, Quaternion.LookRotation(hit.normal), 0) as GameObject;
            _par = par;

            // TODO: make it PhotonNetwork.Destroy
            //Destroy(par, 0.2f); // local destroy
            // PhotonNetwork.Destroy(par);

            if (hit.transform.tag == "Player")
            {
                Debug.Log("hit target!");
                hit.transform.GetComponent<PhotonView>().RPC("applyDamage", PhotonTargets.All, Random.Range(damage,maxDamage), PhotonNetwork.playerName);

                // Add score to the current player
                PhotonNetwork.player.AddScore(1);
                Debug.Log("Current player's score " + PhotonNetwork.player.GetScore());
            }
        }
    }

    public void reloadAmmo()
    {
        if (ammoAvailable <= 0) return;

        // render animation on local and network
        fpAnimationManager.playAnimation(fpReload);
        tpAnimationManager.stopAnimation();
        tpAnimationManager.playAnimation(tpReload);

        ammo = clipSize;
        ammoAvailable -= clipSize;
    }

    public void aim()
    {
        if (!canAim) return;
        fpsCam.fieldOfView = aimFOV;

        // hide the gun parts and render the scope
        setActive(false);
        isAiming = true;
    }

    public void aimOut()
    {
        if (!canAim) return;
        fpsCam.fieldOfView = regFOV;

        // re-enable the hidden gun parts and hide the scope
        setActive(true);
        isAiming = false;
    }

    private void setActive(bool active)
    {
        foreach (GameObject part in partsToDisable)
        {
            part.SetActive(active);
        }
    }

    /*
     * Renders the number of ammos left, and current player's score, and the aiming scope
     */
    void OnGUI()
    {
        GUI.Box(new Rect(120, 10, 120, 30), "Ammo | " + ammo + "/" + ammoAvailable);

        // render scope
        if (isAiming)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), scope, ScaleMode.ScaleAndCrop);
        }
    }

    

}
