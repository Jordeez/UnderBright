using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapToggleUI : MonoBehaviour
{
    [Header("Map UI Settings")]
    public CanvasGroup mapPanel;
    public float fadeDuration = 0.2f;

    private bool isMapVisible = false;
    private Coroutine fadeCoroutine;

    void Start()
    {
        // Ensure map starts hidden
        mapPanel.alpha = 0f;
        mapPanel.interactable = false;
        mapPanel.blocksRaycasts = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMap();
        }
    }

    void ToggleMap()
    {
        isMapVisible = !isMapVisible;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeMap(isMapVisible));
    }

    IEnumerator FadeMap(bool show)
    {
        float startAlpha = mapPanel.alpha;
        float endAlpha = show ? 1f : 0f;

        mapPanel.interactable = show;
        mapPanel.blocksRaycasts = show;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            mapPanel.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        mapPanel.alpha = endAlpha;
    }
}
