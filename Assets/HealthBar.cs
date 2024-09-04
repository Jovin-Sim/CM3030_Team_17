using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public Text valuetext;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        UpdateValueText(health);

        fill.color = gradient.Evaluate(1f);
    }

    public void SetMaxArmour(float health)
    {
        slider.maxValue = health;
        slider.value = 0;
        UpdateValueText(0);

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        UpdateValueText(health);


        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    void UpdateValueText(float health)
    {
        if (valuetext != null)
        {
            int truncatedHealth = Mathf.FloorToInt(health);
            valuetext.text = truncatedHealth.ToString();
            valuetext.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
