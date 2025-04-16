using UnityEngine;
using MoreMountains.Feedbacks;
public class Respawn : MonoBehaviour
{
    public float fallThreshold = -10f; // If the player falls below this, they respawn
    private CameraController cameraController;
    public MMF_Player DyingFeedback;
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
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sound Effects/Player Sounds/Player Dying");
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