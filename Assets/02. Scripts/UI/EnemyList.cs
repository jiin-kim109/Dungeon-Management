using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyList : MonoBehaviour {

    [SerializeField]
    private GameObject slotPrefab;
    [SerializeField]
    private Transform listContent;

    public void DisplayEnemyList(List<HeroForm> forms)
    {
        for(int i=0; i< listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        for(int i=0; i<forms.Count; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab);
            slotObj.transform.SetParent(listContent);

            string enemyName = forms[i].name.Substring(4);
            slotObj.name = enemyName;
            slotObj.GetComponentInChildren<Image>().sprite = forms[i].sprite;
            slotObj.GetComponentInChildren<TextMeshProUGUI>().text = enemyName;
        }
    }

    public void RemoveElements()
    {
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }
    }
}
