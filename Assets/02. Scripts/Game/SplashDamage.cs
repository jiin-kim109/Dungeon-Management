using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashDamage : MonoBehaviour {
   
    private Vector2 posOrigin;
    private static float distance = 0.5f;
    private static float maxTargetNum = 3;

    //===
    public void SetScale(float scale)
    {
        distance *= scale;
    }

	public void GiveSplashDamage(Hero target, Vector2 pos, float damage)
    {
        List<Hero> targets = new List<Hero>();
        Transform heroContent = target.transform.parent;
        for(int i=0; i< heroContent.childCount; i++)
        {
            Hero hero = heroContent.GetChild(i).GetComponent<Hero>();
            Vector2 heroPos = hero.transform.position;
            if(Mathf.Abs(heroPos.x - pos.x) <= distance)
            {
                targets.Add(hero);
                if(targets.Count >= maxTargetNum) { break; }
            }
        }
        foreach(Hero tg in targets)
        {
            tg.TakeDamage(damage);
        }
    }
}
