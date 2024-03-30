using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{
    public bool isDead;
    public bool isUnflinching;
    public bool isInvincible;
    private GameObject obj;
    public event Action<GameObject> DieEvent;
    public event Action DamageEvent;
    private int dmgBst;
    [SerializeField]public Attack[] attacks;
    

    [SerializeField] private GaugeBar healthBar;
    [SerializeField] private GaugeBar staminaBar;
    [SerializeField] private TextMeshProUGUI healthBarTextMesh;
    [SerializeField] private CharacterBaseStats baseStats;
    [SerializeField] private CharacterEquipmentData equipmentData;
    [SerializeField] protected GameObject healthUI;
    [SerializeField] protected float healthDisplayDistance;

    private float currentHp;
    private float currentStamina;
    private Coroutine recoveringStamina;
    public float CurrentHpPercent => currentHp / GetMaxHp();
    public float CurrentStamina => currentStamina;

    private void Start()
    {
        int maxHp = GetMaxHp();
        currentHp = maxHp;
        healthBar.SetBar(maxHp);
        UpdateHPBar();
        if (this.gameObject.CompareTag("Player"))
        {
            int maxStamina = GetMaxStamina();
            currentStamina = maxStamina;
            staminaBar.SetBar(maxStamina);
            UpdateStaminaBar();
        }
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(1f);
        if (this.gameObject.CompareTag("Player"))
        {
            GetComponent<PlayerStateMachine>().retryMenuUI.SetActive(true);
            yield break;
        }
        healthBar.gameObject.SetActive(false);
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }

    public bool TryDealDamage(float damage)
    {
        if (isDead || isInvincible) return false;

        DamageEvent?.Invoke();

        currentHp = Mathf.Max(currentHp - damage, 0);
        UpdateHPBar();
        if (currentHp == 0) Die();
        return true;
    }

    public void Die()
    {
        isDead = true;
        DieEvent?.Invoke(this.gameObject);
        StartCoroutine(DestroyCoroutine());
    }

    public bool TryUseStamina(float amount)
    {
        if (currentStamina == 0) return false;
        currentStamina = Mathf.Max(currentStamina - amount, 0f);
        UpdateStaminaBar();
        if (currentStamina == 0) RecoverStamina(2.5f);
        else RecoverStamina(1.5f);
        return true;
    }

    public void RecoverHp(int amount)
    {
        int maxHp = Mathf.RoundToInt(GetMaxHp());
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        UpdateHPBar();
    }

    public void InstantRecoverStamina(int amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, GetMaxStamina());
        UpdateStaminaBar();
    }

    public void UpdateHPBar()
    {
        int maxHp = Mathf.RoundToInt(GetMaxHp());
        healthBar.ChangeBar(Mathf.RoundToInt(currentHp));
        if (healthBarTextMesh != null) healthBarTextMesh.text = $"HP {currentHp}/{maxHp}";
    }

    public void UpdateStaminaBar()
    {
        int maxStamina = Mathf.RoundToInt(GetMaxStamina());
        staminaBar.ChangeBar(Mathf.RoundToInt(currentStamina));
    }

    public int GetMaxHp()
    {
        return Mathf.RoundToInt(baseStats.hp + equipmentData.TotalHp());
    }

    public int GetMaxStamina()
    {
        return Mathf.RoundToInt(baseStats.stamina + equipmentData.TotalStamina());
    }

    public float GetAttack()
    {
        return baseStats.attack + equipmentData.TotalAttack();
    }

    public float GetDefense()
    {
        return baseStats.defense + equipmentData.TotalDefense();
    }

    public WeaponItemData GetCurrentWeaponData()
    {
        string itemGuid = equipmentData.equippingItemGuids[0];
        if (!string.IsNullOrEmpty(itemGuid))
        {
            WeaponItemData weapon = (WeaponItemData)ItemDatabase.Instance.GetItemData(equipmentData.equippingItemGuids[0]);
            return weapon;
        }
        else return null;
    }

    public void ChangeWeapon(string itemGuid)
    {
        equipmentData.Equip(itemGuid);
    }

    public void increaseDamage(int amount, float duration = 5)
    {
        dmgBst = amount;
        foreach (Attack attack in attacks)
        {
            attack.damage += amount;
        }
        obj = FindObjectOfType<CoinManager>().gameObject;
        obj.transform.Find("DamageBuff").gameObject.SetActive(true);
        Invoke("turnOff", duration);
        Invoke("endEffectWeapon", duration);
    }
    private void endEffectWeapon()
    {
        foreach (Attack attack in attacks)
        {
            attack.damage -= dmgBst;
        }
    }

    public void Revive()
    {
        isDead = false;
        int maxHp = GetMaxHp();
        currentHp = maxHp;
        UpdateHPBar();
    }
    private void turnOff()
    {
        obj.transform.Find("DamageBuff").gameObject.SetActive(false);
    }

    public void RecoverStamina(float delayTime)
    {
        if (recoveringStamina != null)
        {
            StopCoroutine(recoveringStamina);
            recoveringStamina = null;
        }
        recoveringStamina = StartCoroutine(RecoverStaminaCoroutine(delayTime));
    }

    private IEnumerator RecoverStaminaCoroutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        int maxStamina = GetMaxStamina();
        while (currentStamina != maxStamina)
        {
            currentStamina = Mathf.Min(currentStamina + 1, maxStamina);
            UpdateStaminaBar();
            yield return new WaitForEndOfFrame();
        }
        recoveringStamina = null;
    }
}
