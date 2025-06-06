﻿// Copyright (c) Pixel Crushers. All rights reserved.

using FMODUnity;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.Wrappers
{

    /// <summary>
    /// This wrapper class keeps references intact if you switch between the 
    /// compiled assembly and source code versions of the original class.
    /// </summary>
    [HelpURL("http://www.pixelcrushers.com/dialogue_system/manual2x/html/standard_bark_u_i.html")]
    [AddComponentMenu("Pixel Crushers/Dialogue System/UI/Standard UI/Bark/Standard Bark UI")]
    public class StandardBarkUI : PixelCrushers.DialogueSystem.StandardBarkUI
    {
        [EventRef]
        public string barkAppearSFX = "event:/UI/Tooltip_Hover";

        public void OnBarkStart()
        {
            if (!string.IsNullOrEmpty(barkAppearSFX))
            {
                RuntimeManager.PlayOneShot(barkAppearSFX, transform.position);
            }
        }
    }

}
