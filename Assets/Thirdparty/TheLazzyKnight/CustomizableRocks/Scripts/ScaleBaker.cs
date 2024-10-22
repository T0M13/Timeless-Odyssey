using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace CustomizableRocks
{
    public class ScaleBaker : MonoBehaviour
    {
        private Material[] materialsArr;
        private MeshRenderer[] renderersArr;
        void Awake()
        {


            GameObject[] rocks = GameObject.FindGameObjectsWithTag("CRocks");

            BakeScale(rocks);
            GetMaterials(rocks);
            ChangeMaterials(1);

        }
        void OnApplicationQuit()
        {
            ChangeMaterials(0);
        }

        void GetMaterials(GameObject[] rocks)
        {
            List<MeshRenderer> renderersList = new List<MeshRenderer>();
            List<Material> materialsList = new List<Material>();

            foreach (GameObject rock in rocks)
            {
                MeshRenderer renderer = rock.GetComponent<MeshRenderer>();
                renderersList.Add(renderer);
            }

            renderersArr = renderersList.ToArray();
            foreach (MeshRenderer renderer in renderersArr)
            {
                Material mat = renderer.sharedMaterial;
                materialsList.Add(mat);
            }

            materialsArr = materialsList.ToArray();

        }

        void BakeScale(GameObject[] rocks)
        {

            foreach (GameObject rock in rocks)
            {
                MeshFilter meshFilter = rock.GetComponent<MeshFilter>();
                Vector3 objScale = rock.transform.lossyScale;
                objScale = new Vector3(Mathf.Abs(objScale.x), Mathf.Abs(objScale.y), Mathf.Abs(objScale.z));
                float objSize = (objScale.x + objScale.y + objScale.z) / 3;
                objSize /= 100;
                Color color = new Color(objSize, 0, 0, 0);
                Mesh mesh = meshFilter.mesh;

                Color[] vcolors = new Color[mesh.vertexCount];

                for (int i = 0; i < vcolors.Length; i++)
                {
                    vcolors[i] = color;
                }


                mesh.colors = vcolors;
            }
        }

        void ChangeMaterials(int toggle)
        {
            foreach (Material mat in materialsArr)
            {
                mat.SetInt("_Use_Vert_Col_As_Scale", toggle);
            }
        }
    }

}
