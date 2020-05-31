using System;
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
    public int teamId;
    public List<Mouse> mousesTeammates = new List<Mouse>();
    public static List<Mouse> allMouses = new List<Mouse>();

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
        allMouses.Add(this);
    }
    
    public void Init(GameObject mousesRoot, int teamId, ref List<Mouse> mousesTeammates, int nameId)
    {
        transform.parent = mousesRoot.transform;
        this.teamId = teamId;
        this.mousesTeammates = mousesTeammates;
        gameObject.name = "Mouse " + teamId + ":" + nameId;
    }

    public void UpdateTeammateList()
    {
        mousesTeammates = allMouses.FindAll(x => x.teamId == this.teamId);
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
            navmeshAgent.SetDestination(spawnPoint);
        }
        else
        {
            navmeshAgent.SetDestination(targetFood.transform.position);
        }
    }

    Food GetClosestFood()
    {
        Food closestFood = null;
        float distanceToClosest = 10000;

        if(evolutionProccessManager.foods.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < evolutionProccessManager.foods.Count; i++)
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

    private void OnTriggerEnter(Collider other)
    {
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

    public void OnDestroy()
    {
        mousesTeammates.Remove(this);
        allMouses.Remove(this);
    }
}
