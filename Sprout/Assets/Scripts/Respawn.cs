using UnityEngine;
using MoreMountains.Feedbacks;
using FMODUnity;
public class Respawn : MonoBehaviour
{
    public float fallThreshold = -10f; // If the player falls below this, they respawn
    private CameraController cameraController;
    public MMF_Player DyingFeedback;
    [EventRef] public string dyingEvent = "event:/Sound Effects/Player Sounds/Player_Dying";
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
    }

    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        //$Player Dying
        DyingFeedback.PlayFeedbacks();
        FMODUnity.RuntimeManager.PlayOneShot(dyingEvent, transform.position);
        if (cameraController != null && cameraController.currentCheckpoint != null)
        {
            transform.position = cameraController.currentCheckpoint.position;
            Debug.Log("Player respawned at checkpoint: " + cameraController.currentCheckpoint.position);
        }
        else
        {
            Debug.LogWarning("No valid checkpoint found! Respawning at default start position.");
            transform.position = Vector3.zero;
        }
    }
}