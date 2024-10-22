using UnityEditor;
using UnityEngine;

namespace CustomizableRocks 
{
    [CustomEditor(typeof(RocksMatReplacer))]
    public class RocksMatReplacerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RocksMatReplacer materialReplacer = (RocksMatReplacer)target;

            if (GUILayout.Button("Replace Materials"))
            {
                materialReplacer.ReplaceMaterials();
            }
        }
    }
}
