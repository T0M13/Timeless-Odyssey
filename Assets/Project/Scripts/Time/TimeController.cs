using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetGlobalTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void SetLocalTimeScale(GameObject obj, float scale)
    {
        ObjectTimeManager timeManager = obj.GetComponent<ObjectTimeManager>();
        if (timeManager != null)
        {
            timeManager.SetTimeScale(scale);
        }
    }

    public void ResetLocalTimeScale(GameObject obj)
    {
        ObjectTimeManager timeManager = obj.GetComponent<ObjectTimeManager>();
        if (timeManager != null)
        {
            timeManager.ResetTimeScale();
        }
    }
}
