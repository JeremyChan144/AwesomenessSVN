using UnityEngine;

public class ScaleUpOrDown : MonoBehaviour
{
    public float startScale = 6f; // Starting scale
    public float endScale = 2.1315f; // Ending scale
    public float duration = 2f; // Duration for scaling

    private bool isScaling = true; // Flag to track if scaling is active
    private float timer = 0f; // Timer to track progress

    private RectTransform rectTransform;
    private Transform objectTransform;
    private bool isUIElement = false;

    void Start()
    {
        // Check if it's a UI element (RectTransform) or a world object (Transform)
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            isUIElement = true;
            rectTransform.localScale = Vector3.one * startScale;
        }
        else
        {
            objectTransform = GetComponent<Transform>();
            objectTransform.localScale = Vector3.one * startScale;
        }
    }

    void Update()
    {
        if (isScaling)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration); // Normalize time (0 to 1)

            // Calculate the new scale
            float currentScale = Mathf.Lerp(startScale, endScale, t);

            // Apply scale to the correct object type
            if (isUIElement)
            {
                rectTransform.localScale = Vector3.one * currentScale;
            }
            else
            {
                objectTransform.localScale = Vector3.one * currentScale;
            }

            // Stop scaling when duration is reached
            if (t >= 1f)
            {
                isScaling = false;
            }
        }
    }
}