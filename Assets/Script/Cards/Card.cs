using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private int cardID;  // Unique identifier for the card
    [SerializeField] private Image frontImage;
    [SerializeField] private Image backImage;
    [SerializeField] private Button cardBtn;

    private CardData cardData;
    private bool isFlipped = false;
    private bool isMatched = false;

    [SerializeField] private UIAnimator matchedAnim;
    [SerializeField] private ParticleSystem matchedParticle;

    // Initialize the card with card data
    public void Initialize(CardData data)
    {
        cardData = data;
        cardID = cardData.cardID;  // Set the card's unique ID from the data
        frontImage.sprite = cardData.frontSprite;
        backImage.gameObject.SetActive(true);  // Show back by default
        frontImage.gameObject.SetActive(false); // Hide front until flipped
        matchedParticle.gameObject.SetActive(false);
    }

    // Handle card click event
    public void OnCardClicked()
    {
        if (isFlipped || isMatched || GameManager.instance.IsFlippingCards) return; // Ignore if already flipped or matched

        FlipCard();  // Flip the card

        // Notify the GameManager about the card flip
        CardEvent.onCardFlip?.Invoke(this);
        
    }

    // Flip the card with a simple LeanTween X-axis flip
    public void FlipCard()
    {
        
        isFlipped = !isFlipped;

        // Animate the card flip using LeanTween (scale on X-axis)
        LeanTween.scaleX(gameObject, 0, 0.1f).setOnComplete(() =>
        {
            frontImage.gameObject.SetActive(isFlipped);  // Show front image if flipped
            backImage.gameObject.SetActive(!isFlipped); // Show back image if not flipped

            // Animate the card back to original scale (complete the flip)
            LeanTween.scaleX(gameObject, 1, 0.1f);
        });
    }

    public int CardID => cardID;
    // Disable the card (after a match is found)
    public void DisableCard()
    {
        isMatched = true;
        cardBtn.interactable = false; // Disable interaction with this card
    }

    public void PlayMatchedAnimation() { 
        matchedAnim?.PlayAnimations();
        matchedParticle.gameObject.SetActive(true);
    }

    public void ResetCard() {
        isFlipped = false;
        isMatched = false;
        cardBtn.interactable = true; // enable interaction with this card

    }
}
