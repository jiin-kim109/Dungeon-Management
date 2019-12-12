using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "GameDataObject/New Curse")]
public class CurseForm : ScriptableObject {

    public string id;
    public Sprite sprite;
    public Grade grade;
    public bool isFloating = false;
    public float floating_Offset_y;

    [Space(8)]
    public float curseApplyRate;
    public float speedSetRate;
    public float damageSetRate;

    [Space(8)]
    public string spellName;
    public float spell_offset_x;
    public float spell_offset_y;
}
