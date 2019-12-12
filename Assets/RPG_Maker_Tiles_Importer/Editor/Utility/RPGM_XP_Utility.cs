#define DEBUG   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class RPGM_XP_Utility  {

    public static Texture2D VXACE_2_XP(Texture2D input)
    {
        int tmpWT = input.width / 3;
        int tmpHT = input.height / 4;
        Texture2D outputT = new Texture2D(tmpWT * 2, tmpHT * 3);

        //top parts
        outputT.SetPixels(0, outputT.height - tmpHT, tmpWT, tmpHT,
            input.GetPixels(0, input.height - tmpHT, tmpWT, tmpHT));
        outputT.SetPixels(tmpWT, outputT.height - tmpHT, tmpWT, tmpHT,
            input.GetPixels(tmpWT * 2, input.height - tmpHT, tmpWT, tmpHT));

        //left side
        outputT.SetPixels(0, outputT.height - tmpHT * 2, tmpWT / 2, tmpHT,
            input.GetPixels(0, input.height - tmpHT * 2, tmpWT / 2, tmpHT));
        outputT.SetPixels(0, 0, tmpWT / 2, tmpHT,
            input.GetPixels(0, 0, tmpWT / 2, tmpHT));

        //right side
        outputT.SetPixels(outputT.width - tmpWT / 2, outputT.height - tmpHT * 2, tmpWT / 2, tmpHT,
            input.GetPixels(input.width - tmpWT / 2, input.height - tmpHT * 2, tmpWT / 2, tmpHT));
        outputT.SetPixels(outputT.width - tmpWT / 2, 0, tmpWT / 2, tmpHT,
            input.GetPixels(input.width - tmpWT / 2, 0, tmpWT / 2, tmpHT));

        //central part
        outputT.SetPixels(tmpWT, outputT.height - tmpHT * 2, tmpWT / 2, tmpHT,
            input.GetPixels(tmpWT * 2, input.height - tmpHT * 2, tmpWT / 2, tmpHT));

        outputT.SetPixels(tmpWT / 2, outputT.height - tmpHT * 2, tmpWT / 2, tmpHT,
            input.GetPixels(tmpWT / 2, input.height - tmpHT * 2, tmpWT / 2, tmpHT));

        outputT.SetPixels(tmpWT / 2, 0, tmpWT / 2, tmpHT,
            input.GetPixels(tmpWT / 2, 0, tmpWT / 2, tmpHT));

        outputT.SetPixels(tmpWT, 0, tmpWT / 2, tmpHT,
            input.GetPixels(tmpWT * 2, 0, tmpWT / 2, tmpHT));

        outputT.Apply();

        return outputT;
    }

    public static void RPGM_XP_Water_Slice(Texture2D img, out int wBlock, out int hBlock, out int mini_tile_w, out int mini_tile_h,
    out List<Texture2D> sub_blocks_water)
    {
        sub_blocks_water = new List<Texture2D>();
        //sub_blocks_to_import = new List<bool>();
        Vector2Int sub_size = new Vector2Int(img.width / 4, img.height); //that is a fixed number of blocks
                                                                             //divide in sub blocks
        for (int y = 0; y < 1; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                Texture2D sub = new Texture2D(sub_size.x, sub_size.y);
                sub.SetPixels(img.GetPixels(x * sub_size.x, 0, sub_size.x, sub_size.y));
                sub.Apply();
                sub_blocks_water.Add(sub); //all the animation tile for water
            }
        }
        
        mini_tile_w = sub_blocks_water[0].width / 6;
        wBlock = mini_tile_w * 2;
        mini_tile_h = sub_blocks_water[0].height / 8;
        hBlock = mini_tile_h * 2;
    }

    /// <summary>
    /// Genetate the A1_Tile or the water tile from a sprite sheet image that contais all the tile
    /// </summary>
    /// <param name="frame1"></param>
    /// <param name="frame2"></param>
    /// <param name="frame3"></param>
    /// <param name="rule_tiles"></param>
    /// <param name="wBlock"></param>
    public static void Generate_Water_Tile_SS(string source_File_Path, string frame1, string frame2, string frame3, string frame4,
    Dictionary<byte, int> rule_tiles, int wBlock)
    {
        //create the auto tile for the animation water 
        string atile_path = string.Format(@"{0}/_{1}/{2}.asset", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path), Path.GetFileNameWithoutExtension(frame1));
        string dir = string.Format(@"{0}/_{1}", Tiles_Utility.Auto_Tile_Folder_Path, Path.GetFileNameWithoutExtension(source_File_Path));
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        //A1_Water_Tile atile = new A1_Water_Tile();
        RPGM_XP_Water_Tile atile = ScriptableObject.CreateInstance<RPGM_XP_Water_Tile>();

        if (File.Exists(atile_path))
        {
            atile = AssetDatabase.LoadAssetAtPath<RPGM_XP_Water_Tile>(atile_path);
        }

        EditorUtility.SetDirty(atile);

        if (atile != null)
        {
            atile.frame1 = new Sprite[256];
            atile.frame2 = new Sprite[256];
            atile.frame3 = new Sprite[256];
            atile.frame4 = new Sprite[256];
            object[] vars1 = AssetDatabase.LoadAllAssetsAtPath(frame1);
            object[] vars2 = AssetDatabase.LoadAllAssetsAtPath(frame2);
            object[] vars3 = AssetDatabase.LoadAllAssetsAtPath(frame3);
            object[] vars4 = AssetDatabase.LoadAllAssetsAtPath(frame4);

            int cc = 0;
            foreach (var kvp in rule_tiles)
            {
                Sprite tmp1 = vars1[kvp.Value + 1] as Sprite;
                Sprite tmp2 = vars2[kvp.Value + 1] as Sprite;
                Sprite tmp3 = vars3[kvp.Value + 1] as Sprite;
                Sprite tmp4 = vars4[kvp.Value + 1] as Sprite;
                if (cc == 0) //setting the sprite for the tile palette
                {
                    cc++;
                    atile.sprite = tmp1;
                    atile.preview = tmp1;
                }
                atile.frame1[kvp.Key] = tmp1;
                atile.frame2[kvp.Key] = tmp2;
                atile.frame3[kvp.Key] = tmp3;
                atile.frame4[kvp.Key] = tmp4;
            }
        }
        if (File.Exists(atile_path))
            AssetDatabase.SaveAssets();
        else
            AssetDatabase.CreateAsset(atile, atile_path);
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
    public static Dictionary<byte, Texture2D> Generate_Final_Tiles_RPGMXP(int mini_tile_w, int mini_tile_h, Texture2D[] bottom_left_mini_tiles, Texture2D[] bottom_right_mini_tiles,
        Texture2D[] top_left_mini_tiles, Texture2D[] top_right_mini_tiles, Dictionary<byte, int> rule_tiles)
    {
        Dictionary<byte, Texture2D> final_pieces = new Dictionary<byte, Texture2D>(); //pezzi finali da considerare per la creazione dell'immagine

        rule_tiles.Clear(); //lista delle regole di assegnamento

        List<string> used_Combination = new List<string>(); //dictionary of used combinations
        foreach (var comb in Tiles_Utility.ByteCombination())
        {
            Texture2D tmp = new Texture2D(mini_tile_w * 2, mini_tile_h * 2); //to make parametric 
            #region DO NOT TOUCH! This is the magic
            int bl = Select_Mini_Tile(comb, 5, 8, 7, 2, 4, 1);
            int br = Select_Mini_Tile(comb, 7, 6, 4, 5, 2, 3);
            int tr = Select_Mini_Tile(comb, 4, 1, 2, 7, 5, 8);
            int tl = Select_Mini_Tile(comb, 2, 3, 5, 4, 7, 6);

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
    public static int Select_Mini_Tile(byte value, int b1, int b2, int b3, int bx, int by, int bz)
    {
        bool bit1 = Tiles_Utility.GetBit(value, b1);
        bool bit2 = Tiles_Utility.GetBit(value, b2);
        bool bit3 = Tiles_Utility.GetBit(value, b3);
        bool bitX = Tiles_Utility.GetBit(value, bx);
        bool bitY = Tiles_Utility.GetBit(value, by);
        bool bitZ = Tiles_Utility.GetBit(value, bz);

        //corner
        if (!bit1 && !bit3)
            return 0; //mini_tiles[0];//corner

        //center
        if (bit1 && bit2 && bit3 && bitX && bitY && bitZ)
            return 4;
        else if (bit1 && bit2 && bit3 && !bitX && !bitY && !bitZ) //angolo con i vicini a left and bottom uguali, vicini right and up diversi
            return 8;
        else if (bit1 && bit2 && bit3 && !bitX && bitY) //angolo con i vicini a left and bottom uguali, sopra diverso right uguali
            return 7;
        else if (bit1 && bit2 && bit3 && bitX && !bitY) //angolo con i vicini a left and bottom uguali, sopra ugiali right doversi
            return 5;

        if (!bit1 && bit3)
            if (bitX)
                return 3;
            else
                return 6;

        //up side
        if (bit1 && !bit3)
            if (bitY)
                return 1;
            else
                return 2;

        if (bit1 && !bit2 && bit3)
        {
            return 9; //mini_tiles[9];//tip
        }

        return 4; //mini_tiles[4];
    }

    /// <summary>
    /// Split a single sub block into tne mini tile to be reworked
    /// </summary>
    /// <param name="img">sub block to be reworked. It MUST be an RPG Maker XP</param >
    /// <param name="wBlock">width of the mini tile block</param>
    /// <param name="hBlock">height of the mini tile block</param>
    /// <param name="bottom_left_mini_tiles">destination array</param>
    /// <param name="bottom_right_mini_tiles">destination array/param>
    /// <param name="top_left_mini_tiles">destination array</param>
    /// <param name="top_right_mini_tiles">destination array</param>
    public static void Generate_Mini_Tile_RPGMXP(Texture2D img, int wBlock, int hBlock,
        out Texture2D[] bottom_left_mini_tiles, out Texture2D[] bottom_right_mini_tiles,
        out Texture2D[] top_left_mini_tiles, out Texture2D[] top_right_mini_tiles)
    {
        //Create the mini tile array
        int asize = 11;
        bottom_left_mini_tiles = new Texture2D[asize];
        bottom_right_mini_tiles = new Texture2D[asize];
        top_left_mini_tiles = new Texture2D[asize];
        top_right_mini_tiles = new Texture2D[asize];

        Texture2D[,] rawPieces = new Texture2D[8, 6];
        int cc = 0;
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Texture2D tmp = new Texture2D(wBlock, hBlock);

                Color[] pixels = img.GetPixels(x * wBlock, (7 - y) * hBlock, wBlock, hBlock);
                tmp.SetPixels(0, 0, wBlock, hBlock, pixels);
                tmp.Apply();
                rawPieces[y, x] = tmp;
                cc++;
            }
        }

        cc = 0;
        //bottom_left_mini_tiles
        bottom_left_mini_tiles[0] = rawPieces[7, 0];
        bottom_left_mini_tiles[1] = rawPieces[7, 2];
        bottom_left_mini_tiles[2] = rawPieces[7, 4];
        bottom_left_mini_tiles[3] = rawPieces[5, 0];
        bottom_left_mini_tiles[4] = rawPieces[5, 2];
        bottom_left_mini_tiles[5] = rawPieces[5, 4]; 
        bottom_left_mini_tiles[6] = rawPieces[3, 0];
        bottom_left_mini_tiles[7] = rawPieces[3, 2];
        bottom_left_mini_tiles[8] = rawPieces[3, 4]; 
        bottom_left_mini_tiles[9] = rawPieces[1, 4];         

        cc = 0;
        //bottom_right_mini_tiles
        bottom_right_mini_tiles[0] = rawPieces[7, 5];
        bottom_right_mini_tiles[1] = rawPieces[5, 5];
        bottom_right_mini_tiles[2] = rawPieces[3, 5];
        bottom_right_mini_tiles[3] = rawPieces[7, 3];
        bottom_right_mini_tiles[4] = rawPieces[5, 3]; 
        bottom_right_mini_tiles[5] = rawPieces[3, 3];
        bottom_right_mini_tiles[6] = rawPieces[7, 1];
        bottom_right_mini_tiles[7] = rawPieces[5, 1];
        bottom_right_mini_tiles[8] = rawPieces[3, 1];
        bottom_right_mini_tiles[9] = rawPieces[1, 5];

        cc = 0;
        //top_right_mini_tiles
        top_right_mini_tiles[0] = rawPieces[2, 5];
        top_right_mini_tiles[1] = rawPieces[2, 3];
        top_right_mini_tiles[2] = rawPieces[2, 1];
        top_right_mini_tiles[3] = rawPieces[4, 5];
        top_right_mini_tiles[4] = rawPieces[4, 3];
        top_right_mini_tiles[5] = rawPieces[4, 1]; 
        top_right_mini_tiles[6] = rawPieces[6, 5];
        top_right_mini_tiles[7] = rawPieces[6, 3]; //
        top_right_mini_tiles[8] = rawPieces[6, 1]; //
        top_right_mini_tiles[9] = rawPieces[0, 5];

        cc = 0;
        //top_left_mini_tiles
        top_left_mini_tiles[0] = rawPieces[2, 0];
        top_left_mini_tiles[1] = rawPieces[4, 0];
        top_left_mini_tiles[2] = rawPieces[6, 0];
        top_left_mini_tiles[3] = rawPieces[2, 2];
        top_left_mini_tiles[4] = rawPieces[4, 2];
        top_left_mini_tiles[5] = rawPieces[6, 2]; //
        top_left_mini_tiles[6] = rawPieces[2, 4];
        top_left_mini_tiles[7] = rawPieces[4, 4]; //
        top_left_mini_tiles[8] = rawPieces[6, 2]; //
        top_left_mini_tiles[9] = rawPieces[0, 4];
    }

    ////center
    //if (bit1 && bit2 && bit3)
    //{
    //    return 8;
    //}

    //if (!bit1 && bit3)
    //    if (bitX)
    //        return 3;
    //    else
    //        return 6;

    ////up side
    //if (bit1 && !bit3)
    //    if (bitY)
    //        return 1;
    //    else
    //        return 2;

    //if (bit1 && !bit2 && bit3)
    //{
    //    return 9; //mini_tiles[9];//tip
    //}

    //return 4; //mini_tiles[4];
}
