using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DataHandler : MonoBehaviour {

    public static float rankUpExtraRate = 0;
    [SerializeField]
    private int dungeonUpgradeCost;
    public int DungeonUpgradeCost { get { return dungeonUpgradeCost; } }

    [Space(8)]
    [Header("[  몬스터 룸")]
    [SerializeField]
    private Monster m_monsterPrefab;
    public Monster monsterPrefab { get { return m_monsterPrefab; } }
    [SerializeField]
    private List<MonsterForm> monsterForms = new List<MonsterForm>();
    [SerializeField]
    private List<MonsterForm> defaultGivenMonsterForms = new List<MonsterForm>();
    public List<Monster> playerMonsters { get; private set; }
    [Header("[  UI")]
    [Space(6)]
    [SerializeField]
    private Button dungeonUpgradeUI_monster_button;
    [SerializeField]
    private TextMeshProUGUI dungeonUpgradeUI_monster_level;
    [SerializeField]
    private TextMeshProUGUI dungeonUpgradeUI_monster_content;
    [SerializeField]
    private TextMeshProUGUI dungeonUpgradeUI_monster_cost;
    [Header("[  Parameters")]
    [Space(6)]
    [SerializeField]
    private float monster_respawnCooltime_downRateByLevel;
    [SerializeField]
    private float monster_healthRegen_upRateByLevel;

    [Space(10)]
    [Header("[  함정 룸")]
    [SerializeField]
    private Trap m_trapPrefab;
    public Trap trapPrefab { get { return m_trapPrefab; } }
    [SerializeField]
    private List<TrapForm> trapForms = new List<TrapForm>();
    [SerializeField]
    private List<TrapForm> defaultGivenTrapForms = new List<TrapForm>();
    public List<Trap> playerTraps { get; private set; }
    [Header("[  UI")]
    [Space(6)]
    [SerializeField]
    private Button dungeonUpgradeUI_trap_button;
    [SerializeField]
    private TextMeshProUGUI dungeonUpgradeUI_trap_level;
    [SerializeField]
    private TextMeshProUGUI dungeonUpgradeUI_trap_content;
    [SerializeField]
    private TextMeshProUGUI dungeonUpgradeUI_trap_cost;
    [Header("[  Parameters")]
    [Space(6)]
    [SerializeField]
    private float trap_fireCooltime_downRateByLevel;

    [Space(10)]
    [Header("[  저주 룸")]
    [SerializeField]
    private Curse m_cursePrefab;
    public Curse cursePrefab { get { return m_cursePrefab; } }
    [SerializeField]
    private List<CurseForm> curseForms = new List<CurseForm>();
    [SerializeField]
    private List<CurseForm> defaultGivenCurseForms = new List<CurseForm>();
    public List<Curse> playerCurses { get; private set; }
    [Header("[  UI")]
    [Space(6)]
    [SerializeField]
    private Button dungeonUpgradeUI_curse_button;
    [SerializeField]
    private TextMeshProUGUI dungeonUpgradeUI_curse_level;
    [SerializeField]
    private TextMeshProUGUI dungeonUpgradeUI_curse_content;
    [SerializeField]
    private TextMeshProUGUI dungeonUpgradeUI_curse_cost;
    [Header("[  Parameters")]
    [Space(6)]
    [SerializeField]
    private float curse_fireCooltime_downRateByLevel;
    [SerializeField]
    private float curse_duration_upRateByLevel;

    [Space(10)]
    [Header("[  보스 룸")]
    [SerializeField]
    private Boss m_bossPrefab;
    public Boss bossPrefab { get { return m_bossPrefab; } }
    [SerializeField]
    private List<BossForm> bossForms = new List<BossForm>();
    public List<Boss> playerBosses { get; private set; }
    public BossUpgrade bossUpgrade;

    [System.Serializable]
    private struct GradeColor
    {
        public Grade grade;
        public Color color;
    }
    [Space(10)]
    [SerializeField]
    private List<GradeColor> gradeColors = new List<GradeColor>();
    public Color GetGradeColor(Grade grade)
    {
        foreach (GradeColor gradeColor in gradeColors)
        {
            if (gradeColor.grade == grade)
            {
                return gradeColor.color;
            }
        }
        return gradeColors[gradeColors.Count - 1].color;
    }

    [System.Serializable]
    private struct GradePercentage
    {
        public Grade grade;
        public float gradeUpPercent;
    }
    [Space(10)]
    [SerializeField]
    private List<GradePercentage> gradePercentages = new List<GradePercentage>();
    public float GetGradeUpPercentage(Grade currentGrade)
    {
        Grade targetGrade = (Grade)((int)currentGrade + 1);
        foreach (GradePercentage gradePer in gradePercentages)
        {
            if (gradePer.grade == targetGrade)
            {
                return gradePer.gradeUpPercent;
            }
        }
        return 100;
    }

    public delegate void DataUpdated();
    public DataUpdated dataUpdated;

    //---------------Instance------------------
    private static DataHandler _instance = null;
    public static DataHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(DataHandler)) as DataHandler;
                if (_instance == null) { Debug.Log("DataHandler가 없음"); }
            }
            return _instance;
        }
    }
    //-----------------------------------------

    void Awake()
    {
        playerMonsters = new List<Monster>();
        playerTraps = new List<Trap>();
        playerCurses = new List<Curse>();
        playerBosses = new List<Boss>();
    }

    public void Initalize()
    {
        dungeonUpgradeUI_monster_button.onClick.AddListener(delegate { OnClick_DungeonUpgrade(0); });
        dungeonUpgradeUI_trap_button.onClick.AddListener(delegate { OnClick_DungeonUpgrade(1); });
        dungeonUpgradeUI_curse_button.onClick.AddListener(delegate { OnClick_DungeonUpgrade(2); });

        Load();
        UpdateUI();
    }

    //===
    /*
     * (Monster.RespawnCooltime, HealthRegenPercentage)
     * (Trap.FireCooltime)
     * (Curse.FireCooltime, Curse.Duration)
     * */
    public void Save()
    {
        GameManager.Instance.playerData.dungeonUpgradeData = new List<string>();
        string monsterData = "("; //(respawnCooltime, regenPercentage)
        monsterData += Monster.RespawnCooltime.ToString() + ", ";
        monsterData += Monster.HealthRegenPercentage.ToString();
        monsterData += ")";
        GameManager.Instance.playerData.dungeonUpgradeData.Add(monsterData);
        //PlayerPrefs.SetString("dataHandler_dungeonUpgrade_" + 0.ToString(), monsterData);

        string trapData = "("; //(fireCooltime)
        trapData += Trap.FireCooltime.ToString();
        trapData += ")";
        GameManager.Instance.playerData.dungeonUpgradeData.Add(trapData);
        //PlayerPrefs.SetString("dataHandler_dungeonUpgrade_" + 1.ToString(), trapData);

        string curseData = "("; //(fireCooltime, curseDuration)
        curseData += Curse.FireCooltime.ToString() + ", ";
        curseData += Curse.CurseDuration.ToString();
        curseData += ")";
        GameManager.Instance.playerData.dungeonUpgradeData.Add(curseData);
        //PlayerPrefs.SetString("dataHandler_dungeonUpgrade_" + 2.ToString(), curseData);
    }
    void Load()
    {
        if (GameManager.Instance.playerData.isNewBegin) {
            rankUpExtraRate = 0;
            return;
        }

        List<string> datas = GameManager.Instance.playerData.dungeonUpgradeData;
        /*List<string> datas = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            string data = PlayerPrefs.GetString("dataHandler_dungeonUpgrade_" + i.ToString());
            datas.Add(data);
        }*/

        //Monster
        datas[0] = datas[0].Trim(new char[] { '(', ')' });
        datas[0] = datas[0].Replace(" ", "");
        string[] monsterDatas = datas[0].Split(',');
        Monster.RespawnCooltime = float.Parse(monsterDatas[0]);
        Monster.HealthRegenPercentage = float.Parse(monsterDatas[1]);

        //Trap
        datas[1] = datas[1].Trim(new char[] { '(', ')' });
        datas[1] = datas[1].Replace(" ", "");
        string[] trapDatas = datas[1].Split(',');
        Trap.FireCooltime = float.Parse(trapDatas[0]);

        //Curse
        datas[2] = datas[2].Trim(new char[] { '(', ')' });
        datas[2] = datas[2].Replace(" ", "");
        string[] curseData = datas[2].Split(',');
        Curse.FireCooltime = float.Parse(curseData[0]);
        Curse.CurseDuration = float.Parse(curseData[1]);
    }

    //=== Monster
    public void OnClick_DungeonUpgrade(int index)
    {
        if (GameManager.Instance.playerData.gem < dungeonUpgradeCost)
        {
            EffectHandler.Instance.SystemMessage("Not enough gem");
        }
        else
        {
            switch (index)
            {
                case 0:
                    DungeonUpgrade_Monster();
                    GameManager.Instance.playerData.gem -= dungeonUpgradeCost;
                    break;
                case 1:
                    DungeonUpgrade_Trap();
                    GameManager.Instance.playerData.gem -= dungeonUpgradeCost;
                    break;
                case 2:
                    DungeonUpgrade_Curse();
                    GameManager.Instance.playerData.gem -= dungeonUpgradeCost;
                    break;
            }
            AudioHandler.Instance.PlaySFX(AudioHandler.Instance.sfxFolder[0]);
        }
    }
    void DungeonUpgrade_Monster()
    {
        GameManager.Instance.playerData.dungeonUpgradeLevel_monster += 1;
        Monster.RespawnCooltime -= Monster.RespawnCooltime * (monster_respawnCooltime_downRateByLevel / 100);
        Monster.HealthRegenPercentage += monster_healthRegen_upRateByLevel;
        UpdateUI();
    }
    void DungeonUpgrade_Trap()
    {
        GameManager.Instance.playerData.dungeonUpgradeLevel_trap += 1;
        Trap.FireCooltime -= Trap.FireCooltime * (trap_fireCooltime_downRateByLevel / 100);
        UpdateUI();
    }
    void DungeonUpgrade_Curse()
    {
        GameManager.Instance.playerData.dungeonUpgradeLevel_curse += 1;
        Curse.FireCooltime -= Curse.FireCooltime * (curse_fireCooltime_downRateByLevel / 100);
        Curse.CurseDuration += Curse.CurseDuration * (curse_duration_upRateByLevel / 100);
        UpdateUI();
    }

    void UpdateUI()
    {
        dungeonUpgradeUI_monster_level.text = "Lv. " + GameManager.Instance.playerData.dungeonUpgradeLevel_monster.ToString();
        dungeonUpgradeUI_monster_content.text = "Breeding Skills\n<size=25%>\n\n" + "<size=80%>respawn cooltime : " + Monster.RespawnCooltime.ToString() + "s"
            + "\nhealth regen : " + Monster.HealthRegenPercentage + "%/s";
        dungeonUpgradeUI_monster_cost.text = "<sprite=0>" + dungeonUpgradeCost.ToString();

        dungeonUpgradeUI_trap_level.text = "Lv. " + GameManager.Instance.playerData.dungeonUpgradeLevel_trap.ToString();
        dungeonUpgradeUI_trap_content.text = "Elemental Powers\n" + "<size=25%>\n\n<size=80%>spirits' attack speed : " + Trap.FireCooltime.ToString() + "s";
        dungeonUpgradeUI_trap_cost.text = "<sprite=0>" + dungeonUpgradeCost.ToString();

        dungeonUpgradeUI_curse_level.text = "Lv. " + GameManager.Instance.playerData.dungeonUpgradeLevel_curse.ToString();
        dungeonUpgradeUI_curse_content.text = "Creepy Atmosphere\n<size=25%>\n" + "<size=80%>totems' casting speed : " + Curse.FireCooltime.ToString() + "s"
            + "\nweakness duration : " + Curse.CurseDuration.ToString() + "s";
        dungeonUpgradeUI_curse_cost.text = "<sprite=0>" + dungeonUpgradeCost.ToString();
    }

    //===
    public MonsterForm GetMonsterForm(string form_id)
    {
        foreach(MonsterForm form in monsterForms)
        {
            if(form_id == form.id)
            {
                return form;
            }
        }
        return null;
    }
    public List<MonsterForm> GetDefaultMonsterForms(int count)
    {
        List<MonsterForm> list = new List<MonsterForm>();
        for(int i=0; i<count; i++)
        {
            int idx = Random.Range(0, defaultGivenMonsterForms.Count);
            list.Add(defaultGivenMonsterForms[idx]);
        }
        return list;
    }
    public List<MonsterForm> GetMonsterFormsAtGrade(Grade grade)
    {
        List<MonsterForm> list = new List<MonsterForm>();
        foreach (MonsterForm form in monsterForms)
        {
            if (form.grade == grade)
            {
                list.Add(form);
            }
        }
        return list;
    }
    public List<MonsterForm> GetMonsterFormsAtGrade(Grade grade, int count)
    {
        List<MonsterForm> list = new List<MonsterForm>();
        foreach (MonsterForm form in monsterForms)
        {
            if (form.grade == grade)
            {
                list.Add(form);
            }
        }
        List<MonsterForm> forms = new List<MonsterForm>();
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, list.Count);
            forms.Add(list[idx]);
        }
        return forms;
    }
    public List<MonsterForm> GetMonsterFormsAndUpGrade(Grade grade, int count)
    {
        List<MonsterForm> list = new List<MonsterForm>();
        foreach(MonsterForm form in monsterForms)
        {
            if((int)form.grade > (int)grade)
            {
                list.Add(form);
            }
        }
        List<MonsterForm> forms = new List<MonsterForm>();
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, list.Count);
            forms.Add(list[idx]);
        }
        return forms;
    }

    //===
    public TrapForm GetTrapForm(string form_id)
    {
        foreach (TrapForm form in trapForms)
        {
            if (form_id == form.id)
            {
                return form;
            }
        }
        return null;
    }
    public List<TrapForm> GetDefaultTrapForms(int count)
    {
        List<TrapForm> list = new List<TrapForm>();
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, defaultGivenTrapForms.Count);
            list.Add(defaultGivenTrapForms[idx]);
        }
        return list;
    }
    public List<TrapForm> GetTrapFormsAtGrade(Grade grade)
    {
        List<TrapForm> list = new List<TrapForm>();
        foreach (TrapForm form in trapForms)
        {
            if (form.grade == grade)
            {
                list.Add(form);
            }
        }
        return list;
    }
    public List<TrapForm> GetTrapFormsAtGrade(Grade grade, int count)
    {
        List<TrapForm> list = new List<TrapForm>();
        foreach (TrapForm form in trapForms)
        {
            if (form.grade == grade)
            {
                list.Add(form);
            }
        }
        List<TrapForm> forms = new List<TrapForm>();
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, list.Count);
            forms.Add(list[idx]);
        }
        return forms;
    }
    public List<TrapForm> GetTrapFormsAndUpGrade(Grade grade, int count)
    {
        List<TrapForm> list = new List<TrapForm>();
        foreach (TrapForm form in trapForms)
        {
            if ((int)form.grade > (int)grade)
            {
                list.Add(form);
            }
        }
        List<TrapForm> forms = new List<TrapForm>();
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, list.Count);
            forms.Add(list[idx]);
        }
        return forms;
    }

    //===
    public CurseForm GetCurseForm(string form_id)
    {
        foreach (CurseForm form in curseForms)
        {
            if (form_id == form.id)
            {
                return form;
            }
        }
        return null;
    }
    public List<CurseForm> GetDefaultCurseForms(int count)
    {
        List<CurseForm> list = new List<CurseForm>();
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, defaultGivenCurseForms.Count);
            list.Add(defaultGivenCurseForms[idx]);
        }
        return list;
    }
    public List<CurseForm> GetCurseFormsAtGrade(Grade grade)
    {
        List<CurseForm> list = new List<CurseForm>();
        foreach (CurseForm form in curseForms)
        {
            if (form.grade == grade)
            {
                list.Add(form);
            }
        }
        return list;
    }
    public List<CurseForm> GetCurseFormsAtGrade(Grade grade, int count)
    {
        List<CurseForm> list = new List<CurseForm>();
        foreach (CurseForm form in curseForms)
        {
            if (form.grade == grade)
            {
                list.Add(form);
            }
        }
        List<CurseForm> forms = new List<CurseForm>();
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, list.Count);
            forms.Add(list[idx]);
        }
        return forms;
    }
    public List<CurseForm> GetCurseFormsAndUpGrade(Grade grade, int count)
    {
        List<CurseForm> list = new List<CurseForm>();
        foreach (CurseForm form in curseForms)
        {
            if ((int)form.grade > (int)grade)
            {
                list.Add(form);
            }
        }
        List<CurseForm> forms = new List<CurseForm>();
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, list.Count);
            forms.Add(list[idx]);
        }
        return forms;
    }

    //===
    public BossForm GetBossForm(string boss_id)
    {
        foreach(BossForm form in bossForms)
        {
            if(boss_id == form.id)
            {
                return form;
            }
        }
        return null;
    }
    public BossForm GetRandomBoss()
    {
        int index = Random.Range(0, bossForms.Count);
        BossForm form = bossForms[index];
        return form;
    }
}
