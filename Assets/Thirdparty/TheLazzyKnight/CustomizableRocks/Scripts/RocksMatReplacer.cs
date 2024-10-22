using UnityEngine;


namespace CustomizableRocks
{
    public class RocksMatReplacer : MonoBehaviour
    {

        public Material cliffs_A;
        public Material cliffs_B12;
        public Material cliffs_B34;
        public Material rocks_ABC;


        public void ReplaceMaterials()
        {
            ReplaceMaterial("Rock_", rocks_ABC);
            ReplaceMaterial("Cliff_A", cliffs_A);
            ReplaceMaterial("Cliff_B1", cliffs_B12);
            ReplaceMaterial("Cliff_B2", cliffs_B12);
            ReplaceMaterial("Cliff_B3", cliffs_B34);
            ReplaceMaterial("Cliff_B4", cliffs_B34);
        }

        private void ReplaceMaterial(string name, Material newMaterial)
        {
            if (newMaterial == null) return;


            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in objects)
            {

                if (obj.name.StartsWith(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = newMaterial;
                        Debug.Log($"Material of {obj.name} replaced with {newMaterial.name}");
                    }
                }
            }
        }



    }
}
