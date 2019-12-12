using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class A5_Tile : Auto_Tile_Base
{

}

#if UNITY_EDITOR
[CustomEditor(typeof(A5_Tile))]
public class A5_Editor : Editor
{
    private A5_Tile tile { get { return (target as A5_Tile); } }


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