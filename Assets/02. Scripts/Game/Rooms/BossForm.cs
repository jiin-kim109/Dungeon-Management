using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "GameDataObject/New Boss")]
public class BossForm : ScriptableObject {

    public string id;
    public Sprite sprite;

    [Space(8)]
    public float hp;
    public float damage;

    [Space(8)]
    public string spellName;
    public float spell_offset_x;
    public float spell_offset_y;

    [Space(8)]
    public float damageRateToSpell;
    public float spellCooltime;
    public float damageRadiusScale = 1f;
}

