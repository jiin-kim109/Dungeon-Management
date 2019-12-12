using UnityEngine;
using UnityEditor;

namespace UnityEditor
{
    [CustomEditor(typeof(Auto_Tile_Base))]
    public class Auto_Tile_Base_Editor : Editor
    {

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            Auto_Tile_Base myTile = target as Auto_Tile_Base;

            if (myTile == null) return null;

            var preview = AssetPreview.GetAssetPreview(myTile.preview);

            if (preview == null)
                return null;

            Texture2D cache = new Texture2D(width, height);
            EditorUtility.CopySerialized(preview, cache);

            return cache;
        }
    }
}
