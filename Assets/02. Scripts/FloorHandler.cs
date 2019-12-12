using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorHandler : MonoBehaviour {
    
    [SerializeField]
    private GameObject contentObj;
    [SerializeField]
    private Floor floorPrefab;
    [SerializeField]
    private Portal dungeonPortal;

    [Space(8)]
    [SerializeField]
    private float aliginSpace;
    [SerializeField]
    private List<Floor> m_floors = new List<Floor>();
    public List<Floor> floors { get { return m_floors; } }
    public int currentFloor { get { return m_floors.Count; } }

    [Space(8)]
    [SerializeField]
    private List<Room> roomPrefabs = new List<Room>();

    //---------------Instance------------------
    private static FloorHandler _instance = null;
    public static FloorHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(FloorHandler)) as FloorHandler;
                if (_instance == null) { Debug.Log("FloorHandler가 없음"); }
            }
            return _instance;
        }
    }
    //-----------------------------------------

    //===
    public void Save()
    {
        List<string> floorData = new List<string>();
        for (int i = 0; i < m_floors.Count; i++)
        {
            string data = m_floors[i].Save();
            floorData.Add(data);
        }
        GameManager.Instance.playerData.floorData = floorData;
    }
    public void Load()
    {
        List<string> floorData = GameManager.Instance.playerData.floorData;
        for (int i = 0; i < floorData.Count; i++)
        {
            if (i < m_floors.Count)
            {
                m_floors[i].Load(floorData[i], i);
            }
            else
            {
                Floor floor = InstantiateFloor();
                floor.Load(floorData[i], i);

                m_floors.Add(floor);
            }
        }
        dungeonPortal.SetTarget(m_floors[m_floors.Count-1].entrance);
    }

    //===
    public void BuildFloor(List<Room> rooms)
    {
        Floor floor = InstantiateFloor();
        floor.Build(rooms, m_floors.Count);

        m_floors.Add(floor);
        dungeonPortal.SetTarget(floor.entrance);
        GameManager.Instance.SaveGame();
    }

    public void BuildFloor()
    {
        Floor floor = InstantiateFloor();
        floor.Build(CreateRandomRooms(), m_floors.Count);

        m_floors.Add(floor);
        dungeonPortal.SetTarget(floor.entrance);
        GameManager.Instance.SaveGame();
    }
    public void BuildBossFloor()
    {
        Floor floor = InstantiateFloor();
        floor.Build(CreateBossRooms(), m_floors.Count);

        m_floors.Add(floor);
        dungeonPortal.SetTarget(floor.entrance);
        GameManager.Instance.SaveGame();
    }
    List<Room> CreateRandomRooms()
    {
        List<Room> list = new List<Room>();
        while (true)
        {
            int index = Random.Range(0, roomPrefabs.Count);
            if (roomPrefabs[index].type == RoomType.마왕 || roomPrefabs[index].type == RoomType.Boss)
                continue;

            int size = 0;
            foreach(Room rm in list)
            {
                size += rm.size;
            }
            size += roomPrefabs[index].size;
            if (size > Floor.MaxRoomCapacity)
                continue;
            else if (size == Floor.MaxRoomCapacity)
            {
                list.Add(roomPrefabs[index]);
                break;
            }
            else
                list.Add(roomPrefabs[index]);
        }
        return list;
    }
    List<Room> CreateBossRooms()
    {
        List<Room> list = new List<Room>();
        foreach (Room rm in roomPrefabs)
        {
            if (rm.type == RoomType.Boss)
            {
                list.Add(rm);
                break;
            }
        }
        return list;
    }

    //===
    Floor InstantiateFloor()
    {
        Floor floor = Instantiate(floorPrefab);
        floor.name = "floor_" + m_floors.Count.ToString();
        floor.transform.SetParent(contentObj.transform);
        floor.transform.position = m_floors[m_floors.Count - 1].transform.position;
        floor.portal.SetTarget(m_floors[m_floors.Count - 1].entrance);
        Transform heroContent = m_floors[m_floors.Count - 1].transform.Find("Heroes");
        int count = heroContent.childCount;
        /*for (int i=0; i< count; i++)
        {
            Transform targetContent = floor.transform.Find("Heroes");
            Hero hero = heroContent.GetChild(0).GetComponent<Hero>();
            hero.Jump();
            heroContent.GetChild(0).SetParent(targetContent);
        }*/
        foreach (Floor fl in m_floors)
        {
            fl.transform.position = fl.transform.position + new Vector3(0, -aliginSpace, 0);
        }

        return floor;
    }
    public Floor FindFloorByPosition(Vector2 position)
    {
        foreach(Floor floor in floors)
        {
            if (floor.floorCollider.bounds.Contains(position))
            {
                return floor;
            }
        }
        return null;
    }
    public Room FindRoomPrefab(string room_id)
    {
        foreach(Room prefab in roomPrefabs)
        {
            if(prefab.id == room_id)
            {
                return prefab;
            }
        }
        return null;
    }
    public List<Room> FindTypeRooms(RoomType type)
    {
        List<Room> rooms = new List<Room>();
        foreach(Room prefab in roomPrefabs)
        {
            if(type == prefab.type)
            {
                rooms.Add(prefab);
            }
        }
        return rooms;
    }
    public Room FindTypeAndSizeRoom(RoomType type, int size)
    {
        Room room = null;
        foreach (Room prefab in roomPrefabs)
        {
            if (type == prefab.type && size == prefab.size)
            {
                room = prefab;
            }
        }
        return room;
    }
}
