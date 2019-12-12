using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold_And_Gem : MonoBehaviour
{
    [SerializeField]
    private int goldAmount;
    [SerializeField]
    private int gemAmount;

    private int loadableCount = 0;

    public void Init()
    {
        if (GameManager.Instance.playerData.isNewBegin)
        {
            loadableCount = GameManager.Instance.playerData.cashStock;
        }
    }
    public void Apply()
    {
        GameManager.Instance.playerData.gold += goldAmount;
        GameManager.Instance.playerData.gem += gemAmount;
        GameManager.Instance.playerData.cashStock++;
        GameManager.Instance.SaveGame();
        UIHandler.Instance.UpdateUI();
    }
    public void Load()
    {
        for (int i = 0; i < loadableCount; i++)
        {
            GameManager.Instance.playerData.gold += goldAmount;
            GameManager.Instance.playerData.gem += gemAmount;
        }
        loadableCount = 0;
        GameManager.Instance.SaveGame();
        UIHandler.Instance.UpdateUI();
    }
}
