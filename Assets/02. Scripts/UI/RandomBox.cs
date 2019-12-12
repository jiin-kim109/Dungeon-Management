using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomBox : MonoBehaviour {

    [SerializeField]
    private Button purchaseButton;
    [SerializeField]
    private TextMeshProUGUI costTextMesh;
    [SerializeField]
    private int saleCost;
    [SerializeField]
    private int boxCount;

    [Space(10)]
    [SerializeField]
    private GameObject randomBoxUI;
    [SerializeField]
    private TextMeshProUGUI goldObtainTextMesh;
    [SerializeField]
    private TextMeshProUGUI gemObtainTextMesh;
    [SerializeField]
    private List<Sprite> iconByIndex = new List<Sprite>();  
    [SerializeField]
    private List<Image> slotImages = new List<Image>();

    //===
    public void initialize()
    {
        purchaseButton.onClick.AddListener(delegate { OnClick_purchaseBox(); });
        randomBoxUI.gameObject.SetActive(false);
        UpdateUI();
    }

    //===
    void OnClick_purchaseBox()
    {
        int price = boxCount * DataHandler.Instance.DungeonUpgradeCost - saleCost;
        if (GameManager.Instance.playerData.gem < price)
        {
            EffectHandler.Instance.SystemMessage("Not enough gem");
        }
        else
        {
            GameManager.Instance.playerData.gem += saleCost;
            OpenRandomBox();
        }
        UpdateUI();
    }
    void OpenRandomBox()
    {
        int goldAmount = 0;
        int gemAmount = 0;
        for(int i=0; i<boxCount; i++)
        {
            int pick = Random.Range(0, 6);
            switch (pick)
            {
                case 0: //몬스터
                    DataHandler.Instance.OnClick_DungeonUpgrade(0);
                    break;
                case 1: //정령
                    DataHandler.Instance.OnClick_DungeonUpgrade(1);
                    break;
                case 2: //토템
                    DataHandler.Instance.OnClick_DungeonUpgrade(2);
                    break;
                case 3: //돈
                    int goldGivenAmount = HeroSpawner.Instance.GivenGoldByHero * 30;
                    GameManager.Instance.playerData.gold += goldGivenAmount;
                    goldAmount += goldGivenAmount;
                    break;
                case 4: //보석
                    int gemGiven = 3;
                    GameManager.Instance.playerData.gem += gemGiven;
                    gemAmount += gemGiven;
                    break;
                case 5: //보스
                    DataHandler.Instance.bossUpgrade.OnClick_UpgradeBoss();
                    break;
            }
            slotImages[i].sprite = iconByIndex[pick];
        }
        randomBoxUI.gameObject.SetActive(true);
        if (goldAmount > 0)
            goldObtainTextMesh.text = "<sprite=3> " + goldAmount.ToString();
        else
            goldObtainTextMesh.text = "";

        if (gemAmount > 0)
            gemObtainTextMesh.text = "<sprite=0>" + gemAmount.ToString();
        else
            gemObtainTextMesh.text = "";
        EffectHandler.Instance.PlayFlashEffect();
        AudioHandler.Instance.PlaySFX(AudioHandler.Instance.sfxFolder[1]);
        GameManager.Instance.SaveGame();
        StartCoroutine(thr_beOpenTimer());
    }
    IEnumerator thr_beOpenTimer()
    {
        float time = 1.8f;
        yield return new WaitForSeconds(time);
        ScreenTouch.uiTouchEvent += CloseRandomBox;
    }
    void CloseRandomBox()
    {
        ScreenTouch.uiTouchEvent -= CloseRandomBox;
        EffectHandler.Instance.StopFlashEffect();
        randomBoxUI.gameObject.SetActive(false);
    }

    //===
    void UpdateUI()
    {
        int price = boxCount * DataHandler.Instance.DungeonUpgradeCost - saleCost;
        costTextMesh.text = "<sprite=0>" + price.ToString();
    }
}
