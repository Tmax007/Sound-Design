using UnityEngine;
using FMODUnity;
public class SpiritFlowerDoor : MonoBehaviour
{
    public SpiritFlower[] spiritFlowers;

    private int numOfSpiritFlowers = 0;
    private int numOfGrownFlowers = 0;

    private float lerpAlpha = 0f;
    public float timeToOpenDoor = 0.5f;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool hasAllFlowerGrown = false;
    private bool doorSoundPlayed = false;

    void Start()
    {
        numOfSpiritFlowers = spiritFlowers.Length;

        startPosition = transform.position;
        endPosition = transform.position - new Vector3(0, 3f, 0);
    }

    void Update()
    {
        CheckFlowers();
        HandleOpenDoor();
    }

    void CheckFlowers()
    {
        numOfGrownFlowers = 0;
        foreach (var flower in spiritFlowers)
        {
            if (flower.hasGrown)
            {
                numOfGrownFlowers++;
            }
        }

        hasAllFlowerGrown = (numOfGrownFlowers == numOfSpiritFlowers);
    }

    void HandleOpenDoor()
    {
        if (hasAllFlowerGrown)
        {
            if (!doorSoundPlayed)
            {
                // $Flower Door Open
                RuntimeManager.PlayOneShot("event:/Sound Effects/Flower_Door_Open", transform.position);
                doorSoundPlayed = true;
            }

            lerpAlpha += Time.deltaTime / timeToOpenDoor;
        }
        else
        {
            doorSoundPlayed = false; // Reset if not all flowers are grown
            lerpAlpha -= Time.deltaTime / timeToOpenDoor;
        }

        lerpAlpha = Mathf.Clamp01(lerpAlpha);
        transform.position = Vector3.Lerp(startPosition, endPosition, lerpAlpha);
    }
}
