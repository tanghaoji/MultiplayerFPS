using UnityEngine;
using System.Collections;

/**
 * This class renders the player animations locally
 */
 // TODO: make a super class for AnimationManager
public class AnimationManager : MonoBehaviour {
    
    // Animation sources for fp(locally)
    public Animation fpAnimation;

    /**
     * A generic method to play an animation clip, it will play locally or remotely depending on the context
     */
    public void playAnimation(AnimationClip animationClip)
    {
        playFpAnimation(animationClip);
    }

    /**
     * Tp only, returns true if this animationClip will not overwrite animation that is currently playing
     * false, otherwise 
     */
    public bool canPlay(AnimationClip animationClip)
    {
        return true;
    }

    public bool isPlaying()
    {
        return fpAnimation.isPlaying;
    }

    // renders First Person animation on local
    private void playFpAnimation(AnimationClip animationClip)
    {
        fpAnimation.CrossFade(animationClip.name);
    }

    public void stopPlay(AnimationClip animationClip)
    {
        fpAnimation.Stop(animationClip.name);
    }

    public void stopAnimation()
    {
        fpAnimation.Stop();
    }

}
