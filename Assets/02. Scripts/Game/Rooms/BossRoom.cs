using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour {

    private const float spawnOffset_x = 1.2f;

    private Boss boss = new Boss();

    //===
    void Awake()
    {
        GameManager.Instance.saveEvents.AddListener(delegate { Save(); });
    }

    void Save()
    { //(saveID, boss_id)
        Room room = GetComponent<Room>();
        string value = "(" + room.saveID + ", ";
        value += boss.form.id;
        boss.Save(room.saveID + "_boss");
        GameManager.Instance.playerData.bossData.Add(value);
    }
    public void Load()
    {
        if (GameManager.Instance.playerData.isNewBegin)
        {
            Build();
            return;
        }

        Room room = GetComponent<Room>();
        List<string> datas = GameManager.Instance.playerData.bossData;
        foreach (string data in datas)
        {
            string[] values = data.Trim(new char[] { '(', ')' }).Replace(" ", "").Split(',');
            if (values[0] == room.saveID)
            {
                Boss boss = Instantiate(DataHandler.Instance.bossPrefab);
                boss.transform.SetParent(transform);
                boss.Load(values[1], room.saveID + "_boss");

                this.boss = boss;
                return;
            }
        }
        //Debug.Log("오류! 룸 정보를 불러오지 못함");
    }
    public void Build()
    {
        Room room = GetComponent<Room>();
        BossForm form = BossUpgrade.currentBossForm;
        Boss boss = Instantiate(DataHandler.Instance.bossPrefab);
        boss.transform.SetParent(transform);
        Vector3 pos = room.roomCollider.bounds.center + new Vector3(spawnOffset_x, 0, 0);
        boss.Create(form, pos);

        this.boss = boss;
    }
}
