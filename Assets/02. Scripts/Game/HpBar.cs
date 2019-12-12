using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour {

    [SerializeField]
    private SpriteRenderer hpBar;
    private float scaleMax;

    void Awake()
    {
        scaleMax = hpBar.transform.localScale.x;
    }

    public void UpdateBar(float value, float max)
    {
        if(value > max) { value = max; }
        else if (value < 0) { value = 0; }

        Vector3 scaleVector = hpBar.transform.localScale;
        scaleVector.x = scaleMax * (value / max);
        hpBar.transform.localScale = scaleVector;
    }
}
