using FMODUnity;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueLida : MonoBehaviour
{
    [Tooltip("Default SFX for NPCs without specific voice lines.")]
    [EventRef] public string defaultNpcSFX = "event:/Character/NPC_Non_Voice";

    public void OnConversationLine(Subtitle subtitle)
    {
        if (subtitle == null || subtitle.dialogueEntry == null || subtitle.speakerInfo == null)
            return;

        string speakerName = subtitle.speakerInfo.transform?.name;
        int entryID = subtitle.dialogueEntry.id;

        // Try to fetch the conversation title
        string conversationTitle = "";
        if (subtitle.dialogueEntry.conversationID >= 0)
        {
            var conversation = DialogueManager.masterDatabase?.GetConversation(subtitle.dialogueEntry.conversationID);
            if (conversation != null)
            {
                conversationTitle = conversation.Title;
            }
        }

        // Check if the line belongs to Lida's conversation
        if (speakerName == "Lida" && conversationTitle == "Lida")
        {
            string voiceEvent = $"event:/Character/Lida_Dialogue_{entryID}";
            RuntimeManager.PlayOneShot(voiceEvent, subtitle.speakerInfo.transform.position);
        }
        else if (!string.IsNullOrEmpty(defaultNpcSFX))
        {
            RuntimeManager.PlayOneShot(defaultNpcSFX, subtitle.speakerInfo.transform.position);
        }
    }
}