using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningManager : MonoBehaviour {

    [SerializeField]
    private string menuSceneName;
    [SerializeField]
    private float waittingTime;

    [Space(8)]
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private string slideAnimName;

    void Start()
    {
        StartCoroutine(thr_slideShow());
    }

    IEnumerator thr_slideShow()
    {
        anim.Play(slideAnimName);
        yield return new WaitForEndOfFrame();
        float time = anim.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(time + waittingTime);
        SceneManager.LoadScene(menuSceneName);
    }
}
