using TMPro;
using UnityEngine;

public class PointsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        PlayerStats.Instance.OnScoreChanged += UpdateScoreUI;
        UpdateScoreUI(PlayerStats.Instance.score); // inicializar texto
    }

    void UpdateScoreUI(int newScore)
    {
        scoreText.text = " " + newScore;
    }
}
