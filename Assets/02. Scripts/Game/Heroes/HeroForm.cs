using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "GameDataObject/New Hero")]
public class HeroForm : ScriptableObject {

    public string id;

    public float hp;
    public float damage;
    public float moveSpeed;

    [Space(8)]
    public float goldGivenMult = 1f;

    [Space(8)]
    public Sprite sprite;
    public Sprite sprite_hit;
    public Sprite sprite_dead;
}
