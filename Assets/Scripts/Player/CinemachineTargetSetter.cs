using Unity.Cinemachine;
using UnityEngine;

public class CinemachineTargetSetter : MonoBehaviour
{
    private void Start()
    {
        CinemachineCamera vcam = GetComponent<CinemachineCamera>();

        if (PlayerStats.Instance != null)
        {
            vcam.Follow = PlayerStats.Instance.transform;
            vcam.LookAt = PlayerStats.Instance.transform;
        }
    }
}
