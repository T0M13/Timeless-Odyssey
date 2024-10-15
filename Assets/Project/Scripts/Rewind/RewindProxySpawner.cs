using UnityEngine;

public class RewindProxySpawner : MonoBehaviour
{
    [SerializeField] private GameObject rewindProxyPrefab;
    [SerializeField] private RewindProxy proxyInstance;

    // Static parent object to hold all the proxies, shared by all instances
    private static GameObject proxyParent;

    public GameObject RewindProxyPrefab { get => rewindProxyPrefab; set => rewindProxyPrefab = value; }

    private void Start()
    {
        // Check if the proxy parent exists; if not, create it
        if (proxyParent == null)
        {
            proxyParent = new GameObject("GlobalProxyParent");
        }

        SpawnProxy();
    }

    public void SpawnProxy()
    {
        // Instantiate the proxy under the single global proxy parent
        GameObject proxyObj = Instantiate(RewindProxyPrefab, transform.position, Quaternion.identity, proxyParent.transform);
        proxyInstance = proxyObj.GetComponent<RewindProxy>();

        if (gameObject.GetComponent<Collider>())
        {
            Collider originalCollider = gameObject.GetComponent<Collider>();
            AddColliderToProxy(proxyObj, originalCollider);
        }

        proxyInstance.OriginalObject = this.gameObject;
    }

    private void AddColliderToProxy(GameObject proxyObj, Collider originalCollider)
    {
        if (originalCollider is BoxCollider)
        {
            BoxCollider originalBox = originalCollider as BoxCollider;
            BoxCollider proxyBox = proxyObj.AddComponent<BoxCollider>();
            proxyBox.center = originalBox.center;
            proxyBox.size = originalBox.size;
            proxyBox.isTrigger = true;
        }
        else if (originalCollider is SphereCollider)
        {
            SphereCollider originalSphere = originalCollider as SphereCollider;
            SphereCollider proxySphere = proxyObj.AddComponent<SphereCollider>();
            proxySphere.center = originalSphere.center;
            proxySphere.radius = originalSphere.radius;
            proxySphere.isTrigger = true;
        }
        else if (originalCollider is CapsuleCollider)
        {
            CapsuleCollider originalCapsule = originalCollider as CapsuleCollider;
            CapsuleCollider proxyCapsule = proxyObj.AddComponent<CapsuleCollider>();
            proxyCapsule.center = originalCapsule.center;
            proxyCapsule.radius = originalCapsule.radius;
            proxyCapsule.height = originalCapsule.height;
            proxyCapsule.direction = originalCapsule.direction;
            proxyCapsule.isTrigger = true;
        }
        else if (originalCollider is MeshCollider)
        {
            MeshCollider originalMesh = originalCollider as MeshCollider;
            MeshCollider proxyMesh = proxyObj.AddComponent<MeshCollider>();
            proxyMesh.sharedMesh = originalMesh.sharedMesh;
            proxyMesh.convex = originalMesh.convex;
            proxyMesh.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("Unsupported collider type: " + originalCollider.GetType().Name);
        }
    }
}
