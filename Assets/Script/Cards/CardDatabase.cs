using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "MemoryGame/CardDatabase")]
public class CardDatabase : ScriptableObject
{
    public List<CardData> allCards;

    // Method to get a random set of cards based on the number of pairs required
    public List<CardData> GetRandomCards(int numberOfPairs)
    {
        // Shuffle the list and take the required number of pairs
        List<CardData> selectedCards = new List<CardData>(allCards.OrderBy(x => Random.value).Take(numberOfPairs));
        return selectedCards;
    }
}
