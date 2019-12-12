using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "GameDataObject/New Trap")]
public class TrapForm : ScriptableObject {

    public string id;
    public Sprite sprite;
    public Grade grade;
    public bool isFloating = false;
    public float floating_Offset_y;

    [Space(8)]
    public float damage;
    public int hitCount = 1;

    [Space(8)]
    public string spellName;
    public float spell_offset_x = 0;
    public float spell_offset_y = 0;
}
