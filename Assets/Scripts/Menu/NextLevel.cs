using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private float transitionDelay = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Jugador alcanzó el EndPoint, iniciando transición al siguiente nivel...");
            StartCoroutine(LoadNextLevelCoroutine());
        }
    }

    private IEnumerator LoadNextLevelCoroutine()
    {
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger("End");
            yield return new WaitForSeconds(transitionDelay);
        }

        // Usa el sistema central para mantener consistencia
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextLevel();
        }
        else
        {
            // Fallback: si no hay GameManager, carga la siguiente escena directamente
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (transitionAnim != null)
        {
            yield return new WaitForSeconds(0.1f);
            transitionAnim.SetTrigger("Start");
        }
    }
}
