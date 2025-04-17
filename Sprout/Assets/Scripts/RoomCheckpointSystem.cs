using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class RoomCheckpointSystem : MonoBehaviour
{
    public Transform respawnPoint;

    [Header("FMOD Triggered Music (Optional)")]
    [EventRef] public string musicToPlay;

    [Header("FMOD Triggered Ambience (Optional)")]
    [EventRef] public string ambienceToPlay;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;

        CameraController cam = FindObjectOfType<CameraController>();
        if (cam != null)
        {
            cam.SetCheckpoint(respawnPoint);
        }

        AudioManager.Instance?.PlayMusic(musicToPlay);
        AudioManager.Instance?.PlayAmbience(ambienceToPlay);
    }
}