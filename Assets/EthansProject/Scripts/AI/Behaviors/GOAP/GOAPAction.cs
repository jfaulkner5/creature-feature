using System.Collections.Generic;
using UnityEngine;

public abstract class GOAPAction : MonoBehaviour {

    private HashSet<KeyValuePair<string, object>> preconditions;
    private HashSet<KeyValuePair<string, object>> effects;

    private bool inRange = false;

    public float cost = 1;

    public GameObject target;

    public GOAPAction ()
    {
        preconditions = new HashSet<KeyValuePair<string, object>>();
        effects = new HashSet<KeyValuePair<string, object>>();
    }

    public void DoReset()
    {
        inRange = false;
        target = null;
        Reset();       
    }

    //      -------------VVVVV <- why's this blue?!?!  - Found out that Reset is a mono method that is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time... coool, another thing that could have been usefull
    public abstract void Reset();

    public abstract bool IsDone();

    public abstract bool CheckProcPreconditions(GameObject agent);

    public abstract bool Preform(GameObject agent);

    public abstract bool RequiresInRange();

    public bool IsInRange()
    {
        return inRange;
    }

    public void SetInRange(bool _inRange)
    {
        inRange = _inRange;
    }
    
    public void AddPrecondition(string key, object value)
    {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }

    public void AddEffect(string key, object value)
    {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }

    public void RemovePrecondition (string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);


        foreach (KeyValuePair<string, object> pair in effects)
        {
            if (pair.Key.Equals(key))
                remove = pair;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
        {
            effects.Remove(remove);
        }
    }

    public HashSet<KeyValuePair<string, object>> Preconditions
    {
        get { return preconditions; }
    }

    public HashSet<KeyValuePair<string, object>> Effects
    {
        get { return effects; }
    }

}
