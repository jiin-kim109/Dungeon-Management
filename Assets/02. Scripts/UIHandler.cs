using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour {

    [SerializeField]
    private Image shadowBgImg;
    [SerializeField]
    private IAPManager m_IAP;
    public IAPManager IAP { get { return m_IAP; } }
    [SerializeField]
    private AdmobManager m_admobManager;
    public AdmobManager admobManager { get { return m_admobManager; } }
    [SerializeField]
    private FloorBuilder m_floorBuilder;
    public FloorBuilder floorBuilder { get { return m_floorBuilder; } }
    [SerializeField]
    private BossUpgrade m_bossUpgrade;
    public BossUpgrade bossUpgrade { get { return m_bossUpgrade; } }

    [Space(8)]
    [SerializeField]
    private TextMeshProUGUI playerGoldText;
    [SerializeField]
    private TextMeshProUGUI playerGemText;
    [SerializeField]
    private TextMeshProUGUI playerScoreTextMesh;

    [Space(8)]
    [SerializeField]
    private TextMeshProUGUI cheatModeIndicator;

    public delegate void UIContentsToUpdate(PlayerData playerData);
    public static UIContentsToUpdate UIContents;

    //---------------Instance------------------
    private static UIHandler _instance = null;
    public static UIHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(UIHandler)) as UIHandler;
                if (_instance == null) { Debug.Log("UIHandler가 없음"); }
            }
            return _instance;
        }
    }
    //-----------------------------------------

    void Awake()
    {
        shadowBgImg.gameObject.SetActive(false);
        UIContents += UpdateUI;
    }

    //===
    public void Initialize()
    {
        UIContents(GameManager.Instance.playerData);
    }

    //===
    public void UpdateUI(PlayerData playerData)
    {
        playerGoldText.text = "<sprite=3> " + playerData.gold.ToString();
        playerGemText.text = "<sprite=0> " + playerData.gem.ToString();
        playerScoreTextMesh.text = "score: " + playerData.score.ToString();
        if (playerData.cheat_isActive)
            cheatModeIndicator.gameObject.SetActive(true);
        else
            cheatModeIndicator.gameObject.SetActive(false);
    }
    public void UpdateUI()
    {
        UIContents(GameManager.Instance.playerData);
    }


    //=== Screen Effects
    public void DisplayShadow()
    {
        shadowBgImg.gameObject.SetActive(true);
    }
    public void HideShadow()
    {
        shadowBgImg.gameObject.SetActive(false);
    }
}
