using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Auto_Tile_Base : Tile
{
    /// <summary>
    /// ID used for comparison
    /// </summary>
    [SerializeField]
    public long id_tile_type = System.DateTime.Now.Ticks;

    /// <summary>
    /// Preview Field. Still to be used
    /// </summary>
    [SerializeField]
    public Sprite preview;
#if UNITY_EDITOR
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        //called when click che mouse on the tilemap. Check if I need to refresh near tile
        //base.RefreshTile(position, tilemap);
        tilemap.RefreshTile(position);
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0) continue;
                Vector3Int tmp_pos = position + new Vector3Int(x, y, 0);
                Auto_Tile_Base tmp_Tile = tilemap.GetTile(tmp_pos) as Auto_Tile_Base;
                if (tmp_Tile != null //&& tmp_Tile.id_tile_type == this.id_tile_type
                    )
                    tilemap.RefreshTile(tmp_pos);
            }
        }
    }
#endif
    /// <summary>
    /// Compute the near cell status in the form of a byte mask
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    protected virtual byte Compute_Neighbours(Vector3Int pos, ITilemap map)
    {
        int res = 0;
        Vector3Int tmp_pos = pos + Vector3Int.right + Vector3Int.up;
        Auto_Tile_Base tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        if (tmp_Tile != null && (tmp_Tile as A1_WaterFall_Tile != null || tmp_Tile.id_tile_type == this.id_tile_type))
            res |= (1 << 0);

        tmp_pos = pos + Vector3Int.up;
        tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        if (tmp_Tile != null && (tmp_Tile as A1_WaterFall_Tile != null || tmp_Tile.id_tile_type == this.id_tile_type))
            res |= (1 << 1);

        tmp_pos = pos + Vector3Int.up + Vector3Int.left;
        tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        if (tmp_Tile != null && (tmp_Tile as A1_WaterFall_Tile != null || tmp_Tile.id_tile_type == this.id_tile_type))
            res |= (1 << 2);

        tmp_pos = pos + Vector3Int.right;
        tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        if (tmp_Tile != null && (tmp_Tile as A1_WaterFall_Tile != null || tmp_Tile.id_tile_type == this.id_tile_type))
            res |= (1 << 3);

        tmp_pos = pos + Vector3Int.left;
        tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        if (tmp_Tile != null && (tmp_Tile as A1_WaterFall_Tile != null || tmp_Tile.id_tile_type == this.id_tile_type))
            res |= (1 << 4);

        tmp_pos = pos + Vector3Int.down + Vector3Int.right;
        tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        if (tmp_Tile != null && (tmp_Tile as A1_WaterFall_Tile != null || tmp_Tile.id_tile_type == this.id_tile_type))
            res |= (1 << 5);

        tmp_pos = pos + Vector3Int.down;
        tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        if (tmp_Tile != null && (tmp_Tile as A1_WaterFall_Tile != null || tmp_Tile.id_tile_type == this.id_tile_type))
            res |= (1 << 6);

        tmp_pos = pos + Vector3Int.down + Vector3Int.left;
        tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        if (tmp_Tile != null && (tmp_Tile as A1_WaterFall_Tile != null || tmp_Tile.id_tile_type == this.id_tile_type))
            res |= (1 << 7);

        return (byte)res;
    }
}