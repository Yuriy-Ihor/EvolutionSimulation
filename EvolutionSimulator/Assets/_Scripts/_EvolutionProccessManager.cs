using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class _EvolutionProccessManager : MonoBehaviour
{
    public GameConfig gameConfig;
    public int currentTurn = 0;

    public List<GameObject> mouse1spawnPoses;
    public List<GameObject> mouse2spawnPoses;

    public GameObject root;
    
    void Start()
    {
        SpawnMouses(gameConfig.mouse1prefab, gameConfig.mouse1count, mouse1spawnPoses);
        SpawnMouses(gameConfig.mouse2prefab, gameConfig.mouse2count, mouse2spawnPoses);
    }

    void SpawnMouses(Mouse mousePrefab, int count, List<GameObject> spawnPoses)
    {
        for(int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = spawnPoses[i].transform.position;
            var newMouse = Instantiate(mousePrefab, spawnPosition, Quaternion.identity);  
            newMouse.transform.parent = root.transform;
        }
    }

}
