#define DEBUG   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;


public class A4_Single_Wall_Impoter : A4_Tiles_Importer_Window
{
    [MenuItem("Tools/RPGM Importer/A4/A4 Single Wall")]
    public new static void ShowWindow()
    {
        EditorWindow.GetWindow<A4_Single_Wall_Impoter>(false, "A4 Single Wall Impoter");
    }

    protected override void CutLayout()
    {
        img = new Texture2D(1, 1);
        byte[] bytedata = File.ReadAllBytes(path);
        img.LoadImage(bytedata);

        //get the sliced part
        Vector2Int sub_size = new Vector2Int(img.width, img.height); //that is a fixed number of blocks
                                                                     //divide in sub blocks

        mini_tile_w = sub_size.x / 4;
        wBlock = mini_tile_w * 2;
        mini_tile_h = sub_size.y / 4;
        hBlock = mini_tile_h * 2;

        //get the sliced part
        sub_blocks_wall = new List<Texture2D>();
        sub_blocks_wall.Add(img);

        sub_blocks_wall_to_import = new List<bool>();
        for (int i = 0; i < sub_blocks_wall.Count; i++)
            sub_blocks_wall_to_import.Add(false);
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
                    EditorUtility.DisplayDialog("Selection error!", "You have to select a file or an A4 file compatibile with RPG MAKER tile set", "OK");
            }
        }
    }
}
