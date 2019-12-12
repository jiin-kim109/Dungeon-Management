using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildRoomDisplay : MonoBehaviour {

    [SerializeField]
    private Sprite nullSprite;

    [System.Serializable]
    private struct RoomDisplay
    {
        public GameObject display;
        public List<Image> icons;
    }
    [Space(6)]
    [SerializeField]
    private RoomDisplay room_1_1_1;
    [SerializeField]
    private RoomDisplay room_1_2;
    [SerializeField]
    private RoomDisplay room_2_1;
    [SerializeField]
    private RoomDisplay room_3;


    //===
    public void Display(List<Room> selectedRooms)
    {
        Reset();

        int count = selectedRooms.Count;
        switch (count)
        {
            case 0:
                DisplayForm_1_1_1(nullSprite, nullSprite, nullSprite);
                break;
            case 1:
                if (selectedRooms[0].size == 3)
                {
                    DisplayForm_3(selectedRooms[0].icon);
                }
                else if(selectedRooms[0].size == 2)
                {
                    DisplayForm_2_1(selectedRooms[0].icon, null);
                }
                else
                {
                    DisplayForm_1_1_1(selectedRooms[0].icon, nullSprite, nullSprite);
                }
                break;
            case 2:
                if (selectedRooms[0].size == 1)
                {
                    if(selectedRooms[1].size == 1)
                    {
                        DisplayForm_1_1_1(selectedRooms[0].icon, selectedRooms[1].icon, nullSprite);
                    }
                    else
                    {
                        DisplayForm_1_2(selectedRooms[0].icon, selectedRooms[1].icon);
                    }
                }
                else if(selectedRooms[0].size == 2)
                {
                    DisplayForm_2_1(selectedRooms[0].icon, selectedRooms[1].icon);
                }
                break;
            case 3:
                DisplayForm_1_1_1(selectedRooms[0].icon, selectedRooms[1].icon, selectedRooms[2].icon);
                break;
        }
    }
    public void Reset()
    {
        for (int i = 0; i < room_1_1_1.icons.Count; i++)
        {
            room_1_1_1.icons[i].sprite = null;
        }
        for (int i = 0; i < room_1_2.icons.Count; i++)
        {
            room_1_2.icons[i].sprite = null;
        }
        for (int i = 0; i < room_2_1.icons.Count; i++)
        {
            room_2_1.icons[i].sprite = null;
        }
        for (int i = 0; i < room_3.icons.Count; i++)
        {
            room_3.icons[i].sprite = null;
        }

        room_1_1_1.display.SetActive(true);
        room_1_2.display.SetActive(false);
        room_2_1.display.SetActive(false);
        room_3.display.SetActive(false);
    }

    //===
    public void DisplayForm_1_1_1(Sprite icon_0, Sprite icon_1, Sprite icon_2)
    {
        Deactive();
        room_1_1_1.display.SetActive(true);

        room_1_1_1.icons[0].sprite = icon_0;
        room_1_1_1.icons[1].sprite = icon_1;
        room_1_1_1.icons[2].sprite = icon_2;
    }
    public void DisplayForm_1_2(Sprite icon_0, Sprite icon_1)
    {
        Deactive();
        room_1_2.display.SetActive(true);

        room_1_2.icons[0].sprite = icon_0;
        room_1_2.icons[1].sprite = icon_1;
    }
    public void DisplayForm_2_1(Sprite icon_0, Sprite icon_1)
    {
        Deactive();
        room_2_1.display.SetActive(true);

        room_2_1.icons[0].sprite = icon_0;
        room_2_1.icons[1].sprite = icon_1;
    }
    public void DisplayForm_3(Sprite icon_0)
    {
        Deactive();
        room_3.display.SetActive(true);

        room_3.icons[0].sprite = icon_0;
    }

    void Deactive()
    {
        room_1_1_1.display.SetActive(false);
        room_1_2.display.SetActive(false);
        room_2_1.display.SetActive(false);
        room_3.display.SetActive(false);
    }
}
