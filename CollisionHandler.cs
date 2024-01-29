using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    // Specify the scene name to transition to
    public string sceneToLoad;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a specific tag or layer
        if (collision.gameObject.tag ==("Player"))
        {
            // Load the specified scene
            SceneManager.LoadScene(2);
        }
    }
}
