using UnityEngine;
using System.Collections;

/**
 * This class connects to the mp server and creates the main player
 **/
public class RoomManager : Photon.MonoBehaviour {

    // if you change this version number, people with old versions cannot play until update
    public string verNum = "0.1";
    public string roomName = "room01";
    public Transform spawnPoint;
    public GameObject playerPref;
    public bool isConnected = false;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(verNum);
        Debug.Log("Starting Connection");
    }

    public void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, null, null);
        Debug.Log("Starting Server!");
    }

    public void OnJoinedRoom()
    {
        isConnected = true;
        spawnPlayer();
    }

    /**
     * Creates a player 
     */
    public void spawnPlayer()
    {
        GameObject pl = PhotonNetwork.Instantiate(playerPref.name, spawnPoint.position, spawnPoint.rotation, 0) as GameObject;

        // Enable the player script
        pl.GetComponent<RigidbodyFPSController>().enabled = true;

        // Enable the camera of the player
        pl.GetComponent<RigidbodyFPSController>().fpsCam.SetActive(true);
    }

}
