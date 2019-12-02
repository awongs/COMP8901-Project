using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // The character to track for.
    public Character character;

    // The health bar slider.
    public Slider slider;

    // The maximum health value of the character.
    private float m_maxHealth;

    private void Start()
    {
        m_maxHealth = character.health;
    }

    private void Update()
    {
        if (character != null)
        {
            slider.value = character.health / m_maxHealth;
        }
    }
}
