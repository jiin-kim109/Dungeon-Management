using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapRoom : MonoBehaviour {

    private const float spawnAligin = 0.85f;

    [SerializeField]
    private int capacity;

    private List<Trap> traps = new List<Trap>();

    //===
    void Awake()
    {
        GameManager.Instance.saveEvents.AddListener(delegate { Save(); });
    }

    void Save()
    { //(saveID, trap_id, trap_id, trap_id)
        Room room = GetComponent<Room>();
        string value = "(" + room.saveID + ", ";
        for (int i = 0; i < traps.Count; i++)
        {
            if (i == traps.Count - 1)
            {
                value += traps[i].form.id + ")";
            }
            else
            {
                value += traps[i].form.id + ", ";
            }
            traps[i].Save(room.saveID + "_trap_" + i.ToString());
        }
        PlayerPrefs.SetInt(room.saveID + "_trapCount", traps.Count);
        GameManager.Instance.playerData.trapData.Add(value);
    }
    public void Load()
    { //(saveID, trap_id, trap_id, trap_id)
        if (GameManager.Instance.playerData.isNewBegin)
        {
            Build();
            return;
        }

        Room room = GetComponent<Room>();
        List<string> datas = GameManager.Instance.playerData.trapData;
        foreach (string data in datas)
        {
            string[] values = data.Trim(new char[] { '(', ')' }).Replace(" ", "").Split(',');
            if (values[0] == room.saveID)
            {
                int trapCount = PlayerPrefs.GetInt(room.saveID + "_trapCount");
                for (int i = 0; i < trapCount; i++)
                {
                    Trap trap = Instantiate(DataHandler.Instance.trapPrefab);
                    trap.transform.SetParent(transform);
                    trap.Load(values[i + 1], room.saveID + "_trap_" + i.ToString());

                    traps.Add(trap);
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
        List<TrapForm> preForms = DataHandler.Instance.GetDefaultTrapForms(capacity);
        List<TrapForm> forms = new List<TrapForm>();
        if (minGrade != Grade.E)
        {
            foreach (TrapForm mf in preForms)
            {
                if ((int)minGrade <= (int)mf.grade)
                {
                    forms.Add(mf);
                }
            }

            if (forms.Count == 0)
            {
                forms = DataHandler.Instance.GetTrapFormsAtGrade(minGrade, capacity);
            }
            else if (forms.Count < capacity)
            {
                while (forms.Count < capacity)
                {
                    forms.Add(forms[Random.Range(0, forms.Count)]);
                }
            }
            else if (forms.Count > capacity)
            {
                List<TrapForm> newForms = new List<TrapForm>();
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

        foreach (TrapForm form in forms)
        {
            Trap trap = Instantiate(DataHandler.Instance.trapPrefab);
            trap.transform.SetParent(transform);
            int space = (traps.Count + 1) / 2;
            if (traps.Count % 2 == 1) { space *= -1; }
            Vector3 pos = room.roomCollider.bounds.center + new Vector3(spawnAligin * space, 0, 0);
            trap.Create(form, pos);

            traps.Add(trap);
        }
    }
}
