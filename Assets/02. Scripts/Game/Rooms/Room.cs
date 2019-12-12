using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour {

    public string saveID { get; private set; }

    [SerializeField]
    private string m_id;
    public string id { get { return m_id; } }
    [SerializeField]
    private Sprite m_icon;
    public Sprite icon { get { return m_icon; } }
    [SerializeField]
    private RoomType m_type;
    public RoomType type { get { return m_type; } }
    [SerializeField]
    private int m_size;
    public int size { get { return m_size; } }

    [SerializeField]
    private int capacity;
    public int Capacity { get { return capacity; } }

    [HideInInspector]
    public Collider2D roomCollider;

    [Space(10)]
    public UnityEvent buildEvent;
    public UnityEvent loadEvent;

    //===
    void Awake()
    {
        roomCollider = GetComponent<Collider2D>();
    }

    //===
    public void Load(int floorIndex, int roomIndex)
    {
        saveID = "floor_" + floorIndex.ToString() + "_room_" + roomIndex.ToString();
        loadEvent.Invoke();
    }
    public void Build(int floorIndex, int roomIndex)
    {
        saveID = "floor_" + floorIndex.ToString() + "_room_" + roomIndex.ToString();
        buildEvent.Invoke();
    }
}


public enum RoomType
{
    마왕,

    //일반
    Monster,
    마력생산,
    Spirit,
    Totem,
    Boss,

    //특수
    전송
}

/*  Room Info
 *  
    00: EndRoom

  //일반
   ID
    01: Monster_size_1
    02: Monster_size_2
    03: GoldMaker_size_1
    04: GoldMaker_size_2
    05: Trap_size_1
    06: Trap_size_2
    07: Debuf_size_1
    08: Boss_size_3

    //특수
    09: CallBack_size_1

*/