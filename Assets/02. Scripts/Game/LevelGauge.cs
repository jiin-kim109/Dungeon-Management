using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGauge : MonoBehaviour {

    private float maxScale_x;

    void Awake()
    {
        maxScale_x = transform.localScale.x;
    }

    public void UpdateBar(float currentValue, float maxValue)
    {
        if(currentValue < 0) { currentValue = 0; }
        float progress = currentValue / maxValue;

        float scale = maxScale_x * progress;
        transform.localScale = new Vector2(scale, transform.localScale.y);
    }
}
