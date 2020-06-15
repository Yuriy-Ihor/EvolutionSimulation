using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class EvolutionProccessManager : UnitySingleton<EvolutionProccessManager>
{
    public GameConfig gameConfig;
   
    public GameObject mousesRoot;
    public GameObject foodRoot;

    public List<Food> foods = new List<Food>();
    public bool autoTurn { get { return autoNewTurn.isOn; } }

    [Header("Team 1")]
    public int team_1_id = 1;
    public List<GameObject> mouse1spawnPoses;

    [Header("Team 2")]
    public int team_2_id = 2;
    public List<GameObject> mouse2spawnPoses;

    [Header("Turn info")]
    public int currentTurn = 0;
    public int foodSpawnRadius = 10;
    public bool turnStarted = false;

    public bool allFoodIsEaten
    { 
        get
        {
            return foods.Count == 0;
        }
    }
    public bool AreMousesOnSpawn
    {
        get
        {
            foreach (Mouse mouse in Mouse.allMouses)
            {
                if (!mouse.isStayingOnSpawn)
                {
                    return false;
                }
            }

            return true;
        }
    }

    [Header("UI elements")]
    public Text currentTurnText;
    public Toggle autoNewTurn;
    public Button newTurnButton;
    public Button resetButton;
    public InputField timeScaleInput;

    void Start()
    {
        CreateMousesList(gameConfig.mouse1prefab, gameConfig.mouse1count, mouse1spawnPoses, team_1_id);
        CreateMousesList(gameConfig.mouse2prefab, gameConfig.mouse2count, mouse2spawnPoses, team_2_id);

        timeScaleInput.text = "1";
        newTurnButton.onClick.AddListener(StartTurn);
        resetButton.onClick.AddListener(Reset);
        timeScaleInput.onValueChanged.AddListener(delegate {Time.timeScale = int.Parse(timeScaleInput.text); });
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        if(turnStarted)
        {
            if(allFoodIsEaten && AreMousesOnSpawn)
            {
                EndTurn();
            }
        }
    }

    void EndTurn()
    {
        CheckMousesGatheredFood();
        turnStarted = false;

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
        currentTurnText.text = currentTurn.ToString();
    }

    void CreateMousesList(Mouse mousePrefab, int count, List<GameObject> spawnPoses, int teamId)
    {
        for(int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = spawnPoses[i].transform.position;
            Mouse newMouse = Instantiate(mousePrefab, spawnPosition, Quaternion.identity);
            newMouse.Init(mousesRoot, teamId, i, 0);
        }
    }

    void CheckMousesGatheredFood()
    {
        List<Mouse> mouses = new List<Mouse>(Mouse.allMouses);

        for (int i = 0; i < mouses.Count; i++)
        {
            Mouse mouse = mouses[i];
            if(mouse == null)
            {
                continue;
            }

            switch (mouse.foodGathered)
            {
                case 0:
                case 1:
                    mouse.stop = true;
                    break;
                case 2:
                    break;
                default:
                    int reproduceCount = mouse.foodGathered - 2;
                    mouse.foodGathered = 0;
                    Debug.Log("Mouse " + mouse.gameObject.name + " will reproduce " + reproduceCount);
                    for(int j = 0; j < reproduceCount; j++)
                    {
                        mouse.ReproduceSelf(j);
                    }
                    break;
            }

            mouse.foodGathered = 0;
        }
    }
   

    void SpawnFood()
    {
        for(int i = 0; i < gameConfig.foodCount; i++)
        {
            Vector2 spawnPosition = UnityEngine.Random.insideUnitCircle * foodSpawnRadius;
            Food newFood = Instantiate(gameConfig.foodPrefab, new Vector3(spawnPosition.x, 0, spawnPosition.y), Quaternion.identity);
            newFood.transform.parent = foodRoot.transform;
            newFood.gameObject.name = "Food " + i;
            foods.Add(newFood);
        }
    }
}
