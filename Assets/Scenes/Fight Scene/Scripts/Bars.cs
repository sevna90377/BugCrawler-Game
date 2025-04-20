using UnityEngine;
using UnityEngine.UI;

public class Bars : MonoBehaviour
{
    public Slider healthSlider;
    public Slider energySlider;

    public void SetMax(int health, int energy)
    {
        healthSlider.maxValue = health;
        energySlider.maxValue = energy;
    }

    public void setHealth(int health)
    {
        healthSlider.value = health;
    }

    public void setEnergy(int energy)
    {
        energySlider.value = energy;
    }
}
