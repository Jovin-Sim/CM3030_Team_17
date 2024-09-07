using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Tooltip("The slider for the health bar")]
    public Slider slider;
    [Tooltip("The color gradient")]
    public Gradient gradient;
    [Tooltip("The health bar")]
    public Image fill;
    [Tooltip("The text displaying the current health")]
    public Text valuetext;

    /// <summary>
    /// Set the max value of the health bar
    /// </summary>
    /// <param name="health">The max health</param>
    public void SetMaxHealth(float health)
    {
        // Change the slider's max value
        slider.maxValue = health;
        // Update the value text
        UpdateValueText(health);

        // Change the fill color of the health bar
        fill.color = gradient.Evaluate(1f);
    }

    /// <summary>
    /// Set the value of the health bar
    /// </summary>
    /// <param name="health">The health</param>
    public void SetHealth(float health)
    {
        // Change the slider's value
        slider.value = health;
        // Update the value text
        UpdateValueText(health);

        // Change the fill color of the health bar
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    /// <summary>
    /// Updates the value in the text
    /// </summary>
    /// <param name="health">The health</param>
    void UpdateValueText(float health)
    {
        // Check if there is a text object to change
        if (valuetext != null)
        {
            // Get the truncated health
            int truncatedHealth = Mathf.CeilToInt(health);

            // Change the text and color
            valuetext.text = truncatedHealth.ToString();
            valuetext.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
