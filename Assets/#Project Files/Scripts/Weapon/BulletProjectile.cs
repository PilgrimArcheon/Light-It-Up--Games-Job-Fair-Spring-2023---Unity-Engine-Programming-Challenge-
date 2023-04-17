using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour 
{
    [SerializeField] private Transform muzzlePrefab;//muzzle particle Prefab
    [SerializeField] private Transform hitEffect;//Hit Particle Prefab
    public AudioSource sfx;//Audio Source
    public AudioClip shootSound, hitSound;//SoundFX
    private Rigidbody bulletRigidbody;

    private void Awake() 
    {
        bulletRigidbody = GetComponent<Rigidbody>();// Get Rigidvody Comp
    }

    private void Start() 
    {
        float speed = 150f;
        bulletRigidbody.velocity = transform.forward * speed;//Set Move Speed

        if (muzzlePrefab != null) 
        {
			var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);//Instantaite muzzle
			muzzleVFX.transform.forward = gameObject.transform.forward;//Set muzzle Transform
		}
    }

    private void OnCollisionEnter(Collision other) //Collide with game Objects
    {
        if(other.gameObject.CompareTag("Enemy"))//Check if is Enemy...
        {
            other.gameObject.GetComponent<EnemyAI>().TakeDamage(10);//Give Damage...
        }
        else if(other.gameObject.CompareTag("Player"))//Check if Player... 
        {
            other.gameObject.GetComponent<PlayerStats>().TakeDamage(10);//Give Player Damage
        }
        Instantiate(hitEffect, transform.position, Quaternion.identity);//Instantaite Hit FX
        sfx.PlayOneShot(hitSound);//Play Hit Sfx
        Destroy(gameObject);//Destroy GO
    }
}