using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnCollision : MonoBehaviour
{

    public static OnCollision Instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
             Instance = this;
        }else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void sceneToMoveTo()
    {
        SceneManager.LoadScene(2);
    }
}
