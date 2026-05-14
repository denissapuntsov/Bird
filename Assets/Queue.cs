using System;
using System.Collections.Generic;
using UnityEngine;

public class Queue : MonoBehaviour
{
    [SerializeField] private GameObject tileTrigger;
    public List<MovingCharacter> characters;
    private TileContainer _entryPoint;
    public TileContainer exitPoint;
    public CharacterPath path;

    private void OnEnable()
    {
        SetupQueue();
    }

    private void SetupQueue()
    {
        _entryPoint = path[^1];
        var entryTrigger = Instantiate(tileTrigger, _entryPoint.transform).GetComponent<TileTrigger>();
        entryTrigger.onMovingCharacterEnter.AddListener(EnterQueue);
    }

    public void EnterQueue(MovingCharacter character)
    {
        if (characters.Contains(character)) return;
        characters.Add(character);
        character.Move(path[characters.Count - 1]);
        MovingCharacter lastInQueue;
        if (characters.Count > 0)
        {
            // subscribe to last character's tile getting free (Owner == null) to move onto it
            lastInQueue = characters[characters.Count - 1];
            lastInQueue.OccupiedTile.onTileOwnerChange.AddListener((TileContainer tile, MovingCharacter owner) =>
            {
                if (!owner)
                {
                    character.Move(tile);
                }
            });
        }
    }

    public void ExitQueue(MovingCharacter character)
    {
        characters.Remove(character);
    }

    public void IncrementQueue()
    {
        
    }
}
