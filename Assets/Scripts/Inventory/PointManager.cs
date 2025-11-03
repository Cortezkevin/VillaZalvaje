using System;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance;
    [SerializeField] private int ammountPoints;
    public event Action<int> OnPointsChanged;
    private void Awake()
    {
        if (PointManager.Instance == null)
        {
            PointManager.Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int points)
    {
        ammountPoints += points;
        OnPointsChanged?.Invoke(ammountPoints);
    }

    public int GetPoints()
    {
        return ammountPoints;
    }
}
