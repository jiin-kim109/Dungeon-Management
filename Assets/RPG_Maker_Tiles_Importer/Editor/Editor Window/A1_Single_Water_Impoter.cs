#define DEBUG   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class A1_Single_Water_Impoter : A1_Tiles_Importer_Window
{
    [MenuItem("Tools/RPGM Importer/A1/A1 Single Animated Water")]
    public new static void ShowWindow()
    {
        EditorWindow.GetWindow<A1_Single_Water_Impoter>(false, "A1 Single Animated Water Importer");
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnGUI()
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

        Tiles_A1_Utility.A1_Tile_Slice_Water_Singleton(img, out wBlock, out hBlock, out mini_tile_w, out mini_tile_h, out sub_blocks_water);
        sub_blocks_water_to_import = new List<bool>();
        for (int i = 0; i < sub_blocks_water.Count / 3; i++)
            sub_blocks_water_to_import.Add(false);
    }
}
