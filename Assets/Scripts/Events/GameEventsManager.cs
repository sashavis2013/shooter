using System;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        Instance = this;
    }

    public event Action OnPlayerDeath;
    public void PlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }
    
}
