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

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colliding with " + collision.gameObject.name);
        OnIsEaten();
    }

    private void OnIsEaten()
    {
        isEaten = true;
        evolutionProccessManager.eatenFoods++;
        GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
