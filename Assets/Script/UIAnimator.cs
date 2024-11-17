using UnityEngine;
using UnityEngine.Events;

public class UIAnimator : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject objectToAnimate; // The target object to animate
    public bool playOnEnable = false;
    public float delayTime = 0f;
    public LeanTweenType easeType = LeanTweenType.easeInOutQuad;

    [Header("Position Settings")]
    public bool animatePosition = false;
    public Vector3 targetPosition = Vector3.zero;
    public float positionTime = 1f;

    [Header("Scale Settings")]
    public bool animateScale = false;
    public Vector3 startScale = Vector3.one;
    public Vector3 endScale = Vector3.one * 1.5f;
    public float scaleTime = 1f;

    [Header("Fade Settings")]
    public bool animateFade = false;
    [Range(0f, 1f)] public float startAlpha = 1f;
    [Range(0f, 1f)] public float endAlpha = 0f;
    public float fadeTime = 1f;

    [Header("Events")]
    public UnityEvent onStartEvent;
    public UnityEvent onCompleteEvent;

    private LTDescr positionTween;
    private LTDescr scaleTween;
    private LTDescr fadeTween;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        if (!playOnEnable)
        {
            StartAnimations();
        }
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            StartAnimations();
        }
    }

    private void StartAnimations()
    {
        // If no object is assigned, use the GameObject this script is attached to
        if (objectToAnimate == null)
        {
            objectToAnimate = gameObject;
        }

        // Cancel any existing tweens on the target object
       // LeanTween.cancel(objectToAnimate);

        // Trigger the start event
        onStartEvent?.Invoke();

        // Animate Position
        if (animatePosition)
        {
            positionTween = LeanTween.move(objectToAnimate, targetPosition, positionTime)
                .setEase(easeType)
                .setDelay(delayTime)
                .setOnComplete(() => onCompleteEvent?.Invoke());
        }

        // Animate Scale
        if (animateScale)
        {
            objectToAnimate.transform.localScale = startScale; // Set to start scale
            scaleTween = LeanTween.scale(objectToAnimate, endScale, scaleTime)
                .setEase(easeType)
                .setDelay(delayTime)
                .setOnComplete(() => onCompleteEvent?.Invoke());
        }

        // Animate Fade
        if (animateFade)
        {
            // Try to get or add a CanvasGroup component for fading
            if (canvasGroup == null)
            {
                canvasGroup = objectToAnimate.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = objectToAnimate.AddComponent<CanvasGroup>();
                }
            }

            canvasGroup.alpha = startAlpha; // Set to start alpha
            fadeTween = LeanTween.alphaCanvas(canvasGroup, endAlpha, fadeTime)
                .setEase(easeType)
                .setDelay(delayTime)
                .setOnComplete(() => onCompleteEvent?.Invoke());
        }
    }

    // Optional: Public methods to trigger animations manually
    public void PlayAnimations()
    {
        StartAnimations();
    }
}
