using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField]
    private Button openButton;
    [SerializeField]
    private GameObject blackBackgroundImg;
    [SerializeField]
    private string menuSceneName;
    [SerializeField]
    private string bgImgAnimName;

    [Space(10)]
    [SerializeField]
    private Button cashShopButton;
    [SerializeField]
    private Button mainMenuButton;
    [SerializeField]
    private Button creditButton;
    [SerializeField]
    private Button cancelButton;

    [Space(10)]
    [SerializeField]
    private GameObject menuTab;
    [SerializeField]
    private GameObject cashTab;
    [SerializeField]
    private GameObject creditTab;

    private bool offFunction = false;

    private bool isOn = false;
    private bool isCash = false;
    private bool isCredit = false;

    void Start()
    {
        isOn = false;
        menuTab.SetActive(false);
        cashTab.SetActive(false);
        creditTab.SetActive(false);
        blackBackgroundImg.SetActive(false);

        openButton.onClick.AddListener(delegate { OnClick_OpenButton(); });

        cashShopButton.onClick.AddListener(delegate { OnClick_CashButton(); });
        creditButton.onClick.AddListener(delegate { OnClick_CreditButton(); });
        mainMenuButton.onClick.AddListener(delegate { OnClick_MainMenuButton(); });
        cancelButton.onClick.AddListener(delegate { OnClick_CancelButton(); });
    }

    private void DeactiveAll()
    {
        isOn = false;
        isCash = false;
        isCredit = false;
        blackBackgroundImg.SetActive(false);
        menuTab.SetActive(false);
        cashTab.SetActive(false);
        creditTab.SetActive(false);
    }
    private void CloseTabsExceptMenu()
    {
        isCash = false;
        isCredit = false;
        cashTab.SetActive(false);
        creditTab.SetActive(false);
    }

    public void OnClick_OpenButton()
    {
        if (offFunction) { return; }
        if (isOn)
        {
            DeactiveAll();
        }
        else
        {
            isOn = true;
            blackBackgroundImg.SetActive(true);
            menuTab.SetActive(true);
        }
    }

    public void OnClick_CashButton()
    {
        if (offFunction) { return; }
        if (isCash)
        {
            isCash = false;
            cashTab.SetActive(false);
            menuTab.SetActive(true);
        }
        else
        {
            CloseTabsExceptMenu();
            isCash = true;
            menuTab.SetActive(false);
            cashTab.SetActive(true);
        }
    }

    public void OnClick_CreditButton()
    {
        if (offFunction) { return; }
        if (isCredit)
        {
            isCredit = false;
            creditTab.SetActive(false);
        }
        else
        {
            CloseTabsExceptMenu();
            isCredit = true;
            creditTab.SetActive(true);
        }
    }

    public void OnClick_MainMenuButton()
    {
        if (offFunction) { return; }
        StartCoroutine(thr_backToMenu());
    }
    IEnumerator thr_backToMenu()
    {
        DeactiveAll();
        blackBackgroundImg.SetActive(true);
        offFunction = true;
        
        Animator anim = blackBackgroundImg.GetComponent<Animator>();
        anim.Play(bgImgAnimName);

        AudioHandler.Instance.StopAudio();
        UIHandler.Instance.admobManager.DestroyAdmob();

        float time = 2.5f;
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(menuSceneName);
    }

    public void OnClick_CancelButton()
    {
        DeactiveAll();
    }
}
