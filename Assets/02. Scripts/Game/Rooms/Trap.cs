using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public static float FireCooltime = 4f;

    public static float EnhancedRate = 1f;
    public static int ExtraAttackCount = 0;

    [SerializeField]
    private Radar radar;

    [Space(8)]
    public TrapForm form;
    [SerializeField]
    private float damage = 0;
    public float Damage { get { return damage; } }
    [SerializeField]
    private int hitCount = 0;

    private float fireCooltimer;

    private SpriteRenderer renderer;
    private Rigidbody2D rig;
    private Vector3 spawnPos;
    private Vector3 spawnLocalPos;

    private const float damageInterval = 0.2f;
    private const float floating_distance = 0.13f;
    private const float floatingSpeed = 0.0025f;

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

        ApplyForm(DataHandler.Instance.GetTrapForm(form_id));

        Vector3 pos = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        spawnPos = pos;
        spawnLocalPos = new Vector3(-1.97f, -1, 0);
        transform.position = spawnPos;
        transform.localPosition = new Vector3(transform.localPosition.x, spawnLocalPos.y, transform.localPosition.x);

        fireCooltimer = float.Parse(values[3]);
    }

    //===
    public void Create(TrapForm form, Vector3 spawnPos)
    {
        if (GameManager.Instance.playerData.isNewBegin)
            EnhancedRate = 1f;

        fireCooltimer = FireCooltime;
        this.spawnPos = spawnPos;
        ApplyForm(form);
        transform.position = spawnPos;

        spawnLocalPos = new Vector3(-1.97f, -1, 0);
        transform.localPosition = transform.localPosition = new Vector3(transform.localPosition.x, spawnLocalPos.y, transform.localPosition.x);
    }
    public void ApplyForm(TrapForm form)
    {
        this.form = form;
        damage = form.damage * EnhancedRate;
        hitCount = form.hitCount;

        renderer.sprite = form.sprite;
        gameObject.AddComponent<BoxCollider2D>();

        StartCoroutine(thr_fire());
        if (form.isFloating) { StartCoroutine(thr_floating()); }
        DataHandler.Instance.playerTraps.Add(this);
        if (DataHandler.Instance.dataUpdated != null)
        {
            DataHandler.Instance.dataUpdated();
        }
    }
    public void ChangeForm(TrapForm form)
    {
        this.form = form;
        renderer.sprite = form.sprite;
        Destroy(GetComponent<BoxCollider2D>());
        gameObject.AddComponent<BoxCollider2D>();
    }
    public void ReStat()
    {
        damage = form.damage * EnhancedRate;
        hitCount = form.hitCount;
    }

    //===
    IEnumerator thr_fire()
    {
        while (true)
        {
            fireCooltimer -= Time.deltaTime;
            if(fireCooltimer <= 0)
            {
                fireCooltimer = FireCooltime;

                Vector3 pos = transform.position;
                pos += new Vector3(form.spell_offset_x, form.spell_offset_y + 
                    (spawnLocalPos.y - transform.localPosition.y)+0.25f, 0);
                EffectHandler.Instance.SpellEffect(form.spellName, pos);
                List<Hero> targets = radar.targetHeroes;
                foreach(Hero hero in targets)
                {
                    float dm = damage / form.hitCount;
                    StartCoroutine(thr_multiDamage(hero, dm, form.hitCount + ExtraAttackCount));
                }
            }
            yield return null;
        }
    }
    IEnumerator thr_floating()
    {
        float stand_posY = spawnLocalPos.y + form.floating_Offset_y;
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
                Vector3 radarPos = radar.transform.localPosition;
                radarPos.y -= floatingSpeed;
                radar.transform.localPosition = radarPos;
            }
            else
            {
                Vector3 pos = transform.localPosition;
                pos.y -= floatingSpeed;
                transform.localPosition = pos;
                if(transform.localPosition.y <= stand_posY - floating_distance)
                {
                    isUp = true;
                }
                Vector3 radarPos = radar.transform.localPosition;
                radarPos.y += floatingSpeed;
                radar.transform.localPosition = radarPos;
            }
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).transform.localPosition = new Vector3(0, -0.2f, 0);
            yield return null;
        }
    }

    IEnumerator thr_multiDamage (Hero target, float damage, int count)
    {
        for(int i=0; i<count; i++)
        {
            target.TakeDamageWithoutForce(damage);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
