#define DEBUG   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class A1_Tiles_Importer_Window : EditorWindow
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
    /// List of the sub pieces from the tile set
    /// </summary>
    protected List<Texture2D> sub_blocks_water;

    /// <summary>
    /// List of the sub pieces from the tile set
    /// </summary>
    protected List<Texture2D> sub_blocks_floating;

    /// <summary>
    /// List of the sub pieces from the tile set
    /// </summary>
    protected List<Texture2D> sub_blocks_twister;

    /// <summary>
    /// List of boolean to select the block to import
    /// </summary>
    protected List<bool> sub_blocks_water_to_import;

    /// <summary>
    /// List of boolean to select the block to import
    /// </summary>
    protected List<bool> sub_blocks_floating_to_import;

    /// <summary>
    /// List of boolean to select the block to import
    /// </summary>
    protected List<bool> sub_blocks_twister_to_import;

    /// <summary>
    /// Contains the list of frame to show the animation in the editor
    /// </summary>
    protected List<List<Texture2D>> waterFallFrame_Animation = new List<List<Texture2D>>();

    /// <summary>
    /// The file path of the original image
    /// </summary>
    protected string path;

    protected int wBlock = 32, hBlock = 32;
    protected int mini_tile_w = 16, mini_tile_h = 16;

    /// Set to generate a single sprite sheet file or multiple single image
    /// </summary>
    protected bool generate_sprite_sheet_image = true;

    /// <summary>
    /// Frame count for the animation in the editor
    /// </summary>
    protected int frameCount = 0;

    protected int animFrame = 4;

    [MenuItem("Tools/RPGM Importer/A1/A1 FULL Layout")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<A1_Tiles_Importer_Window>(false, "A1 Full Layout Importer");
    }

    protected virtual void Update()
    {
        frameCount++;
        Repaint();
    }

    protected virtual void CutLayout()
    {
        img = new Texture2D(1, 1);
        byte[] bytedata = File.ReadAllBytes(path);
        img.LoadImage(bytedata);

        //get the sliced part
        Tiles_A1_Utility.A1_Tile_Slice_File(img, out wBlock, out hBlock, out mini_tile_w, out mini_tile_h, out sub_blocks_water, out sub_blocks_twister, out sub_blocks_floating);
        sub_blocks_water_to_import = new List<bool>();
        for (int i = 0; i < sub_blocks_water.Count / 3; i++)
            sub_blocks_water_to_import.Add(false);

        sub_blocks_floating_to_import = new List<bool>();
        for (int i = 0; i < sub_blocks_floating.Count; i++)
            sub_blocks_floating_to_import.Add(false);

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

    protected virtual void Select_Image()
    {
        if (GUILayout.Button("Load Image"))
        {
            path = EditorUtility.OpenFilePanel("Load Tile Set", ".", "");
            if (path != null && path != "" && File.Exists(path) && path.Contains("A1"))
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

    protected virtual void OnGUI()
    {
        generate_sprite_sheet_image = GUILayout.Toggle(generate_sprite_sheet_image, "Generate Sprite Sheet Image");
        Select_Image();

        if (img == null) return;

        if (GUILayout.Button("Generate Tiles"))
        {
            //generate the floating tile. They are A2 style tile
            A2_Tiles_Importer_Window.Generate_Tiles(path, sub_blocks_floating, sub_blocks_floating_to_import,
                mini_tile_w, mini_tile_h, wBlock, hBlock, generate_sprite_sheet_image);

            //generate waterfall tile style
            Generate_Twister_Tiles(path, sub_blocks_twister, sub_blocks_twister_to_import,
                mini_tile_w, mini_tile_h, wBlock, hBlock, generate_sprite_sheet_image);

            //generate the animated A1 style tile
            Generate_Water_Tiles(path, sub_blocks_water, sub_blocks_water_to_import,
                mini_tile_w, mini_tile_h, wBlock, hBlock, generate_sprite_sheet_image);
        }
        GUILayout.Label("Select the tile you want to import, then click the 'Generate Tiles' Button");
        //can select or deselect all
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All"))
        {
            if(sub_blocks_floating_to_import != null)
                for (int i = 0; i < sub_blocks_floating_to_import.Count; i++)
                    sub_blocks_floating_to_import[i] = true;

            if(sub_blocks_twister_to_import != null)
                for (int i = 0; i < sub_blocks_twister_to_import.Count; i++)
                    sub_blocks_twister_to_import[i] = true;

            if(sub_blocks_water_to_import != null)
                for (int i = 0; i < sub_blocks_water_to_import.Count; i++)
                    sub_blocks_water_to_import[i] = true;
        }
        if (GUILayout.Button("Select None"))
        {
            if (sub_blocks_floating_to_import != null)
                for (int i = 0; i < sub_blocks_floating_to_import.Count; i++)
                    sub_blocks_floating_to_import[i] = false;

            if (sub_blocks_twister_to_import != null)
                for (int i = 0; i < sub_blocks_twister_to_import.Count; i++)
                    sub_blocks_twister_to_import[i] = false;

            if (sub_blocks_water_to_import != null)
                for (int i = 0; i < sub_blocks_water_to_import.Count; i++)
                    sub_blocks_water_to_import[i] = false;
        }
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);
        GUILayout.BeginVertical();
        if (sub_blocks_water != null && sub_blocks_water.Count != 0)
        {
            int frame = (frameCount / 100) % animFrame;
            frame = (animFrame == 4) ? (frame < 3 ? frame : 1) : ((frameCount / 100) % 4);
            GUILayout.Space(10);
            GUILayout.Label("Animated Water Tile");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < sub_blocks_water.Count; i += (animFrame -1) )
            {
                Texture2D sub_water = sub_blocks_water[i + frame];
                sub_blocks_water_to_import[i / (animFrame-1)] = GUILayout.Toggle(sub_blocks_water_to_import[i / (animFrame-1)], sub_water);
            }
            GUILayout.EndHorizontal();
        }

        if (sub_blocks_twister != null && sub_blocks_twister.Count != 0)
        {
            GUILayout.Label("Animated Water fall/twister Tile");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < sub_blocks_twister.Count; i += 1)
            {
                //Texture2D sub_twister = sub_blocks_twister[i];
                int ff = (frameCount / 30) % 3;
                if (waterFallFrame_Animation != null && waterFallFrame_Animation.Count != 0)
                {
                    Texture2D sub_twister = waterFallFrame_Animation[i][ff];
                    sub_blocks_twister_to_import[i] = GUILayout.Toggle(sub_blocks_twister_to_import[i], sub_twister);
                }
            }
            GUILayout.EndHorizontal();
        }

        if (sub_blocks_floating != null && sub_blocks_floating.Count != 0)
        {
            GUILayout.Label("Floating element Tile");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < sub_blocks_floating.Count; i += 1)
            {
                Texture2D sub_float = sub_blocks_floating[i];
                sub_blocks_floating_to_import[i] = GUILayout.Toggle(sub_blocks_floating_to_import[i], sub_float);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// Generate the twister or water fall tile from the image
    /// </summary>
    /// <param name="path"></param>
    /// <param name="sub_blocks_twister"></param>
    /// <param name="sub_blocks_twister_to_import"></param>
    /// <param name="mini_tile_w"></param>
    /// <param name="mini_tile_h"></param>
    /// <param name="wBlock"></param>
    /// <param name="hBlock"></param>
    /// <param name="generate_sprite_sheet_image"></param>
    public void Generate_Twister_Tiles(string path, List<Texture2D> sub_blocks_twister, List<bool> sub_blocks_twister_to_import,
        int mini_tile_w, int mini_tile_h, int wBlock, int hBlock, bool generate_sprite_sheet_image)
    {
        if (sub_blocks_twister == null) return;
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

        Dictionary<byte, int> rule_tiles = new Dictionary<byte, int>();

        //foreach sub pieces in the image. If it's an animated auto tile 3 consecutive sub blocks are 3 frame of the animation
        for (int sub_block_count = 0; sub_block_count < sub_blocks_twister_to_import.Count; sub_block_count++)
        {
            //If the current sub is not selected to process than skip it
            if (!sub_blocks_twister_to_import[sub_block_count]) continue;

            int tiles_counter = 0; //set zero to che final tile counter
            Texture2D sub_piece = sub_blocks_twister[sub_block_count];

            //temp array to store the sub mini tiles
            Texture2D[,] left_mini_tiles, right_mini_tiles;

            //generate the mini tiles to the following computation
            Tiles_A1_Utility.Generate_Mini_Tile_A1_Twister(sub_piece, mini_tile_w, mini_tile_h*2, out left_mini_tiles, out right_mini_tiles);

            if (generate_sprite_sheet_image)
            {
                Texture2D sprite_tiles = new Texture2D(wBlock * 4, hBlock * 3);
                string sprite_sheet_path = string.Format(@"{0}/_Water_fall_{1}_{2}.png", loaded_file_image_path, Path.GetFileNameWithoutExtension(path), sub_block_count);

                //generate and iterate the final tile for the subs pieces
                foreach (Texture2D tile in Tiles_A1_Utility.Generate_Final_Tiles_A1_Twister(mini_tile_w, mini_tile_h*2, left_mini_tiles, right_mini_tiles, rule_tiles))
                {
                    int xx = tiles_counter % 4 * wBlock;
                    int yy = (tiles_counter / 4 * hBlock);
                    sprite_tiles.SetPixels(xx, sprite_tiles.height - yy - hBlock, wBlock, hBlock, tile.GetPixels());
                    tiles_counter++;
                }

                images_path.Add(sprite_sheet_path);
                File.WriteAllBytes(sprite_sheet_path, sprite_tiles.EncodeToPNG());
                AssetDatabase.Refresh();
                TextureImporter importer = AssetImporter.GetAtPath(sprite_sheet_path) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Multiple;
                    importer.filterMode = FilterMode.Point;
                    importer.spritePixelsPerUnit = hBlock;
                    importer.compressionQuality = 0;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.maxTextureSize = sprite_tiles.width;
                    SpriteMetaData[] tmps = new SpriteMetaData[12];
                    string tmpName = Path.GetFileNameWithoutExtension(sprite_sheet_path);
                    for (int j = 0; j < 12; j++)
                    {
                        int xx = j % 4 * wBlock;
                        int yy = (j / 4 + 1) * hBlock;
                        SpriteMetaData smd = new SpriteMetaData();
                        smd = new SpriteMetaData();
                        smd.alignment = 0;
                        smd.border = new Vector4(0, 0, 0, 0);
                        smd.name = string.Format("{0}_{1:00}", tmpName, j);
                        smd.pivot = new Vector2(.5f, .5f);
                        smd.rect = new Rect(xx, sprite_tiles.height - yy, wBlock, hBlock);
                        tmps[j] = smd;
                    }
                    importer.spritesheet = tmps;
                    importer.SaveAndReimport();
                }
            }
            else {
                //create the directory for the final images
                string tile_folder_path = string.Format(@"{0}/_Water_Fall_{1}_{2}", loaded_file_image_path, Path.GetFileNameWithoutExtension(path), sub_block_count);

                //add the path of the this that will contains alla the sub block final tiles
                images_path.Add(tile_folder_path);
                if (!Directory.Exists(tile_folder_path))
                    Directory.CreateDirectory(tile_folder_path);

                //generate and iterate the final tile for the subs pieces
                foreach (Texture2D tile in Tiles_A1_Utility.Generate_Final_Tiles_A1_Twister(mini_tile_w, mini_tile_h * 2, left_mini_tiles, right_mini_tiles, rule_tiles))
                {
                    //save each final tile to its own image
                    var tile_bytes = tile.EncodeToPNG();
                    string tile_file_path = string.Format(@"{0}/_Water_Fall_{1}_{2}_{3:000}.png", tile_folder_path,
                        Path.GetFileNameWithoutExtension(path), sub_block_count, tiles_counter);

                    File.WriteAllBytes(tile_file_path, tile_bytes);
                    tiles_counter++;
                }
            }
        }
        AssetDatabase.Refresh(); //refresh asset database

        //generate the fixed animated water fall tile
        for (int i = 0; i < images_path.Count; i++)
        {
            if (generate_sprite_sheet_image)
            {
                Tiles_A1_Utility.Generate_A1_Twister_Tile_SS(path, images_path[i], rule_tiles, wBlock);
            }
            else
            {
                Tiles_A1_Utility.Generate_A1_Twister_Tile(path, images_path[i], rule_tiles, wBlock);
            }
        }
    }

    /// <summary>
    /// Generate the selected water tile
    /// </summary>
    /// <param name="path"></param>
    /// <param name="sub_blocks_water"></param>
    /// <param name="sub_blocks_water_to_import"></param>
    /// <param name="mini_tile_w"></param>
    /// <param name="mini_tile_h"></param>
    /// <param name="wBlock"></param>
    /// <param name="hBlock"></param>
    /// <param name="generate_sprite_sheet_image"></param>
    public virtual void Generate_Water_Tiles(string path, List<Texture2D> sub_blocks_water, List<bool> sub_blocks_water_to_import,
        int mini_tile_w, int mini_tile_h, int wBlock, int hBlock, bool generate_sprite_sheet_image)
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

        Dictionary<byte, int> rule_tiles = new Dictionary<byte, int>();

        //foreach sub pieces in the image. If it's an animated auto tile 3 consecutive sub blocks are 3 frame of the animation
        for (int sub_block_count = 0; sub_block_count < sub_blocks_water_to_import.Count; sub_block_count++)
        {
            //If the current sub is not selected to process than skip it
            if (!sub_blocks_water_to_import[sub_block_count]) continue;

            for (int i = sub_block_count * 3; i < sub_block_count * 3 + 3; i++)
            {
                int tiles_counter = 0; //set zero to che final tile counter
                Texture2D sub_piece = sub_blocks_water[i];

                //temp array to store the sub mini tiles
                Texture2D[] bottom_left_mini_tiles, bottom_right_mini_tiles, top_left_mini_tiles, top_right_mini_tiles;

                //generate the mini tiles to the following computation
                Tiles_A1_Utility.Generate_Mini_Tile_A1Water(sub_piece, mini_tile_w, mini_tile_h, out bottom_left_mini_tiles, out bottom_right_mini_tiles, out top_left_mini_tiles, out top_right_mini_tiles);

                if (generate_sprite_sheet_image)
                {
                    Texture2D sprite_tiles = new Texture2D(wBlock * 8, hBlock * 6);
                    string sprite_sheet_path = string.Format(@"{0}/_Water_{1}_{2}.png", loaded_file_image_path, Path.GetFileNameWithoutExtension(path), i);

                    //generate and iterate the final tile for the subs pieces
                    foreach (KeyValuePair<byte, Texture2D> kvp in Tiles_A1_Utility.Generate_Final_Tiles_A1_Water(mini_tile_w, mini_tile_h,
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
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Multiple;
                        importer.filterMode = FilterMode.Point;
                        importer.spritePixelsPerUnit = hBlock;
                        importer.compressionQuality = 0;
                        importer.maxTextureSize = sprite_tiles.width;
                        SpriteMetaData[] tmps = new SpriteMetaData[8 * 6];
                        string tmpName = Path.GetFileNameWithoutExtension(sprite_sheet_path);
                        for (int j = 0; j < 48; j++)
                        {
                            int xx = j % 8 * wBlock;
                            int yy = (j / 8 + 1) * hBlock;
                            SpriteMetaData smd = new SpriteMetaData();
                            smd = new SpriteMetaData();
                            smd.alignment = 0;
                            smd.border = new Vector4(0, 0, 0, 0);
                            smd.name = string.Format("{0}_{1:00}", tmpName, j);
                            smd.pivot = new Vector2(.5f, .5f);
                            smd.rect = new Rect(xx, sprite_tiles.height - yy, wBlock, hBlock);
                            tmps[j] = smd;
                        }
                        importer.spritesheet = tmps;
                        importer.SaveAndReimport();
                    }
                }
                else {
                    //create the directory for the final images
                    string tile_folder_path = string.Format(@"{0}/_Water_{1}_{2}", loaded_file_image_path, Path.GetFileNameWithoutExtension(path), i);

                    //add the path of the this that will contains alla the sub block final tiles
                    images_path.Add(tile_folder_path);
                    if (!Directory.Exists(tile_folder_path))
                        Directory.CreateDirectory(tile_folder_path);

                    //generate and iterate the final tile for the subs pieces
                    foreach (KeyValuePair<byte, Texture2D> kvp in Tiles_A1_Utility.Generate_Final_Tiles_A1_Water(mini_tile_w, mini_tile_h,
                        bottom_left_mini_tiles, bottom_right_mini_tiles, top_left_mini_tiles, top_right_mini_tiles, rule_tiles))
                    {
                        //save each final tile to its own image
                        var tile_bytes = kvp.Value.EncodeToPNG();
                        string tile_file_path = string.Format(@"{0}/_Water_{1}_{2}_{3:000}.png", tile_folder_path,
                            Path.GetFileNameWithoutExtension(path), sub_block_count, tiles_counter);

                        File.WriteAllBytes(tile_file_path, tile_bytes);
                        tiles_counter++;
                    }
                }
            }
        }
        AssetDatabase.Refresh(); //refresh asset database

        //generate the fixed Auto tiles
        for (int i = 0; i < images_path.Count; i +=  3)
        {
            if (generate_sprite_sheet_image)
            {
                Tiles_A1_Utility.Generate_A1_Water_Tile_SS(path, images_path[i], images_path[i + 1], images_path[i + 2], rule_tiles, wBlock);
            }
            else
                Tiles_A1_Utility.Generate_A1_Water_Tile(path, images_path[i], images_path[i + 1], images_path[i + 2], rule_tiles, wBlock);
        }
    }
}
