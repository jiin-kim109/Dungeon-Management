using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRoom : MonoBehaviour {

    private const float spawnAligin = 0.85f;

    [SerializeField]
    private int capacity;
    private List<Monster> monsters = new List<Monster>();

    //===
    void Awake()
    {
        GameManager.Instance.saveEvents.AddListener(delegate { Save(); });
    }

    void Save()
    { //(saveID, monsterID, monsterID, monsterID)
        Room room = GetComponent<Room>();
        string value = "(" + room.saveID + ", ";
        for(int i=0; i<monsters.Count; i++)
        {
            if(i == monsters.Count - 1)
            {
                value += monsters[i].form.id + ")";
            }
            else
            {
                value += monsters[i].form.id + ", ";
            }
            monsters[i].Save(room.saveID + "_monster_" + i.ToString());
        }
        PlayerPrefs.SetInt(room.saveID + "_monsterCount", monsters.Count);
        GameManager.Instance.playerData.monsterData.Add(value);
    }
    public void Load()
    {
        if (GameManager.Instance.playerData.isNewBegin)
        {
            Build();
            return;
        }

        Room room = GetComponent<Room>();
        List<string> datas = GameManager.Instance.playerData.monsterData;
        foreach(string data in datas)
        {
            string[] values = data.Trim(new char[] { '(', ')' }).Replace(" ", "").Split(',');
            if(values[0] == room.saveID)
            {
                int monsterCount = PlayerPrefs.GetInt(room.saveID + "_monsterCount");
                for (int i = 0; i < monsterCount; i++)
                {
                    Monster monster = Instantiate(DataHandler.Instance.monsterPrefab);
                    monster.transform.SetParent(transform);
                    monster.Load(values[i + 1], room.saveID + "_monster_" + i.ToString());

                    monsters.Add(monster);
                }
                return;
            }
        }
        //Debug.Log("오류! 룸 정보를 불러오지 못함");
    }
    public void Build()
    {
        Room room = GetComponent<Room>();

        Grade minGrade = UIHandler.Instance.floorBuilder.GetMinimumGrade(HeroSpawner.currentWave);
        List<MonsterForm> preForms = DataHandler.Instance.GetDefaultMonsterForms(capacity);
        List<MonsterForm> forms = new List<MonsterForm>();
        if (minGrade != Grade.E)
        {
            foreach (MonsterForm mf in preForms)
            {
                if((int)minGrade <= (int)mf.grade)
                {
                    forms.Add(mf);
                }
            }
            if(forms.Count == 0)
            {
                forms = DataHandler.Instance.GetMonsterFormsAtGrade(minGrade, capacity);
            }
            else if (forms.Count < capacity)
            {
                while(forms.Count < capacity)
                {
                    forms.Add(forms[Random.Range(0, forms.Count)]);
                }
            }
            else if (forms.Count > capacity)
            {
                List<MonsterForm> newForms = new List<MonsterForm>();
                for (int i = 0; i < capacity; i++)
                {
                    int idx = Random.Range(0, forms.Count);
                    newForms.Add(forms[idx]);
                }
                forms = newForms;
            }
        }
        else
            forms = preForms;
        foreach (MonsterForm form in forms)
        {
            Monster monster = Instantiate(DataHandler.Instance.monsterPrefab);
            monster.transform.SetParent(transform);
            int space = (monsters.Count + 1) / 2;
            if (monsters.Count % 2 == 1) { space *= -1; }
            Vector3 pos = room.roomCollider.bounds.center + new Vector3(spawnAligin * space, 0, 0);
            monster.Create(form, pos);

            monsters.Add(monster);
        }
    }


    //===
}
