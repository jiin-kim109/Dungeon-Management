using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [SerializeField]
    private string gameSceneName;
    [SerializeField]
    private AudioHandler audioHandler;

    [Space(10)]
    [SerializeField]
    private AudioClip buttonClickSound;
    [SerializeField]
    private float waitingTime = 2f;

    [Space(10)]
    [SerializeField]
    private Animator buttonAnim;
    [SerializeField]
    private Button newGameButton;
    [SerializeField]
    private string buttonClickedAnimName;
    [SerializeField]
    private Color buttonChangeColor;
    [SerializeField]
    private Color newGameChangeColor;

    void Start()
    {
        audioHandler.Initialize(1);
    }

    public void OnClick_PlayGame()
    {
        buttonAnim.Play(buttonClickedAnimName);
        buttonAnim.GetComponent<Image>().color = buttonChangeColor;
        StartCoroutine(thr_playGame());
    }

    public void OnClick_PlayNewGame()
    {
        PlayerData currentData = new PlayerData();
        currentData = SaveLoad.LoadPlayerData(currentData);
        int stock = currentData.cashStock;
        bool cheat = currentData.cheat_isAllowed;
        SaveLoad.DeletePlayerData();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("CashStock", stock);
        if (cheat)
            PlayerPrefs.SetInt("Cheat", 1);
        else
            PlayerPrefs.SetInt("Cheat", 0);
        newGameButton.GetComponent<Image>().color = newGameChangeColor;
        StartCoroutine(thr_playGame());
    }

    IEnumerator thr_playGame()
    {
        audioHandler.PlaySFX(buttonClickSound);

        yield return new WaitForSeconds(waitingTime);
        Destroy(audioHandler.gameObject);
        SceneManager.LoadScene(gameSceneName);
    }
}
