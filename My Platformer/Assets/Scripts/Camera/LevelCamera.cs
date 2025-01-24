using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelCamera : MonoBehaviour
{
    private CinemachineVirtualCamera Cinemachine;

    private void Awake()
    {
        Cinemachine = GetComponentInChildren<CinemachineVirtualCamera>(true);
        EnableCamera(false);
    }

    public void EnableCamera(bool enable)
    {
        Cinemachine.gameObject.SetActive(enable);
    }

    public void SetNewTarger(Transform newTarget)
    {
        Cinemachine.Follow = newTarget;
    }
}
