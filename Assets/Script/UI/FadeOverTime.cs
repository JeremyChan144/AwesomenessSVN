using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlphaFade : MonoBehaviour
{
    private Graphic uiElement;
    private SpriteRenderer sprite;

    public float startAlpha = 1f, endAlpha = 0f, delay = 1f, fadeDuration = 1f;

    private void Start()
    {
        uiElement = GetComponent<Graphic>();
        sprite = GetComponent<SpriteRenderer>();

        if (uiElement == null && sprite == null)
        {
            Debug.LogError("No UI Image or SpriteRenderer found on " + gameObject.name);
            return;
        }

        SetAlpha(startAlpha);
        StartCoroutine(FadeAlpha());
    }

    private IEnumerator FadeAlpha()
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f, alpha;
        while (elapsed < fadeDuration)
        {
            alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            SetAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetAlpha(endAlpha);
        if (Mathf.Approximately(endAlpha, 0f)) Destroy(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        if (uiElement) uiElement.color = new Color(uiElement.color.r, uiElement.color.g, uiElement.color.b, alpha);
        if (sprite) sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
    }
}