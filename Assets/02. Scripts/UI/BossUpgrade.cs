using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUpgrade : MonoBehaviour {

    [Header("- Boss Purchase")]
    [Header("[  UI")]
    [SerializeField]
    private GameObject bossPurchase_slotObj;
    [SerializeField]
    private Image bossPurchaseUI_potrait;
    [SerializeField]
    private Button bossPurchaseUI_button;
    [SerializeField]
    private TextMeshProUGUI bossPurchaseUI_contentTextMesh;
    [SerializeField]
    private TextMeshProUGUI bossPurchaseUI_costTextMesh;

    public static BossForm currentBossForm;
    public static BossForm nextBossForm;

    [Space(6)]
    [SerializeField]
    private float bossPurchaseCost_initial;
    [SerializeField]
    private float bossPurchaseCost_upRateByCount;
    private int bossPurchaseCost
    {
        get
        {
            float value = bossPurchaseCost_initial;
            for (int i = 0; i < GameManager.Instance.playerData.bossPurchaseCount; i++)
            {
                value = value * (1 + (bossPurchaseCost_upRateByCount / 100));
            }
            return (int)value;
        }
    }

    [Space(8)]
    [SerializeField]
    private Button rerollButton;
    [SerializeField]
    private TextMeshProUGUI rerollTextMesh;
    [SerializeField]
    private int reroll_cost_gem_initial;
    [SerializeField]
    private int reroll_costUp_byCount;
    private int rerollCount = 0;

    private int reroll_cost_gem
    {
        get
        {
            int value = reroll_cost_gem_initial;
            for(int i=0; i<rerollCount; i++)
            {
                value += reroll_costUp_byCount;
            }
            return value;
        }
    }

    [Header("- Boss Upgrade")]
    [Space(12)]
    [SerializeField]
    private int maxUpgradeLevel;

    [Header("[  UI")]
    [Space(8)]
    [SerializeField]
    private Button bossUpgradeUI_button;
    [SerializeField]
    private TextMeshProUGUI bossUpgradeUI_currentLevel;
    [SerializeField]
    private TextMeshProUGUI bossUpgradeUI_contentTextMesh;
    [SerializeField]
    private TextMeshProUGUI bossUpgradeUI_costTextMesh;
    [Header("[  Parameters")]
    [Space(6)]
    [SerializeField]
    private float boss_enhanced_upRateByLevel;
    [SerializeField]
    private float boss_respawnCooltime_downRateByLevel;
    [SerializeField]
    private float boss_healthRegen_upRateByLevel;


    public void Initalize()
    {
        bossPurchaseUI_button.onClick.AddListener(delegate { OnClick_PurchaseBoss(); });
        bossUpgradeUI_button.onClick.AddListener(delegate { OnClick_UpgradeBoss(); });
        rerollButton.onClick.AddListener(delegate { OnClick_Reroll(); });

        Load();
        UpdateUI();
    }

    //(Boss.Enhanced, Boss.RespawnCooltime, Boss.HealthRegenPercentage)
    public void Save()
    {
        string bossData = "(";
        bossData += Boss.EnhancedRate.ToString() + ", ";
        bossData += Boss.RespawnCooltime.ToString() + ", ";
        bossData += Boss.HealthRegenPercentage.ToString();
        bossData += ")";
        //GameManager.Instance.playerData.bossUpgrade = bossData;
        PlayerPrefs.SetString("BossUpgrade", bossData);

        if (currentBossForm != null)
        {
            PlayerPrefs.SetString("CurrentBossForm_Id", currentBossForm.id);
        }
        //GameManager.Instance.playerData.nextBossId = nextBossForm.id;
        if (nextBossForm != null)
            PlayerPrefs.SetString("NextBossForm_Id", nextBossForm.id);

        PlayerPrefs.SetInt("BossRerollCount", rerollCount);
    }
    void Load()
    {
        if (GameManager.Instance.playerData.isNewBegin)
        {
            currentBossForm = null;
            nextBossForm = DataHandler.Instance.GetRandomBoss();
            rerollCount = 0;
            return;
        }

        string value = PlayerPrefs.GetString("BossUpgrade");
        value = value.Trim(new char[] { '(', ')' });
        value = value.Replace(" ", "");
        string[] values = value.Split(',');
        Boss.EnhancedRate = float.Parse(values[0]);
        Boss.RespawnCooltime = float.Parse(values[1]);
        Boss.HealthRegenPercentage = float.Parse(values[2]);

        currentBossForm = DataHandler.Instance.GetBossForm(PlayerPrefs.GetString("CurrentBossForm_Id"));
        nextBossForm = DataHandler.Instance.GetBossForm(PlayerPrefs.GetString("NextBossForm_Id"));

        rerollCount = PlayerPrefs.GetInt("BossRerollCount");
    }

    //===
    void OnClick_PurchaseBoss()
    {
        if (GameManager.Instance.playerData.gold < bossPurchaseCost)
        {
            EffectHandler.Instance.SystemMessage("Not enough gold");
        }
        else
        {
            PurchaseBoss();
        }
    }
    public void OnClick_UpgradeBoss()
    {
        if (GameManager.Instance.playerData.gem < DataHandler.Instance.DungeonUpgradeCost)
        {
            EffectHandler.Instance.SystemMessage("Not enough gem");
        }
        else if(GameManager.Instance.playerData.dungeonUpgradeLevel_boss >= maxUpgradeLevel)
        {
            EffectHandler.Instance.SystemMessage("No more upgrade is available");
        }
        else
        {
            UpgradeBoss();
        }
    }
    void OnClick_Reroll()
    {
        if (GameManager.Instance.playerData.gem < reroll_cost_gem)
        {
            EffectHandler.Instance.SystemMessage("Not enough gem");
        }
        else
        {
            Reroll();
        }
    }

    //===
    void PurchaseBoss()
    {
        GameManager.Instance.playerData.gold -= bossPurchaseCost;
        GameManager.Instance.playerData.bossPurchaseCount += 1;

        UIHandler.Instance.floorBuilder.SetActiveBossButton(true);

        currentBossForm = nextBossForm;
        nextBossForm = DataHandler.Instance.GetRandomBoss();

        UpdateUI();
        AudioHandler.Instance.PlaySFX(AudioHandler.Instance.sfxFolder[0]);
        GameManager.Instance.SaveGame();
    }
    void UpgradeBoss()
    {
        GameManager.Instance.playerData.gem -= DataHandler.Instance.DungeonUpgradeCost;
        GameManager.Instance.playerData.dungeonUpgradeLevel_boss += 1;

        Boss.EnhancedRate += Boss.EnhancedRate * (boss_enhanced_upRateByLevel / 100);
        Boss.RespawnCooltime -= Boss.RespawnCooltime * (boss_respawnCooltime_downRateByLevel / 100);
        Boss.HealthRegenPercentage += boss_healthRegen_upRateByLevel;

        UpdateUI();
        AudioHandler.Instance.PlaySFX(AudioHandler.Instance.sfxFolder[0]);
        GameManager.Instance.SaveGame();
    }
    void Reroll()
    {
        GameManager.Instance.playerData.gem -= reroll_cost_gem;
        rerollCount += 1;

        currentBossForm = nextBossForm;
        nextBossForm = DataHandler.Instance.GetRandomBoss();

        UpdateUI();
        AudioHandler.Instance.PlaySFX(AudioHandler.Instance.sfxFolder[0]);
        GameManager.Instance.SaveGame();
    }

    public void UpdateUI()
    {
        if (UIHandler.Instance.floorBuilder.isBossEnabled)
        {
            bossPurchase_slotObj.gameObject.SetActive(false);
        }
        else
        {
            bossPurchase_slotObj.gameObject.SetActive(true);
        }

        bossPurchaseUI_potrait.sprite = nextBossForm.sprite;
        bossPurchaseUI_costTextMesh.text = "<sprite=3>" + bossPurchaseCost.ToString();
        bossPurchaseUI_contentTextMesh.text = "<color=black>Boss Monster<size=25%>\n<size=70%>"
            + "<color=purple>" + nextBossForm.name.Substring(4) + "<color=black><size=25%>\n\n"
            + "<size=60%>The boss button in the build menu will be enabled one time";

        bossUpgradeUI_currentLevel.text = "Lv. " + GameManager.Instance.playerData.dungeonUpgradeLevel_boss.ToString();
        bossUpgradeUI_contentTextMesh.text = "Enhance Boss Monsters\n<size=25%>\n" + "<size=60%>damage/health x " + (Boss.EnhancedRate*100).ToString() + "%"
            + "\nrespawn cooltime : " + Boss.RespawnCooltime.ToString() + "s"
            + "\nhelath regen : " + Boss.HealthRegenPercentage + "%/s";
        bossUpgradeUI_costTextMesh.text = "<sprite=0>" + DataHandler.Instance.DungeonUpgradeCost.ToString();

        rerollTextMesh.text = "<sprite=0>" + reroll_cost_gem.ToString();
    }
}
