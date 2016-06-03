using UnityEngine;
using System.Collections;

/**
 * This class connects to the mp server and creates the main player
 */
public class RoomManager : Photon.MonoBehaviour {

    // if you change this version number, people with old versions cannot play until update
    public string verNum = "0.1";
    
    public Transform spawnPoint;
    public GameObject playerPref;

    public string roomName;
    public string playerName;
    public bool isIdle = false;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(verNum);
        roomName = "Room " + Random.Range(0, 999);
        playerName = "Player " + Random.Range(0, 20);
        Debug.Log("Starting Connection");
    }

    public void OnJoinedLobby()
    {
        isIdle = true;
        Debug.Log("Starting Server!");
    }

    public void OnJoinedRoom()
    {
        PhotonNetwork.playerName = playerName;
        isIdle = false;
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

        // Disable the graphic of the player on local
        pl.GetComponent<RigidbodyFPSController>().graphics.SetActive(false);
    }

    void OnGUI()
    {
        if (isIdle)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 300));

            playerName = GUILayout.TextField(playerName);
            roomName = GUILayout.TextField(roomName);
            if (GUILayout.Button("Create"))
            {
                PhotonNetwork.JoinOrCreateRoom(roomName, null, null);
            }

            // Display a list of available rooms on the server
            foreach (RoomInfo game in PhotonNetwork.GetRoomList())
            {
                if (GUILayout.Button(game.name + " " + game.playerCount + "/" + game.maxPlayers))
                {
                    PhotonNetwork.JoinOrCreateRoom(game.name, null, null);
                }
            }

            GUILayout.EndArea();
        }
    }

}
