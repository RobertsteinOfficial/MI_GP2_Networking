using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private GameObject healthBarParent;
    [SerializeField] private Image healthBarImage;

    private void OnEnable()
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDisable()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    private void HandleHealthUpdated(int maxHealth, int currentHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
