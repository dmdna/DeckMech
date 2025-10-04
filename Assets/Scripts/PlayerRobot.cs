using System;
using UnityEngine;

[Serializable]
public class PlayerRobot
{
    // identity
    public int playerId;

    // chosen armor pieces
    public CardData helmetCard;
    public CardData chestCard;
    public CardData gauntletCard; // used for both arms
    public CardData legCard;      // used for both legs

    // calculated stats
    public int hp;
    public int thermal;
    public int freeze;
    public int electric;
    public int voidRes;
    public int impact;

    public PlayerRobot() { ResetStats(); }

    public PlayerRobot(int id)
    {
        playerId = id;
        ResetStats();
    }

    public void ResetStats()
    {
        hp = thermal = freeze = electric = voidRes = impact = 0;
    }

    public void RecalculateStats()
    {
        ResetStats();

        AddStatsFromCard(helmetCard);
        AddStatsFromCard(chestCard);
        AddStatsFromCard(gauntletCard);
        AddStatsFromCard(legCard);
    }

    void AddStatsFromCard(CardData card)
    {
        if (card == null) return;
        hp += card.hp;
        thermal += card.thermal;
        freeze += card.freeze;
        electric += card.electric;
        voidRes += card.voidRes;
        impact += card.impact;
    }

    public int GetResistance(DamageType type)
    {
        switch (type)
        {
            case DamageType.Thermal: return thermal;
            case DamageType.Freeze: return freeze;
            case DamageType.Electric: return electric;
            case DamageType.Void: return voidRes;
            case DamageType.Impact: return impact;
            default: return 0;
        }
    }

    public string ToDebugString()
    {
        return $"Player {playerId} Robot — HP:{hp} | T:{thermal} F:{freeze} E:{electric} V:{voidRes} I:{impact}";
    }
}
