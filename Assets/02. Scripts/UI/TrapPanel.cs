using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrapPanel : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI trapCountTextMesh;
    [SerializeField]
    private TextMeshProUGUI costTextMesh;
    [SerializeField]
    private int upgradeCost
    {
        get
        {
            float value = upgradeCost_initial;
            for (int i = 0; i < GameManager.Instance.playerData.trapUpgradeCount; i++)
            {
                value = value * (1 + (upgradeCost_upRateByCount / 100));
            }
            return (int)value;
        }
    }
    [SerializeField]
    private float percentageMult = 1;
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
        PlayerPrefs.SetFloat("TrapEnhancedRate", Trap.EnhancedRate);
    }
    void Load()
    {
        if (GameManager.Instance.playerData.isNewBegin)
        {
            Trap.EnhancedRate = 1f;
        }
        else
        {
            Trap.EnhancedRate = PlayerPrefs.GetFloat("TrapEnhancedRate");
        }
    }

    //===
    void UpdateList()
    {
        List<Trap> trapList = DataHandler.Instance.playerTraps;
        if (trapList.Count < contentObj.transform.childCount)
        {
            for (int i = trapList.Count; i < contentObj.transform.childCount; i++)
            {
                contentObj.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else if (trapList.Count > contentObj.transform.childCount)
        {
            for (int i = contentObj.transform.childCount; i < trapList.Count; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab).gameObject;
                slotObj.transform.SetParent(contentObj.transform);
            }
        }

        for (int i = 0; i < trapList.Count; i++)
        {
            GameObject slotObj = contentObj.transform.GetChild(i).gameObject;
            slotObj.SetActive(true);
            slotObj.transform.Find("icon").GetComponent<Image>().sprite
                = trapList[i].form.sprite;
            int damage = (int)trapList[i].Damage;
            slotObj.transform.Find("contentTextMesh").GetComponent<TextMeshProUGUI>().text
                = trapList[i].form.name.Substring(4) + "\n<size=70%>\n<size=85%>damage : "
                + damage.ToString();
            TextMeshProUGUI gradeTextMesh = slotObj.transform.Find("gradeTextMesh").GetComponent<TextMeshProUGUI>();
            gradeTextMesh.text = trapList[i].form.grade.ToString();
            if (trapList[i].form.grade == Grade.X) { gradeTextMesh.text = "?"; }
            gradeTextMesh.color = DataHandler.Instance.GetGradeColor(trapList[i].form.grade);
        }

        trapCountTextMesh.text = "Possessing\nSpirits\n<size=140%>\n" + trapList.Count.ToString();
        costTextMesh.text = "Upgrade\n<sprite=3> " + upgradeCost.ToString();
    }

    //===
    public void OnClick_Upgrade()
    {
        if (DataHandler.Instance.playerTraps.Count <= 0)
        {
            EffectHandler.Instance.SystemMessage("No spirits to upgrade");
        }
        else if (GameManager.Instance.playerData.gold < upgradeCost)
        {
            EffectHandler.Instance.SystemMessage("Not enough gold");
        }
        else
        {
            GameManager.Instance.playerData.gold -= upgradeCost;
            GameManager.Instance.playerData.trapUpgradeCount++;

            Trap.EnhancedRate += enhancedRate_upRateByCount / 100;

            for (int i = 0; i < DataHandler.Instance.playerTraps.Count; i++)
            {
                GameObject slotObj = contentObj.transform.GetChild(i).gameObject;
                Animator anim = slotObj.GetComponent<Animator>();
                anim.Play("expand");

                Trap trap = DataHandler.Instance.playerTraps[i];
                float per = Random.Range(0, 100);
                float percent = DataHandler.Instance.GetGradeUpPercentage(trap.form.grade);
                percent = percent + (percent * (DataHandler.rankUpExtraRate / 100)) * percentageMult;
                if (per <= 50 + percent / 2 &&
                    per >= 50 - percent / 2)
                {
                    Grade targetGrade = (Grade)((int)trap.form.grade + 1);
                    List<TrapForm> supForms = DataHandler.Instance.GetTrapFormsAtGrade(targetGrade);
                    int index = Random.Range(0, supForms.Count);
                    trap.ChangeForm(supForms[index]);
                }
                trap.ReStat();
            }
            AudioHandler.Instance.PlaySFX(AudioHandler.Instance.sfxFolder[0]);
            GameManager.Instance.SaveGame();
            UpdateList();
        }
    }
}
