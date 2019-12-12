using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class Tiles_A2_Utility {


    /// <summary>
    /// Slice the input file to extrapolate the sub block for the A2 style file
    /// </summary>
    /// <param name="img">Input texture file</param>
    /// <param name="wBlock">output width of the final block</param>
    /// <param name="hBlock">output height of the final block</param>
    /// <param name="mini_tile_w">output width of the mini block to rework the file</param>
    /// <param name="mini_tile_h">output height of the mini block to rework the file</param>
    /// <returns></returns>
    public static List<Texture2D> A2_Tile_Slice_File(Texture2D img, out int wBlock, out int hBlock, out int mini_tile_w, out int mini_tile_h)
    {
        List<Texture2D> sub_blocks = new List<Texture2D>();
        //sub_blocks_to_import = new List<bool>();
        Vector2Int sub_size = new Vector2Int(img.width / 8, img.height / 4); //that is a fixed number of blocks
                                                                             //divide in sub blocks
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Texture2D sub = new Texture2D(sub_size.x, sub_size.y);
                sub.SetPixels(img.GetPixels(x * sub_size.x, (3 - y) * sub_size.y, sub_size.x, sub_size.y));
                sub.Apply();
                sub_blocks.Add(sub);
                //sub_blocks_to_import.Add(false); //set the selected sub to false
            }
        }
        mini_tile_w = sub_size.x / 4;
        wBlock = mini_tile_w * 2;
        mini_tile_h = sub_size.y / 6;
        hBlock = mini_tile_h * 2;
        return sub_blocks;
    }

    /// <summary>
    /// Split a single sub block into tne mini tile to be reworked
    /// </summary>
    /// <param name="img">sub block to be reworked. It MUST be an A1 Water Tile, A2 Terrain or A3 TOP-cealing tile</param>
    /// <param name="wBlock">width of the mini tile block</param>
    /// <param name="hBlock">height of the mini tile block</param>
    /// <param name="bottom_left_mini_tiles">destination array</param>
    /// <param name="bottom_right_mini_tiles">destination array/param>
    /// <param name="top_left_mini_tiles">destination array</param>
    /// <param name="top_right_mini_tiles">destination array</param>
    public static void Generate_Mini_Tile_A2(Texture2D img, int wBlock, int hBlock,
        out Texture2D[] bottom_left_mini_tiles, out Texture2D[] bottom_right_mini_tiles,
        out Texture2D[] top_left_mini_tiles, out Texture2D[] top_right_mini_tiles)
    {
        Tiles_A1_Utility.Generate_Mini_Tile_A1Water(img, wBlock, hBlock, out bottom_left_mini_tiles, out bottom_right_mini_tiles, out top_left_mini_tiles, out top_right_mini_tiles);
    }


    /// <summary>
    /// Select the correct mini tile for the combination/value. 
    /// WORK WITH THE A1 Water Like Tile (NOT THE WATER FALL OR TWISTER), FOR THE TERRAIN AND THE A4 TOP-CEALING TILE
    /// This is the MAGIC to generate the final tiles. DO NOT TOUCH PLEASE
    /// </summary>
    /// <param name="value"></param>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    /// <param name="b3"></param>
    /// <param name="bx"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    public static int Select_Mini_Tile_A2_Terrain(byte value, int b1, int b2, int b3, int bx, int by)
    {
        return Tiles_A1_Utility.Select_Mini_Tile_A1Water(value, b1, b2, b3, bx, by);
    }

    /// <summary>
    /// Generate all the final tiles combination for the A1 Water tile, the A3 terrai and A4 top-celing tile,
    /// and set up other collection to generate rule tiles etc
    /// </summary>
    /// <param name="bottom_left_mini_tiles"></param>
    /// <param name="bottom_right_mini_tiles"></param>
    /// <param name="top_left_mini_tiles"></param>
    /// <param name="top_right_mini_tiles"></param>
    /// <param name="rule_tiles"></param>
    /// <returns></returns>
    public static Dictionary<byte, Texture2D> Generate_Final_Tiles_A2_Terrain(int mini_tile_w, int mini_tile_h, Texture2D[] bottom_left_mini_tiles, Texture2D[] bottom_right_mini_tiles,
        Texture2D[] top_left_mini_tiles, Texture2D[] top_right_mini_tiles, Dictionary<byte, int> rule_tiles)
    {
        return Tiles_A1_Utility.Generate_Final_Tiles_A1_Water(mini_tile_w, mini_tile_h, bottom_left_mini_tiles, bottom_right_mini_tiles, top_left_mini_tiles, top_right_mini_tiles, rule_tiles);
    }



    /// <summary>
    /// Genetate the A2_Tile or the A4Top tile from a sprite sheet image that contains al the tile
    /// </summary>
    /// <param name="tile_path">Path of the sprite sheet image</param>
    /// <param name="rule_tiles">Rule to crate the auto tile</param>
    public static void Generate_A2_Tile_SS(string source_File_Path, string tile_path, Dictionary<byte, int> rule_tiles)
    {
        //create the auto tile
        string atile_path = string.Format(@"{0}/_{1}/{2}.asset", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path),
            Path.GetFileNameWithoutExtension(tile_path));
        string dir = string.Format(@"{0}/_{1}", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path));
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        //A2_Tile atile = new A2_Tile();
        A2_Tile atile = ScriptableObject.CreateInstance<A2_Tile>();
        if (File.Exists(atile_path))
        {
            atile = AssetDatabase.LoadAssetAtPath<A2_Tile>(atile_path);
            
        }
        EditorUtility.SetDirty(atile);

        if (atile != null)
        {
            atile.tile_Variants = new Sprite[rule_tiles.Count];
            object[] vars = AssetDatabase.LoadAllAssetsAtPath(tile_path);

            int cc = 0;
            foreach (var kvp in rule_tiles)
            {
                Sprite tmp = vars[kvp.Value + 1] as Sprite;
                if (cc == 0) //setting the sprite for the tile palette
                {
                    cc++;
                    atile.sprite = tmp;
                    atile.preview = tmp;
                }
                atile.tile_Variants[kvp.Key] = tmp;
            }
        }
        if (File.Exists(atile_path))
            AssetDatabase.SaveAssets();
        else
            AssetDatabase.CreateAsset(atile, atile_path);
    }

    /// <summary>
    /// Generate the A2_Tile or the A4Top tiles from the single image tile
    /// </summary>
    /// <param name="tile_path">Path of the dir where the single image are located</param>
    /// <param name="rule_tiles">Rule to create the tile</param>
    /// <param name="wBlock">width of the block</param>
    public static void Generate_A2_Tile(string source_File_Path, string tile_path, Dictionary<byte, int> rule_tiles, int wBlock)
    {
        //create the auto tile
        string atile_path = string.Format(@"{0}/_{1}/{2}.asset", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path),
            Path.GetFileNameWithoutExtension(tile_path));
        string dir = string.Format(@"{0}/_{1}", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path));
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        A2_Tile atile = ScriptableObject.CreateInstance<A2_Tile>();
        if (File.Exists(atile_path))
        {
            atile = AssetDatabase.LoadAssetAtPath<A2_Tile>(atile_path);
            
        }
        EditorUtility.SetDirty(atile);

        if (atile != null)
        {
            atile.tile_Variants = new Sprite[rule_tiles.Count];
            string[] tiles = Directory.GetFiles(tile_path, "*.png");
            foreach (var fTile in tiles)
            {
                //set the image importer setting
               Tiles_Utility.Set_Impoter_Settings(fTile, wBlock);
            }

            int cc = 0;
            //StreamWriter myFile = new StreamWriter(@"C:\tmp\file.txt");
            foreach (var kvp in rule_tiles)
            {
                Sprite tmp = AssetDatabase.LoadAssetAtPath<Sprite>(tiles[kvp.Value]);
                if (cc == 0) //setting the sprite for the tile palette
                {
                    cc++;
                    atile.sprite = tmp;
                    atile.preview = tmp;
                }
                atile.tile_Variants[kvp.Key] = tmp;
                //myFile.WriteLine(string.Format("{0},{1}", kvp.Key, kvp.Value));
            }
            //myFile.Close();
        }
        if (File.Exists(atile_path))
            AssetDatabase.SaveAssets();
        else
            AssetDatabase.CreateAsset(atile, atile_path);
    }
}
