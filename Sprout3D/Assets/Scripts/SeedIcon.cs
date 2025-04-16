using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SeedIcon : MonoBehaviour
{
    public Sprite seedSprite;
    public Sprite fullGrowthSprite;
    public Sprite halfGrowthSprite;
    public Sprite dyingSprite;

    private GrowableSproutBaseClass plantedSeed;

    private Image seedImage;

    public bool hasBeenPlanted { private set; get; } = false ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        seedImage = GetComponent<Image>();

        seedImage.sprite = seedSprite;

        UnAssignSeedIcon();
    }

    private void FixedUpdate()
    {
        if(hasBeenPlanted && plantedSeed == null)
        {
            UnAssignSeedIcon();
        }

        if(hasBeenPlanted && plantedSeed != null)
        {
            CheckSeedGrowthProgress(plantedSeed.GetGrowthProgress());

            if(plantedSeed.hasBeenOvercharged)
            {
                OverchargeSeedIcon();
            }
            else
            {
                StopOverchargeSeedIcon();
            }
        }
    }

    public void CheckSeedGrowthProgress(float growthProgress)
    {
        if (hasBeenPlanted)
        {
            if (growthProgress > 0.66)
            {
                seedImage.sprite = fullGrowthSprite;
            }
            else if(growthProgress > 0.33 && growthProgress < 0.66)
            {
                seedImage.sprite = halfGrowthSprite;
            }
            else if (growthProgress > 0 && growthProgress < 0.33)
            {
                seedImage.sprite = dyingSprite;
            }
            if(growthProgress <= 0)
            {
                seedImage.sprite = seedSprite;
            }
        }
        else
        {
            Debug.LogWarning("Seed has not been Planted yet, call SetPlantedSeedIcon");
        }
    }

    public void AssignSeedToIcon(GrowableSproutBaseClass seed)
    {
        plantedSeed = seed;

        seedImage.rectTransform.localScale = Vector3.one * 2;

        seedImage.color = new Color(1f, 1f, 1f);

        hasBeenPlanted = true;
    }

    public void UnAssignSeedIcon()
    {
        plantedSeed = null;

        seedImage.rectTransform.localScale = Vector3.one;

        seedImage.color = new Color(0.5f, 0.5f, 0.5f);

        hasBeenPlanted = false;
    }

    public void OverchargeSeedIcon()
    {
        seedImage.color = new Color(1f, 0.78f, 1f);
    }

    public void StopOverchargeSeedIcon()
    {
        seedImage.color = Color.white;
    }
}
