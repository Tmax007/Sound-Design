using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SurroundingGrowthEffect : MonoBehaviour
{
    [SerializeField] private float surroundingGrowthRadius = 3f;
    [SerializeField] private float innerCircleProportion = 0.8f;
    [SerializeField] private float timeToGrowCircle = 0.5f;
    [SerializeField] private float fullGrowthTime = 3f;
    [SerializeField] private float timeToUnGrow = 2f;
    [SerializeField] private int numberOfSprouts = 10;

    [SerializeField] private GameObject[] sproutsToGrow;

    public AnimationCurve growthSizeCurve;

    public float circleSizeProgress = 0f;
    public List<Transform> placedSprouts;

    public bool automaticGrowth = false;
    public bool hasReachedFullGrowth = false;
    public bool isGrowing = false;
    public bool isUngrowing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlaceSprouts();

        if(automaticGrowth)
        {
            isGrowing = true;

            Invoke("StartUngrowth", fullGrowthTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isGrowing && circleSizeProgress < 1f && !hasReachedFullGrowth)
        {
            HandleSproutGrowth();
        }
        else
        {
           // hasReachedFullGrowth = true;
        }

        if(isUngrowing)
        {
            HandleSproutUngrowth();
        }
    }

    private void PlaceSprouts()
    {
        for (int i = 0; i < numberOfSprouts; i++)
        {
            Vector2 offset = Random.insideUnitCircle * surroundingGrowthRadius;

            int randomSproutIndex = Random.Range(0, sproutsToGrow.Length);

            RaycastHit hit;

            if(Physics.Raycast(transform.position + new Vector3(offset.x, 0.5f, offset.y), Vector3.down, out hit, 1f))
            {
                if(hit.transform.CompareTag("GrowableGround"))
                {
                    GameObject sprout = Instantiate(sproutsToGrow[randomSproutIndex], transform.position + new Vector3(offset.x, 0, offset.y), Quaternion.identity);

                    sprout.transform.localScale = Vector3.zero;

                    placedSprouts.Add(sprout.transform);
                }
            }
        }
    }

    private void HandleSproutGrowth()
    {
        circleSizeProgress += Time.deltaTime / timeToGrowCircle;
        circleSizeProgress = Mathf.Clamp(circleSizeProgress, 0f, 1f);

        float circleRadius = Mathf.Lerp(0, surroundingGrowthRadius + (surroundingGrowthRadius * (1f - innerCircleProportion)), circleSizeProgress);
        float innerToOuterDistance = surroundingGrowthRadius - surroundingGrowthRadius * innerCircleProportion;

        foreach (Transform t in placedSprouts)
        {
            float distanceToCenter = Vector3.Distance(transform.position, t.position);

            if (distanceToCenter < circleRadius)
            {
                float positionInCircle = circleRadius - distanceToCenter;

                float sizeMask = Mathf.Clamp(positionInCircle / innerToOuterDistance, 0f, 1f);

                float size = growthSizeCurve.Evaluate(sizeMask);

                t.localScale = new Vector3(size, size, size);
            }
        }
    }

    private void HandleSproutUngrowth()
    {
        circleSizeProgress -= Time.deltaTime / timeToUnGrow;
        circleSizeProgress = Mathf.Clamp(circleSizeProgress, 0f, 1f);

        foreach (Transform t in placedSprouts)
        {
            t.localScale = new Vector3(circleSizeProgress, circleSizeProgress, circleSizeProgress);
        }
    }

    public void StartUngrowth()
    {
        isUngrowing = true;
        isGrowing = false;
    }

    public void StartGrowth()
    {
        isGrowing = true; 
        isUngrowing = false;
    }

}
