using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using System;

public class GamePlayManager : MonoBehaviour
{
    [Range(5f, 20f)]
    [SerializeField]
    private int foodPerSpawn = 10;
    [Range(2f, 6f)]
    [SerializeField]
    private float timeBetweenSpawn = 4f;
    [Range(50f, 150f)]
    [SerializeField]
    private int foodLimit = 50;
    [SerializeField]
    private Transform floorTransform;
    [SerializeField]
    private GameObject foodPrefab;


    public static GamePlayManager Instance;
    private float timeLeft { get; set; }
    private int foodCount { get; set; }
    private float maxX { get; set; }
    private float minX { get; set; }
    private float maxZ { get; set; }
    private float minZ { get; set; }

    private void Start()
    {
        timeLeft = timeBetweenSpawn;
        foodCount = 0;
        maxX = (floorTransform.localScale.x - 2) / 2;
        minX = -(floorTransform.localScale.x - 2) / 2;
        maxZ = (floorTransform.localScale.z - 2) / 2;
        minZ = -(floorTransform.localScale.z - 2) / 2;
        Instance = this;
    }
    private void Update()
    {
        if(foodCount - foodPerSpawn > foodLimit)
        {
            return;
        }
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
        }
        else
        {
            SpawnFood();
            timeLeft = timeBetweenSpawn;
        }
    }
    private void SpawnFood()
    {
        if (foodPrefab == null)
        {
            Debug.LogError("Missing food prefab");
            return;
        }
        System.Random random = new();
        foreach(int i in Enumerable.Range(0, foodPerSpawn))
        {
            float randX = (float)(minX + random.NextDouble() * (maxX - minX));
            float randZ = (float)(minZ + random.NextDouble() * (maxZ - minZ));
            PhotonNetwork.InstantiateRoomObject(
                foodPrefab.name,
                new Vector3(randX, 1, randZ),
                Quaternion.identity);
            foodCount++;
        }
    }
    public void OnFoodEaten()
    {
        foodCount--;
    }
}
