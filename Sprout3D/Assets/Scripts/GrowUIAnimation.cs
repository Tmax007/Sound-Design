using System;
using System.Collections;
using Unity.VisualScripting;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GrowUIAnimation : MonoBehaviour
{
    Vector3 startingPosition = Vector3.zero;
    Vector3 endPosition = Vector3.zero;
    public float timeToReachSeed = 1f;
    float progress = 0f;
    public ParticleSystem trail;
    public ParticleSystem endEffect;
    public ParticleSystem startEffect;
    public Transform sprite;
    public bool hasReachedEnd = false;
    public float downwardsOffset = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //$Mana UI Bar Usage
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Using Mana Bar");
        transform.position = startingPosition;
        trail.Play();
        startEffect.Play();
    }

    // Update is called once per frame
    void Update()
    {
        progress += Time.deltaTime / timeToReachSeed;
        progress = Mathf.Clamp01(progress);

        float curvedX = Mathf.Pow(progress, 3);
        float curvedZ = Mathf.Pow(progress, 0.5f);

        float xPos =  Mathf.Lerp(startingPosition.x, endPosition.x, curvedX);
        float yPos = Mathf.Lerp(startingPosition.y, endPosition.y, progress);
        float zPos = Mathf.Lerp(startingPosition.z, endPosition.z, curvedZ);

        transform.position = new Vector3(xPos, yPos, zPos);

        if(progress >= 1f && !hasReachedEnd)
        {
            sprite.gameObject.SetActive(false);
            hasReachedEnd = true;
            endEffect.Play();
            trail.Stop();
            DestroyDelay(2f);
        }
    }

    IEnumerator DestroyDelay(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }

    public void InitializeAnimation(Vector3 startPos, Vector3 endPos)
    {
        startingPosition = startPos;
        endPosition = endPos;
    }
}
