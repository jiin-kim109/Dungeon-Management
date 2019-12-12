using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField]
    private string compID;

    public void OnClick_ShowRank()
    {
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate(Authenticator);
    }

    private void Authenticator(bool isSuccess)
    {
        if (isSuccess)
        {
            int highScore = GameManager.Instance.playerData.score;
            Social.ReportScore((long)highScore, compID, (bool success) =>
            {
                if (success)
                {
                    PlayGamesPlatform.Instance.ShowLeaderboardUI(compID);
                }
                else
                {

                }
            });
        }
        else
        {

        }
    }
}
