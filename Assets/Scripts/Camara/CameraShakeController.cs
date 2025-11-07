using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        // Obtener el componente de ruido correctamente
        noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            Debug.LogError("No se encontró CinemachineBasicMultiChannelPerlin en la cámara.");
        }
    }

    public void Shake(float amplitude, float frequency, float duration)
    {
        Debug.Log("Shake ejecutado en: " + Time.time);
        StartCoroutine(DoShake(amplitude, frequency, duration));
    }


    private IEnumerator DoShake(float amplitude, float frequency, float duration)
    {
        if (noise == null)
            yield break;

        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0;
        noise.FrequencyGain = 0;
    }
}
