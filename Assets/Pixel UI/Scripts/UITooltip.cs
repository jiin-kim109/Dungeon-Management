/******************************************************************************************
 * Name: UITooltip.cs
 * Created by: Jeremy Voss
 * Created on: 02/20/2019
 * Last Modified: 02/27/2019
 * Description:
 * Used to display tooltips while the cursor is hovered over an object.
 ******************************************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace PixelsoftGames.PixelUI
{
    public class UITooltip : MonoBehaviour
    {
        #region Singleton Implementation

        static UITooltip _Instance = null;
        public static UITooltip Instance { get { return _Instance; } }

        #endregion

        #region Fields & Properties

        [SerializeField]
        [Tooltip("The text being displayed by this tooltip.")]
        Text tooltip = null;

        #endregion

        #region Monobehavior Callbacks

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                _Instance = this;
        }

        private void OnEnable()
        {
            transform.position = Input.mousePosition;
            if (Input.mousePosition.x > Screen.width / 2)
                GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
            else
                GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Input.mousePosition;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                _Instance = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the text for the tooltip.
        /// </summary>
        /// <param name="text">The text to be displayed.</param>
        public void SetText(string text)
        {
            if (tooltip != null)
                tooltip.text = text;
        }

        #endregion
    }
}