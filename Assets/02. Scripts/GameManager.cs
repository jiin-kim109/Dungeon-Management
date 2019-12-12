using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private PlayerData m_playerData;
    public PlayerData playerData { get { return m_playerData; } }
    [SerializeField]
    private float autoSaveCooltime = 5f;

    //---------------Instance------------------
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
                if (_instance == null) { Debug.Log("HeroGenerator가 없음"); }
            }
            return _instance;
        }
    }
    //-----------------------------------------

    [Space(6)]
    [SerializeField]
    private ScreenTouch m_screenTouch;
    public ScreenTouch screenTouch { get { return m_screenTouch; } }

    [Space(12)]
    [SerializeField]
    private UnityEvent initializeEvents;
    public UnityEvent saveEvents = new UnityEvent();

    //===
    void Awake()
    {
        int heroLayerIndex = LayerMask.NameToLayer("Heroes");
        int dungeonObjectLayerIndex = LayerMask.NameToLayer("DungeonObjects");
        int trapLayerIndex = LayerMask.NameToLayer("Traps&Curses");
        int monsterLayerIndex = LayerMask.NameToLayer("Monsters");
        Physics2D.IgnoreLayerCollision(heroLayerIndex, heroLayerIndex);
        Physics2D.IgnoreLayerCollision(dungeonObjectLayerIndex, dungeonObjectLayerIndex);
        Physics2D.IgnoreLayerCollision(dungeonObjectLayerIndex, trapLayerIndex);
        Physics2D.IgnoreLayerCollision(heroLayerIndex, trapLayerIndex);
        Physics2D.IgnoreLayerCollision(monsterLayerIndex, monsterLayerIndex);
        //Screen.SetResolution(1440, 2560, true);

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 0;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    void Start()
    {
        LoadGame();
        StartCoroutine(thr_main());
    }
	public void SaveGame()
    {
        playerData.monsterData = new List<string>();
        playerData.trapData = new List<string>();
        playerData.curseData = new List<string>();
        playerData.bossData = new List<string>();

        saveEvents.Invoke();
        if (playerData.isNewBegin)
        {
            playerData.isNewBegin = false;
        }
        PlayerPrefs.SetInt("CashStock", playerData.cashStock);
        SaveLoad.SavePlayerData(m_playerData);
    }
    public void Checker()
    {
        //Debug.Log("확인");
    }
    public void LoadGame()
    {
        SaveLoad.LoadPlayerData(m_playerData);
        if (playerData.isNewBegin)
        {
            int cashCount = PlayerPrefs.GetInt("CashStock");
            bool cheat;
            if (PlayerPrefs.GetInt("Cheat") == 1)
                cheat = true;
            else
                cheat = false;

            playerData.cashStock = cashCount;
            playerData.cheat_isAllowed = cheat;

            PlayerPrefs.DeleteAll();
        }
    }


    //===
    IEnumerator thr_main()
    {
        initializeEvents.Invoke();
        StartCoroutine(thr_saveRoutine());
        yield return null;
    }

    //===
    IEnumerator thr_saveRoutine()
    {
        float timer = autoSaveCooltime;
        while (true)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                SaveGame();
                timer = autoSaveCooltime;
            }
            yield return null;
        }
    }
}
