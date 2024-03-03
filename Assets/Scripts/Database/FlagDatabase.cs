using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagDatabase: SingletonMonobehaviour<FlagDatabase>
{
    [SerializeField] private bool[] flags = new bool[1000];
    
    public bool CheckFlag(int index)
    {
        return flags[index];
    }

    public void RaiseFlag(int index)
    {
        flags[index] = true;
    }
}
