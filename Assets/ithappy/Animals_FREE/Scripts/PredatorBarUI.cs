using UnityEngine;
using UnityEngine.UI;

public class PredatorBarUI : MonoBehaviour
{
    public Slider hungerSlider;
    public Slider staminaSlider;

    public Predator animal;

    private void Update()
    {
        if (animal == null) return;

        if (hungerSlider != null)
        {
            hungerSlider.value = animal.hunger / 100f;
        }

        if (staminaSlider != null)
        {
            staminaSlider.value = animal.stamina / 100f;
        }
    }
}

