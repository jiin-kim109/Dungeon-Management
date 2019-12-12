using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RPGM_XP_Water_Tile : A1_Water_Tile
{
    [SerializeField]
    public Sprite[] frame4;

#if UNITY_EDITOR


    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        //return base.GetTileAnimationData(position, tilemap, ref tileAnimationData);

        //need to update the current sprite of this tile based on the near tile
        byte status = Compute_Neighbours(position, tilemap);
        tileAnimationData.animatedSprites = new Sprite[] { frame1[status], frame2[status], frame3[status], frame4[status] };
        tileAnimationData.animationStartTime = this.animation_Start_time;
        tileAnimationData.animationSpeed = this.animation_Speed;

        return true;
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(RPGM_XP_Water_Tile))]
public class RPGM_XP_Water_Editor : Editor
{
    private RPGM_XP_Water_Tile tile { get { return (target as RPGM_XP_Water_Tile); } }

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
