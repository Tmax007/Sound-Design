using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    [Header("Dissipation settings")]
    public float timeToSubside = 0.5f;
    public Vector3 subsideDirection = Vector3.down;
    public float subsideDistance = 3f;

    [Header("Scene references")]
    public Transform poisonGasses;          // parent containing the VFX
    public BoxCollider poisonArea;          // automatically fetched in Start()

    [Header("Runtime info (read‑only)")]
    public bool currentlyBlown = false;     // set by WindTurbine
    public bool gasIsActive = true;      // true while the cloud hurts the player

    // -----------------------------------------------------------------------
    // ‑‑ private fields
    // -----------------------------------------------------------------------
    float subsideProgress;
    Vector3 startPos;
    Vector3 targetPos;

    EventInstance gasLoopInstance;
    bool hasFiredDisappearSFX = false;      // ensures single one‑shot

    // -----------------------------------------------------------------------
    // unity flow
    // -----------------------------------------------------------------------
    void Start()
    {
        poisonArea = GetComponent<BoxCollider>();
        startPos = poisonGasses.position;
        targetPos = startPos + subsideDirection * subsideDistance;

        // create and start the looping gas bed
        gasLoopInstance = RuntimeManager.CreateInstance("event:/Sound Effects/Poison Gas/Poison_Gas");
        gasLoopInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        gasLoopInstance.start();
    }

    void Update()
    {
        // --------------------------------------------------------------
        // update subside progress (0 = full cloud, 1 = disappeared)
        // --------------------------------------------------------------
        if (currentlyBlown)
            subsideProgress += Time.deltaTime / timeToSubside;
        else
            subsideProgress -= Time.deltaTime / timeToSubside;

        subsideProgress = Mathf.Clamp01(subsideProgress);

        // --------------------------------------------------------------
        // state switches
        // --------------------------------------------------------------
        if (subsideProgress >= 1f)
        {
            poisonGasses.gameObject.SetActive(false);
            gasIsActive = false;
        }
        else
        {
            gasIsActive = true;
            if (!poisonGasses.gameObject.activeSelf)
                poisonGasses.gameObject.SetActive(true);
        }

        // --------------------------------------------------------------
        // move the VFX
        // --------------------------------------------------------------
        poisonGasses.position = Vector3.Lerp(startPos, targetPos, subsideProgress);

        // --------------------------------------------------------------
        // audio transitions
        // --------------------------------------------------------------
        if (currentlyBlown && !hasFiredDisappearSFX)
        {
            // first frame of being blown → stop loop & fire one‑shot
            gasLoopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            RuntimeManager.PlayOneShot("event:/Sound Effects/Poison Gas/Poison_Gas_Disappearing", transform.position);
            hasFiredDisappearSFX = true;
        }
        else if (!currentlyBlown && hasFiredDisappearSFX)
        {
            // wind stopped → restart loop and allow another future one‑shot
            gasLoopInstance.start();
            hasFiredDisappearSFX = false;
        }
    }

    // -----------------------------------------------------------------------
    // called from WindTurbine
    // -----------------------------------------------------------------------
    public void StartBlowing(Vector3 direction)
    {
        currentlyBlown = true;
        subsideDirection = direction.normalized;
        targetPos = startPos + subsideDirection * subsideDistance;
    }

    public void StopBlowing()
    {
        currentlyBlown = false;
    }

    // -----------------------------------------------------------------------
    // clean‑up
    // -----------------------------------------------------------------------
    void OnDestroy()
    {
        if (gasLoopInstance.isValid())
        {
            gasLoopInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            gasLoopInstance.release();
        }
    }
}
