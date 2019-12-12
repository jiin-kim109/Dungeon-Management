using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;


public class Tiles_Utility {

    /// <summary>
    /// Path where the script will save the final image of the tileset
    /// </summary>
    public static string final_image_folder_path = @"Assets/RPG_Maker_Tiles_Importer/_rtp_import";

    /// <summary>
    /// Path where the script will save the generated Auto Tile
    /// </summary>
    public static string Auto_Tile_Folder_Path = @"Assets/RPG_Maker_Tiles_Importer/_rtp_auto_tiles";

    /// <summary>
    /// Generate all the byte from 0 to 255
    /// </summary>
    /// <returns></returns>
    public static List<byte> ByteCombination()
    {
        List<byte> list = new List<byte>();
        for (int i = 0; i < 256; i++)
            list.Add((byte)i);
        return list;
    }

    /// <summary>
    /// Generate the 4 combination for the left and right cell only
    /// </summary>
    /// <returns></returns>
    public static List<byte> Left_Right_Combination()
    {
        List<byte> list = new List<byte>();
        byte b1 = 0;

        byte b2 = 0;
        b2 |= (1 << 3);

        byte b3 = 0;
        b3 |= (1 << 4);

        byte b4 = 0;
        b4 |= (1 << 3);
        b4 |= (1 << 4);

        list.Add(b1);
        list.Add(b2);
        list.Add(b3);
        list.Add(b4);
        return list;
    }

    /// <summary>
    /// Generate the combination for the wall tile
    /// </summary>
    /// <returns></returns>
    public static List<byte> Wall_Combination()
    {
        List<byte> list = new List<byte>();
        for (int i = 0; i < 16; i++)
            list.Add(0);
        list[1] |= (0 << 6) | (0 << 4) | (0 << 3) | (1 << 1); //0001
        list[2] |= (0 << 6) | (0 << 4) | (1 << 3) | (0 << 1); //0010
        list[3] |= (0 << 6) | (0 << 4) | (1 << 3) | (1 << 1); //0011
        list[4] |= (0 << 6) | (1 << 4) | (0 << 3) | (0 << 1); //0100
        list[5] |= (0 << 6) | (1 << 4) | (0 << 3) | (1 << 1); //0101
        list[6] |= (0 << 6) | (1 << 4) | (1 << 3) | (0 << 1); //0110
        list[7] |= (0 << 6) | (1 << 4) | (1 << 3) | (1 << 1); //0111
        list[8] |= (1 << 6) | (0 << 4) | (0 << 3) | (0 << 1); //1000
        list[9] |= (1 << 6) | (0 << 4) | (0 << 3) | (1 << 1); //1001
        list[10] |= (1 << 6) | (0 << 4) | (1 << 3) | (0 << 1); //1010
        list[11] |= (1 << 6) | (0 << 4) | (1 << 3) | (1 << 1); //1011
        list[12] |= (1 << 6) | (1 << 4) | (0 << 3) | (0 << 1); //1100
        list[13] |= (1 << 6) | (1 << 4) | (0 << 3) | (1 << 1); //1101
        list[14] |= (1 << 6) | (1 << 4) | (1 << 3) | (0 << 1); //1110
        list[15] |= (1 << 6) | (1 << 4) | (1 << 3) | (1 << 1); //1111

        return list;
    }

    /// <summary>
    /// Set the import setting for the auto contains image tile
    /// </summary>
    /// <param name="fTile"></param>
    /// <param name="wBlock"></param>
    public static void Set_Impoter_Settings(string fTile, int wBlock)
    {
        TextureImporter importer = AssetImporter.GetAtPath(fTile) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.spritePixelsPerUnit = wBlock;
            importer.compressionQuality = 0;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = wBlock;
            importer.SaveAndReimport();
        }
    }



    /// <summary>
    /// Select a single bit from the byte
    /// </summary>
    /// <param name="b">the byte</param>
    /// <param name="bitNumber">bit numeber, form 1 to 8</param>
    /// <returns></returns>
    public static bool GetBit(byte b, int bitNumber)
    {
        return (b & (1 << bitNumber - 1)) != 0;
    }


}
