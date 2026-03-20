using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetDEMO : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerInventory>())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
