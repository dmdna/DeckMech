using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public List<CardData> armorCards;
    public List<CardData> attackCards;

    private Queue<CardData> armorDeck;
    private Queue<CardData> attackDeck;

    private List<CardData> armorDiscard = new List<CardData>();
    private List<CardData> attackDiscard = new List<CardData>();

    void Start()
    {
        ShuffleArmorDeck();
        ShuffleAttackDeck();
    }

    void ShuffleArmorDeck()
    {
        List<CardData> shuffled = new List<CardData>(armorCards);
        for (int i = 0; i < shuffled.Count; i++)
        {
            CardData temp = shuffled[i];
            int rand = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[rand];
            shuffled[rand] = temp;
        }
        armorDeck = new Queue<CardData>(shuffled);
    }

    void ShuffleAttackDeck()
    {
        List<CardData> shuffled = new List<CardData>(attackCards);
        for (int i = 0; i < shuffled.Count; i++)
        {
            CardData temp = shuffled[i];
            int rand = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[rand];
            shuffled[rand] = temp;
        }
        attackDeck = new Queue<CardData>(shuffled);
    }

    public CardData DrawArmorCard()
    {
        if (armorDeck.Count == 0)
        {
            if (armorDiscard.Count == 0) return null;
            armorCards = new List<CardData>(armorDiscard);
            armorDiscard.Clear();
            ShuffleArmorDeck();
        }
        return armorDeck.Dequeue();
    }

    public void DiscardArmor(CardData card)
    {
        armorDiscard.Add(card);
    }

    public CardData DrawAttackCard()
    {
        if (attackDeck.Count == 0)
        {
            if (attackDiscard.Count == 0) return null;
            attackCards = new List<CardData>(attackDiscard);
            attackDiscard.Clear();
            ShuffleAttackDeck();
        }
        return attackDeck.Dequeue();
    }
    public int RemainingArmorCards()
    {
        return armorDeck.Count;
    }

    public void DiscardAttack(CardData card)
    {
        attackDiscard.Add(card);
    }
}
