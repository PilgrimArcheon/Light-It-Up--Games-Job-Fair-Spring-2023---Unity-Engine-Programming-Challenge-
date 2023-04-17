using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public CanvasGroup canvasGroup;//Containing all UI GameObjects for Damage Indication on Player
    PlayerStats stats;//Get Player Stats
    float maxHealth;//Player's Health
    float damageValue = 0;
    // Start is called before the first frame update
    void Start()
    {
        stats = FindObjectOfType<PlayerStats>();
        maxHealth = stats.health;//Get Players Health
    }

    // Update is called once per frame
    void Update()
    {
        //Indicate Damage receieved using UI Flashes
        float health = stats.health;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, damageValue, 20 * Time.deltaTime);
        if(health <= 50f)
            damageValue = 1f - (health/maxHealth);
        else
            damageValue = 0f;
    }
}
