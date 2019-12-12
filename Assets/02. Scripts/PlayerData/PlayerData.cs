using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData {

    public bool isNewBegin = true;
    public int restartCount = 0;
    public int cashStock = 0;
    public bool cheat_isAllowed = false;
    public bool cheat_isActive = false;

    [Header("[  Save Data")]
    [Space(8)]
    [SerializeField]
    private int m_score = 0;
    public int score
    {
        get
        {
            return m_score;
        }
        set
        {
            m_score = value;
            UIHandler.Instance.UpdateUI(this);
        }
    }
    [Space(6)]
    [SerializeField]
    private int m_gold = 0;
    public int gold
    {
        get
        {
            return m_gold;
        }
        set
        {
            if(!cheat_isActive)
                m_gold = value;
            UIHandler.Instance.UpdateUI();
        }
    }
    [SerializeField]
    private int m_gem = 0;
    public int gem
    {
        get
        {
            return m_gem;
        }
        set
        {
            if (!cheat_isActive)
                m_gem = value;
            UIHandler.Instance.UpdateUI();
            GameManager.Instance.SaveGame();
        }
    }
    [Space(6)]
    public int playerAttackUpgradeLevel = 0;

    [Space(10)]
    public List<string> floorData = new List<string>();
    [Space(6)]
    public int floorBossCount = 0;
    [Space(6)]
    public List<string> monsterData = new List<string>();
    public int monsterUpgradeCount = 0;

    public List<string> trapData = new List<string>();
    public int trapUpgradeCount = 0;

    public List<string> curseData = new List<string>();
    public int curseUpgradeCount = 0;
    [Space(4)]
    public List<string> bossData = new List<string>();
    public int bossUpgradeCount = 0;
    public string bossUpgrade;

    //===
    [Space(10)]
    public int dungeonUpgradeLevel_monster = 0;
    public int dungeonUpgradeLevel_trap = 0;
    public int dungeonUpgradeLevel_curse = 0;
    public int dungeonUpgradeLevel_boss = 0;
    public List<string> dungeonUpgradeData = new List<string>();

    [Space(4)]
    public int bossPurchaseCount = 0;

    [Space(4)]
    public int goldGivenUpgradeCount = 0;
    public int rankUpUpgradeCount = 0;
}
