using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    [SerializeField] Animator transitionAnim;
    public void jugar()
    {
        StartCoroutine(LoadLevel());
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego");
        Application.Quit();
    }
    IEnumerator LoadLevel()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        transitionAnim.SetTrigger("Start");
    }
}
