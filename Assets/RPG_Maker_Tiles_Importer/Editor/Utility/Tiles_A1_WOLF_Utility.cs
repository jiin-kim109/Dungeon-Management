using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class Tiles_A1_WOLF_Utility {
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
        out List<Texture2D> sub_blocks_water)
    {
        sub_blocks_water = new List<Texture2D>();

        Vector2Int sub_size = new Vector2Int(img.width / 3, img.height); //that is a fixed number of blocks
                                                                             //divide in sub blocks
        for (int y = 0; y < 1; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                Texture2D sub = new Texture2D(sub_size.x, sub_size.y);
                sub.SetPixels(img.GetPixels(x * sub_size.x, -y * sub_size.y, sub_size.x, sub_size.y));
                sub.Apply();

                sub_blocks_water.Add(sub); //all the animation tile for water
            }
        }
        mini_tile_w = sub_size.x / 2;
        wBlock = mini_tile_w * 2;
        mini_tile_h = sub_size.y / 10;
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

        Texture2D[,] rawPieces = new Texture2D[10, 2];
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Texture2D tmp = new Texture2D(wBlock, hBlock);

                Color[] pixels = img.GetPixels(x * wBlock, (9 - y) * hBlock, wBlock, hBlock);
                tmp.SetPixels(0, 0, wBlock, hBlock, pixels);
                tmp.Apply();
                rawPieces[y, x] = tmp;
            }
        }

        //bottom_left_mini_tiles
        bottom_left_mini_tiles[0] = rawPieces[1, 0];
        bottom_left_mini_tiles[1] = rawPieces[5, 1];
        bottom_left_mini_tiles[2] = rawPieces[5, 0];
        bottom_left_mini_tiles[3] = rawPieces[2, 0];
        bottom_left_mini_tiles[4] = rawPieces[8, 1];
        bottom_left_mini_tiles[5] = rawPieces[8, 0];
        bottom_left_mini_tiles[6] = rawPieces[3, 0];
        bottom_left_mini_tiles[7] = rawPieces[9, 1];
        bottom_left_mini_tiles[8] = rawPieces[9, 0];
        bottom_left_mini_tiles[9] = rawPieces[7, 0];

        //bottom_right_mini_tiles
        bottom_right_mini_tiles[0] = rawPieces[1, 1];
        bottom_right_mini_tiles[1] = rawPieces[2, 1];
        bottom_right_mini_tiles[2] = rawPieces[3, 1];
        bottom_right_mini_tiles[3] = rawPieces[5, 0];
        bottom_right_mini_tiles[4] = rawPieces[8, 0];
        bottom_right_mini_tiles[5] = rawPieces[9, 0];
        bottom_right_mini_tiles[6] = rawPieces[5, 1];
        bottom_right_mini_tiles[7] = rawPieces[8, 1];
        bottom_right_mini_tiles[8] = rawPieces[9, 1];
        bottom_right_mini_tiles[9] = rawPieces[7, 1];

        //top_right_mini_tiles
        top_right_mini_tiles[0] = rawPieces[0, 1];
        top_right_mini_tiles[1] = rawPieces[4, 0];
        top_right_mini_tiles[2] = rawPieces[4, 1];
        top_right_mini_tiles[3] = rawPieces[3, 1];
        top_right_mini_tiles[4] = rawPieces[9, 0];
        top_right_mini_tiles[5] = rawPieces[9, 1];
        top_right_mini_tiles[6] = rawPieces[2, 1];
        top_right_mini_tiles[7] = rawPieces[8, 0];
        top_right_mini_tiles[8] = rawPieces[8, 1];
        top_right_mini_tiles[9] = rawPieces[6, 1];

        //top_left_mini_tiles
        top_left_mini_tiles[0] = rawPieces[0, 0];
        top_left_mini_tiles[1] = rawPieces[3, 0];
        top_left_mini_tiles[2] = rawPieces[2, 0];
        top_left_mini_tiles[3] = rawPieces[4, 1];
        top_left_mini_tiles[4] = rawPieces[9, 1];
        top_left_mini_tiles[5] = rawPieces[8, 1];
        top_left_mini_tiles[6] = rawPieces[4, 0];
        top_left_mini_tiles[7] = rawPieces[9, 0];
        top_left_mini_tiles[8] = rawPieces[8, 0];
        top_left_mini_tiles[9] = rawPieces[6, 0];
    }

    public static void Generate_Mini_Tile_A1Water_bis(Texture2D img, int wBlock, int hBlock,
    out Texture2D[] bottom_left_mini_tiles, out Texture2D[] bottom_right_mini_tiles,
    out Texture2D[] top_left_mini_tiles, out Texture2D[] top_right_mini_tiles)
    {
        //Create the mini tile array
        bottom_left_mini_tiles = new Texture2D[10];
        bottom_right_mini_tiles = new Texture2D[10];
        top_left_mini_tiles = new Texture2D[10];
        top_right_mini_tiles = new Texture2D[10];

        Texture2D[,] rawPieces = new Texture2D[10, 2];
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Texture2D tmp = new Texture2D(wBlock, hBlock);

                Color[] pixels = img.GetPixels(x * wBlock, (9 - y) * hBlock, wBlock, hBlock);
                tmp.SetPixels(0, 0, wBlock, hBlock, pixels);
                tmp.Apply();
                rawPieces[y, x] = tmp;
            }
        }

        //bottom_left_mini_tiles
        bottom_left_mini_tiles[0] = rawPieces[1, 0];
        bottom_left_mini_tiles[1] = rawPieces[5, 1];
        bottom_left_mini_tiles[2] = rawPieces[5, 0];
        bottom_left_mini_tiles[3] = rawPieces[2, 0];
        bottom_left_mini_tiles[4] = rawPieces[9, 0];
        bottom_left_mini_tiles[5] = rawPieces[9, 1];
        bottom_left_mini_tiles[6] = rawPieces[3, 0];
        bottom_left_mini_tiles[7] = rawPieces[8, 0];
        bottom_left_mini_tiles[8] = rawPieces[8, 1];
        bottom_left_mini_tiles[9] = rawPieces[7, 0];

        //bottom_right_mini_tiles
        bottom_right_mini_tiles[0] = rawPieces[1, 1];
        bottom_right_mini_tiles[1] = rawPieces[2, 1];
        bottom_right_mini_tiles[2] = rawPieces[3, 1];
        bottom_right_mini_tiles[3] = rawPieces[5, 0];
        bottom_right_mini_tiles[4] = rawPieces[9, 1];
        bottom_right_mini_tiles[5] = rawPieces[8, 1];
        bottom_right_mini_tiles[6] = rawPieces[5, 1];
        bottom_right_mini_tiles[7] = rawPieces[9, 0];
        bottom_right_mini_tiles[8] = rawPieces[8, 0];
        bottom_right_mini_tiles[9] = rawPieces[7, 1];

        //top_right_mini_tiles
        top_right_mini_tiles[0] = rawPieces[0, 1];
        top_right_mini_tiles[1] = rawPieces[4, 0];
        top_right_mini_tiles[2] = rawPieces[4, 1];
        top_right_mini_tiles[3] = rawPieces[3, 1];
        top_right_mini_tiles[4] = rawPieces[8, 1];
        top_right_mini_tiles[5] = rawPieces[8, 0];
        top_right_mini_tiles[6] = rawPieces[2, 1];
        top_right_mini_tiles[7] = rawPieces[9, 1];
        top_right_mini_tiles[8] = rawPieces[9, 0];
        top_right_mini_tiles[9] = rawPieces[6, 1];

        //top_left_mini_tiles
        top_left_mini_tiles[0] = rawPieces[0, 0];
        top_left_mini_tiles[1] = rawPieces[3, 0];
        top_left_mini_tiles[2] = rawPieces[2, 0];
        top_left_mini_tiles[3] = rawPieces[4, 1];
        top_left_mini_tiles[4] = rawPieces[8, 0];
        top_left_mini_tiles[5] = rawPieces[9, 0];
        top_left_mini_tiles[6] = rawPieces[4, 0];
        top_left_mini_tiles[7] = rawPieces[8, 1];
        top_left_mini_tiles[8] = rawPieces[9, 1];
        top_left_mini_tiles[9] = rawPieces[6, 0];
    }
}
