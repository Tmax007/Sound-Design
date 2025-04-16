using UnityEngine;

public class Lever : MonoBehaviour, IInteractable, IVineInteractable
{
    public bool startActivated;

    private float timeSinceActivated;
    private float timeToReturn = 1.5f;
    private float returnTimer = 0f;
    private bool isCurrentlyInteracted = false;
    private bool isInVineHold = false;
    public bool isCurrentlyActive { get; private set; } = false;
    [SerializeField] private Transform leverTransform;
    [SerializeField] private MeshRenderer indicator;
    [SerializeField] private GameObject[] targets;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        returnTimer = timeToReturn;
        DeactivateLever();

        foreach (GameObject target in targets)
        {
            if (target != null)
            {
                if (target.GetComponent<IActivatable>() != null)
                {
                    if(!startActivated)
                    {
                        DeactivateLever();
                        target.GetComponent<IActivatable>().Deactivate();
                    }
                    if(startActivated)
                    {
                        ActivateLever();
                        target.GetComponent<IActivatable>().Activate();
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(isCurrentlyInteracted == false)
        //{
        //    if(isInVineHold == false)
        //    {
        //        returnTimer -=Time.deltaTime;
        //        if(returnTimer < 0 && isCurrentlyActive)
        //        {
        //            isCurrentlyActive = false;
        //            DeactivateLever();
        //            foreach (GameObject target in targets)
        //            {
        //                if (target != null)
        //                {
        //                    if (target.GetComponent<IActivatable>() != null)
        //                    {
        //                        target.GetComponent<IActivatable>().Deactivate();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }

    public void OnStartInteract()
    {
        if (isCurrentlyInteracted == false)
        {
            isCurrentlyInteracted = true;
            if(isCurrentlyActive)
            {
                isCurrentlyActive = true;
                DeactivateLever();
            }
            else if (!isCurrentlyActive)
            {
                isCurrentlyActive = true;
                ActivateLever();
            }
        }

        //if(!isInVineHold)
        //{
        //    if(isCurrentlyInteracted == false)
        //    {
        //        isCurrentlyInteracted = true;
        //        isCurrentlyActive = true;
        //        returnTimer = timeToReturn;
        //        ActivateLever();
        //        foreach(GameObject target in targets)
        //        {
        //            if(target != null)
        //            {
        //                if(target.GetComponent<IActivatable>() != null)
        //                {
        //                    target.GetComponent<IActivatable>().Activate();
        //                }
        //            }
        //        }
        //    }
        //}
    }

    public void OnStopInteract()
    {
        if(!isInVineHold)
        {
            isCurrentlyInteracted = false; 
        }
    }

    private void ActivateLever()
    {
        leverTransform.transform.rotation = Quaternion.Euler(leverTransform.eulerAngles.x, leverTransform.eulerAngles.y, 30f);
        indicator.material.color = Color.green;
        isCurrentlyActive = true;

        foreach (GameObject target in targets)
        {
            if (target != null)
            {
                if (target.GetComponent<IActivatable>() != null)
                {
                    target.GetComponent<IActivatable>().Activate();
                }
            }
        }
    }

    private void DeactivateLever()
    {
        leverTransform.transform.rotation = Quaternion.Euler(leverTransform.eulerAngles.x, leverTransform.eulerAngles.y, -30f);
        indicator.material.color = Color.red;
        isCurrentlyActive = false;

        foreach (GameObject target in targets)
        {
            if (target != null)
            {
                if (target.GetComponent<IActivatable>() != null)
                {
                    target.GetComponent<IActivatable>().Deactivate();
                }
            }
        }
    }

    public void OnVineGrowth()
    {
        isInVineHold = true;
    }

    public void OnVineStopGrowth()
    {
        isInVineHold = false;
        isCurrentlyInteracted = false;
    }
}
