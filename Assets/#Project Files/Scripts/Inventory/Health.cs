using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float healthValue = 25f;//HealthValue per Reload

    void OnTriggerEnter(Collider other)//Use Ammo Item (i.e Reload)
    {
        if(!other.CompareTag("Player"))
            return;
        if(FindObjectOfType<PlayerStats>().health < 70.0f)
        {
            FindObjectOfType<PlayerStats>().health += healthValue;;// Health Stats Update
        }
        Destroy(gameObject);
    }
}