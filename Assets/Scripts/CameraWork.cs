using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWork : MonoBehaviour
{
    [SerializeField]
    private float distance = 7.0f;
    [SerializeField]
    private float height = 5.0f;
    [SerializeField]
    private Vector3 centerOffset = new Vector3(0, 0, 0);
    [SerializeField]
    private bool followOnStart = false;
    [Range(5f, 15f)]
    [SerializeField]
    private float smoothSpeed = 10f;

    private Transform cameraTransform { get; set; }
    private bool isFollowing { get; set; }
    private Vector3 cameraOffset = Vector3.zero;

    void Start()
    {
        // Start following the target if wanted.
        if (followOnStart)
        {
            OnStartFollowing();
        }
    }
    void LateUpdate()
    {
        if (cameraTransform == null && isFollowing)
        {
            OnStartFollowing();
        }

        if (isFollowing)
        {
            Follow();
        }
    }
    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        Cut();
    }
    void Follow()
    {
        cameraOffset.z = distance;
        cameraOffset.y = height;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);
        cameraTransform.LookAt(this.transform.position + centerOffset);

    }
    void Cut()
    {
        cameraOffset.z = distance;
        cameraOffset.y = height;
        cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);
        cameraTransform.LookAt(this.transform.position + centerOffset);
    }
}
