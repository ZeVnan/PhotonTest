using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 45f, 0f);
    [SerializeField]
    private Text playerNameText;
    [SerializeField]
    private Text playerScoreText;

    private PlayerManager target;
    private Transform targetTransform;
    private Renderer targetRenderer;
    private CanvasGroup _canvasGroup;
    private Vector3 targetPosition;

    void Awake()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }
    void LateUpdate()
    {
        if (targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += 1.0f;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }
    public void SetTarget(PlayerManager _target)
    {

        if (_target == null)
        {
            Debug.LogError("Missing target");
            return;
        }
        this.target = _target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponentInChildren<Renderer>();
        this.target.ScoreUpdated += OnScoreUpdated;
        if (playerNameText != null)
        {
            playerNameText.text = $"Name: {this.target.photonView.Owner.NickName}";
        }
        if (playerScoreText != null)
        {
            playerScoreText.text = $"Score: 0";
        }
    }
    private void OnScoreUpdated(int score)
    {
        playerScoreText.text = $"Score: {score}";
    }
}
