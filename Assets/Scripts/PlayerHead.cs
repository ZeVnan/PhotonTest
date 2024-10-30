using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    public event Action<int> FoodEaten;
    private PhotonView parentPhotonView;
    private void Start()
    {
        parentPhotonView = this.transform.parent.gameObject.GetComponent<PhotonView>();
        PlayerManager parent = this.transform.parent.GetComponent<PlayerManager>();
        FoodEaten += parent.OnFoodEaten;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!parentPhotonView.IsMine)
        {
            return;
        }
        switch (other.gameObject.name)
        {
            case "Tail":
                OnCollisionWithTail(other);
                break;
            case "Food(Clone)":
                OnCollisionWithFood(other);
                break;
        }
    }
    private void OnCollisionWithTail(Collider other)
    {
        PhotonView otherPhotonView = other.transform.parent.gameObject.GetComponent<PhotonView>();
        if (otherPhotonView != parentPhotonView)
        {
            GameNetworkingManager.Instance.LeaveRoom();
        }
    }
    private void OnCollisionWithFood(Collider other)
    {
        PhotonView otherPhotonView = other.gameObject.GetComponent<PhotonView>();
        if (otherPhotonView != parentPhotonView)
        {
            GamePlayManager.Instance.OnFoodEaten();
            FoodEaten.Invoke(1);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(other.gameObject);
            }
            else
            {
                int viewId = other.gameObject.GetComponent<PhotonView>().ViewID;
                PhotonView.Get(this).RPC(
                    "DestroyFoodRequest",
                    RpcTarget.MasterClient,
                    viewId);
            }
        }
    }
}
