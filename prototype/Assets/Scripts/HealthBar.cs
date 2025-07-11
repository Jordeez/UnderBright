using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider slider;       // drag your Slider here
    [SerializeField] private Image fillImage;    // (optional) coloured fill

    [Header("Animation")]
    [Tooltip("Units per second the bar moves toward the target value")]
    [SerializeField] private float slideSpeed = 200f;

    private float _targetValue;                   // where the bar is heading

    void Awake()
    {
        // Fallback: grab the Slider automatically if left unassigned
        if (slider == null) slider = GetComponent<Slider>();
        if (slider == null)
            Debug.LogError($"No Slider component found on {name}!");
    }

    void Update()
    {
        // Smoothly move the slider toward the target value
        if (slider != null && !Mathf.Approximately(slider.value, _targetValue))
        {
            float step = slideSpeed * Time.deltaTime;
            slider.value = Mathf.MoveTowards(slider.value, _targetValue, step);
        }
    }

    /// <summary>Set the maximum HP – call once on spawn/init.</summary>
    public void SetMaxHealth(int max)
    {
        if (slider == null) return;

        slider.maxValue = max;
        slider.value = max;
        _targetValue = max;   // keep in sync
    }

    /// <summary>Update current HP – call whenever HP changes.</summary>
    public void SetHealth(int current)
    {
        if (slider == null) return;

        _targetValue = Mathf.Clamp(current, 0, slider.maxValue);
        // Optional: change colour based on % HP
        if (fillImage != null)
        {
            float t = _targetValue / slider.maxValue;
            fillImage.color = Color.Lerp(Color.red, Color.green, t);
        }
    }
}
