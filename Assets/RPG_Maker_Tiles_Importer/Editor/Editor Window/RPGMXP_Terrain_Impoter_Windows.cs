#define DEBUG   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class RPGMXP_Terrain_Impoter_Windows : A2_Single_Terrain_Importer {

    [MenuItem("Tools/RPGM XP/Terrain Auto Tile")]
    public new static void ShowWindow()
    {
        EditorWindow.GetWindow<RPGMXP_Terrain_Impoter_Windows>(false, "Terrain Importer");
    }

    protected override void CutLayout()
    {
        img = new Texture2D(1, 1);
        byte[] bytedata = File.ReadAllBytes(path);
        img.LoadImage(bytedata);

        //img = RPGM_XP_Utility.VXACE_2_XP(tmp);

        //sub block slicing
        //get the sliced part
        Vector2Int sub_size = new Vector2Int(img.width, img.height); //that is a fixed number of blocks
                                                                     //divide in sub blocks

        mini_tile_w = sub_size.x / 6;
        wBlock = mini_tile_w * 2;
        mini_tile_h = sub_size.y / 8;
        hBlock = mini_tile_h * 2;

        sub_blocks = new List<Texture2D>();
        sub_blocks.Add(img);

        sub_blocks_to_import = new List<bool>();
        foreach (var t in sub_blocks)
            sub_blocks_to_import.Add(false);
    }

    void OnGUI()
    {
        Select_Image();

        if (img == null) return;

        if (GUILayout.Button("Generate Tiles"))
        {
            //generate the final tiles for the tile palette
            Generate_Tiles(path, sub_blocks, sub_blocks_to_import, mini_tile_w, mini_tile_h, wBlock, hBlock, generate_sprite_sheet_image);
        }

        GUILayout.Label("Select the tile you want to import, then click the 'Generate Tiles' Button");
        int sub_counter = 0;
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        foreach (var sub in sub_blocks)
        {
            //toggle to select the sub block of the image
            sub_blocks_to_import[sub_counter] = GUILayout.Toggle(sub_blocks_to_import[sub_counter], sub);
            sub_counter++;
            if (sub_counter % 8 == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
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

    public static new void Generate_Tiles(string path, List<Texture2D> sub_blocks, List<bool> sub_blocks_to_import, int mini_tile_w, int mini_tile_h, int wBlock, int hBlock, bool generate_sprite_sheet_image)
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

        List<string> images_path = new List<string>();//list of the path of the impoted tiles

        Dictionary<byte, int> rule_tiles = new Dictionary<byte, int>(); //dictionary for the tile rules

        //foreach sub pieces in the image
        for (int sub_block_count = 0; sub_block_count < sub_blocks.Count; sub_block_count++)
        {
            if (!sub_blocks_to_import[sub_block_count]) continue; //If the current sub is not selected to process than skip it

            int tiles_counter = 0; // counter to enumerate the sprite

            Texture2D sub_piece = sub_blocks[sub_block_count]; //get the texture            

            //temp array to store the sub mini tiles
            Texture2D[] bottom_left_mini_tiles, bottom_right_mini_tiles, top_left_mini_tiles, top_right_mini_tiles;

            //generate the mini tiles to the following computation
            RPGM_XP_Utility.Generate_Mini_Tile_RPGMXP(sub_piece, mini_tile_w, mini_tile_h, out bottom_left_mini_tiles, out bottom_right_mini_tiles, out top_left_mini_tiles, out top_right_mini_tiles);

            int wb = 8, hb = 7;
            Texture2D sprite_tiles = new Texture2D(wBlock * wb, hBlock * hb);
            int sprite_tile_width = Mathf.Max(wBlock * wb, wBlock * hb);
            string sprite_sheet_path = string.Format(@"{0}/_{1}_{2}.png", loaded_file_image_path, Path.GetFileNameWithoutExtension(path), sub_block_count);

            //generate and iterate the final tile for the subs pieces
            foreach (KeyValuePair<byte, Texture2D> kvp in RPGM_XP_Utility.Generate_Final_Tiles_RPGMXP(mini_tile_w, mini_tile_h,
                bottom_left_mini_tiles, bottom_right_mini_tiles, top_left_mini_tiles, top_right_mini_tiles, rule_tiles))
            {
                int xx = tiles_counter % 8 * wBlock;
                int yy = tiles_counter / 8 * hBlock;
                sprite_tiles.SetPixels(xx, sprite_tiles.height - yy - hBlock, wBlock, hBlock, kvp.Value.GetPixels());
                tiles_counter++;
            }

            images_path.Add(sprite_sheet_path);
            File.WriteAllBytes(sprite_sheet_path, sprite_tiles.EncodeToPNG());
            AssetDatabase.Refresh();
            TextureImporter importer = AssetImporter.GetAtPath(sprite_sheet_path) as TextureImporter;
            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<Sprite>(sprite_sheet_path));
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.filterMode = FilterMode.Point;
                importer.spritePixelsPerUnit = hBlock;
                importer.compressionQuality = 0;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = sprite_tile_width;
                SpriteMetaData[] tmps = new SpriteMetaData[wb * hb];
                string tmpName = Path.GetFileNameWithoutExtension(sprite_sheet_path);
                for (int i = 0; i < wb*hb; i++)
                {
                    int xx = i % 8 * wBlock;
                    int yy = (i / 8 + 1) * hBlock;
                    SpriteMetaData smd = new SpriteMetaData();
                    smd = new SpriteMetaData();
                    smd.alignment = 0;
                    smd.border = new Vector4(0, 0, 0, 0);
                    smd.name = string.Format("{0}_{1:00}", tmpName, i);
                    smd.pivot = new Vector2(.5f, .5f);
                    smd.rect = new Rect(xx, sprite_tiles.height - yy, wBlock, hBlock);
                    tmps[i] = smd;
                }
                importer.spritesheet = tmps;
                importer.SaveAndReimport();
            }
        }
        AssetDatabase.Refresh(); //refresh asset database

        //generate the A2_tile Auto tiles
        for (int i = 0; i < images_path.Count; i += 1)
        {
            string str = images_path[i];
            Tiles_A2_Utility.Generate_A2_Tile_SS(path, str, rule_tiles);
        }
    }
}
