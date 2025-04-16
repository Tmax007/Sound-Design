using UnityEngine;
using MoreMountains.Feedbacks;
public class NPCFeedback : MonoBehaviour
{
    [Header("Feedbacks")]
    public MMF_Player IdleFeedback;
    public MMF_Player TalkingFeedback;

    private bool isTalking = false;

    void Start()
    {
        PlayIdle(); 
    }

    public void PlayIdle()
    {
        if (IdleFeedback != null && !IdleFeedback.IsPlaying)
        {
            IdleFeedback.PlayFeedbacks();
        }
    }

    public void PlayTalking()
    {
        if (IdleFeedback != null && IdleFeedback.IsPlaying)
        {
            IdleFeedback.StopFeedbacks();
        }

        if (TalkingFeedback != null)
        {
            TalkingFeedback.PlayFeedbacks();
            isTalking = true;
        }
    }

    public void StopTalking()
    {
        if (TalkingFeedback != null && TalkingFeedback.IsPlaying)
        {
            TalkingFeedback.StopFeedbacks();
        }

        isTalking = false;
        PlayIdle(); // Resume idle when done talking
    }

    public bool IsTalking() => isTalking;
}