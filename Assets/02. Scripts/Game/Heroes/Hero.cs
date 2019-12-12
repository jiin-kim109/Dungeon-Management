using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hero : MonoBehaviour {

    public HeroForm form;

    [System.Serializable]
    public class HeroStatus
    {
        public float hp;
        public float damage;
        public float moveSpeed;

        public void Load(HeroForm form)
        {
            hp = form.hp * HeroSpawner.EnhancedRate;
            damage = form.damage * HeroSpawner.EnhancedRate;
            moveSpeed = form.moveSpeed;
        }
    }
    [Space(8)]
    [SerializeField]
    private HeroStatus m_status = new HeroStatus();
    public HeroStatus status { get { return m_status; } }

    private SpriteRenderer renderer;
    private Rigidbody2D rig;
    private Animator anim;

    private const float hitVector_x = 1.9f;
    private const float hitVector_y = 5.0f;

    private const float freezeDuration = 0.4f;
    public bool isAir { get; private set; }
    public bool isAlive { get; private set; }
    public bool isPause { get; private set; }
    public bool isPassed { get; private set; }

    private bool isCursed = false;
    private float currentMoveSpeedRate = 1f;
    private float currentDamageTakeRate = 1f;

    private Coroutine behavCo;
    private Coroutine cureCo;
    private Coroutine flashCo;

    public delegate void PortalEvent(Portal portal);
    public PortalEvent portalEvent;

    //===
    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rig.bodyType = RigidbodyType2D.Kinematic;
        isAir = true;
        isAlive = true;
        isPause = false;
        isPassed = false;
    }
    void Start()
    {
        renderer.material.shader = HeroSpawner.Instance.heroShader;
        Activate();
    }

    //===
    public void Activate()
    {
        rig.bodyType = RigidbodyType2D.Dynamic;
        renderer.sprite = form.sprite;

        behavCo = StartCoroutine(thr_behavior());
    }

    public void Save(string save_id)
    { //DataForm: (pos_x, pos_y, pos_z, form_id, hp)
        if (!isAlive) { return; }
        try
        {
            string value = "(" + transform.position.x + ", " + transform.position.y + ", " + transform.position.z;

            value += ", " + form.id;
            value += ", " + m_status.hp.ToString();
            value += ")";

            PlayerPrefs.SetString(save_id, value);
        }
        catch (MissingReferenceException e) { }
        catch (System.NullReferenceException e) { }
    }
    public void Load(string save_id)
    { //(pos_x, pos_y, pos_z, form_id, hp)
        string value = PlayerPrefs.GetString(save_id.ToString());
        value = value.Trim(new char[] { '(', ')' });
        value = value.Replace(" ", "");

        string[] values = value.Split(',');
        //Position
        Vector3 pos = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        transform.position = pos;

        //Hero Data
        HeroForm form = HeroSpawner.Instance.FindHeroForm(values[3]);
        ApplyForm(form);
        m_status.hp = float.Parse(values[4]);
        if (m_status.hp <= 0)
        {
            StartCoroutine(thr_dead());
        }

        //Content Object
        Floor floor = FloorHandler.Instance.FindFloorByPosition(pos);
        if (floor != null)
        {
            transform.SetParent(floor.transform.Find("Heroes"));
        }
    }

    //===
    public void ApplyForm(HeroForm form)
    {
        this.form = form;
        renderer.sprite = form.sprite;
        m_status.Load(form);
    }


    //===
    public void TakeDamage(float damage)
    {
        if (!isAlive || isPassed) { return; }

        StopAnim();
        renderer.sprite = form.sprite_hit;
        if (!isAir)
        {
            rig.velocity = new Vector2(-hitVector_x, hitVector_y);
        }
        isAir = true;
        damage = damage * currentDamageTakeRate;

        m_status.hp -= damage;
        if (m_status.hp <= 0)
        {
            StartCoroutine(thr_dead());
        }
        int value = (int)damage;
        if (isCursed) { EffectHandler.Instance.TextPopup(value.ToString(), transform, Color.yellow, 1.5f); }
        else { EffectHandler.Instance.TextPopup(value.ToString(), transform, Color.white); }
    }
    public void TakeDamageWithoutForce(float damage)
    {
        if (isAir || !isAlive || isPassed ) { return; }
        StartCoroutine(thr_standDamage(damage * currentDamageTakeRate));
    }
    IEnumerator thr_standDamage(float damage)
    {
        StopAnim();
        renderer.sprite = form.sprite_hit;

        m_status.hp -= damage;
        int value = (int)damage;
        if (isCursed) { EffectHandler.Instance.TextPopup(value.ToString(), transform, Color.yellow, 1.5f); }
        else { EffectHandler.Instance.TextPopup(value.ToString(), transform, Color.white); }
        isPause = true;
        rig.velocity = new Vector2(0, hitVector_y * 0.7f);
        yield return new WaitForSeconds(freezeDuration);
        isPause = false;
        if (m_status.hp <= 0)
        {
            StartCoroutine(thr_dead());
        }
        else
        {
            PlayWalkAnim();
        }
    }
    public void TakeDamageWithFlash(float damage)
    {
        if (!isAlive || isPassed) { return; }
        damage = damage * currentDamageTakeRate;

        m_status.hp -= damage;
        if(flashCo != null) { StopCoroutine(flashCo); }
        flashCo = StartCoroutine(thr_damageFlash());

        int value = (int)damage;
        if (isCursed) { EffectHandler.Instance.TextPopup(value.ToString(), transform, Color.yellow, 1.5f); }
        else { EffectHandler.Instance.TextPopup(value.ToString(), transform, Color.white); }
        if (m_status.hp <= 0)
        {
            StartCoroutine(thr_dead());
        }
    }
    public void TakeDamageWithFlashStrong(float damage)
    {
        if (!isAlive || isPassed) { return; }
        damage = damage * currentDamageTakeRate;

        m_status.hp -= damage;
        if (flashCo != null) { StopCoroutine(flashCo); }
        flashCo = StartCoroutine(thr_damageFlash());

        int value = (int)damage;
        EffectHandler.Instance.TextPopup(value.ToString(), transform, Color.yellow, 1.5f); 
        if (m_status.hp <= 0)
        {
            StartCoroutine(thr_dead());
        }
    }
    IEnumerator thr_damageFlash()
    {
        renderer.material.SetFloat("_FlashAmount", 0.5f);
        yield return new WaitForSeconds(0.2f);
        renderer.material.SetFloat("_FlashAmount", 0);
    }

    public void Jump()
    {
        if(!isAir && isAlive)
        {
            StartCoroutine(thr_jump());
        }
    }
    IEnumerator thr_jump()
    {
        rig.velocity = new Vector2(0, 3.2f);
        renderer.sprite = form.sprite_hit;
        isAir = true;

        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.15f);
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void GetCursed(float speedRate, float damageRate)
    {
        bool flag = false;
        if (currentMoveSpeedRate >= speedRate) 
        {
            currentMoveSpeedRate = speedRate;
            flag = true;
        }
        if(currentDamageTakeRate <= damageRate)
        {
            currentDamageTakeRate = damageRate;
            flag = true;
        }
        if (flag)
        {
            isCursed = true;
            renderer.color = Color.grey;
            EffectHandler.Instance.TextPopup("Weakness", transform, Color.grey, 0.85f);
            if(cureCo != null) { StopCoroutine(cureCo); }
            StartCoroutine(thr_curing());
        }
    }
    public void BackToNormal()
    {
        isCursed = false;
        currentMoveSpeedRate = 1f;
        currentDamageTakeRate = 1f;
        renderer.color = Color.white;
    }
    IEnumerator thr_curing()
    {
        yield return new WaitForSeconds(Curse.CurseDuration);
        BackToNormal();
    }

    //===
    void PlayWalkAnim()
    {
        anim.enabled = true;
        anim.Play("walk_" + form.id);
    }
    void StopAnim()
    {
        anim.enabled = false;
    }

    //===
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (isAir && !isPause && isAlive)
            {
                isAir = false;
                renderer.sprite = form.sprite;
                PlayWalkAnim();
            }
        }
    }


    //===
    public void Dead()
    {
        if (isPassed)
            return;
        StartCoroutine(thr_dead());
    }
    public void DeadWithEffect(string spellName)
    {
        if (isPassed)
            return;
        StartCoroutine(thr_deadWithEffect(spellName));
    }

    public void PassTheGoal()
    {
        isPassed = true;
        status.hp = 999999;
        HeroSpawner.Instance.HeroPassTheGoal();
        Destroy(gameObject);
    }
    IEnumerator thr_behavior()
    {
        while (true)
        {
            if (!isAir && !isPause)
            {
                rig.velocity = new Vector2(status.moveSpeed * currentMoveSpeedRate, rig.velocity.y);
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator thr_dead()
    {
        isAlive = false;
        HeroSpawner.Instance.HeroDead();

        if(behavCo != null) { StopCoroutine(behavCo); }
        renderer.sprite = form.sprite_dead;
        anim.enabled = false;

        yield return new WaitForSeconds(1f);
        Color color = renderer.color;
        float speed = 0.05f;
        while (true)
        {
            color.r -= speed;
            color.g -= speed;
            color.b -= speed;
            if(color.r < 0) { color.r = 0; }
            if (color.g < 0) { color.g = 0; }
            if (color.b < 0) { color.b = 0; }
            renderer.color = color;

            if(color.r <= 0 && color.g <= 0 && color.b <= 0)
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
        yield return null;
    }
    IEnumerator thr_deadWithEffect(string spellName)
    {
        isPause = true;
        isAlive = false;
        EffectHandler.Instance.SpellEffect(spellName, transform.position + new Vector3(0.05f, 0, 0));
        HeroSpawner.Instance.HeroDead();
        if (behavCo != null) { StopCoroutine(behavCo); }
        yield return new WaitForSeconds(0.40f);

        anim.enabled = false;
        Destroy(gameObject);
        yield return null;
    }
}
