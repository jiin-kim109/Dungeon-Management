using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class A1_WaterFall_Tile : A1_Water_Tile
{
#if UNITY_EDITOR
    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        //return base.GetTileAnimationData(position, tilemap, ref tileAnimationData);

        //need to update the current sprite of this tile based on the near tile
        byte status = Compute_Neighbours(position, tilemap);
        tileAnimationData.animatedSprites = new Sprite[] { frame1[status], frame2[status], frame3[status] };
        tileAnimationData.animationStartTime = this.animation_Start_time;
        tileAnimationData.animationSpeed = this.animation_Speed;

        return true;
    }
#endif

    protected override byte Compute_Neighbours(Vector3Int pos, ITilemap map)
    {
        int res = 0;
        Vector3Int tmp_pos = pos + Vector3Int.right;

        Auto_Tile_Base tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        if (tmp_Tile != null && tmp_Tile.id_tile_type == this.id_tile_type)
            res |= (1 << 3);

        tmp_pos = pos + Vector3Int.left;
        tmp_Tile = map.GetTile(tmp_pos) as Auto_Tile_Base;
        if (tmp_Tile != null && tmp_Tile.id_tile_type == this.id_tile_type)
            res |= (1 << 4);

        return (byte)res;
    }

}


#if UNITY_EDITOR
[CustomEditor(typeof(A1_WaterFall_Tile))]
public class A1_Waterfall_Editor : Editor
{
    private A1_WaterFall_Tile tile { get { return (target as A1_WaterFall_Tile); } }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        float oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 210;

        EditorGUI.BeginChangeCheck();
        tile.preview = (Sprite)EditorGUILayout.ObjectField("Preview", tile.preview, typeof(Sprite), false, null);

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(tile);

        EditorGUIUtility.labelWidth = oldLabelWidth;
    }
}
#endif