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

        [KSPField(isPersistant = true)]
        public bool canStage = true;

        private float countDownInitiated = 0.0f;
        private bool abortCountdown = false;

        public override void OnAwake()
        {
            this.Log("OnAwake");
        }

        public override void OnStart(PartModule.StartState state)
        {
            this.Log("OnStart");
            part.stackIcon.SetIconColor(XKCDColors.FireEngineRed);
            part.ActivatesEvenIfDisconnected = true;

            if (!canStage)
            {
                Invoke("DisableStaging", 0.5f);
            }
            else
            {
                UpdateStagingEvents();
            }

            GameEvents.onVesselChange.Add(OnVesselChange);
        }

        public override void OnInitialize()
        {
            this.Log("OnInitialize");
        }

        public override void OnActive()
        {
            this.Log("Activating!");
            if (canStage && countDownInitiated == 0.0f)
            {
                ExplodeAllEvent();
            }
        }

        public void OnVesselChange(Vessel newVessel)
        {
            if (newVessel == this.vessel && !canStage)
            {
                Invoke("DisableStaging", 0.5f);
            }
        }

        public override string GetInfo()
        {
            return "Default delay = " + timeDelay + " (tweakable)";
        }

        [KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Enable Staging")]
        public void EnableStaging()
        {
            part.deactivate();
            part.inverseStage = Math.Min(Staging.lastStage, part.inverseStage);
            part.stackIcon.CreateIcon();
            Staging.SortIcons();

            canStage = true;
            UpdateStagingEvents();
        }

        [KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Disable Staging")]
        public void DisableStaging()
        {
            part.stackIcon.RemoveIcon();
            Staging.SortIcons();

            canStage = false;
            UpdateStagingEvents();
        }

        private void UpdateStagingEvents()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                if (canStage)
                {
                    Events["EnableStaging"].guiActiveEditor = false;
                    Events["DisableStaging"].guiActiveEditor = true;
                }
                else
                {
                    Events["EnableStaging"].guiActiveEditor = true;
                    Events["DisableStaging"].guiActiveEditor = false;
                }
            }
        }

        [KSPEvent(guiActive = true, guiName = "Self Destruct!", guiActiveUnfocused = true, unfocusedRange = 8.0f)]
        public void ExplodeAllEvent()
        {
            countDownInitiated = Time.time;
            StartCoroutine(DoSelfDestruct());
            UpdateSelfDestructEvents();
        }

        [KSPEvent(guiActive = true, guiName = "Explode!", guiActiveUnfocused = true, unfocusedRange = 8.0f)]
        public void ExplodeEvent()
        {
            part.explode();
        }

        [KSPEvent(guiActive = false, guiActiveUnfocused = false, guiName = "Abort Self Destruct", unfocusedRange = 8.0f)]
        public void AbortSelfDestruct()
        {
            abortCountdown = true;
        }

        [KSPAction("Self Destruct!")]
        public void ExplodeAllAction(KSPActionParam param)
        {
            if (countDownInitiated == 0.0f)
            {
                countDownInitiated = Time.time;
                StartCoroutine(DoSelfDestruct());
                UpdateSelfDestructEvents();
            }
        }

        [KSPAction("Explode!")]
        public void ExplodeAction(KSPActionParam param)
        {
            if (countDownInitiated == 0.0f)
            {
                part.explode();
            }
        }

        private void UpdateSelfDestructEvents()
        {
            if (countDownInitiated == 0.0f)
            {
                // countdown has not been started
                BaseEvent explodeAll = Events["ExplodeAllEvent"];
                explodeAll.guiActive = explodeAll.guiActiveUnfocused = true;
                BaseEvent explode = Events["ExplodeEvent"];
                explode.guiActive = explode.guiActiveUnfocused = true;
                BaseEvent abort = Events["AbortSelfDestruct"];
                abort.guiActive = abort.guiActiveUnfocused = false;
            }
            else
            {
                // countdown has been started
                BaseEvent explodeAll = Events["ExplodeAllEvent"];
                explodeAll.guiActive = explodeAll.guiActiveUnfocused = false;
                BaseEvent explode = Events["ExplodeEvent"];
                explode.guiActive = explode.guiActiveUnfocused = false;
                BaseEvent abort = Events["AbortSelfDestruct"];
                abort.guiActive = abort.guiActiveUnfocused = true;
            }
        }

        private IEnumerator<WaitForSeconds> DoSelfDestruct()
        {
            ScreenMessage msg = ScreenMessages.PostScreenMessage("Self destruct sequence initiated.", timeDelay, ScreenMessageStyle.UPPER_CENTER);

            while ((Time.time - countDownInitiated) < timeDelay && !abortCountdown)
            {
                float remaining = timeDelay - (Time.time - countDownInitiated);
                msg.message = "Self destruct sequence initiated: " + remaining.ToString("#0");
                yield return new WaitForSeconds(0.2f);
            }

            if (!abortCountdown)
            {
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
            else
            {
                // reset
                msg.startTime = Time.time;
                msg.duration = 5.0f;
                msg.message = "Self destruct sequence stopped.";

                part.deactivate();
                countDownInitiated = 0.0f;
                abortCountdown = false;
                UpdateSelfDestructEvents();
            }
        }
    }
}
