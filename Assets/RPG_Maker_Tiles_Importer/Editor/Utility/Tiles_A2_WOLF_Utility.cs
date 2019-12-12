using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class Tiles_A2_WOLF_Utility {

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
        Tiles_A1_WOLF_Utility.Generate_Mini_Tile_A1Water(img, wBlock, hBlock, out bottom_left_mini_tiles, out bottom_right_mini_tiles, out top_left_mini_tiles, out top_right_mini_tiles);
    }
}
