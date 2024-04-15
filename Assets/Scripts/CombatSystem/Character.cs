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
    public bool isPerfectBlock;
    public bool isBlocking;
    private GameObject obj;
    public event Action<GameObject> DieEvent;
    public event Action DamageEvent;
    public event Action BlockEvent;
    private int dmgBst;
    [SerializeField]public Attack[] attacks;
    

    [SerializeField] private GaugeBar healthBar;
    [SerializeField] private GaugeBar staminaBar;
    [SerializeField] private GaugeBar expBar;
    [SerializeField] private TextMeshProUGUI healthBarTextMesh;
    [SerializeField] private TextMeshProUGUI levelTextMesh;
    [SerializeField] private CharacterBaseStats baseStats;
    [SerializeField] private CharacterEquipmentData equipmentData;
    [SerializeField] protected GameObject healthUI;
    [SerializeField] protected float healthDisplayDistance;
    [SerializeField] private GameObject levelUpEffectPrefab;

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

            int requiredExp = Mathf.RoundToInt(10 + Mathf.Pow(baseStats.level, 2f));
            expBar.SetBar(requiredExp);
            UpdateExpBar();

            levelTextMesh.text = $"LV {baseStats.level}";
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
        if (isPerfectBlock)
        {
            Debug.Log("Perfect Block!");
            BlockEvent?.Invoke();
            return false;
        }
        else if (isBlocking)
        {
            Debug.Log("Block!");
            BlockEvent?.Invoke();
            currentHp = (int)Mathf.Max(currentHp - damage * 0.1f, 0);
            UpdateHPBar();
            if (currentHp == 0) Die();
            return false;
        }
        else
        {
            DamageEvent?.Invoke();
            currentHp = (int)Mathf.Max(currentHp - damage, 0);
            UpdateHPBar();
            if (currentHp == 0) Die();
            return true;
        }
    }

    public void Die()
    {
        isDead = true;
        DieEvent?.Invoke(this.gameObject);
        StartCoroutine(DestroyCoroutine());
    }

    public bool TryUseStamina(float amount)
    {
        if (isDead) return false;
        if (currentStamina == 0) return false;
        currentStamina = Mathf.Max(currentStamina - amount, 0f);
        UpdateStaminaBar();
        if (currentStamina == 0) RecoverStamina(2.5f);
        else RecoverStamina(1.5f);
        return true;
    }

    public void RecoverHp(int amount)
    {
        if (isDead) return;
        int maxHp = Mathf.RoundToInt(GetMaxHp());
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        UpdateHPBar();
    }

    public void InstantRecoverStamina(int amount)
    {
        if (isDead) return;
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

    public void UpdateExpBar()
    {
        expBar.ChangeBar(Mathf.RoundToInt(baseStats.experience));
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
        return baseStats.attack;// + equipmentData.TotalAttack();
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
        if (isDead) return;
        equipmentData.Equip(itemGuid);
    }

    public void increaseDamage(int amount, float duration = 5)
    {
        if (isDead) return;
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

    public void GainExp(int amount)
    {
        baseStats.experience += amount;
        UpdateExpBar();
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int requiredExp = Mathf.RoundToInt(10 + Mathf.Pow(baseStats.level, 2f));
        if (baseStats.experience >= requiredExp)
        {
            baseStats.experience -= requiredExp;
            UpdateExpBar();
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Instantiate(levelUpEffectPrefab, transform.position, transform.rotation);
        baseStats.level++;
        levelTextMesh.text = $"LV {baseStats.level}";
        baseStats.hp = (int)(baseStats.hp * 1.1f);
        baseStats.stamina = Mathf.Min((int)(baseStats.stamina * 1.1f), 200f);
        baseStats.attack = (int)(baseStats.attack * 1.2f);
        baseStats.defense = (int)(baseStats.defense * 1.2f);

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

            int requiredExp = Mathf.RoundToInt(10 + Mathf.Pow(baseStats.level, 2f));
            expBar.SetBar(requiredExp);
            UpdateExpBar();
        }
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
