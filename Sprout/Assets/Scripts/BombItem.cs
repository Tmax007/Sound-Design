using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using FMODUnity;
using UnityEngine;
using MoreMountains.Feedbacks;
public class Bomb : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionDelay = 3f;
    private bool isActivated = false;
    private bool isDestroyed = false; // Prevent multiple respawns

    public GameObject explosionEffect;
    public ParticleSystem fuseParticleEffect;
    public ParticleSystem explosionParticleEffect;
    public ParticleSystem debriParticleEffect;

    private ThrowableBaseClass throwable;
    private SphereCollider sphereCollider;
    private BoxCollider boxCollider;

    private BombSpawner spawner;
    public float respawnThresholdY = -10f; // Set threshold for falling bombs

    public MMF_Player BombExplodeFeedback;

    private void Start()
    {
        throwable = GetComponent<ThrowableBaseClass>();
        sphereCollider = GetComponent<SphereCollider>();
        boxCollider = GetComponent<BoxCollider>();

        throwable.onHitAfterDropped += ActivateBombFuse;
        throwable.onHitAfterThrown += ActivateBombFuse;

        explosionEffect.SetActive(false);
    }

    public void SetSpawner(BombSpawner bombSpawner)
    {
        spawner = bombSpawner;
    }

    public void ActivateBombFuse()
    {
        if (!isActivated)
        {
            StartCoroutine(BombFuseDelay(explosionDelay));
            isActivated = true;

            // Play fuse sound with FMOD
            RuntimeManager.PlayOneShot("event:/Sound Effects/Bomb Sounds/Bomb_Fuse", transform.position);
            fuseParticleEffect.Play();
        }
    }

    IEnumerator BombFuseDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        Explode();
    }

    void Explode()
    {
        // Play explosion sound with FMOD
        RuntimeManager.PlayOneShot("event:/Sound Effects/Bomb Sounds/Bomb_Exploding", transform.position);

        fuseParticleEffect.Stop();
        Instantiate(explosionParticleEffect, transform.position, Quaternion.identity);
        Instantiate(debriParticleEffect, transform.position, Quaternion.identity);

        sphereCollider.enabled = false;
        boxCollider.enabled = false;

        explosionEffect.SetActive(true);
        explosionEffect.transform.localScale = Vector3.one * explosionRadius * 2;

        Collider[] objects = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider obj in objects)
        {
            if (obj.GetComponent<BreakableByBomb>() != null)
            {
                obj.GetComponent<BreakableByBomb>().Break();
            }
        }

        Invoke("DestroySelf", 0.5f);
    }

    void DestroySelf()
    {
        if (!isDestroyed) // Prevent double respawning
        {
            isDestroyed = true;
            if (spawner != null)
            {
                spawner.BombDestroyed();
            }
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (transform.position.y < respawnThresholdY && !isDestroyed)
        {
            DestroySelf();
        }
    }
}