using UnityEngine;

public class UnlockVineSeedTriger : MonoBehaviour
{
    public NewPlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(playerController != null)
        {
            playerController.UnlockVineSeeds();
        }

        else
        {
            Debug.LogError(gameObject.name + " has no player instance set");
        }
    }
}
