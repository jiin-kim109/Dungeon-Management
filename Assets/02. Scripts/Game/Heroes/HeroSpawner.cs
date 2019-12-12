using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleHealthBarUtility;
using UnityEngine.UI;
using TMPro;

public class HeroSpawner : MonoBehaviour {

    private static string m_heroSaveID = "hero_";
    public static string heroSaveID { get { return m_heroSaveID; } }
    private static string m_spawnPoolsaveID = "spawnPool_";
    public static string spawnPoolsaveID { get { return m_spawnPoolsaveID; } }
    private const int PassMaxNum = 5;

    public static float EnhancedRate
    {
        get
        {
            float value = 1f;
            for (int i = 0; i < currentWave; i++)
            {
                value = value * (1 + (enhancedRate_upPerWave / 100));
            }
            return value;
        }
    }
    public static float GoldGivenExtraMult = 1f;
    public int GivenGoldByHero
    {
        get
        {
            float value = goldGivenPerHero * GoldGivenExtraMult;
            return (int)value;
        }
    }

    private const float enhancedRate_upPerWave = 15;

    public static int currentWave = 0;
    private static int heroDeathCount = 0;
    private static int passCount = 0;

    [Header("[  References")]
    [SerializeField]
    private Shader m_heroShader;
    public Shader heroShader { get { return m_heroShader; } }
    [SerializeField]
    private Collider2D spawnPoint;
    [SerializeField]
    private float scorePerHero;
    [SerializeField]
    private float goldGivenPerHero;
    [SerializeField]
    private float gemObtainPercent;
    [Space(6)]
    [SerializeField]
    private EnemyList enemyList;
    [Space(6)]
    [SerializeField]
    private TMP_SpriteAsset spriteAsset_gold;
    [SerializeField]
    private TMP_SpriteAsset spriteAsset_gem;
    [SerializeField]
    private Button wavePlayButton;
    [SerializeField]
    private TextMeshProUGUI currentWaveTextMesh;
    [SerializeField]
    private TextMeshProUGUI remainCountTextMesh;

    [Space(10)]
    [Header("[  Heroes")]
    [SerializeField]
    private Hero heroPrefab;

    [Space(8)]
    [SerializeField]
    private List<WaveSetting> waveSettings;
    [System.Serializable]
    private struct WaveSetting
    {
        public int waveNum;
        public float normalSpawnCooltime;
        public float waveSpawnCooltime;
        public int passCount;
        public List<HeroForm> spawnHeroForms;

        [Space(6)]
        public float scoreMult;
    }
    [Space(6)]
    [SerializeField]
    private List<Hero> heroes = new List<Hero>();

    private bool isOnWave = false;
    private bool noReward = false;

    //---------------Instance------------------
    private static HeroSpawner _instance = null;
    public static HeroSpawner Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(HeroSpawner)) as HeroSpawner;
                if (_instance == null) { Debug.Log("HeroGenerator가 없음"); }
            }
            return _instance;
        }
    }
    //-----------------------------------------

    void Awake()
    {
        currentWave = 0;
        heroDeathCount = 0;

        GoldGivenExtraMult = 1f;

        passCount = 0;
    }

    //===
    public void Save()
    {
        //현재 모든 영웅 정보
        for (int i=0; i< heroes.Count; i++)
        {
            heroes[i].Save(m_heroSaveID + i.ToString());
        }
        int heroCount = heroes.Count;
        PlayerPrefs.SetInt("heroCount", heroCount);
        PlayerPrefs.SetInt("CurrentWave", currentWave);
        int trig;
        if (isOnWave)
            trig = 1;
        else
            trig = 0;
        PlayerPrefs.SetInt("IsOnWave", trig);
    }
    public void Load()
    {
        if (GameManager.Instance.playerData.isNewBegin)
        {
            StartSpawner();
            return;
        }

        //현재 모든 영웅 정보
        heroes = new List<Hero>();
        int heroCount = PlayerPrefs.GetInt("heroCount");
        for (int i=0; i<heroCount; i++)
        {
            Hero hero = Instantiate(heroPrefab);
            hero.Load(m_heroSaveID + i.ToString());

            heroes.Add(hero);
        }
        currentWave = PlayerPrefs.GetInt("CurrentWave");
        int trig = PlayerPrefs.GetInt("IsOnWave");
        if (trig >= 1)
            isOnWave = true;
        else
            isOnWave = false;
        currentWaveTextMesh.text = "Wave " + currentWave.ToString();
        StartSpawner();
        UIHandler.Instance.floorBuilder.UpdateUI();
    }


    //===
    void StartSpawner()
    {
        wavePlayButton.onClick.AddListener(delegate { OnClick_PlayWave(); });
        remainCountTextMesh.gameObject.SetActive(false);
        StartCoroutine(thr_spawnRoutine());
    }
    void Spawn(HeroForm form)
    {
        Hero hero = Instantiate(heroPrefab);
        hero.transform.position = spawnPoint.bounds.center;
        hero.transform.SetParent(spawnPoint.transform.parent.transform.Find("Heroes").transform);
        hero.ApplyForm(form);

        heroes.Add(hero);
    }

    //===
    IEnumerator thr_spawnRoutine()
    {
        bool startWithWave = false;
        if(isOnWave) { startWithWave = true; }

        Coroutine waveSpawn = null;
        Coroutine normalSpawn = null;
        currentWaveTextMesh.text = "Wave " + currentWave.ToString();
        while (true)
        {
            if (!startWithWave)
            {
                normalSpawn = StartCoroutine(thr_normalSpawn(currentWave));
                if (waveSpawn != null)
                    StopCoroutine(waveSpawn);
            }
            else
            {
                WaveSetting ws = FindWaveSetting(currentWave);
                enemyList.DisplayEnemyList(ws.spawnHeroForms);
                OnClick_PlayWave();
                startWithWave = false;
            }

            while (!isOnWave) { yield return null; }
            if (normalSpawn != null)
                StopCoroutine(normalSpawn);
            noReward = true;
            foreach (Hero delHr in heroes)
            {
                delHr.DeadWithEffect("disappear");
            }
            noReward = false;
            waveSpawn = StartCoroutine(thr_waveSpawn(currentWave));
            while (isOnWave) { yield return null; }
            UIHandler.Instance.floorBuilder.UpdateUI();
            if (passCount >= PassMaxNum)
                noReward = true;
            foreach (Hero delHr in heroes)
            {
                delHr.DeadWithEffect("disappear");
            }
            GameManager.Instance.SaveGame();
            wavePlayButton.gameObject.SetActive(true);
            remainCountTextMesh.gameObject.SetActive(false);
            yield return null;
        }
    }
    IEnumerator thr_normalSpawn(int waveNum)
    {
        WaveSetting ws = FindWaveSetting(waveNum);
        enemyList.DisplayEnemyList(ws.spawnHeroForms);
        float spawnCooltime = ws.normalSpawnCooltime;
        while (true)
        {
            spawnCooltime -= Time.deltaTime;
            if(spawnCooltime <= 0)
            {
                spawnCooltime = ws.normalSpawnCooltime;
                int index = Random.Range(0, ws.spawnHeroForms.Count);
                Spawn(ws.spawnHeroForms[index]);
            }
            yield return null;
        }
    }
    IEnumerator thr_waveSpawn(int waveNum)
    {
        WaveSetting ws = FindWaveSetting(waveNum);
        float spawnCooltime = ws.waveSpawnCooltime;
        isOnWave = true;
        passCount = 0;
        heroDeathCount = 0;
        remainCountTextMesh.text = "(" + heroDeathCount.ToString() + " /" + ws.passCount.ToString() + ")";

        while (heroDeathCount < ws.passCount) 
        {
            if (passCount >= PassMaxNum)
            {
                currentWave--;
                break;
            }
            spawnCooltime -= Time.deltaTime;
            if(spawnCooltime <= 0)
            {
                spawnCooltime = ws.waveSpawnCooltime;
                int index = Random.Range(0, ws.spawnHeroForms.Count);
                Spawn(ws.spawnHeroForms[index]);
            }
            yield return null;
        }
        currentWave++;
        isOnWave = false;
        currentWaveTextMesh.text = "Wave " + currentWave.ToString();
        yield return null;
    }

    //===
    public void HeroPassTheGoal()
    {
        passCount++;

        WaveSetting ws = FindWaveSetting(currentWave);
        float scoreGiven = scorePerHero * ws.scoreMult * 10;
        int score = (int)scoreGiven;
        GameManager.Instance.playerData.score -= score;
        //EffectHandler.Instance.TextPopup("-" + score.ToString(), deadHero.transform, Color.red, 1f);
    }
    public void HeroDead()
    {
        List<Hero> newList = new List<Hero>();
        Hero deadHero = null;
        foreach (Hero hero in heroes)
        {
            if (hero.isAlive)
            {
                newList.Add(hero);
            }
            else
            {
                deadHero = hero;
            }
        }
        heroes = newList;
        if (isOnWave)
        {
            heroDeathCount++;
            remainCountTextMesh.text = "(" + heroDeathCount.ToString() + " /" + FindWaveSetting(currentWave).passCount.ToString() + ")";
        }

        if (!noReward)
        {
            WaveSetting ws = FindWaveSetting(currentWave);
            float scoreGiven = scorePerHero * ws.scoreMult;
            int score = (int)scoreGiven;
            GameManager.Instance.playerData.score += score;
            if (deadHero != null)
            {
                float goldGiven = goldGivenPerHero * GoldGivenExtraMult * deadHero.form.goldGivenMult;
                int gold = (int)goldGiven;
                GameManager.Instance.playerData.gold += gold;

                EffectHandler.Instance.TextPopup("<sprite=3>+" + gold.ToString(), spriteAsset_gold, deadHero.transform, Color.yellow, 1.2f);
                float percent = Random.Range(0, 100);
                if (percent >= 50 - gemObtainPercent && percent <= 50 + gemObtainPercent)
                {
                    GameManager.Instance.playerData.gem += 1;
                    EffectHandler.Instance.TextPopup("<sprite=0>+" + 1.ToString(), spriteAsset_gem, deadHero.transform, Color.blue, 1.4f);
                }
            }
        }
    }

    //===
    public HeroForm FindHeroForm(string form_id)
    {
        foreach(WaveSetting ws in waveSettings)
        {
            foreach(HeroForm form in ws.spawnHeroForms)
            {
                if (form.id == form_id)
                {
                    return form;
                }
            }
        }
        return null;
    }
    private WaveSetting FindWaveSetting(int waveNum)
    {
        foreach(WaveSetting ws in waveSettings)
        {
            if(ws.waveNum == waveNum)
            {
                return ws;
            }
        }
        return waveSettings[waveSettings.Count - 1];
    }

    //===
    void OnClick_PlayWave()
    {
        isOnWave = true;
        wavePlayButton.gameObject.SetActive(false);
        remainCountTextMesh.gameObject.SetActive(true);

        List<Monster> monsters = DataHandler.Instance.playerMonsters;
        foreach(Monster m in monsters)
        {
            m.Relieve();
        }
        List<Boss> bosses = DataHandler.Instance.playerBosses;
        foreach(Boss b in bosses)
        {
            b.Relieve();
        }
    }
}
