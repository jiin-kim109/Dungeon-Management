using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class Tiles_A1_Utility {
    /// <summary>
    /// Slice input file to extrapolate the sub block for the A1 Style file
    /// </summary>
    /// <param name="img">Input texture file</param>
    /// <param name="wBlock">output width of the final block</param>
    /// <param name="hBlock">output height of the final block</param>
    /// <param name="mini_tile_w">output width of the mini block to rework the file</param>
    /// <param name="mini_tile_h">output height of the mini block to rework the file</param>
    /// <param name="sub_blocks_water">list of the water sub blocks</param>
    /// <param name="sub_blocks_twister">list of the water fall and twist blocks</param>
    public static void A1_Tile_Slice_File(Texture2D img, out int wBlock, out int hBlock, out int mini_tile_w, out int mini_tile_h,
        out List<Texture2D> sub_blocks_water, out List<Texture2D> sub_blocks_twister, out List<Texture2D> floating_blocks)
    {
        sub_blocks_water = new List<Texture2D>();
        sub_blocks_twister = new List<Texture2D>();
        floating_blocks = new List<Texture2D>();
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
                if (x == 7 || (x == 3 && (y == 2 || y == 3)))
                    sub_blocks_twister.Add(sub); // add waterfall and twister
                else if(y == 1 && (x == 0 || x == 1 || x == 2))
                {
                    //Color[] water = sub_blocks_water[x].GetPixels();
                    //Color[] floating = sub.GetPixels();
                    //for (int i = 0; i < water.Length; i++)
                    //{
                    //    floating[i] = Color.Lerp(water[i], floating[i], floating[i].a);
                    //}
                    //sub.SetPixels(floating);
                    //sub.Apply();
                    sub_blocks_water.Add(sub); //second type water
                }
                else if (x == 3 && (y == 0 || y == 1))
                {
                    floating_blocks.Add(sub);
                    //Color[] floating = sub.GetPixels();
                    //sub = new Texture2D(sub_size.x, sub_size.y);
                    //for (int w = 0; w < 3; w++) {
                    //    Color[] water = sub_blocks_water[w].GetPixels();
                    //    for (int i = 0; i < water.Length; i++)
                    //    {
                    //        floating[i] = Color.Lerp(water[i], floating[i], floating[i].a);
                    //    }
                    //    sub.SetPixels(floating);
                    //    sub.Apply();
                    //    sub_blocks_water.Add(sub); //add the 2 floating element
                    //}
                }
                else
                    sub_blocks_water.Add(sub); //all the animation tile for water
            }
        }
        mini_tile_w = sub_size.x / 4;
        wBlock = mini_tile_w * 2;
        mini_tile_h = sub_size.y / 6;
        hBlock = mini_tile_h * 2;
    }

    /// <summary>
    /// Slice the input file. The file contains ONLY one water animation
    /// </summary>
    /// <param name="img"></param>
    /// <param name="wBlock"></param>
    /// <param name="hBlock"></param>
    /// <param name="mini_tile_w"></param>
    /// <param name="mini_tile_h"></param>
    /// <param name="sub_blocks_water"></param>
    public static void A1_Tile_Slice_Water_Singleton(Texture2D img, out int wBlock, out int hBlock, out int mini_tile_w, out int mini_tile_h,
    out List<Texture2D> sub_blocks_water)
    {
        sub_blocks_water = new List<Texture2D>();
        //sub_blocks_to_import = new List<bool>();
        Vector2Int sub_size = new Vector2Int(img.width / 3, img.height / 1); //that is a fixed number of blocks
                                                                             //divide in sub blocks
        for (int y = 0; y < 1; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                Texture2D sub = new Texture2D(sub_size.x, sub_size.y);
                sub.SetPixels(img.GetPixels(x * sub_size.x, 0, sub_size.x, sub_size.y));
                sub.Apply();
                sub_blocks_water.Add(sub); //all the animation tile for water
            }
        }
        mini_tile_w = sub_size.x / 4;
        wBlock = mini_tile_w * 2;
        mini_tile_h = sub_size.y / 6;
        hBlock = mini_tile_h * 2;
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
    public static void Generate_Mini_Tile_A1Water(Texture2D img, int wBlock, int hBlock,
        out Texture2D[] bottom_left_mini_tiles, out Texture2D[] bottom_right_mini_tiles,
        out Texture2D[] top_left_mini_tiles, out Texture2D[] top_right_mini_tiles)
    {
        //Create the mini tile array
        bottom_left_mini_tiles = new Texture2D[10];
        bottom_right_mini_tiles = new Texture2D[10];
        top_left_mini_tiles = new Texture2D[10];
        top_right_mini_tiles = new Texture2D[10];

        Texture2D[,] rawPieces = new Texture2D[6, 4];
        int cc = 0;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 6; y++)
            {
                Texture2D tmp = new Texture2D(wBlock, hBlock);

                Color[] pixels = img.GetPixels(x * wBlock, (5 - y) * hBlock, wBlock, hBlock);
                tmp.SetPixels(0, 0, wBlock, hBlock, pixels);
                tmp.Apply();
                rawPieces[y, x] = tmp;
                cc++;
            }
        }

        //bottom_left_mini_tiles
        bottom_left_mini_tiles[0] = rawPieces[5, 0];
        bottom_left_mini_tiles[1] = rawPieces[5, 1];
        bottom_left_mini_tiles[2] = rawPieces[5, 2];
        bottom_left_mini_tiles[3] = rawPieces[4, 0];
        bottom_left_mini_tiles[4] = rawPieces[4, 1];
        bottom_left_mini_tiles[5] = rawPieces[4, 2];
        bottom_left_mini_tiles[6] = rawPieces[3, 0];
        bottom_left_mini_tiles[7] = rawPieces[3, 1];
        bottom_left_mini_tiles[8] = rawPieces[3, 2];
        bottom_left_mini_tiles[9] = rawPieces[1, 2];

        //bottom_right_mini_tiles
        bottom_right_mini_tiles[0] = rawPieces[5, 3];
        bottom_right_mini_tiles[1] = rawPieces[4, 3];
        bottom_right_mini_tiles[2] = rawPieces[3, 3];
        bottom_right_mini_tiles[3] = rawPieces[5, 2];
        bottom_right_mini_tiles[4] = rawPieces[4, 2];
        bottom_right_mini_tiles[5] = rawPieces[3, 2];
        bottom_right_mini_tiles[6] = rawPieces[5, 1];
        bottom_right_mini_tiles[7] = rawPieces[4, 1];
        bottom_right_mini_tiles[8] = rawPieces[3, 1];
        bottom_right_mini_tiles[9] = rawPieces[1, 3];

        //top_right_mini_tiles
        top_right_mini_tiles[0] = rawPieces[2, 3];
        top_right_mini_tiles[1] = rawPieces[2, 2];
        top_right_mini_tiles[2] = rawPieces[2, 1];
        top_right_mini_tiles[3] = rawPieces[3, 3];
        top_right_mini_tiles[4] = rawPieces[3, 2];
        top_right_mini_tiles[5] = rawPieces[3, 1];
        top_right_mini_tiles[6] = rawPieces[4, 3];
        top_right_mini_tiles[7] = rawPieces[4, 2];
        top_right_mini_tiles[8] = rawPieces[4, 1];
        top_right_mini_tiles[9] = rawPieces[0, 3];

        //top_left_mini_tiles
        top_left_mini_tiles[0] = rawPieces[2, 0];
        top_left_mini_tiles[1] = rawPieces[3, 0];
        top_left_mini_tiles[2] = rawPieces[4, 0];
        top_left_mini_tiles[3] = rawPieces[2, 1];
        top_left_mini_tiles[4] = rawPieces[3, 1];
        top_left_mini_tiles[5] = rawPieces[4, 1];
        top_left_mini_tiles[6] = rawPieces[2, 2];
        top_left_mini_tiles[7] = rawPieces[3, 2];
        top_left_mini_tiles[8] = rawPieces[4, 2];
        top_left_mini_tiles[9] = rawPieces[0, 2];
    }


    /// <summary>
    /// Generate the mini tiles for the water_fall/twiester tile 
    /// </summary>
    /// <param name="img"></param>
    /// <param name="wBlock"></param>
    /// <param name="hBlock"></param>
    /// <param name="left_mini_tiles"></param>
    /// <param name="right_mini_tiles"></param>
    public static void Generate_Mini_Tile_A1_Twister(Texture2D img, int wBlock, int hBlock, out Texture2D[,] left_mini_tiles, out Texture2D[,] right_mini_tiles)
    {
        Texture2D[,] rawPieces = new Texture2D[3, 4];
        left_mini_tiles = new Texture2D[3, 3];
        right_mini_tiles = new Texture2D[3, 4];
        int cc = 0;
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                Texture2D tmp = new Texture2D(wBlock, hBlock);

                Color[] pixels = img.GetPixels(x * wBlock, (2 - y) * hBlock, wBlock, hBlock);
                tmp.SetPixels(0, 0, wBlock, hBlock, pixels);
                tmp.Apply();
                rawPieces[y, x] = tmp;
                cc++;
            }
        }
        //set left tile
        left_mini_tiles[0, 0] = rawPieces[0, 0];
        left_mini_tiles[0, 1] = rawPieces[0, 1];
        left_mini_tiles[0, 2] = rawPieces[0, 2];

        left_mini_tiles[1, 0] = rawPieces[1, 0];
        left_mini_tiles[1, 1] = rawPieces[1, 1];
        left_mini_tiles[1, 2] = rawPieces[1, 2];

        left_mini_tiles[2, 0] = rawPieces[2, 0];
        left_mini_tiles[2, 1] = rawPieces[2, 1];
        left_mini_tiles[2, 2] = rawPieces[2, 2];

        //set right tile
        right_mini_tiles[0, 0] = rawPieces[0, 3];
        right_mini_tiles[0, 1] = rawPieces[0, 2];
        right_mini_tiles[0, 2] = rawPieces[0, 1];

        right_mini_tiles[1, 0] = rawPieces[1, 3];
        right_mini_tiles[1, 1] = rawPieces[1, 2];
        right_mini_tiles[1, 2] = rawPieces[1, 1];

        right_mini_tiles[2, 0] = rawPieces[2, 3];
        right_mini_tiles[2, 1] = rawPieces[2, 2];
        right_mini_tiles[2, 2] = rawPieces[2, 1];
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
    public static int Select_Mini_Tile_A1Water(byte value, int b1, int b2, int b3, int bx, int by)
    {
        bool bit1 = Tiles_Utility.GetBit(value, b1);
        bool bit2 = Tiles_Utility.GetBit(value, b2);
        bool bit3 = Tiles_Utility.GetBit(value, b3);
        //bool bitX = GetBit(value, bx);
        //bool bitY = GetBit(value, by);

        //corner
        if (!bit1 && !bit3)
            return 0; //mini_tiles[0];//corner

        //center
        if (bit1 && bit2 && bit3)
            return 8;

        if (!bit1 && bit3)
            return 6;

        //up side
        if (bit1 && !bit3)
            return 2;

        if (bit1 && !bit2 && bit3)
        {
            return 9; //mini_tiles[9];//tip
        }

        return 4; //mini_tiles[4];
    }

    /// <summary>
    /// Select the correct mini tile for the combination/value. 
    /// WORK WITH THE A1 Water Like Tile (NOT THE WATER FALL OR TWISTER), FOR THE TERRAIN AND THE A4 TOP-CEALING TILE
    /// This is the MAGIC to generate the final tiles. DO NOT TOUCH PLEASE
    /// </summary>
    /// <param name="value"></param>
    /// <param name="bl"></param>
    /// <param name="br"></param>
    /// <returns></returns>
    public static int Select_Mini_Tile_A1_Twister(byte value, int b1)
    {
        bool bitl = Tiles_Utility.GetBit(value, b1);

        //corner
        if (!bitl)
            return 0; //mini_tiles[0];//corner
        return 2;
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
    public static Dictionary<byte, Texture2D> Generate_Final_Tiles_A1_Water(int mini_tile_w, int mini_tile_h, Texture2D[] bottom_left_mini_tiles, Texture2D[] bottom_right_mini_tiles,
        Texture2D[] top_left_mini_tiles, Texture2D[] top_right_mini_tiles, Dictionary<byte, int> rule_tiles)
    {
        Dictionary<byte, Texture2D> final_pieces = new Dictionary<byte, Texture2D>(); //pezzi finali da considerare per la creazione dell'immagine

        rule_tiles.Clear(); //lista delle regole di assegnamento

        List<string> used_Combination = new List<string>(); //dictionary of used combinations
        foreach (var comb in Tiles_Utility.ByteCombination())
        {
            Texture2D tmp = new Texture2D(mini_tile_w * 2, mini_tile_h * 2); //to make parametric 
            #region DO NOT TOUCH! This is the magic
            int bl = Select_Mini_Tile_A1Water(comb, 5, 8, 7, 2, 4);
            int br = Select_Mini_Tile_A1Water(comb, 7, 6, 4, 5, 2);
            int tr = Select_Mini_Tile_A1Water(comb, 4, 1, 2, 7, 5);
            int tl = Select_Mini_Tile_A1Water(comb, 2, 3, 5, 4, 7);

            tmp.SetPixels(0, 0, mini_tile_w, mini_tile_h, bottom_left_mini_tiles[bl].GetPixels());
            tmp.SetPixels(mini_tile_w, 0, mini_tile_w, mini_tile_h, bottom_right_mini_tiles[br].GetPixels());
            tmp.SetPixels(0, mini_tile_h, mini_tile_w, mini_tile_h, top_left_mini_tiles[tl].GetPixels());
            tmp.SetPixels(mini_tile_w, mini_tile_h, mini_tile_w, mini_tile_h, top_right_mini_tiles[tr].GetPixels());
            tmp.Apply();
            #endregion

            string key = string.Format("{0}{1}{2}{3}", bl, br, tl, tr);
            if (!used_Combination.Contains(key))
            {
                used_Combination.Add(key); //salvo le key degli sprite usate
                final_pieces.Add(comb, tmp);
            }
            int index = used_Combination.IndexOf(key); //retrive the index
            rule_tiles.Add(comb, index);
        }
        return final_pieces;
    }

    /// <summary>
    /// Generate the twister/water fall final tile
    /// </summary>
    /// <param name="mini_tile_w"></param>
    /// <param name="mini_tile_h"></param>
    /// <param name="left_mini_tiles"></param>
    /// <param name="right_mini_tiles"></param>
    /// <param name="rule_tiles"></param>
    /// <returns></returns>
    public static List<Texture2D> Generate_Final_Tiles_A1_Twister(int mini_tile_w, int mini_tile_h, Texture2D[,] left_mini_tiles, Texture2D[,] right_mini_tiles, Dictionary<byte, int> rule_tiles)
    {
        List<Texture2D> final_pieces = new List<Texture2D>(); //pezzi finali da considerare per la creazione dell'immagine

        rule_tiles.Clear(); //lista delle regole di assegnamento

        List<string> used_Combination = new List<string>(); //dictionary of used combinations

        for (int i = 0; i < 3; i++)
        {
            foreach (var comb in Tiles_Utility.Left_Right_Combination())
            {
                Texture2D tmp = new Texture2D(mini_tile_w * 2, mini_tile_h); //to make parametric 
                #region DO NOT TOUCH! This is the magic
                int l = Select_Mini_Tile_A1_Twister(comb, 5);
                int r = Select_Mini_Tile_A1_Twister(comb, 4);

                tmp.SetPixels(0, 0, mini_tile_w, mini_tile_h, left_mini_tiles[i, l].GetPixels());
                tmp.SetPixels(mini_tile_w, 0, mini_tile_w, mini_tile_h, right_mini_tiles[i, r].GetPixels());
                tmp.Apply();
                #endregion

                string key = string.Format("{0}{1}", l, r);
                if (!used_Combination.Contains(key))
                    used_Combination.Add(key); //salvo le key degli sprite usate
                final_pieces.Add(tmp);
                int index = used_Combination.IndexOf(key); //retrive the index
                if (!rule_tiles.ContainsKey(comb))
                    rule_tiles.Add(comb, index);
            }
        }
        return final_pieces;
    }

    /// <summary>
    /// Genetate the A1_Tile or the water tile from the 3 single image tile
    /// </summary>
    /// <param name="tile_path1"></param>
    /// <param name="tile_path2"></param>
    /// <param name="tile_path3"></param>
    /// <param name="rule_tiles"></param>
    /// <param name="wBlock"></param>
    public static void Generate_A1_Water_Tile(string source_File_Path, string frame1, string frame2, string frame3,
        Dictionary<byte, int> rule_tiles, int wBlock)
    {
        //create the auto tile for the animation water 
        string atile_path = string.Format(@"{0}/_{1}/{2}.asset", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path), Path.GetFileNameWithoutExtension(frame1));
        string dir = string.Format(@"{0}/_{1}", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path));
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        A1_Water_Tile atile = ScriptableObject.CreateInstance<A1_Water_Tile>();
        if (File.Exists(atile_path))
        {
            atile = AssetDatabase.LoadAssetAtPath<A1_Water_Tile>(atile_path);
            
        }

        EditorUtility.SetDirty(atile);

        if (atile != null)
        {
            atile.frame1 = new Sprite[256];
            atile.frame2 = new Sprite[256];
            atile.frame3 = new Sprite[256];
            string[] tiles1 = Directory.GetFiles(frame1, "*.png");
            foreach (var fTile in tiles1)
            {
                Tiles_Utility.//set the image importer setting
                Set_Impoter_Settings(fTile, wBlock);
            }

            string[] tiles2 = Directory.GetFiles(frame2, "*.png");
            foreach (var fTile in tiles2)
            {
                //set the image importer setting
                Tiles_Utility.Set_Impoter_Settings(fTile, wBlock);
            }

            string[] tiles3 = Directory.GetFiles(frame3, "*.png");
            foreach (var fTile in tiles3)
            {
                //set the image importer setting
                Tiles_Utility.Set_Impoter_Settings(fTile, wBlock);
            }

            int cc = 0;
            foreach (var kvp in rule_tiles)
            {
                Sprite tmp1 = AssetDatabase.LoadAssetAtPath<Sprite>(tiles1[kvp.Value]);
                Sprite tmp2 = AssetDatabase.LoadAssetAtPath<Sprite>(tiles2[kvp.Value]);
                Sprite tmp3 = AssetDatabase.LoadAssetAtPath<Sprite>(tiles3[kvp.Value]);
                if (cc == 0) //setting the sprite for the tile palette
                {
                    cc++;
                    atile.sprite = tmp1;
                    atile.preview = tmp1;
                }
                atile.frame1[kvp.Key] = tmp1;
                atile.frame2[kvp.Key] = tmp2;
                atile.frame3[kvp.Key] = tmp3;
            }
        }
        if (File.Exists(atile_path))
            AssetDatabase.SaveAssets();
        else
            AssetDatabase.CreateAsset(atile, atile_path);
    }

    /// <summary>
    /// Genetate the A1_Tile or the water tile from a sprite sheet image that contais all the tile
    /// </summary>
    /// <param name="frame1"></param>
    /// <param name="frame2"></param>
    /// <param name="frame3"></param>
    /// <param name="rule_tiles"></param>
    /// <param name="wBlock"></param>
    public static void Generate_A1_Water_Tile_SS(string source_File_Path, string frame1, string frame2, string frame3,
    Dictionary<byte, int> rule_tiles, int wBlock)
    {
        //create the auto tile for the animation water 
        string atile_path = string.Format(@"{0}/_{1}/{2}.asset", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path), Path.GetFileNameWithoutExtension(frame1));
        string dir = string.Format(@"{0}/_{1}", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path));
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        //A1_Water_Tile atile = new A1_Water_Tile();
        A1_Water_Tile atile = ScriptableObject.CreateInstance<A1_Water_Tile>();

        if (File.Exists(atile_path))
        {
            atile = AssetDatabase.LoadAssetAtPath<A1_Water_Tile>(atile_path);
        }

        EditorUtility.SetDirty(atile);

        if (atile != null)
        {
            atile.frame1 = new Sprite[256];
            atile.frame2 = new Sprite[256];
            atile.frame3 = new Sprite[256];
            object[] vars1 = AssetDatabase.LoadAllAssetsAtPath(frame1);
            object[] vars2 = AssetDatabase.LoadAllAssetsAtPath(frame2);
            object[] vars3 = AssetDatabase.LoadAllAssetsAtPath(frame3);

            int cc = 0;
            foreach (var kvp in rule_tiles)
            {
                Sprite tmp1 = vars1[kvp.Value + 1] as Sprite;
                Sprite tmp2 = vars2[kvp.Value + 1] as Sprite;
                Sprite tmp3 = vars3[kvp.Value + 1] as Sprite;
                if (cc == 0) //setting the sprite for the tile palette
                {
                    cc++;
                    atile.sprite = tmp1;
                    atile.preview = tmp1;
                }
                atile.frame1[kvp.Key] = tmp1;
                atile.frame2[kvp.Key] = tmp2;
                atile.frame3[kvp.Key] = tmp3;
            }
        }
        if (File.Exists(atile_path))
            AssetDatabase.SaveAssets();
        else
            AssetDatabase.CreateAsset(atile, atile_path);
    }

    /// <summary>
    /// Generate the A1 Tile for the twister or water fall from a sprite sheet image that contains all the tiles
    /// </summary>
    /// <param name="frame1"></param>
    /// <param name="frame2"></param>
    /// <param name="frame3"></param>
    /// <param name="rule_tiles"></param>
    /// <param name="wBlock"></param>
    public static void Generate_A1_Twister_Tile_SS(string source_File_Path, string frame1, Dictionary<byte, int> rule_tiles, int wBlock)
    {
        //create the auto tile for the animation water 
        string atile_path = string.Format(@"{0}/_{1}/{2}.asset", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path),
            Path.GetFileNameWithoutExtension(frame1));

        string dir = string.Format(@"{0}/_{1}", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path));
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        //A1_Water_Tile atile = new A1_Water_Tile();
        A1_WaterFall_Tile atile = ScriptableObject.CreateInstance<A1_WaterFall_Tile>();
        atile.animation_Speed = 4; //this is a base value for a nice speed animation

        if (File.Exists(atile_path))
        {
            atile = AssetDatabase.LoadAssetAtPath<A1_WaterFall_Tile>(atile_path);
            
        }
        EditorUtility.SetDirty(atile);

        if (atile != null)
        {
            atile.frame1 = new Sprite[256];
            atile.frame2 = new Sprite[256];
            atile.frame3 = new Sprite[256];
            object[] vars1 = AssetDatabase.LoadAllAssetsAtPath(frame1);

            int cc = 0;
            foreach (var kvp in rule_tiles)
            {
                Sprite tmp1 = vars1[kvp.Value + 1] as Sprite;
                Sprite tmp2 = vars1[kvp.Value + 1 + 4] as Sprite;
                Sprite tmp3 = vars1[kvp.Value + 1 + 8] as Sprite;
                if (cc == 0) //setting the sprite for the tile palette
                {
                    cc++;
                    atile.sprite = tmp1;
                    atile.preview = tmp1;
                }
                atile.frame1[kvp.Key] = tmp1;
                atile.frame2[kvp.Key] = tmp2;
                atile.frame3[kvp.Key] = tmp3;
            }
        }
        if (File.Exists(atile_path))
            AssetDatabase.SaveAssets();
        else
            AssetDatabase.CreateAsset(atile, atile_path);
    }

    /// <summary>
    /// Generate the A1 water fall tile from the single images tile
    /// </summary>
    /// <param name="source_File_Path"></param>
    /// <param name="frame1"></param>
    /// <param name="rule_tiles"></param>
    /// <param name="wBlock"></param>
    public static void Generate_A1_Twister_Tile(string source_File_Path, string tiles_folder, Dictionary<byte, int> rule_tiles, int wBlock)
    {
        //create the auto tile for the animation water 
        string atile_path = string.Format(@"{0}/_{1}/{2}.asset", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path),
            Path.GetFileNameWithoutExtension(tiles_folder));

        string dir = string.Format(@"{0}/_{1}", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path));
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        //A1_Water_Tile atile = new A1_Water_Tile();
        A1_WaterFall_Tile atile = ScriptableObject.CreateInstance<A1_WaterFall_Tile>();
        atile.animation_Speed = 4; //this is a base value for a nice speed animation

        if (File.Exists(atile_path))
        {
            atile = AssetDatabase.LoadAssetAtPath<A1_WaterFall_Tile>(atile_path);
            
        }
        EditorUtility.SetDirty(atile);

        if (atile != null)
        {
            atile.frame1 = new Sprite[256];
            atile.frame2 = new Sprite[256];
            atile.frame3 = new Sprite[256];
            string[] tiles = Directory.GetFiles(tiles_folder, "*.png");
            foreach (var fTile in tiles)
            {
                //set the image importer setting
                Tiles_Utility.Set_Impoter_Settings(fTile, wBlock);
            }

            int cc = 0;
            foreach (var kvp in rule_tiles)
            {
                Sprite tmp1 = AssetDatabase.LoadAssetAtPath<Sprite>(tiles[kvp.Value]) as Sprite;
                Sprite tmp2 = AssetDatabase.LoadAssetAtPath<Sprite>(tiles[kvp.Value + 4]) as Sprite;
                Sprite tmp3 = AssetDatabase.LoadAssetAtPath<Sprite>(tiles[kvp.Value + 8]) as Sprite;
                if (cc == 0) //setting the sprite for the tile palette
                {
                    cc++;
                    atile.sprite = tmp1;
                    atile.preview = tmp1;
                }
                atile.frame1[kvp.Key] = tmp1;
                atile.frame2[kvp.Key] = tmp2;
                atile.frame3[kvp.Key] = tmp3;
            }
        }
        if (File.Exists(atile_path))
            AssetDatabase.SaveAssets();
        else
            AssetDatabase.CreateAsset(atile, atile_path);
    }
}
