using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;

public class EvolutionProccessManager : UnitySingleton<EvolutionProccessManager>
{
    public GameConfig gameConfig;

    public List<GameObject> mouse1spawnPoses;
    public List<GameObject> mouse2spawnPoses;

    public List<Mouse> mouses1;
    public List<Mouse> mouses2;
    public GameObject mousesRoot;

    public List<Food> foods = new List<Food>();
    public GameObject foodRoot;

    [Header("Turn info")]
    public int currentTurn = 0;
    public int foodSpawnRadius = 10;
    int _eatenFoods;
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
                allFoodIsEaten = true;
            }
        }
    }
    public bool autoTurn { get { return autoNewTurn.isOn; } }
    public bool allFoodIsEaten;
    public bool turnStarted = false;

    public Toggle autoNewTurn;
    public Button newTurnButton;
    public InputField timeScaleInput;

    void Start()
    {
        mouses1 = CreateMousesList(gameConfig.mouse1prefab, gameConfig.mouse1count, mouse1spawnPoses, 1);
        mouses2 = CreateMousesList(gameConfig.mouse2prefab, gameConfig.mouse2count, mouse2spawnPoses, 2);

        OnEndTurn += CheckMousesGatheredFood;
        OnEndTurn += EndTurn;

        timeScaleInput.text = "1";

        newTurnButton.onClick.AddListener(StartTurn);
        timeScaleInput.onValueChanged.AddListener(delegate {Time.timeScale = int.Parse(timeScaleInput.text); });
    }

    private void Update()
    {
        if(turnStarted)
        {
            if(allFoodIsEaten && AreMousesOnSpawn)
            {
                OnEndTurn.Invoke();
            }
        }
    }

    void EndTurn()
    {
        allFoodIsEaten = false;
        eatenFoods = 0;
        DestroyFoods();

        if (autoTurn)
        {
            StartTurn();
        }
    }

    void StartTurn()
    {
        if(!AreMousesOnSpawn)
        {
            Debug.Log("Mouses are not on spawn! Please wait");
            return;
        }

        turnStarted = true;
        currentTurn++;

        SpawnFood();
    }

    void DestroyFoods()
    {
        foreach (Food food in foods)
        {
            Destroy(food.gameObject);
        }

        foods.Clear();
    }

    public bool AreMousesOnSpawn
    {
        get
        {
            foreach (Mouse mouse in mouses1)
            {
                if (!mouse.isStayingOnSpawn)
                {
                    return false;
                }
            }
            foreach (Mouse mouse in mouses2)
            {
                if (!mouse.isStayingOnSpawn)
                {
                    return false;
                }
            }

            return true;
        }
    }

    List<Mouse> CreateMousesList(Mouse mousePrefab, int count, List<GameObject> spawnPoses, int teamId = 0)
    {
        List<Mouse> mouses = new List<Mouse>();
        for(int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = spawnPoses[i].transform.position;
            var newMouse = Instantiate(mousePrefab, spawnPosition, Quaternion.identity);  
            newMouse.transform.parent = mousesRoot.transform;
            newMouse.teamId = teamId;
            mouses.Add(newMouse);
        }
        return mouses;
    }

    void CheckMousesGatheredFood()
    {
        Debug.Log("Checking mouses");
    }

    void SpawnFood()
    {
        for(int i = 0; i < gameConfig.foodCount; i++)
        {
            Vector2 spawnPosition = UnityEngine.Random.insideUnitCircle * foodSpawnRadius;
            Food newFood = Instantiate(gameConfig.foodPrefab, new Vector3(spawnPosition.x, 0, spawnPosition.y), Quaternion.identity);
            newFood.transform.parent = foodRoot.transform;
            foods.Add(newFood);
        }
    }

    public UnityAction OnEndTurn;
}
