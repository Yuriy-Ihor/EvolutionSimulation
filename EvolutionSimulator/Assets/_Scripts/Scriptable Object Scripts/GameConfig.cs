using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_GameConfig", menuName = "Data/GameConfig")]
public class GameConfig : ScriptableObject
{
    public Mouse mouse1prefab;
    public Mouse mouse2prefab;

    public int mouse1count = 10;
    public int mouse2count = 10;
}
