using UnityEngine;
//using Cinemachine;
public class CameraController : MonoBehaviour
{
    public Transform player;
    public float transitionSpeed = 5f;
    public Transform currentCheckpoint; // The latest checkpoint to respawn at

    private Vector3 targetPosition;
    private float roomWidth = 18f;
    private float roomHeight = 10f;
    private float rotatedCameraOffset;

    void Start()
    {
        AdjustCameraForAspectRatio();
        UpdateCameraPosition();
        transform.position = targetPosition;
        rotatedCameraOffset = transform.position.y / Mathf.Tan(Mathf.Deg2Rad * transform.rotation.eulerAngles.x);

        if (currentCheckpoint == null)
        {
            currentCheckpoint = player; // Fallback to player's start position if no checkpoint
        }
    }

    void LateUpdate()
    {
        AdjustCameraForAspectRatio();
        UpdateCameraPosition();
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * transitionSpeed);

        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnPlayer();
        }
    }

    void UpdateCameraPosition()
    {
        int roomX = Mathf.FloorToInt(player.position.x / roomWidth);
        int roomY = Mathf.FloorToInt(player.position.z / roomHeight);

        Vector3 newTargetPosition = new Vector3(
            roomX * roomWidth + roomWidth / 2f,
            transform.position.y,
            (roomY * roomHeight + roomHeight / 2f) - rotatedCameraOffset
        );

        targetPosition = newTargetPosition;
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Debug.Log("Checkpoint Updated: " + currentCheckpoint.position);
    }

    void RespawnPlayer()
    {
        Debug.Log("Respawning Player at: " + currentCheckpoint.position);
        player.position = currentCheckpoint.position;
    }

    private void AdjustCameraForAspectRatio()
    {
        float targetAspect = 16f / 9f;
        float currentAspect = (float)Screen.width / Screen.height;
        float scaleFactor = targetAspect / currentAspect;

        Camera cam = Camera.main;
        if (currentAspect < targetAspect)
        {
            cam.orthographicSize = 5f * scaleFactor;
        }
    }
}