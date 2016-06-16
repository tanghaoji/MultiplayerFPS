﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum GameState
{
    Offline,
    InLobby,
    InRoom,
    InGame
}


/**
 * This class connects to the mp server and creates the main player
 */
public class RoomManager : Photon.MonoBehaviour {

    // if you change this version number, people with old versions cannot play until update
    public string verNum = "0.1";
    
    public Transform[] spawnPoints;
    public GameObject playerPref;

    public InRoomChat chat;

    public string roomName;
    public string playerName;

    private GameState gameState = GameState.Offline;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(verNum);
        roomName = "Room " + Random.Range(0, 999);
        playerName = "Player " + Random.Range(0, 20);
        gameState = GameState.InLobby;
        Debug.Log("Starting Connection");
    }

    void Update()
    {
        // update the in-room and in-game chat
        if (gameState == GameState.InRoom || gameState == GameState.InGame)
        {
            chat.enabled = true;
        } else
        {
            chat.enabled = false;
        }
    }

    /**
     * Pop up room creation and selection menu
     */
    public void OnJoinedLobby()
    {
        gameState = GameState.InLobby;
        Debug.Log("Starting Server!");
    }

    /** 
     * enter the game
     */
    public void OnJoinedRoom()
    {
        PhotonNetwork.playerName = playerName;
        gameState = GameState.InRoom;
    }

    /**
     * Creates a player at a random spawn point
     */
    public void spawnPlayer()
    {
        gameState = GameState.InGame;

        Transform randomSpawnPt = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject pl = PhotonNetwork.Instantiate(playerPref.name, randomSpawnPt.position, randomSpawnPt.rotation, 0) as GameObject;

        // Enable the player script
        pl.GetComponent<RigidbodyFPSController>().enabled = true;

        // Enable the camera of the player
        pl.GetComponent<RigidbodyFPSController>().fpsCam.SetActive(true);

        // Disable the graphic of the player on local
        pl.GetComponent<RigidbodyFPSController>().graphics.SetActive(false);
    }

    /*
     * Renders the lobby room and player create menu
     */
    void OnGUI()
    {
        // lobby
        if (gameState == GameState.InLobby)
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

        // room
        if (gameState == GameState.InRoom)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 300));
            GUILayout.Box("Score: " + PhotonNetwork.player.GetScore());
            if (GUILayout.Button("Start game"))
            {
                spawnPlayer();
            }

            if (GUILayout.Button("Quit room"))
            {
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene(0);
            }

            GUILayout.EndArea();
        }
    }

}