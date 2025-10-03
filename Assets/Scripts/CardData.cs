using UnityEngine;

public enum CardType { Armor, Attack }
public enum ArmorSlot { Helmet, Chest, Gauntlets, Legs, ATK_Card }
public enum DamageType { Thermal, Freeze, Electric, Void, Impact, ARMOR_Card }

[CreateAssetMenu(fileName = "Card", menuName = "CardData")]
public class CardData : ScriptableObject
{
    public CardType cardType;
    public ArmorSlot armorSlot;
    public DamageType damageType;

    public string cardName;

    // New sprite references
    public Sprite baseArt;
    public Sprite detailsArt;

    // Armor stats
    public int hp;
    public int thermal, freeze, electric, voidRes, impact;

    // Attack stat
    public int damage;
}
