using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;

public class EvolutionProccessManager : UnitySingleton<EvolutionProccessManager>
{
    public GameConfig gameConfig;
    public int currentTurn = 0;

    public List<GameObject> mouse1spawnPoses;
    public List<GameObject> mouse2spawnPoses;
    public GameObject mouseRoot;
    public List<Mouse> mouses1;
    public List<Mouse> mouses2;

    public List<Food> foods = new List<Food>();
    public GameObject foodRoot;
    public int eatenFoods
    {
        get
        {
            return _eatenFoods;
        }
        set
        {
            _eatenFoods = value;
            
            if(_eatenFoods == gameConfig.foodCount)
            {
                turnStarted = false;
                Debug.Log("Turn finished");
                //OnFoodEaten.Invoke();
            }
        }
    }
    int _eatenFoods;
    public bool allFoodIsEaten;

    public Button newTurnButton;
    public bool turnStarted = false;
    
    void Start()
    {
        SpawnMouses(gameConfig.mouse1prefab, gameConfig.mouse1count, mouse1spawnPoses);
        SpawnMouses(gameConfig.mouse2prefab, gameConfig.mouse2count, mouse2spawnPoses);
        newTurnButton.onClick.AddListener(StartTurn);

        //OnFoodEaten += StartTurn;
    }

    void StartTurn()
    {
        turnStarted = true;
        currentTurn++;
        allFoodIsEaten = false;

        foreach(Food food in foods)
        {
            Destroy(food.gameObject);
        }
        foods.Clear();
        SpawnFood();
    }

    void ReturnMousesToStartPositions()
    {
        
    }

    void SpawnMouses(Mouse mousePrefab, int count, List<GameObject> spawnPoses)
    {
        for(int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = spawnPoses[i].transform.position;
            var newMouse = Instantiate(mousePrefab, spawnPosition, Quaternion.identity);  
            newMouse.transform.parent = mouseRoot.transform;
        }
    }

    void SpawnFood()
    {
        for(int i = 0; i < gameConfig.foodCount; i++)
        {
            Vector2 spawnPosition = UnityEngine.Random.insideUnitCircle * 5;
            Food newFood = Instantiate(gameConfig.foodPrefab, new Vector3(spawnPosition.x, 0, spawnPosition.y), Quaternion.identity);
            newFood.transform.parent = foodRoot.transform;
            foods.Add(newFood);
        }
    }

    public delegate void OnFoodEatenDelegate();
    public OnFoodEatenDelegate OnFoodEaten;
}
