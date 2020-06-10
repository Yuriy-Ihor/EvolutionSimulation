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
    public List<Mouse> mouses1;

    [Header("Team 2")]
    public int team_2_id = 2;
    public List<GameObject> mouse2spawnPoses;
    public List<Mouse> mouses2;

    [Header("Turn info")]
    public int currentTurn = 0;
    public int foodSpawnRadius = 10;
    public bool turnStarted = false;

    [SerializeField]
    bool _allFoodIsEaten;
    public bool allFoodIsEaten
    { 
        get
        {
            return foods.Count == 0;
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
        mouses1 = CreateMousesList(gameConfig.mouse1prefab, gameConfig.mouse1count, mouse1spawnPoses, team_1_id);
        mouses2 = CreateMousesList(gameConfig.mouse2prefab, gameConfig.mouse2count, mouse2spawnPoses, team_2_id);

        UpdateMousesLists();

        timeScaleInput.text = "1";
        newTurnButton.onClick.AddListener(StartTurn);
        resetButton.onClick.AddListener(Reset);
        timeScaleInput.onValueChanged.AddListener(delegate {Time.timeScale = int.Parse(timeScaleInput.text); });
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateMousesLists()
    {
        mouses1 = Mouse.allMouses.FindAll(x => x.teamId == team_1_id);
        mouses2 = Mouse.allMouses.FindAll(x => x.teamId == team_2_id);
    }

    private void Update()
    {
        _allFoodIsEaten = allFoodIsEaten;

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

        foreach (Mouse mouse in Mouse.allMouses)
        {
            mouse.UpdateTeammateList();
        }

        UpdateMousesLists();

        turnStarted = false;
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
        currentTurnText.text = currentTurn.ToString();
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

    List<Mouse> CreateMousesList(Mouse mousePrefab, int count, List<GameObject> spawnPoses, int teamId)
    {
        List<Mouse> mouses = new List<Mouse>();
        for(int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = spawnPoses[i].transform.position;
            Mouse newMouse = Instantiate(mousePrefab, spawnPosition, Quaternion.identity);
            newMouse.Init(mousesRoot, teamId, i);
        }
        return mouses;
    }

    void CheckMousesGatheredFood()
    {
        List<Mouse> mouses = new List<Mouse>(Mouse.allMouses);

        for(int i = 0; i < mouses.Count; i++)
        {
            Mouse mouse = mouses[i];
            if(mouse == null)
            {
                mouses.Remove(mouse);
                continue;
            }
            switch (mouse.foodGathered)
            {
                case 0:
                case 1:
                    mouses.Remove(mouse);
                    Destroy(mouse.gameObject);
                    i--;
                    break;
                case 2:
                    mouse.foodGathered = 0;
                    break;
                default:
                    int reproduceCount = mouse.foodGathered - 2;
                    mouse.foodGathered = 0;
                    Debug.Log("Mouse " + mouse.gameObject.name + " will reproduce " + reproduceCount);
                    ReproduceMouse(mouse, reproduceCount);
                    break;
            }
        }
    }
    public void ReproduceMouse(Mouse toCopy, int reproduceCount)
    {
        for (int i = 0; i < reproduceCount; i++)
        {
            Mouse reproductedMouse = Instantiate(toCopy);
            Vector3 spawnPosition = new Vector3((UnityEngine.Random.insideUnitCircle * 1).x, 0, (UnityEngine.Random.insideUnitCircle * 1).y) + toCopy.spawnPoint;
            reproductedMouse.transform.position = spawnPosition;
            reproductedMouse.Init(mousesRoot, toCopy.teamId, i);
        }
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
}
