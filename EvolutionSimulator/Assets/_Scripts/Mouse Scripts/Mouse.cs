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
    public static List<Mouse> allMouses = new List<Mouse>();
    public int generation = 0;

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

    public void Init(GameObject mousesRoot, int teamId, int nameId, int generation)
    {
        transform.parent = mousesRoot.transform;
        this.teamId = teamId;
        this.generation = generation;
        gameObject.name = "Mouse " + this.teamId + ":" + nameId + "; " + this.generation;
        stop = false;
        allMouses.Add(this);
    }

    public bool stop;

    void Update()
    {
        if(stop)
        {
            return;
        }

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

    public void ReproduceSelf(int id = 0)
    {
        Mouse reproductedMouse = Instantiate(this);
        Vector3 spawnPosition = new Vector3((UnityEngine.Random.insideUnitCircle * 1).x, 0, (UnityEngine.Random.insideUnitCircle * 1).y) + this.spawnPoint;
        reproductedMouse.transform.position = spawnPosition;
        reproductedMouse.Init(evolutionProccessManager.mousesRoot, this.teamId, id, this.generation + 1);
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

            if (!food.isEaten)
            {
                Debug.Log("MOUSE: " + gameObject.name + " has eatten " + food.gameObject.name + "; time = " + Time.time);
                foodGathered++; 
            }
        }
    }

    /*
    public void DestroySelf()
    {
        allMouses.Remove(this);
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        allMouses.Remove(this);
    }*/
}
