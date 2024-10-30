using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public event Action<int> ScoreUpdated;
    [SerializeField]
    private GameObject playerUiPrefab;
    [Range(50f, 300f)]
    [SerializeField]
    private float rotationSpeed = 100f;
    [Range(100f, 200f)]
    [SerializeField]
    private float accelerate = 150f;

    public static GameObject LocalPlayerInstance = null;

    private Vector3 direction { get; set; } = new Vector3(0, 0, -1);
    private Vector3 velocity { get; set; } = Vector3.zero;
    private float velocityScale { get; set; } = 8.0f;
    private float velocityScaleMax { get; } = 20.0f;
    private float velocityScaleMin { get; } = 6.0f;
    private Rigidbody rigidBody { get; set; }
    private PlayerUI playerUI { get; set; }
    private bool isLeavingRoom { get; set; }
    private int score { get; set; }
    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        this.transform.rotation = Quaternion.Euler(0, 0, -1);
        this.rigidBody = this.gameObject.GetComponent<Rigidbody>();
        CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();
        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("Missing CameraWork component");
        }
        if (this.playerUiPrefab != null)
        {
            playerUI = Instantiate(this.playerUiPrefab).GetComponent<PlayerUI>();
            this.playerUI.SetTarget(this);
        }
        else
        {
            Debug.LogError("Missing PlayerUI prefab");
        }
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInput();
            Move();
        }
    }
    private void ProcessInput()
    {
        if (Input.GetAxis("Vertical") > 0)
        {
            Accelerate(true);
        }
        else
        {
            Accelerate(false);
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            Rotate(true);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            Rotate(false);
        }
    }
    private void Move()
    {
        velocity = (velocity.normalized + direction.normalized) * velocityScale;
        rigidBody.velocity = velocity;
    }
    private void Accelerate(bool up)
    {
        if (up == true)
        {
            velocityScale += accelerate * Time.deltaTime;
            if (velocityScale > velocityScaleMax)
            {
                velocityScale = velocityScaleMax;
            }
        }
        else
        {
            velocityScale -= accelerate * Time.deltaTime;
            if (velocityScale < velocityScaleMin)
            {
                velocityScale = velocityScaleMin;
            }
        }
    }
    private void Rotate(bool clockwise)
    {
        float angle = rotationSpeed * Time.deltaTime;
        if (clockwise is true)
        {
            this.direction = Quaternion.AngleAxis(angle, Vector3.up) * this.direction;
            this.transform.Rotate(0, angle, 0);
        }
        else
        {
            this.direction = Quaternion.AngleAxis(-angle, Vector3.up) * this.direction;
            this.transform.Rotate(0,- angle, 0);
        }
    }
    public void OnFoodEaten(int givenScore)
    {
        this.score += givenScore;
        ScoreUpdated.Invoke(score);
    }
    public override void OnLeftRoom()
    {
        this.isLeavingRoom = false;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(score);
        }
        else
        {
            score = (int)stream.ReceiveNext();
            ScoreUpdated.Invoke(score);
        }
    }
    [PunRPC]
    public void DestroyFoodRequest(int viewId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView foodPhotonView = PhotonView.Find(viewId);
            if (foodPhotonView != null)
            {
                PhotonNetwork.Destroy(foodPhotonView.gameObject);
            }
        }
    }
}
