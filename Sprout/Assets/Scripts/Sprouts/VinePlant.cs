using System.Collections.Generic;
using NUnit.Framework;
//using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Unity.VisualScripting;
using UnityEngine;

public class VinePlant : MonoBehaviour
{
    //TODO: Refactor by transfering variables and some methods to VineSeed instead to decouple
    //TODO: Functions that is in this script should just be about interaction, changing sizes and states should be handled by the seed script

    private Vector3 startSize;
    private Vector3 startPos;
    private Vector3 growDirection;
    public bool hasBeenPlanted = false;
    private bool hasBeenGrowned = false;
    private bool hasBeenOvercharged = false;

    public float fullLength = 3f;
    public float fullGrowthDuration = 5f;
    public float timeToGrow = 0.5f;

    private float growTimer = 0f;
    private float fullGrowthTimer = 0f;

    //List of all vine interactable objects in the vine's range
    List<IVineInteractable> m_AllVineInteractable =  new List<IVineInteractable>();

    public GrowthTimeBar growthTimeBar;

    [SerializeField] ParticleSystem overchargeParticle;
    public List<Material> vineMaterials = new List<Material>();

    public bool hasUnGrown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hasBeenPlanted = true;

        growthTimeBar.gameObject.SetActive(false);

        Debug.Log("test");

        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<MeshRenderer>() != null)
            {
                vineMaterials.Add(transform.GetChild(i).GetComponent<MeshRenderer>().material);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If the seed has been grown
        if (hasBeenPlanted)
        {
            //grow vine according to the player's facing direciton
            if(growTimer < timeToGrow)
            {
                if(!growthTimeBar.gameObject.activeSelf)
                {
                    growthTimeBar.gameObject.SetActive(true);
                }

                //expand vine colliders
                transform.localScale = startSize + Vector3.right * Mathf.Lerp(0f, fullLength, growTimer/timeToGrow);

                transform.localScale = new Vector3(transform.localScale.x, 1f, 1f);

                //offset vine to make it only grow in one direction
                transform.position = startPos + 0.5f * growDirection * Mathf.Lerp(0f, fullLength, growTimer / timeToGrow);

                growTimer += Time.deltaTime;
            }

            //if vine is at full growth, hold at full growth for a certain amount of duration
            else if (growTimer >= timeToGrow && fullGrowthTimer < fullGrowthDuration)
            {
                fullGrowthTimer += Time.deltaTime;
            }

            //Ungrow vine after full growth timer has reached the threshold
            else if (fullGrowthTimer >= fullGrowthDuration)
            {
                UnGrow();
            }
        }

        growthTimeBar.SetBarProgress(fullGrowthTimer/fullGrowthDuration);

    }

    //Setup variables vine plant variables on enable
    private void OnEnable()
    {
        hasBeenGrowned = true;

        growthTimeBar.gameObject.SetActive(false);

        startSize = new Vector3(1f, 1f, 0f);
        startPos = transform.position;

        growDirection = NewPlayerController.facingDirection;
    }

    private void OnDisable()
    {
        hasBeenGrowned = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //check if other object implements IVineInteractable
        if (other.gameObject.GetComponent<IVineInteractable>() != null)
        {
            //add object into the vine interactables array
            m_AllVineInteractable.Add(other.gameObject.GetComponent<IVineInteractable>());

            //call OnVineGrowth fucntion from IVineInteractable
            other.gameObject.GetComponent<IVineInteractable>().OnVineGrowth();
        }
    }

    public void Overcharge(float duration)
    {
        fullGrowthDuration += duration;
        fullGrowthTimer = 0f;
        growthTimeBar.Overcharge();
        ActivateOverchargeEffect();
    }

    //TODO: turn this to base class function
    public void UnGrow()
    {
        //call OnVineStopGrowth fucntion on all IVineInteractable in the array
        for (int i = 0; i < m_AllVineInteractable.Count; i++)
        {
            m_AllVineInteractable[i].OnVineStopGrowth();
        }

        DeactivateOverchargeEffect();

        hasUnGrown = true;
    }

    public float GetGrowthProgress()
    {
        if(hasBeenPlanted)
        {
            return 1 - (fullGrowthTimer / fullGrowthDuration);
        }
        else
        {
            return 0;
        }
    }

    private void ActivateOverchargeEffect()
    {
        foreach(Material m in vineMaterials)
        {
            m.color = Color.yellow;
        }

        overchargeParticle.Play();
    }

    private void DeactivateOverchargeEffect()
    {
        foreach (Material m in vineMaterials)
        {
            m.color = Color.white;
        }

        overchargeParticle.Stop();
    }

    public void SetGrowthDirection(Vector3 growthDirection)
    {
        if (growDirection == Vector3.forward)
        {
            transform.localRotation = Quaternion.Euler(0, 270f, 0);
        }
        else if (growDirection == Vector3.left)
        {
            transform.localRotation = Quaternion.Euler(0, 180f, 0);
        }
        else if (growDirection == Vector3.back)
        {
            transform.localRotation = Quaternion.Euler(0, 90f, 0);
        }
    }
}
