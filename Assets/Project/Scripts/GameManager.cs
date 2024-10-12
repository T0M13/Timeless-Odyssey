using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Time Scale")]
    [SerializeField] private float defaultTimeFlow = 1;
    [SerializeField] private float currentTimeFlow;
    public float CurrentTimeFlow { get => currentTimeFlow; set => currentTimeFlow = value; }

    private void OnValidate()
    {
        SetTimeFlow(currentTimeFlow);
    }

    private void SetTimeFlow(float value)
    {
        currentTimeFlow = value;
        Time.timeScale = currentTimeFlow;
    }

}
