using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    private Transform target;
    private Rigidbody2D rig;
    private bool isFired = false;

    private const float speed = 5.7f;
    private const float rotateSpeed = 720;

    [HideInInspector]
    public bool isStrong = false;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isFired)
        {
            if(target == null) {
                EffectHandler.Instance.SpellEffect("hitMissile", transform.position);
                Destroy(gameObject);
                return;
            }
            Vector2 direction = (Vector2)target.position - rig.position;
            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, -transform.up).z;

            rig.angularVelocity = -rotateAmount * rotateSpeed;
            rig.velocity = -transform.up * speed;
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Heroes")
        {
            Hero hero = col.GetComponent<Hero>();
            hero.portalEvent -= TargetLost;
            if (isStrong)
                hero.TakeDamageWithFlashStrong(GameManager.Instance.screenTouch.damage * (ScreenTouch.StrongAttackRate/100));
            else
                hero.TakeDamageWithFlash(GameManager.Instance.screenTouch.damage);

            EffectHandler.Instance.SpellEffect("hitMissile2", transform.position, "PlayerAttack", 0);
            rig.velocity = Vector2.zero;
            isFired = false;
            GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(thr_destroyTimer(GetComponent<TrailRenderer>().time));
        }
        if (col.gameObject.tag == "Portal")
        {
            EffectHandler.Instance.SpellEffect("hitMissile2", transform.position, "PlayerAttack", 0);
            rig.velocity = Vector2.zero;
            isFired = false;
            GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(thr_destroyTimer(GetComponent<TrailRenderer>().time));
        }
    }
    void TargetLost(Portal portal)
    {
        target = portal.transform;
    }

    IEnumerator thr_destroyTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    //===
    public void Fire(Hero hero)
    {
        Quaternion rot = Random.rotation;
        rot.eulerAngles = new Vector3(0, 0, rot.eulerAngles.z);
        transform.rotation = rot;

        this.target = hero.transform;
        isFired = true;

        hero.portalEvent += TargetLost;
    }
}
