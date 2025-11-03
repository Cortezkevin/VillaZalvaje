using UnityEditor;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{

    [SerializeField] Animator transitionAnim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(LoadLevel());
        }
    }

    IEnumerator LoadLevel()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        transitionAnim.SetTrigger("Start");
    }
}
