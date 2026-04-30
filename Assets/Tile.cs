using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white);
    }
}
