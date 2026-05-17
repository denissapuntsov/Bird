using System;
using System.Collections.Generic;
using UnityEngine;

public class Queue : MonoBehaviour
{
    public CharacterPath path;
    private TileContainer EntryTile => path[^1];
    private TileContainer ExitTile => path[0];
    [SerializeField] private List<MovingCharacter> queue = new List<MovingCharacter>();

    private TileTrigger _entryTrigger;

    [SerializeField] private GameObject tileTrigger;

    private void OnEnable()
    {
        Setup();
        Increment();
    }

    private void Setup()
    {
        _entryTrigger = Instantiate(tileTrigger, EntryTile.transform).GetComponent<TileTrigger>();
        _entryTrigger.onCharacterEnter.AddListener(Add);
        
        foreach (var character in queue)
        {
            Add(character);
            character.onTileReached.AddListener(CheckForTileReached);
        }
        
        TileManager.instance.onTileFree.AddListener(CheckForTileFree);
    }

    private void CheckForTileFree(TileContainer tile)
    {
        if (path.Contains(tile) && path.IndexOf(tile) <= queue.Count)
        {
            if (TileManager.instance.GetTileOwner(path[path.IndexOf(tile) + 1]))
            {
                TileManager.instance.GetTileOwner(path[path.IndexOf(tile) + 1]).Move(tile);
            }
        }

        if (path.IndexOf(tile) - queue.Count == 1)
        {
            Increment();
        }
    }

    public void Add(MovingCharacter character)
    {
        if (!queue.Contains(character))
        {
            queue.Add(character);
            character.onTileReached.AddListener(CheckForTileReached);
        }
        character.Move(path[queue.IndexOf(character) + 1]);
    }
    
    public void CheckForTileReached(TileContainer tile)
    {
        if (tile == ExitTile)
        {
            Remove(TileManager.instance.GetTileOwner(tile));
        }
    }

    public void Remove(MovingCharacter character)
    {
        Debug.Log(character + " reached end of queue. Removing...");
        queue.Remove(character);
        character.onTileReached.RemoveListener(CheckForTileReached);
        TileManager.instance.Remove(character.OccupiedTile);
        Destroy(character.gameObject);
    }

    public void Increment()
    {
        if (queue.Count == 0) return;
        queue[0].Move(path[0]);
    }
}
