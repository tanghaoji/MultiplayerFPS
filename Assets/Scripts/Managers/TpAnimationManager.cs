using UnityEngine;
using System.Collections;

public class TpAnimationManager : MonoBehaviour {

    public Animation tpAnimation;

    // Network Third Person Animation
    public PhotonView photonView;

    /**
     * A generic method to play an animation clip, it will play locally or remotely depending on the context
     */
    public void playAnimation(AnimationClip animationClip)
    {
        playTpAnimation(animationClip);
    }

    /**
     * Tp only, returns true if this animationClip will not overwrite animation that is currently playing
     * false, otherwise 
     */
    public bool canPlay(AnimationClip animationClip)
    {
        return !tpAnimation.isPlaying || tpAnimation.IsPlaying(animationClip.name);
    }

    public bool isPlaying()
    {
       
        return tpAnimation.isPlaying;
    }

    // renders Third Person animation to network
    private void playTpAnimation(AnimationClip animationClip)
    {
        photonView.RPC("playAnimationPV", PhotonTargets.All, animationClip.name);
    }

    public void stopPlay(AnimationClip animationClip)
    {
        photonView.RPC("stopPlayPV", PhotonTargets.All, animationClip.name);

    }

    public void stopAnimation()
    {
        photonView.RPC("stopAnimationPV", PhotonTargets.All, null);
    }

    [PunRPC]
    public void playAnimationPV(string animationName)
    {
        tpAnimation.CrossFade(animationName);
    }

    [PunRPC]
    public void stopPlayPV(string animationName)
    {
        tpAnimation.Stop(animationName);
    }

    [PunRPC]
    public void stopAnimationPV()
    {
        tpAnimation.Stop();
    }

}
