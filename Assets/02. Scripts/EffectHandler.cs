using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EffectHandler : MonoBehaviour {

    private const float removeWait = 2f;

    [Header("[  UI Effects")]
    [SerializeField]
    private Animator systemMessage;
    [SerializeField]
    private TextMeshProUGUI messageTextMesh;
    [Space(6)]
    [SerializeField]
    private Image uiEffectPrefab;
    [Space(6)]
    [SerializeField]
    private Image shadowBgImage;
    [SerializeField]
    private Animator flashEffectAnimCon;

    [Space(8)]
    [Header("[  Text Popup")]
    [SerializeField]
    private Effect textEffectPrefab;
    [SerializeField]
    private float textOffset_x;
    [SerializeField]
    private float textOffset_y;

    [Space(8)]
    [Header("[  Spell")]
    [SerializeField]
    private Effect spellEffectPrefab;

    //---------------Instance------------------
    private static EffectHandler _instance = null;
    public static EffectHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(EffectHandler)) as EffectHandler;
                if (_instance == null) { Debug.Log("EffectHandler가 없음"); }
            }
            return _instance;
        }
    }
    //-----------------------------------------

    public void TextPopup(string text, Transform target, Color color)
    {
        StartCoroutine(thr_textPopup(text, target, color, 1f));
    }
    public void TextPopup(string text, Transform target, Color color, float scale)
    {
        StartCoroutine(thr_textPopup(text, target, color, scale));
    }
    public void TextPopup(string text, TMP_SpriteAsset asset, Transform target, Color color, float scale)
    {
        StartCoroutine(thr_textPopup(text, asset, target, color, scale));
    }
    public void SpellEffect(string animName, Vector3 pos)
    {
        StartCoroutine(thr_spellEffect(animName, pos, "", 0));
    }
    public void SpellEffect(string animName, Vector3 pos, string sortingLayer, int sortOrder)
    {
        StartCoroutine(thr_spellEffect(animName, pos, sortingLayer, sortOrder));
    }

    public void PlayFlashEffect()
    {
        shadowBgImage.gameObject.SetActive(true);
        flashEffectAnimCon.enabled = true;
        flashEffectAnimCon.Play("flashEffect");
    }
    public void StopFlashEffect()
    {
        shadowBgImage.gameObject.SetActive(false);
        flashEffectAnimCon.enabled = false;
    }

    public void ImageEffect(string animName, Vector3 pos)
    {

    }

    public void SystemMessage(string text)
    {
        messageTextMesh.text = text;
        systemMessage.Play("messagePopup", -1, 0f);
    }

    //===
    IEnumerator thr_textPopup(string text, Transform target, Color color, float scale)
    {
        Effect effect = Instantiate(textEffectPrefab);
        Animator anim = effect.anim;
        GameObject effectObj = effect.gameObject;

        Vector2 pos = target.position;
        pos += new Vector2(textOffset_x, textOffset_y);
        effect.transform.SetParent(transform);
        effect.transform.position = pos;

        Vector3 scalePos = effect.transform.localScale * scale;
        effect.transform.localScale = scalePos;
 
        TextMeshPro mesh = anim.GetComponent<TextMeshPro>();
        mesh.text = text;
        mesh.color = color;

        anim.Play("textPopup");
        yield return new WaitForSeconds(removeWait);
        if (effectObj) { Destroy(effectObj); }
    }
    IEnumerator thr_textPopup(string text, TMP_SpriteAsset asset, Transform target, Color color, float scale)
    {
        Effect effect = Instantiate(textEffectPrefab);
        Animator anim = effect.anim;
        GameObject effectObj = effect.gameObject;

        Vector2 pos = target.position;
        pos += new Vector2(textOffset_x, textOffset_y);
        pos.y += 0.43f;
        effect.transform.SetParent(transform);
        effect.transform.position = pos;

        Vector3 scalePos = effect.transform.localScale * scale;
        effect.transform.localScale = scalePos;

        TextMeshPro mesh = anim.GetComponent<TextMeshPro>();
        mesh.text = text;
        mesh.color = color;
        mesh.spriteAsset = asset;
        mesh.GetComponent<AssignLayer>().ChangeOrder(10);

        anim.Play("textPopup");
        yield return new WaitForSeconds(removeWait);
        if (effectObj) { Destroy(effectObj); }
    }
    IEnumerator thr_spellEffect(string animName, Vector3 pos, string sortingLayer, int sortOrder)
    {
        Effect effect = Instantiate(spellEffectPrefab);
        Animator anim = effect.anim;
        GameObject effectObj = effect.gameObject;

        effect.transform.SetParent(transform);
        effect.transform.position = pos;

        if(sortingLayer != "")
        {
            SpriteRenderer renderer = anim.GetComponent<SpriteRenderer>();
            renderer.sortingLayerName = sortingLayer;
            renderer.sortingOrder = sortOrder;
        }

        anim.Play(animName);
        yield return new WaitForSeconds(removeWait);
        if (effectObj) { Destroy(effectObj); }
    }
}
