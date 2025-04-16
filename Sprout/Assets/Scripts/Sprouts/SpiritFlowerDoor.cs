using UnityEngine;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numOfSpiritFlowers = spiritFlowers.Length;

        startPosition = transform.position;

        endPosition = transform.position - new Vector3(0, 3f, 0);
    }

    // Update is called once per frame
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
            if(flower.hasGrown)
            {
                numOfGrownFlowers++;
            }
        }

        if(numOfGrownFlowers ==  numOfSpiritFlowers)
        {
            hasAllFlowerGrown = true;
        }
        else
        {
            hasAllFlowerGrown = false;
        }
    }

    void HandleOpenDoor()
    {
        if (hasAllFlowerGrown)
        {
            //Flower Door Open
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Flower Door Open");
            lerpAlpha += Time.deltaTime / timeToOpenDoor;
        }
        else
        {
            //Flower Door Close
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Flower Door Open");
            lerpAlpha -= Time.deltaTime / timeToOpenDoor;
        }

        lerpAlpha = Mathf.Clamp01(lerpAlpha);

        transform.position = Vector3.Lerp(startPosition, endPosition, lerpAlpha);
    }
}
