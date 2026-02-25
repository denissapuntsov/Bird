using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public AK.Wwise.Event currentVocalization;
    
    public static PlayerInventory instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
