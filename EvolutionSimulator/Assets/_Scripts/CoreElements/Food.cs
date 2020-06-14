using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Food : MonoBehaviour
{
    public bool isEaten;
    public EvolutionProccessManager evolutionProccessManager;

    void Start()
    {
        evolutionProccessManager = EvolutionProccessManager.GetInstance;
        onIsEatenEvent += OnIsEaten;
    }

    void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnIsEaten()
    {
        isEaten = true;
        evolutionProccessManager.foods.Remove(this);
        Destroy(gameObject);
    }

    public UnityAction onIsEatenEvent;
}
