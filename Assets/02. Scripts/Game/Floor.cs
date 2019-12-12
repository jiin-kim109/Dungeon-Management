using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {

    public const int MaxRoomCapacity = 3;
    public const float RoomAliginSpace = 2f;

    private int floorIndex;

    [SerializeField]
    private GameObject roomContentObj;
    private List<Room> m_rooms = new List<Room>();
    public List<Room> rooms { get { return m_rooms; } }

    [Space(8)]
    public Portal portal;
    public Collider2D entrance;
    public Collider2D floorCollider;

    //===
    public string Save()
    { //(room_0_id, room_1_id, room_2_id)
        string value = "(";
        for(int i=0; i< m_rooms.Count; i++)
        {
            value += m_rooms[i].id;
            if(i != m_rooms.Count - 1) { value += ", "; }
        }
        value += ")";
        return value;
    }
    public void Load(string data, int floorIndex)
    {
        this.floorIndex = floorIndex;
        string value = data;

        value = value.Trim(new char[] { '(', ')' });
        value = value.Replace(" ", "");

        string[] values = value.Split(',');

        int capacity = 0;
        for (int i = 0; i < values.Length; i++)
        {
            Room room = Instantiate(FloorHandler.Instance.FindRoomPrefab(values[i]));
            room.transform.SetParent(roomContentObj.transform);
            room.transform.position = new Vector2(RoomAliginSpace * capacity, transform.position.y);
            room.Load(floorIndex, i);

            m_rooms.Add(room);
            capacity += room.size;
            if (capacity >= MaxRoomCapacity) { break; }
        }
    }
    public void Build(List<Room> roomPrefabs, int floorIndex)
    {
        this.floorIndex = floorIndex;

        int capacity = 0;
        for (int i = 0; i < roomPrefabs.Count; i++)
        {
            Room room = Instantiate(roomPrefabs[i]);
            room.transform.SetParent(roomContentObj.transform);
            room.transform.position = new Vector2(RoomAliginSpace * capacity, transform.position.y);
            room.Build(floorIndex, i);

            m_rooms.Add(room);
            capacity += room.size;
            if (capacity >= MaxRoomCapacity) { break; }
        }
    }
}
