using System.Collections.Generic;
using UnityEngine;

public class TimeTester : MonoBehaviour
{
    [Header("Test Objects")]
    [SerializeField] private List<ObjectTimeManager> objectTimeManagers;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (var item in objectTimeManagers)
            {
                 item.SetTimeScale(0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            foreach (var item in objectTimeManagers)
            {
              item.SetTimeScale(0.5f);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var item in objectTimeManagers)
            {
                item.ResetTimeScale();
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                ObjectTimeManager timeManager = hit.collider.GetComponent<ObjectTimeManager>();
                if (timeManager != null)
                {
                    // Optionally implement Rewind logic if needed
                }
            }
        }
    }
}
