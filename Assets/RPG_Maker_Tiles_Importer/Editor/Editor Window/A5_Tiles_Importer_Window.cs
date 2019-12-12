#define DEBUG   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class A5_Tiles_Importer_Window : EditorWindow
{
    /// <summary>
    /// Scroll position
    /// </summary>
    Vector2 scrollPosition = Vector2.zero;

    /// <summary>
    /// Loaded Image from file
    /// </summary>
    protected Texture2D img = null;

    /// <summary>
    /// List of boolean to select the block to import
    /// </summary>
    protected List<bool> sub_blocks_to_import;

    /// <summary>
    /// List of the sub pieces from the tile set
    /// </summary>
    protected List<Texture2D> sub_blocks;

    protected string path;

    protected int wBlock = 32, hBlock = 32;
    protected int mini_tile_w = 16, mini_tile_h = 16;

    [MenuItem("Tools/RPGM Importer/A5/A5 Full Layout")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<A5_Tiles_Importer_Window>(false, "A5 Full Layout Impoter");
    }

    protected virtual void CutLayout()
    {
        img = new Texture2D(1, 1);
        byte[] bytedata = File.ReadAllBytes(path);
        img.LoadImage(bytedata);

        //get the sliced part
        Tiles_A5_Utility.A5_Tile_Slice_File(img, out wBlock, out hBlock, out sub_blocks);
        sub_blocks_to_import = new List<bool>();
        for (int i = 0; i < sub_blocks.Count; i++)
            sub_blocks_to_import.Add(false);
    }

    protected virtual void Select_Image()
    {
        if (GUILayout.Button("Load Image"))
        {
            path = EditorUtility.OpenFilePanel("Load Tile Set", ".", "");
            if (path != null && path != "" && File.Exists(path) && path.Contains("A5"))
            {
                CutLayout();
            }
            else
            {
                if (path != null && path != "")
                    EditorUtility.DisplayDialog("Selection error!", "You have to select a file or an A5 file compatibile with RPG MAKER tile set", "OK");
            }
        }
    }

    void OnGUI()
    {
        Select_Image();

        if (img == null) return;

        if (GUILayout.Button("Generate Tiles"))
        {
            //generate the top tile. They are A2 style tile
            Generate_Tiles(path, sub_blocks, sub_blocks_to_import, wBlock, hBlock);
        }
        GUILayout.Label("Select the tile you want to import, then click the 'Generate Tiles' Button");
        //can select or deselect all
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All"))
        {
            if (sub_blocks_to_import != null)
                for (int i = 0; i < sub_blocks_to_import.Count; i++)
                    sub_blocks_to_import[i] = true;
        }
        if (GUILayout.Button("Select None"))
        {
            if (sub_blocks_to_import != null)
                for (int i = 0; i < sub_blocks_to_import.Count; i++)
                    sub_blocks_to_import[i] = false;
        }
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < 8*16; i++)
        {
            if (i != 0 && i % 8 == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
            GUILayout.BeginVertical();
            if (sub_blocks != null)
            {
                Texture2D sub_top = sub_blocks[i];
                sub_blocks_to_import[i] = GUILayout.Toggle(sub_blocks_to_import[i], sub_top);
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// Generate the selected tiles
    /// </summary>
    /// <param name="path"></param>
    /// <param name="sub_blocks"></param>
    /// <param name="sub_blocksto_import"></param>
    /// <param name="wBlock"></param>
    /// <param name="hBlock"></param>
    public virtual void Generate_Tiles(string path, List<Texture2D> sub_blocks, List<bool> sub_blocksto_import, int wBlock, int hBlock)
    {
        //create the final directory for the auto tile
        if (!Directory.Exists(Tiles_Utility.Auto_Tile_Folder_Path))
            Directory.CreateDirectory(Tiles_Utility.Auto_Tile_Folder_Path);

        //create the final directory for the generated Images
        if (!Directory.Exists(Tiles_Utility.final_image_folder_path))
            Directory.CreateDirectory(Tiles_Utility.final_image_folder_path);

        //create the folder for that specific file image
        string fileName = Path.GetFileNameWithoutExtension(path);
        string loaded_file_image_path = string.Format(@"{0}/_{1}", Tiles_Utility.final_image_folder_path, fileName); //ex rtp_import\Outside_A2\single_block_folder\final_tile\Image
        if (!Directory.Exists(loaded_file_image_path))
            Directory.CreateDirectory(loaded_file_image_path);

        List<string> images_path = new List<string>();//list of the folder of the imported tiles

        //foreach sub pieces in the image. If it's an animated auto tile 3 consecutive sub blocks are 3 frame of the animation
        for (int sub_block_count = 0; sub_block_count < sub_blocks_to_import.Count; sub_block_count++)
        {
            //If the current sub is not selected to process than skip it
            if (!sub_blocks_to_import[sub_block_count]) continue;

            int tiles_counter = 0; //set zero to che final tile counter
            Texture2D sub_piece = sub_blocks[sub_block_count];

            //save each final tile to its own image
            var tile_bytes = sub_blocks[sub_block_count].EncodeToPNG();
            string tile_file_path = string.Format(@"{0}/_{1}_{2}_{3:000}.png", loaded_file_image_path,
                Path.GetFileNameWithoutExtension(path), sub_block_count, tiles_counter);


            images_path.Add(tile_file_path);

            File.WriteAllBytes(tile_file_path, tile_bytes);
            AssetDatabase.Refresh();
            TextureImporter importer = AssetImporter.GetAtPath(tile_file_path) as TextureImporter;
            
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Point;
                importer.spritePixelsPerUnit = hBlock;
                importer.compressionQuality = 0;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = wBlock;
                importer.SaveAndReimport();
            }

            tiles_counter++;                
        }
        AssetDatabase.Refresh(); //refresh asset database

        //generate the fixed Auto tiles
        for (int i = 0; i < images_path.Count; i ++)
        {
            Tiles_A5_Utility.Generate_A5_Tile(path, images_path[i]);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
