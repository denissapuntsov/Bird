using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileListener : MonoBehaviour
{
    [SerializeField] private List<TileStateEvent> tileStateEvents;
    [SerializeField] private List<CharacterLocationEvent> locationEvents;
    [SerializeField] private List<TileOccupationEvent> tileOccupationEvents;
    
    Dictionary<MovingCharacter, TileContainer> _trackedLastLocations = new Dictionary<MovingCharacter, TileContainer>();
    Dictionary<TileContainer, UnityEvent> _trackedTiles = new Dictionary<TileContainer, UnityEvent>();
    
    private void Awake()
    {
        SubscribeToTileStateEvents();
        SubscribeToLocationEvents();
        SubscribeToTileOccupationEvents();
    }
    private void SubscribeToTileStateEvents()
    {
        foreach (var tileStateEvent in tileStateEvents)
        {
            tileStateEvent.tile.onTileFree.AddListener(() => tileStateEvent.reaction.Invoke());
        }
    }
    private void SubscribeToLocationEvents()
    {
        foreach (var locationEvent in locationEvents)
        {
            _trackedLastLocations.Add(locationEvent.movingCharacter, locationEvent.movingCharacter.OccupiedTile);
            locationEvent.movingCharacter.onLocationChangeStart.AddListener((character, lastLocation) =>
            {
                UpdateTrackedLocations(character, lastLocation);
                locationEvent.reaction.Invoke();
            });
        }
    }

    private void SubscribeToTileOccupationEvents()
    {
        foreach (var tileEvent in tileOccupationEvents)
        {
            _trackedTiles.Add(tileEvent.tile, tileEvent.action);
        }
        GetComponent<MovingCharacter>().onLocationChangeEnd.AddListener(CheckForTrackedTile);
    }

    private void CheckForTrackedTile(TileContainer tile)
    {
        if (!_trackedTiles.ContainsKey(tile)) return;
        _trackedTiles[tile]?.Invoke();
    }

    private void UpdateTrackedLocations(MovingCharacter movingCharacter, TileContainer tileContainer)
    {
        _trackedLastLocations[movingCharacter] = tileContainer;
    }

    public void MoveToLastCharacterLocation(MovingCharacter movingCharacter)
    {
        Debug.Log("moving");
        var tile = _trackedLastLocations[movingCharacter];
        GetComponent<MovingCharacter>().Move(tile);
    }

}

[Serializable]
public class TileStateEvent
{
    public TileContainer tile;
    public UnityEvent reaction;
}

[Serializable]
public class CharacterLocationEvent
{
    public MovingCharacter movingCharacter;
    public UnityEvent reaction;
}

[Serializable]
public class TileOccupationEvent
{
    public TileContainer tile;
    public UnityEvent action;
}
