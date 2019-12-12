#define DEBUG   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class A2_Single_Terrain_Importer : A2_Tiles_Importer_Window
{
    [MenuItem("Tools/RPGM Importer/A2/A2 Single Terrain Tile")]
    public new static void ShowWindow()
    {
        EditorWindow.GetWindow<A2_Single_Terrain_Importer>(false, "A2 Single Terrain Importer");
    }

    protected override void CutLayout()
    {
        img = new Texture2D(1, 1);
        byte[] bytedata = File.ReadAllBytes(path);
        img.LoadImage(bytedata);

        //sub block slicing
        //get the sliced part
        Vector2Int sub_size = new Vector2Int(img.width, img.height); //that is a fixed number of blocks
                                                                     //divide in sub blocks

        mini_tile_w = sub_size.x / 4;
        wBlock = mini_tile_w * 2;
        mini_tile_h = sub_size.y / 6;
        hBlock = mini_tile_h * 2;

        sub_blocks = new List<Texture2D>();
        sub_blocks.Add(img);

        sub_blocks_to_import = new List<bool>();
        foreach (var t in sub_blocks)
            sub_blocks_to_import.Add(false);
    }

    protected override void Select_Image()
    {
        if (GUILayout.Button("Load Image")) //open gile dialog to load the image
        {
            path = EditorUtility.OpenFilePanel("Load Tile Set", ".", "");
            if (path != null && path != "" && File.Exists(path))
            {
                CutLayout();
            }
            else
            {
                if (path != null && path != "")
                    EditorUtility.DisplayDialog("Selection error!", "You have to select a file or an A2 file compatibile with RPG MAKER tile set", "OK");
            }
        }
    }
}
