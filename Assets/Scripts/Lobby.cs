using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject controlPanel;

    private int maxPlayerPerRoom { get; set; } = 10;
    private bool isConnecting { get; set; }
    private string gameVersion { get; set; } = "1.0";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void Connect()
    {
        isConnecting = true;
        controlPanel.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this.gameVersion;
        }
    }
    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new()
        {
            MaxPlayers = this.maxPlayerPerRoom,

        };
        PhotonNetwork.CreateRoom(
            "Room test",
            roomOptions);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
        controlPanel.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }
}
