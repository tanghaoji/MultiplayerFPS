using UnityEngine;
using System.Collections;

/**
 * This class renders the player animations
 **/
public class AnimationManager : MonoBehaviour {

    // The object that is animating
    public Animation animationObject;
    public AnimationClip walk;
    public AnimationClip idle;

    // This is the player that is tracking
    public Rigidbody rigidBody;

    void Update()
    {
        // if the player is walking, play the walk animation
        if (rigidBody.velocity.magnitude >= 0.1)
        {
            playAnimation(walk.name);
        } else
        {
            playAnimation(idle.name);
        }
    }

    public void playAnimation(string animationName)
    {
        animationObject.CrossFade(animationName);
    }

    public void stopAnimation()
    {
        animationObject.Stop();
    }

}
