using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class RoomCheckpointSystem : MonoBehaviour
{
    [Header("Checkpoint (Optional)")]
    public Transform respawnPoint;

    [Header("FMOD Music State (Label Name)")]
    public string musicStateLabel;

    [Header("FMOD Ambience State (Label Name)")]
    public string ambienceStateLabel;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;

        // Set checkpoint (optional)
        if (respawnPoint != null)
        {
            CameraController cam = FindObjectOfType<CameraController>();
            if (cam != null)
            {
                cam.SetCheckpoint(respawnPoint);
            }
        }

        // Set Music and Ambience states
        if (!string.IsNullOrEmpty(musicStateLabel))
        {
            AudioManager.Instance?.SetMusicState(musicStateLabel);
        }

        if (!string.IsNullOrEmpty(ambienceStateLabel))
        {
            AudioManager.Instance?.SetAmbienceState(ambienceStateLabel);
        }
    }
}