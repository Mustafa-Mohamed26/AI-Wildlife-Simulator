using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{
    public Slider hungerSlider;
    public Slider thirstSlider;
    public Slider staminaSlider;

    public GoodAnimal animal;

    private void Update()
    {
        if (animal == null) return;

        if (hungerSlider != null)
        {
            hungerSlider.value = animal.hunger / 200f;
        }

        if (thirstSlider != null)
        {
            thirstSlider.value = animal.thirst / 200f;
        }

        if (staminaSlider != null)
        {
            staminaSlider.value = animal.stamina / 100f;
        }
    }
}
