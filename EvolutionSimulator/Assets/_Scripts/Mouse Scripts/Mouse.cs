﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mouse : MonoBehaviour
{
    public NavMeshAgent navmeshAgent;
    public Food targetFood;
    public EvolutionProccessManager evolutionProccessManager;
    public Vector3 spawnPoint;
    public int foodGathered = 0;

    public bool isStayingOnSpawn
    { 
        get
        {
            return (transform.position - spawnPoint).magnitude < 1;
        }
    }


    void Start()
    {
        spawnPoint = transform.position;
        evolutionProccessManager = EvolutionProccessManager.GetInstance;
        navmeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(!evolutionProccessManager.turnStarted)
        {
            return;
        }

        targetFood = GetClosestFood();

        if (targetFood == null || evolutionProccessManager.allFoodIsEaten)
        {
            ReturnToTheSpawn();
        }
        else
        {
            GoToTheTargetFood();
        }
    }

    Food GetClosestFood()
    {
        Food closestFood = null;
        float distanceToClosest = 10000;

        for (int i = 0; i < evolutionProccessManager.gameConfig.foodCount; i++)
        {
            Food currentFood = evolutionProccessManager.foods[i];
            if (currentFood.isEaten)
            {
                continue;
            }

            float distanceToCurrent = (currentFood.transform.position - this.transform.position).magnitude;

            if(distanceToCurrent < distanceToClosest)
            {
                distanceToClosest = distanceToCurrent;
                closestFood = currentFood;
            }
        }

        return closestFood;
    }

    void ReturnToTheSpawn()
    {
        navmeshAgent.SetDestination(spawnPoint);
    }

    void GoToTheTargetFood()
    {
        navmeshAgent.SetDestination(targetFood.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colliding");
        if(other.gameObject.CompareTag("Food"))
        {
            Food food = other.gameObject.GetComponent<Food>();

            if(food == null)
            {
                Debug.LogError("Lol, It has tag, but no component");
                return;
            }

            if(!food.isEaten)
            {
                foodGathered++;
            }
        }
    }
}
