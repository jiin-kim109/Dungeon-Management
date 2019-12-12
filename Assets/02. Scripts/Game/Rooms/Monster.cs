using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

    public static float RespawnCooltime = 6f;
    public const float HealthRegenCooltime = 1;
    public static float HealthRegenPercentage = 8f;

    public static float EnhancedRate = 1f;

    //===
    [SerializeField]
    private HpBar hpBar;
    [HideInInspector]
    public MonsterForm form;

    [HideInInspector]
    public float maxHp;
    [SerializeField]
    private float hp = 0;
    public float HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            hpBar.UpdateBar(hp, maxHp);
        }
    }
    [SerializeField]
    private float damage = 0;
    public float Damage { get { return damage; } }

    private float respawnCooltimer;
    private float healthRegenCooltimer = HealthRegenCooltime;

    [SerializeField]
    private bool isAlive;

    private SpriteRenderer renderer;
    private Rigidbody2D rig;
    private Vector3 spawnPos;

    private const float shrinkSpeed = 0.05f;
    private const float hitVector_y = 4.0f;

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

        ApplyForm(DataHandler.Instance.GetMonsterForm(form_id));

        respawnCooltimer = float.Parse(values[3]);
        isAlive = bool.Parse(values[4]);
        if (!isAlive) { Dead(); }
    }

    //===
    public void Create(MonsterForm form, Vector3 spawnPos)
    {
        if (GameManager.Instance.playerData.isNewBegin) 
            EnhancedRate = 1f;

        ApplyForm(form);
        respawnCooltimer = RespawnCooltime;
        isAlive = true;

        this.spawnPos = spawnPos;
        transform.position = spawnPos;
    }
    public void ApplyForm(MonsterForm form)
    {
        this.form = form;
        maxHp = form.hp * EnhancedRate;
        hp = maxHp;
        damage = form.damage * EnhancedRate;

        renderer.sprite = form.sprite;
        gameObject.AddComponent<BoxCollider2D>();
        hpBar.UpdateBar(hp, maxHp);
        

        StartCoroutine(thr_healthRegen());
        DataHandler.Instance.playerMonsters.Add(this);
        if(DataHandler.Instance.dataUpdated != null)
        {
            DataHandler.Instance.dataUpdated();
        }
    }
    public void ChangeForm(MonsterForm form)
    {
        this.form = form;
        renderer.sprite = form.sprite;
        Destroy(GetComponent<BoxCollider2D>());
        gameObject.AddComponent<BoxCollider2D>();
    }
    public void ReStat(bool isRecover)
    {
        maxHp = form.hp * EnhancedRate;
        damage = form.damage * EnhancedRate;
        if (isRecover)
            hp = maxHp;
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
        else if(rig.velocity.y <= 0)
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
        Vector3 scaleOrigin = transform.localScale;
        while (true)
        {
            float scale_x = transform.localScale.x - shrinkSpeed;
            float scale_y = transform.localScale.y - shrinkSpeed;
            if (scale_x < 0) { scale_x = 0; }
            if (scale_y < 0) { scale_y = 0; }

            transform.localScale = new Vector3(scale_x, scale_y, transform.localScale.z);
            if (scale_x <= 0 && scale_y <= 0)
            {
                break;
            }
            yield return null;
        }
        rig.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX
            | RigidbodyConstraints2D.FreezeRotation;
        while (true)
        {
            respawnCooltimer -= Time.deltaTime;
            if(respawnCooltimer < 0)
            {
                respawnCooltimer = RespawnCooltime;
                break;
            }
            yield return null;
        }
        rig.constraints = RigidbodyConstraints2D.FreezePositionX
            | RigidbodyConstraints2D.FreezeRotation;
        while (true)
        {
            float scale_x = transform.localScale.x + shrinkSpeed;
            float scale_y = transform.localScale.y + shrinkSpeed;
            if (scale_x > scaleOrigin.x) { scale_x = scaleOrigin.x; }
            if (scale_y > scaleOrigin.y) { scale_y = scaleOrigin.y; }

            transform.localScale = new Vector3(scale_x, scale_y, transform.localScale.z);
            if (scale_x >= scaleOrigin.x && scale_y >= scaleOrigin.y)
            {
                break;
            }
            yield return null;
        }
        hp = form.hp * EnhancedRate;
        isAlive = true;
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
            else if(healthRegenCooltimer <= 0)
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
