/**
 * TacSelfDestruct.cs
 * 
 * Thunder Aerospace Corporation's Self Destruct for the Kerbal Space Program, by Taranis Elsu
 * 
 * (C) Copyright 2013, Taranis Elsu
 * 
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 * 
 * This code is licensed under the Attribution-NonCommercial-ShareAlike 3.0 (CC BY-NC-SA 3.0)
 * creative commons license. See <http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode>
 * for full details.
 * 
 * Attribution — You are free to modify this code, so long as you mention that the resulting
 * work is based upon or adapted from this code.
 * 
 * Non-commercial - You may not use this work for commercial purposes.
 * 
 * Share Alike — If you alter, transform, or build upon this work, you may distribute the
 * resulting work only under the same or similar license to the CC BY-NC-SA 3.0 license.
 * 
 * Note that Thunder Aerospace Corporation is a ficticious entity created for entertainment
 * purposes. It is in no way meant to represent a real entity. Any similarity to a real entity
 * is purely coincidental.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    public class TacSelfDestruct : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Time Delay"),
         UI_FloatRange(minValue = 1.0f, maxValue = 60.0f, stepIncrement = 1.0f, scene = UI_Scene.All)]
        public float timeDelay = 10.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Staging"),
         UI_Toggle(scene = UI_Scene.All)]
        public bool canStage = true;

        [KSPField]
        public string stagingIconName = "FUEL_TANK";

        private float countDownInitiated = 0.0f;
        private bool lastCanStage = true;

        public override void OnAwake()
        {
            this.Log("OnAwake");
            moduleName = "Self Destruct";
        }

        public override void OnStart(PartModule.StartState state)
        {
            this.Log("OnStart");
            part.stagingIcon = stagingIconName;
            part.stackIcon.SetIconColor(XKCDColors.FireEngineRed);
            part.ActivatesEvenIfDisconnected = true;
        }

        public override void OnActive()
        {
            this.Log("Activating!");
            if (canStage)
            {
                ExplodeAllEvent();
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor)
            {
                if (canStage && !lastCanStage)
                {
                    part.inverseStage = Math.Min(Staging.lastStage, part.inverseStage);
                    part.stackIcon.CreateIcon();
                    lastCanStage = canStage;
                    Staging.SortIcons();
                }
                else if (!canStage && lastCanStage)
                {
                    part.stackIcon.RemoveIcon();
                    lastCanStage = canStage;
                    Staging.SortIcons();
                }
            }
        }

        public override string GetInfo()
        {
            return "Default delay = " + timeDelay + " (tweakable)";
        }

        [KSPEvent(guiActive = true, guiName = "Self Destruct!", guiActiveUnfocused = true, unfocusedRange = 8.0f)]
        public void ExplodeAllEvent()
        {
            countDownInitiated = Time.time;
            StartCoroutine(DoSelfDestruct());
        }

        [KSPEvent(guiActive = true, guiName = "Explode!", guiActiveUnfocused = true, unfocusedRange = 8.0f)]
        public void ExplodeEvent()
        {
            part.explode();
        }

        [KSPAction("Self Destruct!")]
        public void ExplodeAllAction(KSPActionParam param)
        {
            countDownInitiated = Time.time;
            StartCoroutine(DoSelfDestruct());
        }

        [KSPAction("Explode!")]
        public void ExplodeAction(KSPActionParam param)
        {
            part.explode();
        }

        private IEnumerator<WaitForSeconds> DoSelfDestruct()
        {
            ScreenMessages.PostScreenMessage("Self destruct sequence initiated.", timeDelay, ScreenMessageStyle.UPPER_CENTER);

            while ((Time.time - countDownInitiated) < timeDelay)
            {
                float remaining = timeDelay - (Time.time - countDownInitiated);
                ScreenMessages.PostScreenMessage(remaining.ToString("#0"), 1.0f, ScreenMessageStyle.UPPER_CENTER);
                yield return new WaitForSeconds(1.0f);
            }

            while (vessel.parts.Count > 0)
            {
                // We do not want to blow up the root part nor the self destruct part until last.
                Part part = vessel.parts.Find(p => p != vessel.rootPart && p != this.part && !p.children.Any());
                if (part != null)
                {
                    part.explode();
                }
                else
                {
                    // Explode the rest of the parts
                    vessel.parts.ForEach(p => p.explode());
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
