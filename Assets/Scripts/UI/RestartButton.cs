using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{

    public void RestartBtnClick()
    {
        FindObjectOfType<DataPersistenceManager>().SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}
