using FMODUnity;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueLida : MonoBehaviour
{
    [Tooltip("Default SFX for NPCs without specific voice lines.")]
    [EventRef] public string defaultNpcSFX = "event:/Character/NPC_Non_Voice";

    private string lastPlayedEvent = "";

    public void OnConversationLine(Subtitle subtitle)
    {
        if (subtitle == null || subtitle.dialogueEntry == null || subtitle.speakerInfo == null)
            return;

        string speakerName = subtitle.speakerInfo.Name?.Trim();
        int entryID = subtitle.dialogueEntry.id;

        // Try to get conversation title
        string conversationTitle = "";
        if (subtitle.dialogueEntry.conversationID >= 0)
        {
            var conversation = DialogueManager.masterDatabase?.GetConversation(subtitle.dialogueEntry.conversationID);
            if (conversation != null)
            {
                conversationTitle = conversation.Title?.Trim();
            }
        }

        // Only Lida's lines get voiced with dedicated audio
        if (string.Equals(speakerName, "Lida", System.StringComparison.OrdinalIgnoreCase) &&
            string.Equals(conversationTitle, "Lida", System.StringComparison.OrdinalIgnoreCase))
        {
            int voiceIndex = GetLidaVoiceIndex(entryID);
            if (voiceIndex > 0)
            {
                string voiceEvent = $"event:/Character/Lida_Dialogue_{voiceIndex}";

                if (lastPlayedEvent != voiceEvent)
                {
                    RuntimeManager.PlayOneShot(voiceEvent, subtitle.speakerInfo.transform.position);
                    lastPlayedEvent = voiceEvent;
                }

                return; // Prevent fallback non-voice SFX from playing
            }
            else
            {
                Debug.LogWarning($"[DialogueLida] No voice mapping found for entry ID {entryID}.");
                return;
            }
        }

        // Fallback SFX for non-Lida characters
        if (!string.IsNullOrEmpty(defaultNpcSFX))
        {
            RuntimeManager.PlayOneShot(defaultNpcSFX, subtitle.speakerInfo.transform.position);
        }
    }

    private int GetLidaVoiceIndex(int entryID)
    {
        return entryID switch
        {
            1 => 1,
            3 => 2,
            4 => 3,
            5 => 4,
            6 => 5,
            7 => 6,
            _ => -1 // Unknown/unmapped
        };
    }
}