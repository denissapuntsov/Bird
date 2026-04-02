using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class SignPivotGroup : MonoBehaviour
{
    [SerializeField] Transform flippedTransform, defaultTransform;
    
    private Direction _direction;
    public Direction Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            
            gameObject.SetActive(_direction != Direction.Empty);

            var offset = value switch
            {
                Direction.North => 0f,
                Direction.East => 90f,
                Direction.South => 180f,
                Direction.West => -90f,
                _ => 0
            };
            
            transform.localRotation = Quaternion.Euler(
                transform.localRotation.x, 
                offset, 
                transform.localRotation.z);

            gameObject.name = "Sign " + Direction + (_isFlipped ? " (Flipped)" : " ");
        }
    }
    
    private bool _isFlipped;

    public bool IsFlipped
    {
        get =>  _isFlipped;
        set
        {
            _isFlipped = value;
            flippedTransform.gameObject.SetActive(_isFlipped);
            defaultTransform.gameObject.SetActive(!_isFlipped);
        }
    }
    public Sprite icon;

    public Material Material
    {
        set
        {
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>(true))
            {
                meshRenderer.sharedMaterial = value;
            }
                
        }
    }
}

public enum Direction
{
    North,
    East,
    South,
    West,
    Empty
}