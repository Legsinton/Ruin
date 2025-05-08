using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
/*
public class SwitchCamera: MonoBehaviour
{
    public CinemachineCamera Camera1;
    public CinemachineCamera Camera2;
    public CinemachineCamera Camera3;
    public CinemachineCamera Camera4;
    public void SwitchCam1()
    {
        CinemachineManager.SwitchCamera(Camera2);
        
    }

    public void UnSwitch()
    {
        CinemachineManager.SwitchCamera(Camera1);

    }

    public void SwitchCam2()
    {
        CinemachineManager.SwitchCamera(Camera3);
       
    }

    public void SwitchCam3()
    {
        CinemachineManager.SwitchCamera(Camera4);
    }
}*/

public class SwitchCamera : MonoBehaviour
{
    public CinemachineCamera[] cameras;
    private CinemachineCamera activeCamera;

    void Start()
    {
        SetCamera(0); // Default to first camera
    }

    public void SetCamera(int index)
    {
        if (index < 0 || index >= cameras.Length) return;

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == index);
            if (i == index)
                activeCamera = cameras[i];
        }
    }

    public CinemachineCamera GetActiveCamera()
    {
        return activeCamera;
    }
}
