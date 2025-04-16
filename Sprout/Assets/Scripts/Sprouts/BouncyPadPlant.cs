using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BouncyPadPlant : MonoBehaviour
{
    public Vector2 launchDirection = Vector2.right;
    public float launchHeight = 2f;
    public float launchDistance = 4f;
    public float timeToDestination = 1f;
    public float timeToApex = 0.3f;
    public float timeToLand = 0.4f;

    public float currentGrowthProgress = 0f;

    private Vector3 ungrownScale = Vector3.one * 0.1f;
    private Vector3 finalScale = Vector3.one;

    public enum CurrentState {unGrown, currentlyGrowing, fullGrown}
    public bool isCurrentlyOvercharged = false;
    public bool hasUngrown = false;

    public CurrentState currentState = CurrentState.unGrown;

    public Texture bouncyPadUp;
    public Texture bouncyPadDown;
    public Texture bouncyPadLeft;
    public Texture bouncyPadRight;

    public Transform Sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleGrowthScale();

        if (launchDirection == Vector2.up)
        {
            Sprite.GetComponent<MeshRenderer>().material.mainTexture = bouncyPadUp;
        }
        else if (launchDirection == Vector2.down)
        {
            Sprite.GetComponent<MeshRenderer>().material.mainTexture = bouncyPadDown;
            Sprite.transform.localPosition = new Vector3(0f, 0.165f, 0f);
        }
        else if (launchDirection == Vector2.left)
        {
            Sprite.GetComponent<MeshRenderer>().material.mainTexture = bouncyPadLeft;
        }
        else if (launchDirection == Vector2.right)
        {
            Sprite.GetComponent<MeshRenderer>().material.mainTexture = bouncyPadRight;
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentState == CurrentState.fullGrown)
        {
            if(other.gameObject.GetComponent<ILaunchable>() != null)
            {
                //$Bounce

                other.gameObject.GetComponent<ILaunchable>().Launch(launchDirection, launchHeight, launchDistance, timeToDestination, transform.position);
            }
        }
    }

    private void HandleGrowthScale()
    {
        currentGrowthProgress = Mathf.Clamp01(currentGrowthProgress);

        transform.localScale = Vector3.Lerp(ungrownScale, finalScale, currentGrowthProgress);

        if(currentGrowthProgress >= 0.99f)
        {
            currentState = CurrentState.fullGrown;
        }
    }

    public void SetGrowthProgress(float growthProgress)
    {
        currentGrowthProgress = growthProgress;
    }

    public void SetCurrentState(CurrentState state)
    {
        currentState = state;
    }

    public void OverCharge()
    {
        //TODO: Change color to overcharge tower

        isCurrentlyOvercharged = true;
    }

    public void StopOvercharge()
    {
        //TODO: Change color to overcharge tower

        isCurrentlyOvercharged = false;
    }

    public void ChangeDirection()
    {
        //TODO: Change sprite to the corresponding direciton
    }

    public void Ungrow()
    {
        hasUngrown = true;

        StopOvercharge();
    }
}
