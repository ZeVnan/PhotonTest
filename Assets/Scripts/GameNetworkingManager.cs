using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameNetworkingManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject playerPrefab;

    public static GameNetworkingManager Instance;

    private void Start()
    {
        Instance = this;
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Lobby");
            return;
        }
        if (playerPrefab == null)
        {
            Debug.LogError("Missing player prefab");
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.Log("Instantiate player");
                PhotonNetwork.Instantiate(
                    this.playerPrefab.name,
                    new Vector3(0f, 1f, 0f),
                    Quaternion.identity,
                    0);
            }
            else
            {
                Debug.Log("No instantiate player");
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public override void OnJoinedRoom()
    {
        if (PlayerManager.LocalPlayerInstance == null)
        {
            PhotonNetwork.Instantiate(
                this.playerPrefab.name,
                new Vector3(0f, 1f, 0f),
                Quaternion.identity,
                0);
        }
    }
    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{

    //}
    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{

    //}
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.Destroy(PlayerManager.LocalPlayerInstance);
        PlayerManager.LocalPlayerInstance = null;
        PhotonNetwork.LeaveRoom();
    }
}
