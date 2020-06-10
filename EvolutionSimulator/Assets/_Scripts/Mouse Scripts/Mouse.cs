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
    public float distanceToSpawn;
    public List<Mouse> mousesTeammates = new List<Mouse>();
    public static List<Mouse> allMouses = new List<Mouse>();

    [SerializeField]
    bool _isStayingOnSpawn;
    public bool isStayingOnSpawn
    { 
        get
        {
            return distanceToSpawn < 1;
        }
    }

    void Start()
    {
        spawnPoint = transform.position;
        evolutionProccessManager = EvolutionProccessManager.GetInstance;
        navmeshAgent = GetComponent<NavMeshAgent>();
    }
    
    public void Init(GameObject mousesRoot, int teamId, int nameId)
    {
        transform.parent = mousesRoot.transform;
        this.teamId = teamId;
        gameObject.name = "Mouse " + teamId + ":" + nameId;
        allMouses.Add(this);
    }

    public void UpdateTeammateList()
    {
        mousesTeammates = allMouses.FindAll(x => x.teamId == this.teamId);
    }

    void Update()
    {
        _isStayingOnSpawn = isStayingOnSpawn;
        distanceToSpawn = (transform.position - spawnPoint).magnitude;

        if (!evolutionProccessManager.turnStarted)
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
        float distanceToClosest = Mathf.Infinity;

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
                Debug.Log(gameObject.name + " has eatten food");
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
