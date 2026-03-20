using System;
using UnityEngine;

public class ResetDEMO : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerInventory>())
        {
            
        }
    }
}
