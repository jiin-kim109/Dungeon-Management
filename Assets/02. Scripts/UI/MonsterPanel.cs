using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterPanel : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI monsterCountTextMesh;
    [SerializeField]
    private TextMeshProUGUI costTextMesh;
    [SerializeField]
    private float percentageMult = 1;
    [SerializeField]
    private int upgradeCost
    {
        get
        {
            float value = upgradeCost_initial;
            for(int i=0; i<GameManager.Instance.playerData.monsterUpgradeCount; i++)
            {
                value = value * (1 + (upgradeCost_upRateByCount / 100));
            }
            return (int)value;
        }
    }
    [SerializeField]
    private float upgradeCost_initial;
    [SerializeField]
    private float upgradeCost_upRateByCount;
    [Space(4)]
    [SerializeField]
    private float enhancedRate_upRateByCount;

    [Space(8)]
    [SerializeField]
    private GameObject slotPrefab;
    [SerializeField]
    private GameObject contentObj;


    //===
    public void Intialize()
    {
        Load();
        DataHandler.Instance.dataUpdated += UpdateList;
        UpdateList();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("MonsterEnhancedRate", Monster.EnhancedRate);
    }
    void Load()
    {
        if (GameManager.Instance.playerData.isNewBegin)
        {
            Monster.EnhancedRate = 1f;
        }
        else
        {
            Monster.EnhancedRate = PlayerPrefs.GetFloat("MonsterEnhancedRate");
        }
    }

    //===
    void UpdateList()
    {
        List<Monster> monsterList = DataHandler.Instance.playerMonsters;
        if(monsterList.Count < contentObj.transform.childCount)
        {
            for(int i=monsterList.Count; i<contentObj.transform.childCount; i++)
            {
                contentObj.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else if(monsterList.Count > contentObj.transform.childCount)
        {
            for(int i=contentObj.transform.childCount; i<monsterList.Count; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab).gameObject;
                slotObj.transform.SetParent(contentObj.transform);
            }
        }

        for(int i=0; i<monsterList.Count; i++)
        {
            GameObject slotObj = contentObj.transform.GetChild(i).gameObject;
            slotObj.SetActive(true);
            slotObj.transform.Find("icon").GetComponent<Image>().sprite 
                = monsterList[i].form.sprite;
            int hp = (int)monsterList[i].maxHp;
            int damage = (int)monsterList[i].Damage;
            slotObj.transform.Find("contentTextMesh").GetComponent<TextMeshProUGUI>().text
                = monsterList[i].form.name.Substring(5) + "\n<size=30%>\n<size=85%>hp : "
                + hp.ToString() + "\ndamage : "
                + damage.ToString();
            TextMeshProUGUI gradeTextMesh = slotObj.transform.Find("gradeTextMesh").GetComponent<TextMeshProUGUI>();
            gradeTextMesh.text = monsterList[i].form.grade.ToString();
            if(monsterList[i].form.grade == Grade.X) { gradeTextMesh.text = "?"; }
            gradeTextMesh.color = DataHandler.Instance.GetGradeColor(monsterList[i].form.grade);
        }

        monsterCountTextMesh.text = "Possessing\nMonsters\n<size=140%>" + monsterList.Count.ToString();
        costTextMesh.text = "Upgrade\n<sprite=3> " + upgradeCost.ToString();
    }

    //===
    public void OnClick_Upgrade()
    {
        if (DataHandler.Instance.playerMonsters.Count <= 0)
        {
            EffectHandler.Instance.SystemMessage("No monsters to upgrade");
        }
        else if (GameManager.Instance.playerData.gold < upgradeCost)
        {
            EffectHandler.Instance.SystemMessage("Not enough gold");
        }
        else
        {
            GameManager.Instance.playerData.gold -= upgradeCost;
            GameManager.Instance.playerData.monsterUpgradeCount++;

            Monster.EnhancedRate += enhancedRate_upRateByCount / 100;

            for (int i = 0; i < DataHandler.Instance.playerMonsters.Count; i++)
            {
                GameObject slotObj = contentObj.transform.GetChild(i).gameObject;
                Animator anim = slotObj.GetComponent<Animator>();
                anim.Play("expand");

                Monster monster = DataHandler.Instance.playerMonsters[i];
                float per = Random.Range(0, 100);
                float percent = DataHandler.Instance.GetGradeUpPercentage(monster.form.grade);
                percent = percent + (percent * (DataHandler.rankUpExtraRate / 100)) * percentageMult;
                if (per <= 50 + percent / 2 &&
                    per >= 50 - percent / 2)
                {
                    Grade targetGrade = (Grade)((int)monster.form.grade + 1);
                    List<MonsterForm> supForms = DataHandler.Instance.GetMonsterFormsAtGrade(targetGrade);
                    if (supForms.Count > 0)
                    {
                        int index = Random.Range(0, supForms.Count);
                        monster.ChangeForm(supForms[index]);
                        monster.ReStat(true);
                    }
                }
                else
                {
                    monster.ReStat(false);
                }
            }
            AudioHandler.Instance.PlaySFX(AudioHandler.Instance.sfxFolder[0]);
            GameManager.Instance.SaveGame();
            UpdateList();
        }
    }
}
