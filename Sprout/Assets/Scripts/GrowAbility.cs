//using UnityEditor.PackageManager;
using UnityEngine;
using System;
using MoreMountains.Feedbacks;
using FMODUnity;

public class GrowAbility : MonoBehaviour
{
    private float growCooldownTimer = 0f;
    private float growCooldownTimeAmount = 0.3f;
    private Vector3 currentFacingDirection;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SurroundingGrowthEffect growEffect;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem growParticles;
    [SerializeField] private float particleMovementSpeed = 13.33f;

    [EventRef] public string growingEvent = "event:/Sound Effects/Player Sounds/Player_Growing";
    [EventRef] public string overgrowingEvent = "event:/Sound Effects/Player Sounds/Player_Overgrowing";
    public GameObject UITest;

    /// a MMF_Player to play when player
    public MMF_Player GrowingFeedback;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Grow ability cooldown 
        if (growCooldownTimer <= 0f)
        { 
            if (Input.GetKeyDown(KeyCode.F))
            {
                currentFacingDirection = NewPlayerController.facingDirection;

                GrowSprout();
                SpawnGrowParticle();

                growCooldownTimer = growCooldownTimeAmount;
            }
        }
        else
        {
            growCooldownTimer -= Time.deltaTime;
        }
    }

    void GrowSprout()
    {
        if(uiManager.currentMana > 0)
        {
            //$Player Growing
            FMODUnity.RuntimeManager.PlayOneShot(growingEvent, transform.position);
            RaycastHit hit;

            GameObject hitObject = null;

            //shoot raycast towards the facing direction
            if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), currentFacingDirection, out hit, 2f) && hit.transform.gameObject.GetComponent<GrowableSproutBaseClass>() != null)
            {
                hitObject = hit.transform.gameObject;
            }
            else
            {
                IsSproutInOverlap(out hitObject);
            }

            if(hitObject != null && hitObject.GetComponent<GrowableSproutBaseClass>() != null)
            {
                //$Player Grow Hit
                if(hitObject.GetComponent<GrowableSproutBaseClass>().hasBeenOvercharged == false)
                {
                    //call grow sprout with a duration parameter
                    hitObject.GetComponent<GrowableSproutBaseClass>().GrowSprout(7f, new Vector2(currentFacingDirection.x, currentFacingDirection.z));
                    hitObject.GetComponent<GrowableSproutBaseClass>().onSproutUngrowth += uiManager.RegenMana;
                    //hit.transform.gameObject.GetComponent<GrowableSproutBaseClass>().onSproutUngrowth += uiManager.AddSeed;
                    uiManager.UseMana();
                    //mana feedback


                    GrowingFeedback.PlayFeedbacks();

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        GameObject cube = Instantiate(UITest, uiManager.GetUILocation(uiManager.currentMana), Quaternion.identity);
                        cube.GetComponent<GrowUIAnimation>().InitializeAnimation(uiManager.GetUILocation(uiManager.currentMana), hitObject.transform.position);
                    }
                }

                else
                {
                    //$Player Grow Overcharge
                    FMODUnity.RuntimeManager.PlayOneShot(growingEvent, transform.position);
                }
            }
            else
            {
                CreateGrowthEventHanlder(0.5f);
            }

        }

        //Debug line
        Debug.DrawLine(transform.position + new Vector3(0, 0.5f, 0),
        transform.position + new Vector3(0, 0.5f, 0) + currentFacingDirection * 2f,
        Color.red, 0.5f);
    }

    private void CreateGrowthEventHanlder(float growTime)
    {
        //GameObject growHandler = new GameObject();
        //growHandler.AddComponent<EmptySproutGrowth>();
        //growHandler.GetComponent<EmptySproutGrowth>().GrowSprout(growTime);
        //growHandler.GetComponent<EmptySproutGrowth>().onSproutUngrowth += uiManager.RegenMana;

        SurroundingGrowthEffect surroundingGrowth = Instantiate(growEffect, transform.position + currentFacingDirection * 2, Quaternion.identity);
        surroundingGrowth.automaticGrowth = true;
    }

    private void SpawnGrowParticle()
    {
        ParticleSystem growParticleInstance = Instantiate(growParticles, transform.position + currentFacingDirection * 0.4f, Quaternion.Euler(105, 0, 0));
        //growParticleInstance.transform.rotation = Quaternion.Euler(100, 0, 0);
        growParticleInstance.GetComponent<Rigidbody>().linearVelocity = currentFacingDirection * particleMovementSpeed;
    }
    
    private bool IsSproutInOverlap(out GameObject sprout)
    {
        //box overlap everything
        Collider[] hitColliders = Physics.OverlapBox(transform.position + new Vector3(0, 0.5f, 0) + currentFacingDirection * 0.5f, Vector3.one * 0.5f + currentFacingDirection * 0.5f, Quaternion.identity);
        GameObject closestSprout = null;
        //get the closest plant
        if(hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject.GetComponent<GrowableSproutBaseClass>() == null)
                {
                    continue;
                }
                else
                {
                    if(closestSprout == null)
                    {
                        closestSprout = hitColliders[i].gameObject;
                    }
                    else
                    {
                        float playerToCurrentClosest = Vector3.Distance(transform.position, closestSprout.transform.position);
                        float playerToNextSprout = Vector3.Distance(transform.position, hitColliders[i].transform.position);

                        if(playerToNextSprout < playerToCurrentClosest)
                        {
                            closestSprout = hitColliders[i].gameObject;
                        }
                    }
                }
            }

            sprout = closestSprout;
            return true;
        }
        else
        {
            sprout = null;
            return false;
        }
    }
}
