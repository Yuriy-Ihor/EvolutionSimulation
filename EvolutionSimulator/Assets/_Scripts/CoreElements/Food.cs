using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public bool isEaten;
    public EvolutionProccessManager evolutionProccessManager;

    void Start()
    {
        evolutionProccessManager = EvolutionProccessManager.GetInstance;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Mouse") && !isEaten)
        {
            OnIsEaten();
        }
    }

    private void OnIsEaten()
    {
        isEaten = true;
        evolutionProccessManager.foods.Remove(this);
        Destroy(gameObject);
    }
}
