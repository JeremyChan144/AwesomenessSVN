using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectCleanup : MonoBehaviour
{
    //private
    private SpawnManager spawnManager;
    private void Start()
    {
        spawnManager = GameObject.FindWithTag("MainCamera").GetComponent<SpawnManager>();
    }

    // This function is called when another collider enters the trigger collider attached to this object.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider has the tag "Enemy"
        if (other.CompareTag("Enemy") || other.CompareTag("Lights"))
        {
            Destroy(other.gameObject); // Destroy the enemy object
        }

        if (other.CompareTag("PlatformDetection"))
        {
            spawnManager.DestroyPlatform();
        }

        if (other.CompareTag("Player"))
        {
            //fast respawn
            other.transform.position = new Vector3(-27.0f, 0.48f, 0.482f);

            // Get the current scene
            //Scene currentScene = SceneManager.GetActiveScene();

            // Reload the current scene
            //SceneManager.LoadScene(currentScene.name);

            //refresh list
            //SpawnManager spawnManager = GameObject.FindWithTag("MainCamera").GetComponent<SpawnManager>();
            //spawnManager.platformList.Clear();
            //spawnManager.notesList.Clear();


        }
    }
}