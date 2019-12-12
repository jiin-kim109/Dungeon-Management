using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    public Collider2D target;

    //===
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Heroes")
        {
            if (target != null)
            {
                col.transform.position = target.GetComponent<Collider2D>().bounds.center;
                col.transform.SetParent(target.transform.parent.GetComponent<FolderGetter>().GetObject().transform);
                Hero hero = col.GetComponent<Hero>();
                if(hero.portalEvent != null) {
                    hero.portalEvent(this);
                    hero.portalEvent = null;
                }
            }
        }
    }

    //===
    public void SetTarget(Collider2D target)
    {
        this.target = target;
    }
}
