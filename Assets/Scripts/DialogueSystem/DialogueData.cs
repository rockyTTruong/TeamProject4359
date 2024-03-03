using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
[CreateAssetMenu(menuName = "Dialogue System/New Dialogue Data")]
public class DialogueData: ScriptableObject
{
    public TextAsset textJson;
    public int[] requiredFlags;
    public int[] raiseFlags;

    public bool MeetRequirement()
    {
        foreach (int flagIndex in raiseFlags)
        {
            if (FlagDatabase.Instance.CheckFlag(flagIndex)) return false;
        }

        foreach (int flagIndex in requiredFlags)
        {
            if (!FlagDatabase.Instance.CheckFlag(flagIndex)) return false;
        }
        return true;
    }

    public void RaiseFlag()
    {
        foreach (int flagIndex in raiseFlags)
        {
            FlagDatabase.Instance.RaiseFlag(flagIndex);
        }
    }
}
