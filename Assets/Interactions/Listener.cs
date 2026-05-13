using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

public class Listener : MonoBehaviour, ISerializationCallbackReceiver
{
    // a dictionary serialized as List<SoundItem> and a List<UnityEvent>
    private Dictionary<SoundItem, Reaction> _reactions = new Dictionary<SoundItem, Reaction>();
    public List<Reaction> reactionsList;
    
    //public UnityEvent onAcceptKey;
    //public SoundItem key;

    public void TryKey(SoundItem playerKey)
    {
        // if dictionary responds to key, invoke the value
        
        if (!_reactions.ContainsKey(playerKey)) return;
        Debug.Log("Accepting");
        var reaction = _reactions[playerKey];
        if (reaction.isInfinite || (!reaction.isInfinite && !reaction.hasBeenTriggered))
        {
            reaction.onAcceptKey.Invoke();
            reaction.hasBeenTriggered = true;
        }
        
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        _reactions = new Dictionary<SoundItem, Reaction>();
        for (int i = 0; i < reactionsList.Count; i++)
        {
            if (reactionsList[i].key == null)
            {
                continue;
            }

            if (_reactions.ContainsKey(reactionsList[i].key) && reactionsList[i].key != null)
            {
                Debug.Log($"Key already present in Dictionary, Reaction {i} will be ignored");
                continue;
            }
            
            _reactions.Add(reactionsList[i].key, reactionsList[i]);
        }
    }
}

[Serializable]
public class Reaction
{
    public SoundItem key;
    public UnityEvent onAcceptKey;
    [NonSerialized] public bool hasBeenTriggered;
    public bool isInfinite = true;
}
