using UnityEngine;
using System.Collections;

/**
 * This file manages all the audio clips of a player.
 * Since the current Unity doesn't support storing AudioClip array in AudioSource in inspector, 
 * we have to use hard coded index to determine with audio to play.
 * 
 * To register an audio, 
 * Step1: add the clip in the array in inspector, 
 * Step2: register the index in the object inspector that will play the audio
 */
public class AudioManager : MonoBehaviour {

    // This array should contains all the audio that will be triggered to play
    public AudioClip[] audios;

    public void localPlayAudio(int index)
    {
        if (index >= audios.Length) return;
        GetComponent<AudioSource>().PlayOneShot(audios[index]);
    }

    [PunRPC]
    public void playAudioClip(int index)
    {
        if (index >= audios.Length) return;
        GetComponent<AudioSource>().PlayOneShot(audios[index]);
    }

}
