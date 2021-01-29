using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueManager : MonoBehaviour
{
    public static ClueManager instance;
    private void Awake()
    {
        if (instance) Destroy(instance);
        instance = this;
    }

    public void AddClue(Clue clue)
    {
        throw new System.NotImplementedException();
    }
}
