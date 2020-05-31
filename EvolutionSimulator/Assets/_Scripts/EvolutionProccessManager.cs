using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class EvolutionProccessManager : UnitySingleton<EvolutionProccessManager>
{
    public GameConfig gameConfig;
    public bool autoTurn { get { return autoNewTurn.isOn; } }

    public List<GameObject> mouse1spawnPoses;
    public List<GameObject> mouse2spawnPoses;

    public int team_1_id = 1;
    public int team_2_id = 2;

    public List<Mouse> mouses1;
    public List<Mouse> mouses2;
    public GameObject mousesRoot;

    public List<Food> foods = new List<Food>();
    public GameObject foodRoot;

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

    [Header("UI elements")]
    public Text currentTurnText;
    public Toggle autoNewTurn;
    public Button newTurnButton;
    public InputField timeScaleInput;

    void Start()
    {
        mouses1 = CreateMousesList(gameConfig.mouse1prefab, gameConfig.mouse1count, mouse1spawnPoses, team_1_id);
        mouses2 = CreateMousesList(gameConfig.mouse2prefab, gameConfig.mouse2count, mouse2spawnPoses, team_2_id);
        UpdateMousesLists();

        timeScaleInput.text = "1";

        newTurnButton.onClick.AddListener(StartTurn);
        timeScaleInput.onValueChanged.AddListener(delegate {Time.timeScale = int.Parse(timeScaleInput.text); });
    }

    void UpdateMousesLists()
    {
        mouses1 = Mouse.allMouses.FindAll(x => x.teamId == team_1_id);
        mouses2 = Mouse.allMouses.FindAll(x => x.teamId == team_2_id);
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
        CheckMousesGatheredFood(ref mouses1);
        CheckMousesGatheredFood(ref mouses2);

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
            newMouse.Init(mousesRoot, teamId, ref mouses, i);
        }
        return mouses;
    }

    void CheckMousesGatheredFood(ref List<Mouse> mouses)
    {
        List<Mouse> newMouses = new List<Mouse>();

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
                    break;
                default:
                    ReproduceMouse(mouse);
                    break;
            }
        }
    }
    public void ReproduceMouse(Mouse toCopy)
    {
        for (int i = 0; i < toCopy.foodGathered - 2; i++)
        {
            Mouse reproductedMouse = Instantiate(toCopy);
            Vector3 spawnPosition = new Vector3((UnityEngine.Random.insideUnitCircle * 2).x, 0, (UnityEngine.Random.insideUnitCircle * 2).y) + toCopy.transform.position;
            reproductedMouse.transform.position = spawnPosition;
            reproductedMouse.Init(mousesRoot, toCopy.teamId, ref toCopy.mousesTeammates, i);
        }
        toCopy.foodGathered = 0;
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
