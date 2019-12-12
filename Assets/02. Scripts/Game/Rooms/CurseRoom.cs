using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseRoom : MonoBehaviour {

    private const float spawnAligin = 0.85f;

    [SerializeField]
    private int capacity;

    private List<Curse> curses = new List<Curse>();

    //===
    void Awake()
    {
        GameManager.Instance.saveEvents.AddListener(delegate { Save(); });
    }

    void Save()
    { //(saveID, curse_id, curse_id, curse_id)
        Room room = GetComponent<Room>();
        string value = "(" + room.saveID + ", ";
        for (int i = 0; i < curses.Count; i++)
        {
            if (i == curses.Count - 1)
            {
                value += curses[i].form.id + ")";
            }
            else
            {
                value += curses[i].form.id + ", ";
            }
            curses[i].Save(room.saveID + "_curse_" + i.ToString());
        }
        PlayerPrefs.SetInt(room.saveID + "_curseCount", curses.Count);
        GameManager.Instance.playerData.curseData.Add(value);
    }
    public void Load()
    { //(saveID, curse_id, curse_id, curse_id)
        if (GameManager.Instance.playerData.isNewBegin)
        {
            Build();
            return;
        }

        Room room = GetComponent<Room>();
        List<string> datas = GameManager.Instance.playerData.curseData;
        foreach (string data in datas)
        {
            string[] values = data.Trim(new char[] { '(', ')' }).Replace(" ", "").Split(',');
            if (values[0] == room.saveID)
            {
                int curseCount = PlayerPrefs.GetInt(room.saveID + "_curseCount");
                for (int i = 0; i < curseCount; i++)
                {
                    Curse curse = Instantiate(DataHandler.Instance.cursePrefab);
                    curse.transform.SetParent(transform);
                    curse.Load(values[i + 1], room.saveID + "_curse_" + i.ToString());

                    curses.Add(curse);
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
        List<CurseForm> preForms = DataHandler.Instance.GetDefaultCurseForms(capacity);
        List<CurseForm> forms = new List<CurseForm>();
        if (minGrade != Grade.E)
        {
            foreach (CurseForm mf in preForms)
            {
                if ((int)minGrade <= (int)mf.grade)
                {
                    forms.Add(mf);
                }
            }
            if (forms.Count == 0)
            {
                forms = DataHandler.Instance.GetCurseFormsAtGrade(minGrade, capacity);
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
                List<CurseForm> newForms = new List<CurseForm>();
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
        
        foreach (CurseForm form in forms)
        {
            Curse curse = Instantiate(DataHandler.Instance.cursePrefab);
            curse.transform.SetParent(transform);
            int space = (curses.Count + 1) / 2;
            if (curses.Count % 2 == 1) { space *= -1; }
            Vector3 pos = room.roomCollider.bounds.center + new Vector3(spawnAligin * space, 0, 0);
            curse.Create(form, pos);

            curses.Add(curse);
        }
    }
}
