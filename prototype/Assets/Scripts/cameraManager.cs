using Cinemachine;
using UnityEngine;

public class CameraShakeCM : MonoBehaviour
{
    public static CameraShakeCM Instance;
    CinemachineImpulseSource source;

    void Awake()
    {
        Instance = this;
        source = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float force = 1f) => source.GenerateImpulse(force);
}
