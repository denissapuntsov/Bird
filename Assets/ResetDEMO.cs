using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetDEMO : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Application.Quit();
    }
}
