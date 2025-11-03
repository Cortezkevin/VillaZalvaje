using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnAtPoint : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawn = GameObject.FindWithTag("SpawnPoint");
        if (spawn != null)
            transform.position = spawn.transform.position;
    }
}
