using UnityEngine;
using Unity.Cinemachine;

public class CinemachineRegister:MonoBehaviour
{
    private void OnEnable()
    {
        CinemachineManager.Register(GetComponent<CinemachineCamera>());
    }
    private void OnDisable()
    {
        CinemachineManager.UnRegister(GetComponent<CinemachineCamera>());
    }
}
