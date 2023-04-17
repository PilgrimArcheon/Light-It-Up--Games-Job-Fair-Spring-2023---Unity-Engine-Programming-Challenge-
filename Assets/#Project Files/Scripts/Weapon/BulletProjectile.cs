using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour 
{
    [SerializeField] private Transform muzzlePrefab;
    [SerializeField] private Transform hitEffect;
    public float rotateAmount;
    public AudioSource sfx;
    public AudioClip shootSound, hitSound;

    private Rigidbody bulletRigidbody;

    private void Awake() 
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start() 
    {
        float speed = 150f;
        bulletRigidbody.velocity = transform.forward * speed;

        if (muzzlePrefab != null) 
        {
			var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
			muzzleVFX.transform.forward = gameObject.transform.forward;
		}
    }

    private void FixedUpdate() 
    {
        transform.Rotate(0, 0, rotateAmount, Space.Self);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyAI>().TakeDamage(10);
        }
        else if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>().TakeDamage(10);
        }
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        sfx.PlayOneShot(hitSound);
        Destroy(gameObject);
    }
}