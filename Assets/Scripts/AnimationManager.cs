using UnityEngine;
using System.Collections;

/**
 * This class renders the player animations locally and to network
 */
public class AnimationManager : MonoBehaviour {

    // TODO: get rid of this flag
    public bool isTp = false;
    
    // Client First Person Animation
    public Animation fpAnimation;
    public AnimationClip fpWalk;
    public AnimationClip fpIdle;

    // Network Third Person Animation
    public PhotonView photonView;
    public Animation tpAnimation;
    public AnimationClip tpIdle;
    public AnimationClip tpRun;
    public AnimationClip tpShoot;
    public AnimationClip tpRunShoot;
    public AnimationClip tpReload;

    // This is the player that is tracking
    public Rigidbody rigidBody;

    void Update()
    {
        // if the player is walking, play the walk animation
        if (rigidBody.velocity.magnitude >= 0.1)
        {
            playFpAnimation(fpWalk);
            playTpAnimation(tpRun);
        } else
        {
            playFpAnimation(fpIdle);

            // so that it will not conflict with reload
            // TODO: put every animation into an array
            if (isTp && !tpAnimation.IsPlaying(tpReload.name) && !tpAnimation.IsPlaying(tpRunShoot.name))
            {
                playTpAnimation(tpIdle);
            }
            
        }
    }

    // TODO: refactor this method
    public void reloadAmmo()
    {
        playTpAnimation(tpReload);
    }

    // TODO: refactor this method
    public void fireShot()
    {
        playTpAnimation(tpRunShoot);
    }

    // renders First Person animation on local
    private void playFpAnimation(AnimationClip animationClip)
    {
        if (isTp) return;
        fpAnimation.CrossFade(animationClip.name);
    }

    // renders Third Person animation to network
    private void playTpAnimation(AnimationClip animationClip)
    {
        if (!isTp) return;
        photonView.RPC("playAnimationPV", PhotonTargets.All, animationClip.name);
    }

    public void stopAnimation()
    {
        if (isTp)
        {
            tpAnimation.Stop();
        } else
        {
            fpAnimation.Stop();
        }
    }

    [PunRPC]
    public void playAnimationPV(string animationName)
    {
        tpAnimation.CrossFade(animationName);
    }

}
