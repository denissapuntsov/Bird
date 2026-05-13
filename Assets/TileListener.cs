using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileListener : MonoBehaviour
{
    [SerializeField] private List<TileStateEvent> tileStateEvents;
    [SerializeField] private List<CharacterLocationEvent> locationEvents;
    
    Dictionary<MovingCharacter, TileContainer> _trackedLastLocations = new Dictionary<MovingCharacter, TileContainer>();

    private void Awake()
    {
        SubscribeToTileEvents();
        SubscribeToLocationEvents();
    }
    private void SubscribeToTileEvents()
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
            locationEvent.movingCharacter.onLocationChange.AddListener((character, lastLocation) =>
            {
                UpdateTrackedLocations(character, lastLocation);
                locationEvent.reaction.Invoke();
            });
        }
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
