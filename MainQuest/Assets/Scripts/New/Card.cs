using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Game/Card")]
public class Card : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName;
    [TextArea] public string description;
    public Sprite cardSprite; // Use your sprite images here
    
    [Header("Card Type")]
    public CardType cardType;
    
    [Header("Stats")]
    public float cooldown = 5f;
    
    [Header("Summon Settings (For Summon Cards)")]
    public GameObject summonPrefab;
    public int summonHealth = 50;
    public int summonDamage = 10;
    public float summonDuration = 30f;
    
    [Header("Heal Settings (For Heal Cards)")]
    public int healAmount = 30;
    public float healRadius = 5f;
}

public enum CardType
{
    Summon,
    Heal
}