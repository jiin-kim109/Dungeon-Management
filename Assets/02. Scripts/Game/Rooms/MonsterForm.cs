using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "GameDataObject/New Monster")]
public class MonsterForm : ScriptableObject {

    public string id;
    public Sprite sprite;
    public Grade grade;

    [Space(8)]
    public float hp;
    public float damage;
}

public enum Grade
{
    E = 0,
    D = 1,
    C = 2,
    B = 3,
    A = 4,
    S = 5,
    U = 6,
    L = 7,
    X = 8,
}
