using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloorBuilder : MonoBehaviour {

    [SerializeField]
    private Image panel;
    [SerializeField]
    private TextMeshProUGUI cost_goldTextMesh;
    [SerializeField]
    private TextMeshProUGUI cost_gemTextMesh;
    [SerializeField]
    private Image bossActiveSign;
    [SerializeField]
    private Button openPanelButton;
    [SerializeField]
    private Button closePanelButton;
    [Space(6)]
    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private Button buildButton;

    [System.Serializable]
    private struct RoomButton
    {
        public Button button;
        public RoomType type;
    }
    [Space(10)]
    [Header("[  Components")]
    [SerializeField]
    private BuildRoomDisplay buildDisplay;
    [SerializeField]
    private TextMeshProUGUI selectedRoomNameText;
    [SerializeField]
    private List<RoomButton> roomButtons;

    private List<Room> selectedRoom = new List<Room>();

    [Space(12)]
    [SerializeField]
    private TextMeshProUGUI minimumGradeTextMesh;
    [System.Serializable]
    private struct MinGradeByLevel
    {
        public int levelFrom;
        public Grade grade;
    }
    [SerializeField]
    private List<MinGradeByLevel> minRankByLevel = new List<MinGradeByLevel>();

    [Space(12)]
    [Header("[  Parameters")]
    [SerializeField]
    private int dungeonBuildCost_gold_initial;
    [SerializeField]
    private int dungeonBuildCost_gold_upRate;
    private int currentBuildCost_gold
    {
        get
        {
            float cost = dungeonBuildCost_gold_initial;
            for (int i = 1; i < FloorHandler.Instance.currentFloor; i++)
            {
                cost = cost * (1 + (dungeonBuildCost_gold_upRate / 100));
            }
            return (int)cost;
        }
    }
    [Space(4)]
    [SerializeField]
    private int dungeonBuildCost_gem_initial;
    [SerializeField]
    private int dungeonBuildCost_gem_upRate;
    private int currentBuildCost_gem
    {
        get
        {
            float cost = dungeonBuildCost_gem_initial;
            for (int i = 1; i < FloorHandler.Instance.currentFloor; i++)
            {
                cost = cost * (1 + (dungeonBuildCost_gem_upRate / 100));
            }
            return (int)cost;
        }
    }

    [Space(6)]
    [SerializeField]
    private float deactiveColorAlpha;

    private bool isOn;
    [HideInInspector]
    public bool isBossEnabled = false;

    //===
    public void Initialize()
    {
        openPanelButton.onClick.AddListener(delegate { OpenPanel(); });
        //openPanelButton.onClick.AddListener(delegate { Build(); });
        closePanelButton.onClick.AddListener(delegate { ClosePanel(); });
        resetButton.onClick.AddListener(delegate { Reset(); });
        buildButton.onClick.AddListener(delegate { Build(); });
        for (int i = 0; i < roomButtons.Count; i++)
        {
            Button button = roomButtons[i].button;
            int index = i;
            button.onClick.AddListener(delegate { OnClick_SelectRoom(index); });
        }

        int bossOnOff = PlayerPrefs.GetInt("bossEnabled");
        if (bossOnOff >= 1) { SetActiveBossButton(true); }
        else { SetActiveBossButton(false); }
        bossActiveSign.gameObject.SetActive(false);

        UpdateUI();
        ClosePanel();
    }

    public Grade GetMinimumGrade(int currentLevel)
    {
        int index = -1;
        int maxLevel = -1;
        for(int i=0; i< minRankByLevel.Count; i++) {
            MinGradeByLevel minByLevel = minRankByLevel[i];
            if (currentLevel >= minByLevel.levelFrom && maxLevel < minByLevel.levelFrom)
            {
                index = i;
                maxLevel = minByLevel.levelFrom;
            }
        }
        if (index == -1)
            return Grade.E;
        else
            return minRankByLevel[index].grade;
    }


    //===
    public void OpenPanel()
    {
        if (GameManager.Instance.playerData.gold < currentBuildCost_gold || 
            GameManager.Instance.playerData.gem < currentBuildCost_gem)
        {
            EffectHandler.Instance.SystemMessage("Not enough gold or gem");
        }
        else
        {
            //UIHandler.Instance.DisplayShadow();
            isOn = true;
            panel.gameObject.SetActive(true);
        }
    }
    public void ClosePanel()
    {
        //UIHandler.Instance.HideShadow();
        isOn = false;
        panel.gameObject.SetActive(false);

        Reset();
    }


    //=== Building Floor
    public void Build()
    {
        GameManager.Instance.playerData.gold -= currentBuildCost_gold;
        GameManager.Instance.playerData.gem -= currentBuildCost_gem;
        foreach (Room r in selectedRoom)
        {
            if (r.type == RoomType.Boss)
            {
                SetActiveBossButton(false);
                break;
            }
        }
        FloorHandler.Instance.BuildFloor(selectedRoom);
        UpdateUI();
        UIHandler.Instance.bossUpgrade.UpdateUI();

        /*if (isBossEnabled)
        {
            FloorHandler.Instance.BuildBossFloor();
            SetActiveBossButton(false);
        }
        else
        {
            if (GameManager.Instance.playerData.gold >= currentBuildCost_gold 
                && GameManager.Instance.playerData.gem >= currentBuildCost_gem)
            {
                GameManager.Instance.playerData.gold -= currentBuildCost_gold;
                GameManager.Instance.playerData.gem -= currentBuildCost_gem;
                FloorHandler.Instance.BuildFloor();
            }
            else
            {
                EffectHandler.Instance.SystemMessage("자원이 부족합니다");
            }
        }*/
        ClosePanel();
    }
    public void UpdateUI()
    {
        cost_goldTextMesh.text = "<sprite=3> " + currentBuildCost_gold.ToString();
        cost_gemTextMesh.text = "<sprite=0>" + currentBuildCost_gem.ToString();

        minimumGradeTextMesh.text = GetMinimumGrade(HeroSpawner.currentWave).ToString();
    }

    public void Reset()
    {
        selectedRoomNameText.text = "";
        buildButton.gameObject.SetActive(false);
        selectedRoom = new List<Room>();
        buildDisplay.Reset();
    }

    public void OnClick_SelectRoom(int index)
    {
        int capacity = 0;
        foreach(Room room in selectedRoom) { capacity += room.size; }
        if(capacity >= Floor.MaxRoomCapacity) { return; }

        List<Room> typeRooms = FloorHandler.Instance.FindTypeRooms(roomButtons[index].type);
        if (selectedRoom.Count == 0 
            || selectedRoom[selectedRoom.Count - 1].type != roomButtons[index].type)
        {
            bool flag = false;
            for(int size=1; size<=Floor.MaxRoomCapacity; size++)
            {
                foreach(Room room in typeRooms)
                {
                    if(room.size == size)
                    {
                        if(capacity + room.size > Floor.MaxRoomCapacity) { return; }
                        selectedRoom.Add(room);
                        flag = true;
                        break;
                    }
                }
                if (flag) { break; }
            }
        }
        else if(selectedRoom[selectedRoom.Count-1].type == roomButtons[index].type)
        {
            bool isSup = false;
            foreach(Room room in typeRooms)
            {
                if(room.size == selectedRoom[selectedRoom.Count - 1].size + 1)
                {
                    selectedRoom.RemoveAt(selectedRoom.Count - 1);
                    selectedRoom.Add(room);
                    isSup = true;
                    break;
                }
            }
            if (!isSup)
            {
                bool flag = false;
                for (int size = 1; size <= Floor.MaxRoomCapacity; size++)
                {
                    foreach (Room room in typeRooms)
                    {
                        if (room.size == size)
                        {
                            if (capacity + room.size > Floor.MaxRoomCapacity) { return; }
                            selectedRoom.Add(room);
                            flag = true;
                            break;
                        }
                    }
                    if (flag) { break; }
                }
            }
        }

        capacity = 0;
        foreach (Room room in selectedRoom) { capacity += room.size; }
        if (capacity == Floor.MaxRoomCapacity) { buildButton.gameObject.SetActive(true); }
        else { buildButton.gameObject.SetActive(false); }

        selectedRoomNameText.text = "";
        int[] unitCount = new int[roomButtons.Count];
        for(int i=0; i<unitCount.Length; i++) { unitCount[i] = 0; }
        for(int i=0; i<selectedRoom.Count; i++)
        {
            for (int j = 0; j < roomButtons.Count; j++)
            {
                if (selectedRoom[i].type == roomButtons[j].type)
                {
                    unitCount[j] += selectedRoom[i].Capacity;
                }
            }
        }
        for(int i=0; i<unitCount.Length; i++)
        {
            if(unitCount[i] > 0)
            {
                selectedRoomNameText.text += roomButtons[i].type.ToString() + "<color=green>x<color=white>" + unitCount[i].ToString() + "  ";
            }
        }
        buildDisplay.Display(selectedRoom);
    }


    //=== etc
    public void SetActiveBossButton(bool onOff)
    {
        isBossEnabled = onOff;
        if (onOff) { PlayerPrefs.SetInt("bossEnabled", 1); }
        else { PlayerPrefs.SetInt("bossEnabled", 0); }

        foreach (RoomButton bt in roomButtons)
        {
            if (bt.type == RoomType.Boss)
            {
                if (onOff)
                {
                    bt.button.enabled = true;
                    Color color = bt.button.GetComponent<Image>().color;
                    color.a = 1f;
                    Image[] icons = bt.button.GetComponentsInChildren<Image>();
                    for (int i = 0; i < icons.Length; i++)
                    {
                        color = icons[i].color;
                        color.a = 1f;
                        icons[i].color = color;
                    }

                    //bossActiveSign.gameObject.SetActive(true);
                    //cost_goldTextMesh.gameObject.SetActive(false);
                    //cost_gemTextMesh.gameObject.SetActive(false);

                    UIHandler.Instance.floorBuilder.isBossEnabled = true;
                }
                else if (!onOff)
                {
                    bt.button.enabled = false;
                    Color color = bt.button.GetComponent<Image>().color;
                    color.a = deactiveColorAlpha;
                    Image[] icons = bt.button.GetComponentsInChildren<Image>();
                    for (int i = 0; i < icons.Length; i++)
                    {
                        color = icons[i].color;
                        color.a = deactiveColorAlpha;
                        icons[i].color = color;
                    }

                    //bossActiveSign.gameObject.SetActive(false);
                    //cost_goldTextMesh.gameObject.SetActive(true);
                    //cost_gemTextMesh.gameObject.SetActive(true);

                    UIHandler.Instance.floorBuilder.isBossEnabled = false;
                }
            }
        }
    }
}
