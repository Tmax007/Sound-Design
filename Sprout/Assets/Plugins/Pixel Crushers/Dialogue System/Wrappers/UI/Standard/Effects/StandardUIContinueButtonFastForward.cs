﻿// Copyright (c) Pixel Crushers. All rights reserved.

using FMODUnity;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.Wrappers
{

    /// <summary>
    /// This wrapper class keeps references intact if you switch between the 
    /// compiled assembly and source code versions of the original class.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/UI/Standard UI/UI Effects/Standard UI Continue Button Fast Forward")]
    public class StandardUIContinueButtonFastForward : PixelCrushers.DialogueSystem.StandardUIContinueButtonFastForward
    {
        [EventRef]
        public string continueClickSFX = "event:/UI/Continue_Button_Click";

        public override void OnFastForward()
        {
            // Play the FMOD sound at this object's position
            if (!string.IsNullOrEmpty(continueClickSFX))
            {
                RuntimeManager.PlayOneShot(continueClickSFX, transform.position);
            }

            // Call the base method to preserve existing behavior
            base.OnFastForward();
        }

    }

}
