using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class CinemachineManager : MonoBehaviour
{
    static List<CinemachineCamera> cameras = new List<CinemachineCamera> ();

    public static CinemachineCamera activeCamera = null;
    
    public static bool IsActiveCamera(CinemachineCamera Camera)
    {
       return Camera == activeCamera;
    }

    public static void SwitchCamera(CinemachineCamera newCamera)
    {
        newCamera.Priority = 10;
        activeCamera = newCamera;
        foreach (CinemachineCamera cam in cameras)
        {
            if (cam != newCamera)
            {
                cam.Priority = 0;
            }
        }
    }
    public static void Register(CinemachineCamera camera)
    {
        cameras.Add(camera);
    }
    public static void UnRegister(CinemachineCamera camera)
    {
        cameras.Remove(camera);
    }
}
