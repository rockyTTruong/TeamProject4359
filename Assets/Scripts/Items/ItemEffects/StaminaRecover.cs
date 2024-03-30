using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effect/Stamina Recover")]
public class StaminaRecover : ItemEffect
{
    [SerializeField] private int amount;
    [SerializeField] private GameObject recoverEffect;

    public override void ResolveEffect(GameObject target)
    {
        if (target.TryGetComponent<Character>(out Character character))
        {
            character.InstantRecoverStamina(amount);
            Instantiate(recoverEffect, target.transform);
        }
    }
}
