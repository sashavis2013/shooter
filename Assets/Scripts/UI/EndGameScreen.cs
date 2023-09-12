using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameScreen : MonoBehaviour
{
    private void Start() 
    {
        GameEventsManager.Instance.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
