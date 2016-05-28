using UnityEngine;
using System.Collections;

public class RoomMan : Photon.MonoBehaviour {

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

    public void spawnPlayer()
    {
        GameObject pl = PhotonNetwork.Instantiate(playerPref.name, spawnPoint.position, spawnPoint.rotation, 0) as GameObject;
        pl.GetComponent<RigidbodyFPSController>().enabled = true;
        pl.GetComponent<RigidbodyFPSController>().fpsCam.SetActive(true);
    }

}
