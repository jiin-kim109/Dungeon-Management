using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat_Mode : MonoBehaviour
{
    public void Init()
    {

    }
    public void Apply()
    {
        GameManager.Instance.playerData.cheat_isAllowed = true;
        GameManager.Instance.playerData.cheat_isActive = true;
    }
    public void Load()
    {
        if (GameManager.Instance.playerData.cheat_isAllowed)
            GameManager.Instance.playerData.cheat_isActive = true;
    }
}
