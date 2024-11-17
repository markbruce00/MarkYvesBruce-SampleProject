using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "MemoryGame/CardData")]
public class CardData : ScriptableObject
{
    public int cardID; // Unique ID for each card
    public Sprite frontSprite; // Sprite for the front of the card
}
