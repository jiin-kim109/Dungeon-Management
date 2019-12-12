using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curse : MonoBehaviour {

    public static float FireCooltime = 3f;
    public static float CurseDuration = 4f;

    public static float ApplyRateEnhancedRate = 1f;
    public static float SpeedSetEnhancedRate = 1f;
    public static float DamageSetEnhancedRate = 1f;

    [SerializeField]
    private Radar radar;

    [Space(8)]
    public CurseForm form;
    private float curseApplyRate;
    public float CurseApplyRate
    {
        get
        {
            return curseApplyRate;
        }
        set
        {
            float rate = value;
            if(rate > 100) { rate = 100; }
            curseApplyRate = rate;
        }
    }
    private float speedSetRate;
    public float SpeedSetRate
    {
        get
        {
            return speedSetRate;
        }
        set
        {
            float rate = value;
            if(rate > 100) { rate = 100; }
            speedSetRate = rate;
        }
    }
    public float damageSetRate;

    private float fireCooltimer;

    private SpriteRenderer renderer;
    private Rigidbody2D rig;
    private Vector3 spawnPos;

    private const float floating_distance = 0.12f;
    private const float floatingSpeed = 0.0015f;

    //===
    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
    }

    public void Save(string save_id)
    { //(pos_x, pos_y, pos_z, cooltimer)
        string value = "(";
        value += spawnPos.x.ToString() + ", ";
        value += spawnPos.y.ToString() + ", ";
        value += spawnPos.z.ToString() + ", ";
        value += fireCooltimer.ToString() + ")";

        PlayerPrefs.SetString(save_id, value);
    }
    public void Load(string form_id, string save_id)
    { //(pos_x, pos_y, pos_z, cooltimer)
        string value = PlayerPrefs.GetString(save_id);
        value = value.Trim(new char[] { '(', ')' });
        value = value.Replace(" ", "");
        string[] values = value.Split(',');

        Vector3 pos = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        spawnPos = pos;
        transform.position = pos;

        fireCooltimer = float.Parse(values[3]);
        ApplyForm(DataHandler.Instance.GetCurseForm(form_id));
    }

    //===
    public void Create(CurseForm form, Vector3 spawnPos)
    {
        if (GameManager.Instance.playerData.isNewBegin)
        {
            ApplyRateEnhancedRate = 1f;
            SpeedSetEnhancedRate = 1f;
            DamageSetEnhancedRate = 1f;
        }

        fireCooltimer = FireCooltime;
        this.spawnPos = spawnPos;
        ApplyForm(form);
        transform.position = spawnPos;
    }
    public void ApplyForm(CurseForm form)
    {
        this.form = form;
        CurseApplyRate = form.curseApplyRate * ApplyRateEnhancedRate;
        SpeedSetRate = form.speedSetRate * SpeedSetEnhancedRate;
        damageSetRate = form.damageSetRate * DamageSetEnhancedRate;

        renderer.sprite = form.sprite;
        gameObject.AddComponent<BoxCollider2D>();

        StartCoroutine(thr_fire());
        if (form.isFloating) { StartCoroutine(thr_floating()); }
        DataHandler.Instance.playerCurses.Add(this);
        if (DataHandler.Instance.dataUpdated != null)
        {
            DataHandler.Instance.dataUpdated();
        }
    }
    public void ChangeForm(CurseForm form)
    {
        this.form = form;
        renderer.sprite = form.sprite;
        Destroy(GetComponent<BoxCollider2D>());
        gameObject.AddComponent<BoxCollider2D>();
    }
    public void ReStat()
    {
        CurseApplyRate = form.curseApplyRate * ApplyRateEnhancedRate;
        SpeedSetRate = form.speedSetRate * SpeedSetEnhancedRate;
        damageSetRate = form.damageSetRate * DamageSetEnhancedRate;
    }

    //===
    IEnumerator thr_fire()
    {
        while (true)
        {
            fireCooltimer -= Time.deltaTime;
            if (fireCooltimer <= 0)
            {
                fireCooltimer = FireCooltime;

                Vector3 pos = transform.position;
                pos += new Vector3(form.spell_offset_x, form.spell_offset_y, 0);
                EffectHandler.Instance.SpellEffect(form.spellName, pos);
                List<Hero> targets = radar.targetHeroes;
                foreach (Hero hero in targets)
                {
                    float rate = Random.Range(0, 100);
                    if(rate <= curseApplyRate)
                    {
                        hero.GetCursed(speedSetRate/100, damageSetRate/100);
                    }
                }
            }
            yield return null;
        }
    }
    IEnumerator thr_floating()
    {
        float stand_posY = spawnPos.y + form.floating_Offset_y;
        rig.bodyType = RigidbodyType2D.Kinematic;
        transform.localPosition = new Vector3(transform.localPosition.x, stand_posY, transform.localPosition.z);

        bool isUp = true;
        while (true)
        {
            if (isUp)
            {
                Vector3 pos = transform.localPosition;
                pos.y += floatingSpeed;
                transform.localPosition = pos;
                if (transform.localPosition.y >= stand_posY + floating_distance)
                {
                    isUp = false;
                }
                Vector3 radarPos = radar.transform.position;
                radarPos.y -= floatingSpeed;
                radar.transform.position = radarPos;
            }
            else
            {
                Vector3 pos = transform.localPosition;
                pos.y -= floatingSpeed;
                transform.localPosition = pos;
                if (transform.localPosition.y <= stand_posY - floating_distance)
                {
                    isUp = true;
                }
                Vector3 radarPos = radar.transform.position;
                radarPos.y += floatingSpeed;
                radar.transform.position = radarPos;
            }
            yield return null;
        }
    }
}
