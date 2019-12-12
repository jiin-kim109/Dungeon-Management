using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class A2_Tile : Auto_Tile_Base
{
    [SerializeField]
    public Sprite[] tile_Variants;
#if UNITY_EDITOR
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        //need to update the current sprite of this tile based on the near tile
        byte status = Compute_Neighbours(position, tilemap);
        tileData.sprite = tile_Variants[status];
        tileData.colliderType = base.colliderType;
    }
#endif
}


#if UNITY_EDITOR
[CustomEditor(typeof(A2_Tile))]
public class A2_Editor : Editor
{
    private A2_Tile tile { get { return (target as A2_Tile); } }


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
