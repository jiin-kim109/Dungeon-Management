using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ScreenTouch : MonoBehaviour
{
    public static float StrongAttackRate = 300;

    [SerializeField]
    private Button upgradeButton;
    [SerializeField]
    private TextMeshProUGUI upgradeLevelTextMesh;
    [SerializeField]
    private TextMeshProUGUI contentTextMesh;
    [SerializeField]
    private TextMeshProUGUI costTextMesh;

    [Space(8)]
    [SerializeField]
    private GameObject missileContentObject;
    [SerializeField]
    private Missile missilePrefab;
    [SerializeField]
    private Missile strongMissilePrefab;
    [SerializeField]
    private float spawnRange;

    public float damage { get { return damage_initial + damage_upPerGrade * GameManager.Instance.playerData.playerAttackUpgradeLevel; } }
    public int cost
    {
        get
        {
            float value = upgradeCost_initial;
            for(int i=0; i<GameManager.Instance.playerData.playerAttackUpgradeLevel; i++)
            {
                value = value * (1 + (upgradeCostRate_byLevel / 100));
            }
            return (int)value;
        }
    }
    [Space(8)]
    [SerializeField]
    private float damage_initial = 3f;
    [SerializeField]
    private float damage_upPerGrade = 2f;
    [SerializeField]
    private int upgradeCost_initial = 100;
    [SerializeField]
    private float upgradeCostRate_byLevel = 30;

    private float strongFirePercent
    {
        get
        {
            float value = strongFirePercent_initial;
            for(int i=0; i<GameManager.Instance.playerData.playerAttackUpgradeLevel; i++)
            {
                value = value + (strongFirePercent_upPerRate);
            }
            if (value > 100)
                value = 100;
            return value;
        }
    }
    [SerializeField]
    private float strongFirePercent_initial;
    [SerializeField]
    private float strongFirePercent_upPerRate;

    private const float shotCooltime = 0.12f;
    private float timer = 0;

    public delegate void UITouchEvent();
    public static UITouchEvent uiTouchEvent;

    //===
    void Awake()
    {
        upgradeButton.onClick.AddListener(delegate { OnClick_Upgrade(); });
    }

    public void Initialize()
    {
        UpdateUI();
    }

    //===
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && uiTouchEvent != null)
        {
            uiTouchEvent();
            uiTouchEvent = null;
        }

        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (Input.GetMouseButtonDown(0) && timer <= 0)
        {
            Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 spawnPos = Camera.main.ScreenToWorldPoint(pos);
            RaycastHit2D[] hits = Physics2D.RaycastAll(spawnPos, Vector2.zero);
            for(int i=0; i<hits.Length; i++)
            {
                if (hits[i])
                {
                    if(hits[i].transform.gameObject.tag == "FloorTouch")
                    {
                        Hero target = null;
                        Transform heroContent = hits[i].transform.parent.Find("Heroes");
                        for(int idx=0; idx<heroContent.childCount; idx++)
                        {
                            if (heroContent.GetChild(idx).GetComponent<Hero>())
                            {
                                Hero tg = heroContent.GetChild(idx).GetComponent<Hero>();
                                if (tg.isAlive)
                                {
                                    if (target == null) { target = tg; }
                                    else if (target.transform.position.x < tg.transform.position.x)
                                    {
                                        target = tg;
                                    }
                                }
                            }
                        }
                        Vector2 firePos = new Vector2(spawnPos.x + Random.Range(-spawnRange, +spawnRange),
                            spawnPos.y + Random.Range(-spawnRange, +spawnRange));
                        EffectHandler.Instance.SpellEffect("spawnMissile", firePos, "PlayerAttack", 0);
                        if (target != null)
                        {
                            timer = shotCooltime;
                            Missile playerAttack;
                            float value = Random.Range(0, 100);
                            if(value >= 50 - strongFirePercent/2 && value <= 50 + strongFirePercent)
                            {
                                playerAttack = Instantiate(strongMissilePrefab);
                                playerAttack.isStrong = true;
                            }
                            else
                            {
                                playerAttack = Instantiate(missilePrefab);
                            }
                            playerAttack.transform.position = firePos;
                            playerAttack.transform.SetParent(missileContentObject.transform);
                            playerAttack.Fire(target);
                        }
                        return;
                    }
                }
            }
        }
    }

    public void OnClick_Upgrade()
    {
        if(GameManager.Instance.playerData.gold < cost)
        {
            EffectHandler.Instance.SystemMessage("Not enough gold");
        }
        else
        {
            GameManager.Instance.playerData.gold -= cost;

            EffectHandler.Instance.ImageEffect("sparkling", upgradeButton.transform.position);
            GameManager.Instance.playerData.playerAttackUpgradeLevel += 1;
            UpdateUI();
            AudioHandler.Instance.PlaySFX(AudioHandler.Instance.sfxFolder[0]);
            GameManager.Instance.SaveGame();
        }
    }
    void UpdateUI()
    {
        upgradeLevelTextMesh.text = "Lv. " + GameManager.Instance.playerData.playerAttackUpgradeLevel.ToString();
        contentTextMesh.text = "Magic Missile<size=25%>\n"
            + "<size=80%>damage : " + damage.ToString() + "\n"
            + "crit hit chance : " + strongFirePercent.ToString() + "%";
        costTextMesh.text = "<sprite=3> " + cost.ToString();
    }
}
