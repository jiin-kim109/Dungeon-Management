#define DEBUG   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class A1_Single_Twister_Importer : A1_Tiles_Importer_Window {

    [MenuItem("Tools/RPGM Importer/A1/A1 Single Animated Twister or Waterfall")]
    public new static void ShowWindow()
    {
        EditorWindow.GetWindow<A1_Single_Twister_Importer>(false, "A1 Single Twister or Waterfall Importer");
    }

    protected override void Update()
    {
        base.Update();
    }

    protected new void OnGUI()
    {
        base.OnGUI();
    }

    protected override void Select_Image()
    {
        if (GUILayout.Button("Load Image"))
        {
            path = EditorUtility.OpenFilePanel("Load Tile Set", ".", "");
            if (path != null && path != "" && File.Exists(path))
            {
                CutLayout();
            }
            else
            {
                if (path != null && path != "")
                    EditorUtility.DisplayDialog("Selection error!", "You have to select a file or an A1 file compatibile with RPG MAKER tile set", "OK");
            }
        }
    }

    protected override void CutLayout()
    {
        img = new Texture2D(1, 1);
        byte[] bytedata = File.ReadAllBytes(path);
        img.LoadImage(bytedata);

        //get the sliced part
        Vector2Int sub_size = new Vector2Int(img.width , img.height ); //that is a fixed number of blocks
                                                                             //divide in sub blocks

        mini_tile_w = sub_size.x / 4;
        wBlock = mini_tile_w * 2;
        mini_tile_h = sub_size.y / 6;
        hBlock = mini_tile_h * 2;

        sub_blocks_twister = new List<Texture2D>();
        sub_blocks_twister.Add(img);

        sub_blocks_twister_to_import = new List<bool>();
        foreach (var t in sub_blocks_twister)
            sub_blocks_twister_to_import.Add(false);

        waterFallFrame_Animation.Clear();
        foreach (Texture2D block in sub_blocks_twister)
        {
            int tw = block.width;
            int th = block.height / 3;
            List<Texture2D> list = new List<Texture2D>();
            for (int i = 0; i < 3; i++)
            {
                Color[] colors = block.GetPixels(0, th * (2 - i), tw, th);
                Texture2D tmpT = new Texture2D(tw, th);
                tmpT.SetPixels(0, 0, tw, th, colors);
                tmpT.Apply();
                list.Add(tmpT);
            }
            waterFallFrame_Animation.Add(list);
        }
    }
}
