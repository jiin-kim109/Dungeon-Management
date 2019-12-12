using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

    public static float RespawnCooltime = 10f;
    public static float HealthRegenCooltime = 1f;
    public static float HealthRegenPercentage = 4f;
    public static float SpellCooltimeRate = 1f;

    public static float EnhancedRate = 1f;

    [SerializeField]
    private Radar radar;
    [SerializeField]
    private SplashDamage splashDamage;

    [Space(8)]
    [SerializeField]
    private HpBar hpBar;
    [SerializeField]
    private GameObject hpBarObj;
    [HideInInspector]
    public BossForm form;

    [HideInInspector]
    public float maxHp;
    [SerializeField]
    private float hp = 0;
    [SerializeField]
    private float damage = 0;

    private float respawnCooltimer;
    private float healthRegenCooltimer = HealthRegenCooltime;
    private float spellCooltimer;

    private bool isAlive;

    private SpriteRenderer renderer;
    private Rigidbody2D rig;
    private Vector3 spawnPos;

    private const float hitVector_y = 3.0f;

    //===
    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
    }

    public void Save(string save_id)
    { //(pos_x, pos_y, pos_z, respawnCooltimer, isAlive)
        string value = "(";
        value += spawnPos.x.ToString() + ", ";
        value += spawnPos.y.ToString() + ", ";
        value += spawnPos.z.ToString() + ", ";
        value += respawnCooltimer.ToString() + ", ";
        value += isAlive.ToString() + ")";

        PlayerPrefs.SetString(save_id, value);
    }
    public void Load(string form_id, string save_id)
    { //(pos_x, pos_y, pos_z, respawnCooltimer, isAlive)
        string value = PlayerPrefs.GetString(save_id);
        value = value.Trim(new char[] { '(', ')' });
        value = value.Replace(" ", "");
        string[] values = value.Split(',');
        Vector3 pos = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        spawnPos = pos;
        transform.position = pos;

        ApplyForm(DataHandler.Instance.GetBossForm(form_id));

        respawnCooltimer = float.Parse(values[3]);
        isAlive = bool.Parse(values[4]);
        if (!isAlive) { Dead(); }
        return;
    }

    //===
    public void Create(BossForm form, Vector3 spawnPos)
    {
        ApplyForm(form);
        respawnCooltimer = RespawnCooltime;
        isAlive = true;

        this.spawnPos = spawnPos;
        transform.position = spawnPos;
    }
    void ApplyForm(BossForm form)
    {
        this.form = form;
        maxHp = form.hp * EnhancedRate;
        hp = maxHp;
        hpBar.UpdateBar(hp, maxHp);
        damage = form.damage * EnhancedRate;

        spellCooltimer = form.spellCooltime * SpellCooltimeRate;

        splashDamage.SetScale(form.damageRadiusScale);
        renderer.sprite = form.sprite;
        gameObject.AddComponent<BoxCollider2D>();

        StartCoroutine(thr_healthRegen());
        StartCoroutine(thr_bossSpell());
        DataHandler.Instance.playerBosses.Add(this);
        if (DataHandler.Instance.dataUpdated != null)
        {
            DataHandler.Instance.dataUpdated();
        }
    }
    public void ReStat()
    {
        maxHp = form.hp * EnhancedRate;
        damage = form.damage * EnhancedRate;
    }

    //===
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Heroes")
        {
            Hero hero = col.gameObject.GetComponent<Hero>();
            if (!isAlive) { Physics2D.IgnoreCollision(col.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>()); return; }
            if (hero.isAir) { Physics2D.IgnoreCollision(col.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>()); return; }
            hero.TakeDamage(damage);
            TakeDamage(hero.status.damage);
        }
    }

    //===
    void TakeDamage(float damage)
    {
        hp -= damage;
        hpBar.UpdateBar(hp, maxHp);
        int value = (int)damage;
        EffectHandler.Instance.TextPopup(value.ToString(), transform, Color.red);
        if (hp <= 0)
        {
            Dead();
        }
        else if (rig.velocity.y <= 0)
        {
            rig.velocity = new Vector2(0, hitVector_y);
        }
    }

    void Dead()
    {
        StartCoroutine(thr_respawn());
    }
    IEnumerator thr_respawn()
    {
        isAlive = false;
        Color colorOrigin = renderer.color;
        Color color = renderer.color;

        float speed = 0.05f;
        while (true)
        {
            color.r -= speed;
            color.g -= speed;
            color.b -= speed;
            if (color.r < 0) { color.r = 0; }
            if (color.g < 0) { color.g = 0; }
            if (color.b < 0) { color.b = 0; }
            renderer.color = color;

            if (color.r <= 0 && color.g <= 0 && color.b <= 0)
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.8f);
        renderer.enabled = false;
        hpBarObj.gameObject.SetActive(false);

        respawnCooltimer = RespawnCooltime;
        while (true)
        {
            respawnCooltimer -= Time.deltaTime;
            if (respawnCooltimer < 0)
            {
                respawnCooltimer = RespawnCooltime;
                break;
            }
            yield return null;
        }

        renderer.enabled = true;
        renderer.color = colorOrigin;
        hp = form.hp * EnhancedRate;
        isAlive = true;
        hpBarObj.gameObject.SetActive(true);
    }
    IEnumerator thr_healthRegen()
    {
        healthRegenCooltimer = HealthRegenCooltime;
        while (true)
        {
            healthRegenCooltimer -= Time.deltaTime;
            if (!isAlive)
            {
                healthRegenCooltimer = HealthRegenCooltime;
                while (!isAlive) { yield return null; }
            }
            else if (healthRegenCooltimer <= 0)
            {
                hp += hp * (HealthRegenPercentage / 100);
                hpBar.UpdateBar(hp, maxHp);
                if (hp > maxHp)
                {
                    hp = maxHp;
                }
                healthRegenCooltimer = HealthRegenCooltime;
            }
            yield return null;
        }
    }

    IEnumerator thr_bossSpell()
    {
        while (true)
        {
            while(!isAlive) { yield return null; }
            spellCooltimer -= Time.deltaTime;
            if (spellCooltimer <= 0)
            {
                spellCooltimer = form.spellCooltime * SpellCooltimeRate;

                List<Hero> targets = radar.targetHeroes;
                if (targets.Count > 0)
                {
                    bool flag = false;
                    Hero hero = targets[Random.Range(0, targets.Count)]; ;
                    while (true)
                    {
                        if (!hero.isAir && hero.isAlive) { break; }
                        else
                        {
                            List<Hero> newList = new List<Hero>();
                            for(int i=0; i<targets.Count; i++)
                            {
                                if (targets[i] != null && targets[i].isAlive)
                                    newList.Add(targets[i]);
                            }
                            targets = newList;
                            if(targets.Count == 0) { flag = true; break; }
                            hero = targets[Random.Range(0, targets.Count)];
                        }
                        yield return null;
                    }
                    if (flag) { continue; }

                    Vector2 pos = hero.transform.position;
                    pos += new Vector2(form.spell_offset_x, form.spell_offset_y);
                    EffectHandler.Instance.SpellEffect(form.spellName, pos);

                    splashDamage.GiveSplashDamage(hero, pos, damage * (form.damageRateToSpell / 100));
                }
            }
            yield return null;
        }
    }

    public void Relieve()
    {
        if (isAlive)
        {
            hp = maxHp;
            hpBar.UpdateBar(hp, maxHp);
        }
        else
        {
            respawnCooltimer = 0.5f;
        }
    }
}
