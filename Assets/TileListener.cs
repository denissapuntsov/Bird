using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileListener : MonoBehaviour
{
    [SerializeField] private List<TileStateEvent> tileStateEvents;

    private void Awake()
    {
        SubscribeToTileEvents();
    }

    private void SubscribeToTileEvents()
    {
        foreach (var tileStateEvent in tileStateEvents)
        {
            tileStateEvent.tile.onTileFree.AddListener(() => tileStateEvent.reaction.Invoke());
        }
    }
}

[Serializable]
public class TileStateEvent
{
    public TileContainer tile;
    public UnityEvent reaction;
}
