using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonUpgrade : MonoBehaviour {

    [Header("[  Slot1 - Gold Given Upgrade")]
    [SerializeField]
    private Button goldGivenUpgradeUI_Button;
    [SerializeField]
    private TextMeshProUGUI goldGivenUpgradeUI_level;
    [SerializeField]
    private TextMeshProUGUI goldGivenUpgradeUI_cost;
    [SerializeField]
    private TextMeshProUGUI goldGivenUI_Contents;
    [SerializeField]
    private float goldGivenRateUp_perCount;
    [SerializeField]
    private float goldGivenUpgradeCost_initial;
    [SerializeField]
    private float goldGivenUpgradeCost_upPerCount;
    private int goldGivenUpgradeCost
    {
        get
        {
            float value = goldGivenUpgradeCost_initial;
            for(int i=0; i<GameManager.Instance.playerData.goldGivenUpgradeCount; i++)
            {
                value = value * (1 + goldGivenUpgradeCost_upPerCount / 100);
            }
            return (int)value;
        }
    }

    [Space(10)]
    [Header("[  Slot2 - Rank Up Rate Upgrade")]
    [SerializeField]
    private Button rankUpUpgradeUI_Button;
    [SerializeField]
    private TextMeshProUGUI rankUpUpgradeUI_level;
    [SerializeField]
    private TextMeshProUGUI rankUpUpgradeUI_cost;
    [SerializeField]
    private TextMeshProUGUI rankUpUpgradeUI_Contents;
    [SerializeField]
    private float rankUpRateUp_perCount;
    [SerializeField]
    private float rankUpUpgradeCost_initial;
    [SerializeField]
    private float rankUpUpgradeCost_upPerCount;
    private int rankUpUpgradeCost
    {
        get
        {
            float value = rankUpUpgradeCost_initial;
            for (int i = 0; i < GameManager.Instance.playerData.rankUpUpgradeCount; i++)
            {
                value = value * (1 + rankUpUpgradeCost_upPerCount / 100);
            }
            return (int)value;
        }
    }

    //===
    public void Initialize()
    {
        goldGivenUpgradeUI_Button.onClick.AddListener(delegate { OnClick_UpgradeGoldGiven(); });
        rankUpUpgradeUI_Button.onClick.AddListener(delegate { OnClick_UpgradeRateUp(); });

        Load();
        UpdateUI();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("GoldGivenMult", HeroSpawner.GoldGivenExtraMult);
        PlayerPrefs.SetFloat("RateUpRate", DataHandler.rankUpExtraRate);
    }
    void Load()
    {
        if (GameManager.Instance.playerData.isNewBegin)
            return;
        HeroSpawner.GoldGivenExtraMult = PlayerPrefs.GetFloat("GoldGivenMult");
        DataHandler.rankUpExtraRate = PlayerPrefs.GetFloat("RateUpRate");
    }

    //===
    void OnClick_UpgradeGoldGiven()
    {
        if (GameManager.Instance.playerData.gold < goldGivenUpgradeCost)
        {
            EffectHandler.Instance.SystemMessage("Not enough gold");
        }
        else
        {
            GameManager.Instance.playerData.gold -= goldGivenUpgradeCost;
            GameManager.Instance.playerData.goldGivenUpgradeCount++;
            HeroSpawner.GoldGivenExtraMult += goldGivenRateUp_perCount;
            UpdateUI();
            AudioHandler.Instance.PlaySFX(AudioHandler.Instance.sfxFolder[0]);
            GameManager.Instance.SaveGame();
        }
    }
    void OnClick_UpgradeRateUp()
    {
        if (GameManager.Instance.playerData.gold < rankUpUpgradeCost)
        {
            EffectHandler.Instance.SystemMessage("Not enough gold");
        }
        else
        {
            GameManager.Instance.playerData.gold -= rankUpUpgradeCost;
            GameManager.Instance.playerData.rankUpUpgradeCount++;
            DataHandler.rankUpExtraRate += rankUpRateUp_perCount;
            UpdateUI();
            AudioHandler.Instance.PlaySFX(AudioHandler.Instance.sfxFolder[0]);
            GameManager.Instance.SaveGame();
        }
    }

    //===
    void UpdateUI()
    {
        goldGivenUpgradeUI_level.text = "Lv. " + GameManager.Instance.playerData.goldGivenUpgradeCount.ToString();
        int goldRate = (int)(HeroSpawner.GoldGivenExtraMult * 100);
        goldGivenUI_Contents.text = "Gold Production\n" + "<size=25%>\n\n"
            + "<size=100%><sprite=3> <size=105%> x" + goldRate.ToString()+"%";
        goldGivenUpgradeUI_cost.text = "<sprite=3>" + goldGivenUpgradeCost.ToString();

        rankUpUpgradeUI_level.text = "Lv. " + GameManager.Instance.playerData.rankUpUpgradeCount.ToString();
        float rankUpExtraRate = DataHandler.rankUpExtraRate;
        rankUpUpgradeUI_Contents.text = "Rank Up\n" + "<size=25%>\n\n"
            + "<size=70%>percentage +" + rankUpExtraRate.ToString() + "%";
        rankUpUpgradeUI_cost.text = "<sprite=3>" + rankUpUpgradeCost.ToString();
    }
}
