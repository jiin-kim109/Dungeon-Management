using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Radar : MonoBehaviour {

    private const float checkDuration = 2f;

    [SerializeField]
    private List<Hero> m_targetHeroes = new List<Hero>();
    public List<Hero> targetHeroes { get { return m_targetHeroes; } }

    void Awake()
    {
        StartCoroutine(thr_checkAlive());
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Heroes")
        {
            m_targetHeroes.Add(col.GetComponent<Hero>());
        }
    }
    public void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.tag == "Heroes")
        {
            Hero hero = col.GetComponent<Hero>();
            if (m_targetHeroes.Contains(hero))
            {
                m_targetHeroes.Remove(hero);
            }
        }
    }

    IEnumerator thr_checkAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkDuration);

            List<Hero> newArr = new List<Hero>();
            foreach(Hero hero in m_targetHeroes)
            {
                if (hero.isAlive)
                {
                    newArr.Add(hero);
                }
            }
            m_targetHeroes = newArr;
        }
    }
}
