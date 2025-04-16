using UnityEngine;

public class SeedTriggers : MonoBehaviour
{
    public enum UnlockType { VineSeed, BouncyPadSeed }
    public UnlockType unlockType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            NewPlayerController playerUnlocks = other.GetComponent<NewPlayerController>();

            if (playerUnlocks != null)
            {
                if (unlockType == UnlockType.VineSeed)
                {
                    playerUnlocks.UnlockVineSeeds();
                }
                else if (unlockType == UnlockType.BouncyPadSeed)
                {
                    playerUnlocks.UnlockBouncyPadSeed();
                }

                Destroy(gameObject);
            }
        }
    }
}