using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomUI : MonoBehaviour {

    [SerializeField]
    private Button dungeonUpgrade_button;
    [SerializeField]
    private GameObject dungeonUpgrade_menu;

    [Space(8)]
    [SerializeField]
    private Button monsterUpgrade_button;
    [SerializeField]
    private GameObject monsterUpgrade_menu;

    [Space(8)]
    [SerializeField]
    private Button wispUpgrade_button;
    [SerializeField]
    private GameObject wispUpgrade_menu;

    [Space(8)]
    [SerializeField]
    private Button totemUpgrade_button;
    [SerializeField]
    private GameObject totemUpgrade_menu;

    private int currentMenuIndex = -1;
    private float waitDelay = 0.08f;
    private Coroutine menuCo;

    //===
    void Awake()
    {
        dungeonUpgrade_button.onClick.AddListener(delegate { OnClick(0); });
        monsterUpgrade_button.onClick.AddListener(delegate { OnClick(1); });
        wispUpgrade_button.onClick.AddListener(delegate { OnClick(2); });
        totemUpgrade_button.onClick.AddListener(delegate { OnClick(3); });

        CloseAllMenu();
    }
    void CloseAllMenu()
    {
        dungeonUpgrade_menu.gameObject.SetActive(false);
        monsterUpgrade_menu.gameObject.SetActive(false);
        wispUpgrade_menu.gameObject.SetActive(false);
        totemUpgrade_menu.gameObject.SetActive(false);
    }

    //===
    public void OnClick(int index)
    {
        if(menuCo != null) { StopCoroutine(menuCo); }
        menuCo = StartCoroutine(thr_openMenu(index));
    }
    IEnumerator thr_openMenu(int index)
    {
        CloseAllMenu();
        if (index == currentMenuIndex)
        {
            currentMenuIndex = -1;
        }
        else
        {
            yield return new WaitForSeconds(waitDelay);
            switch (index)
            {
                case 0:
                    dungeonUpgrade_menu.gameObject.SetActive(true);
                    break;
                case 1:
                    monsterUpgrade_menu.gameObject.SetActive(true);
                    break;
                case 2:
                    wispUpgrade_menu.gameObject.SetActive(true);
                    break;
                case 3:
                    totemUpgrade_menu.gameObject.SetActive(true);
                    break;
            }
            currentMenuIndex = index;
        }
        yield return null;
    }
}
