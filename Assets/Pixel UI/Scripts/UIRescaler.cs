/******************************************************************************************
 * Name: UIRescaler.cs
 * Created by: Jeremy Voss
 * Created on: 01/15/2019
 * Last Modified: 01/16/2019
 * Description:
 * A simple script that pingpongs the X and Y scales between 2 sizes at a particular speed,
 * in my demo I use this for the "breathing icon" effect of the light bulb icon.
 ******************************************************************************************/
using UnityEngine;

namespace PixelsoftGames.PixelUI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIRescaler : MonoBehaviour
    {
        #region Fields & Properties

        [SerializeField]
        [Tooltip("The minimum value to scale to")]
        float minScale = 1f;
        [SerializeField]
        [Tooltip("The maximum value to scale to")]
        float maxScale = 1.5f;
        [Range(1f, 0f)]
        [SerializeField]
        [Tooltip("The speed at which we move between minimum and maximum scales")]
        float speed;

        RectTransform rect;

        #endregion

        #region Monobehaviour Callbacks

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            float scale = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(Time.time / speed, 1f));
            rect.localScale = new Vector3(scale, scale, rect.localScale.z);
        }

        #endregion
    }
}