using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMaster : MonoBehaviour {

    private Animator anim;

    void Awake()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Heroes")
        {
            Hero hero = col.GetComponent<Hero>();
            hero.PassTheGoal();
            anim.Play("getHit");
        }
    }

    public void PlayAnim_UseSpell()
    {
        //anim.Play("useSpell");
    }
}
