using UnityEngine;
using System.Collections.Generic;

public class PlayerCardManager : MonoBehaviour
{
    public List<Card> collectedCards = new List<Card>();

    public void AddCard(Card card)
    {
        if (!collectedCards.Contains(card))
        {
            collectedCards.Add(card);
            Debug.Log($"New card collected: {card.cardName}");
        }
    }

    public bool UseCard(Card card)
    {
        if (collectedCards.Contains(card))
        {
            collectedCards.Remove(card);
            return true;
        }
        return false;
    }
}