﻿/**
 * Kopernicus Planetary System Modifier
 * ------------------------------------------------------------- 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301  USA
 * 
 * This library is intended to be used as a plugin for Kerbal Space Program
 * which is copyright of TakeTwo Interactive. Your usage of Kerbal Space Program
 * itself is governed by the terms of its EULA, not the license above.
 * 
 * https://kerbalspaceprogram.com
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using ModularFI;
using UnityEngine;
using KSP.Localization;

namespace Kopernicus.Components
{
    /// <summary>
    /// An extension for the Solar Panel to calculate the flux properly
    /// </summary>
    public class KopernicusSolarPanels : PartModule
    {
        //Strings for Localization
        private static string SP_status_DirectSunlight = Localizer.Format("#Kopernicus_UI_DirectSunlight");//"Direct Sunlight"
        private static string SP_status_Underwater = Localizer.Format("#Kopernicus_UI_Underwater");//"Underwater"
        private static string button_AbsoluteExposure = Localizer.Format("#Kopernicus_UI_AbsoluteExposure");//"Use absolute exposure"
        private static string button_RelativeExposure = Localizer.Format("#Kopernicus_UI_RelativeExposure");//"Use relative exposure"
        private static string button_Auto = Localizer.Format("#Kopernicus_UI_AutoTracking");//"Auto"
        private static string SelectBody = Localizer.Format("#Kopernicus_UI_SelectBody");//"Select Tracking Body"
        private static string SelectBody_Msg = Localizer.Format("#Kopernicus_UI_SelectBody_Msg");// "Please select the Body you want to track with this Solar Panel."

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "#Kopernicus_UI_TrackingBody", isPersistant = true)]//Tracking Body
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public String trackingBodyName;

        [KSPField(isPersistant = true)]
        private Boolean _manualTracking;

        [KSPField(isPersistant = true)]
        private Boolean _relativeSunAoa;

        private ModuleDeployableSolarPanel[] SPs;

        private static readonly Double StockLuminosity;

        static KopernicusSolarPanels()
        {
            StockLuminosity = LightShifter.Prefab.solarLuminosity;
        }

        public void EarlyPostCalculateTracking()
        {
            Debug.Log("SigmaLog: EarlyPostCalculateTracking: OLD PhysicsGlobals.SolarLuminosityAtHome = " + PhysicsGlobals.SolarLuminosityAtHome);
            PhysicsGlobals.SolarLuminosityAtHome = Double.PositiveInfinity;
            Debug.Log("SigmaLog: EarlyPostCalculateTracking: TEMP1 PhysicsGlobals.SolarLuminosityAtHome = " + PhysicsGlobals.SolarLuminosityAtHome);
        }

        public void LatePostCalculateTracking()
        {
            for (Int32 n = 0; n < SPs.Length; n++)
            {
                ModuleDeployableSolarPanel SP = SPs[n];
                CelestialBody oldBody = SP.trackingBody;
                KopernicusStar oldStar = null;

                for (Int32 s = 0; s < KopernicusStar.Stars.Count; s++)
                {
                    KopernicusStar star = KopernicusStar.Stars[s];

                    if (star.sun == oldBody)
                    {
                        oldStar = star;
                    }
                    else
                    {
                        Test(SP, star);
                    }
                }

                Test(SP, oldStar);
            }

            Debug.Log("SigmaLog: LatePostCalculateTracking: TEMP2 PhysicsGlobals.SolarLuminosityAtHome = " + PhysicsGlobals.SolarLuminosityAtHome);
            PhysicsGlobals.SolarLuminosityAtHome = KopernicusStar.Current.shifter.solarLuminosity;
            Debug.Log("SigmaLog: LatePostCalculateTracking: NEW PhysicsGlobals.SolarLuminosityAtHome = " + PhysicsGlobals.SolarLuminosityAtHome);
            return;
            //for (Int32 n = 0; n < SPs.Length; n++)
            //{
            //    ModuleDeployableSolarPanel SP = SPs[n];
            //    Debug.Log("SigmaLog: LatePostCalculateTracking: SPs[" + n + "] = " + SP + ", DeployState = " + SP?.deployState);

            //    if (SP?.deployState == ModuleDeployablePart.DeployState.EXTENDED)
            //    {
            //        Vector3 normalized = (SP.trackingTransformLocal.position - SP.panelRotationTransform.position).normalized;
            //        LatePostCalculateTracking(SP, normalized);
            //    }
            //}
        }

        void Test(ModuleDeployableSolarPanel SP, KopernicusStar star)
        {
            SP.trackingTransformLocal = star.sun.transform;
            SP.trackingTransformScaled = star.sun.scaledBody.transform;

            PhysicsGlobals.SolarLuminosityAtHome = star.shifter.solarLuminosity;
            vessel.solarFlux = Flux(star);

            Vector3 normalized = (SP.trackingTransformLocal.position - SP.panelRotationTransform.position).normalized;

            FieldInfo Blocker = typeof(ModuleDeployableSolarPanel).GetField("blockingObject", BindingFlags.Instance | BindingFlags.NonPublic);
            String blocker = Blocker.GetValue(SP) as String;
            Boolean trackingLos = SP.CalculateTrackingLOS(normalized, ref blocker);
            Blocker.SetValue(SP, blocker);

            SP.PostCalculateTracking(trackingLos, normalized);
        }

        Double Flux(KopernicusStar star)
        {
            Debug.Log("SigmaLog: KopernicusSolarPanel.Flux: star = " + star?.sun);
            // Get sunVector
            Boolean directSunlight = false;
            Vector3 integratorPosition = vessel.transform.position;
            Vector3d scaledSpace = ScaledSpace.LocalToScaledSpace(integratorPosition);
            Vector3 position = star.sun.scaledBody.transform.position;
            Double scale = Math.Max((position - scaledSpace).magnitude, 1);
            Vector3 sunVector = (position - scaledSpace) / scale;
            Ray ray = new Ray(ScaledSpace.LocalToScaledSpace(integratorPosition), sunVector);

            // Get Solar Flux
            Double realDistanceToSun = 0;
            if (!Physics.Raycast(ray, out RaycastHit raycastHit, Single.MaxValue, ModularFlightIntegrator.SunLayerMask))
            {
                directSunlight = true;
                realDistanceToSun = scale * ScaledSpace.ScaleFactor - star.sun.Radius;
            }
            else if (raycastHit.transform.GetComponent<ScaledMovement>().celestialBody == star.sun)
            {
                realDistanceToSun = ScaledSpace.ScaleFactor * raycastHit.distance;
                directSunlight = true;
            }

            if (directSunlight)
            {
                Debug.Log("SigmaLog: KopernicusSolarPanel.Flux:     PhysicsGlobals.SolarLuminosity = " + PhysicsGlobals.SolarLuminosity + ", PhysicsGlobals.SolarLuminosityAtHome = " + PhysicsGlobals.SolarLuminosityAtHome);
                Debug.Log("SigmaLog: KopernicusSolarPanel.Flux:     return ==> " + (PhysicsGlobals.SolarLuminosity / (12.5663706143592 * realDistanceToSun * realDistanceToSun)));
                return PhysicsGlobals.SolarLuminosity / (12.5663706143592 * realDistanceToSun * realDistanceToSun);
            }

            Debug.Log("SigmaLog: KopernicusSolarPanel.Flux:     return ==> " + 0);
            return 0;
        }

        //public void LatePostCalculateTracking(ModuleDeployableSolarPanel SP, Vector3 trackingDirection)
        //{
        //    FieldInfo trackingLOS = typeof(ModuleDeployableSolarPanel).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name == "trackingLOS");
        //    Boolean trackingLos = (bool)trackingLOS.GetValue(SP);
        //    Debug.Log("SigmaLog: LatePostCalculateTracking: SP = " + SP);

        //    // Maximum values
        //    Double maxEnergy = 0;
        //    KopernicusStar maxStar = null;

        //    // Override layer mask
        //    typeof(ModuleDeployableSolarPanel).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name == "planetLayerMask").SetValue(SP, ModularFlightIntegrator.SunLayerMask);

        //    // Reset values
        //    SP._flowRate = 0;
        //    SP.sunAOA = 0;

        //    // Go through all stars
        //    Int32 stars = KopernicusStar.Stars.Count;
        //    for (Int32 i = 0; i < stars; i++)
        //    {
        //        KopernicusStar star = KopernicusStar.Stars[i];
        //        Debug.Log("SigmaLog: LatePostCalculateTracking: star = " + star?.sun);

        //        // Calculate stuff
        //        Transform sunTransform = star.sun.transform;
        //        Vector3 trackDir = (sunTransform.position - SP.panelRotationTransform.position).normalized;
        //        CelestialBody old = SP.trackingBody;
        //        SP.trackingTransformLocal = sunTransform;
        //        SP.trackingTransformScaled = star.sun.scaledBody.transform;

        //        FieldInfo blockingObject = typeof(ModuleDeployableSolarPanel).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name == "blockingObject");
        //        string blockingObjectName = (string)blockingObject.GetValue(SP);
        //        trackingLos = SP.CalculateTrackingLOS(trackDir, ref blockingObjectName);
        //        blockingObject.SetValue(SP, blockingObjectName);

        //        SP.trackingTransformLocal = old.transform;
        //        SP.trackingTransformScaled = old.scaledBody.transform;

        //        // Calculate sun AOA
        //        Single sunAoa;
        //        if (!trackingLos)
        //        {
        //            sunAoa = 0f;
        //            SP.status = Localizer.Format("#Kopernicus_UI_PanelBlocked", blockingObjectName);//"Blocked by " + blockingObjectName
        //        }
        //        else
        //        {
        //            SP.status = SP_status_DirectSunlight;//"Direct Sunlight"
        //            if (SP.panelType == ModuleDeployableSolarPanel.PanelType.FLAT)
        //            {
        //                sunAoa = Mathf.Clamp(Vector3.Dot(SP.trackingDotTransform.forward, trackDir), 0f, 1f);
        //            }
        //            else if (SP.panelType != ModuleDeployableSolarPanel.PanelType.CYLINDRICAL)
        //            {
        //                sunAoa = 0.25f;
        //            }
        //            else
        //            {
        //                Vector3 direction;
        //                if (SP.alignType == ModuleDeployableSolarPanel.PanelAlignType.PIVOT)
        //                {
        //                    direction = SP.trackingDotTransform.forward;
        //                }
        //                else if (SP.alignType != ModuleDeployableSolarPanel.PanelAlignType.X)
        //                {
        //                    direction = SP.alignType != ModuleDeployableSolarPanel.PanelAlignType.Y
        //                        ? part.partTransform.forward
        //                        : part.partTransform.up;
        //                }
        //                else
        //                {
        //                    direction = part.partTransform.right;
        //                }

        //                sunAoa = (1f - Mathf.Abs(Vector3.Dot(direction, trackDir))) * 0.318309873f;
        //            }
        //        }

        //        // Calculate distance multiplier
        //        Double distMult;
        //        Debug.Log("SigmaLog: LatePostCalculateTracking: !SP.useCurve = " + !SP.useCurve);
        //        if (!SP.useCurve)
        //        {
        //            Debug.Log("SigmaLog: LatePostCalculateTracking: !KopernicusStar.SolarFlux.ContainsKey(star.name) = " + !KopernicusStar.SolarFlux.ContainsKey(star.name));
        //            if (!KopernicusStar.SolarFlux.ContainsKey(star.name))
        //            {
        //                Debug.Log("SigmaLog: LatePostCalculateTracking: skip star ==> " + star?.sun);
        //                continue;
        //            }

        //            distMult = (Single)(KopernicusStar.SolarFlux[star.name] / StockLuminosity);
        //        }
        //        else
        //        {
        //            distMult =
        //             SP.powerCurve.Evaluate((star.sun.transform.position - SP.panelRotationTransform.position).magnitude);
        //        }

        //        // Calculate flow rate
        //        Debug.Log("SigmaLog: LatePostCalculateTracking: sunAoa = " + sunAoa);
        //        Debug.Log("SigmaLog: LatePostCalculateTracking: SP._efficMult = " + SP._efficMult);
        //        Debug.Log("SigmaLog: LatePostCalculateTracking: distMult = " + distMult);
        //        Double panelFlowRate = sunAoa * SP._efficMult * distMult;
        //        Debug.Log("SigmaLog: LatePostCalculateTracking: MAX panelFlowRate = " + panelFlowRate);
        //        if (part.submergedPortion > 0)
        //        {
        //            Double altitudeAtPos =
        //                -FlightGlobals.getAltitudeAtPos
        //                (
        //                    (Vector3d)(((Transform)typeof(ModuleDeployableSolarPanel).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name == "secondaryTransform").GetValue(SP)).position),
        //                    vessel.mainBody
        //                );
        //            altitudeAtPos = (altitudeAtPos * 3 + part.maxDepth) * 0.25;
        //            if (altitudeAtPos < 0.5)
        //            {
        //                altitudeAtPos = 0.5;
        //            }

        //            Double num = 1 / (1 + altitudeAtPos * part.vessel.mainBody.oceanDensity);
        //            if (part.submergedPortion >= 1)
        //            {
        //                panelFlowRate *= num;
        //            }
        //            else
        //            {
        //                panelFlowRate *= UtilMath.LerpUnclamped(1, num, part.submergedPortion);
        //            }

        //            SP.status += ", " + SP_status_Underwater;//Underwater
        //        }
        //        Debug.Log("SigmaLog: LatePostCalculateTracking: UNSUBMERGED panelFlowRate = " + panelFlowRate);

        //        SP.sunAOA += sunAoa;
        //        Double energy = distMult * SP._efficMult;
        //        if (energy > maxEnergy)
        //        {
        //            maxEnergy = energy;
        //            maxStar = star;
        //        }

        //        // Apply the flow rate
        //        SP._flowRate += panelFlowRate;
        //        Debug.Log("SigmaLog: LatePostCalculateTracking: SP._flowRate = " + SP._flowRate);
        //    }

        //    // Sun AOA
        //    SP.sunAOA /= _relativeSunAoa ? stars : 1;
        //    //SP._distMult = Math.Abs(SP._flowRate) > 0.01 ? SP._flowRate / SP._efficMult / SP.sunAOA : 0;

        //    // We got the best star to use
        //    if (maxStar != null && maxStar.sun != SP.trackingBody)
        //    {
        //        if (!_manualTracking)
        //        {
        //            SP.trackingBody = maxStar.sun;
        //            SP.GetTrackingBodyTransforms();
        //        }
        //    }

        //    // Use the flow rate
        //    double tempFlowRate = 0;//SP.resHandler.UpdateModuleResourceOutputs(SP._flowRate);
        //    Debug.Log("SigmaLog: LatePostCalculateTracking: tempFlowRate = " + tempFlowRate);
        //    SP.flowRate = (Single)(tempFlowRate * SP.flowMult);
        //    Debug.Log("SigmaLog: LatePostCalculateTracking: SP.flowRate = " + SP.flowRate);
        //}

        public void EarlyLateUpdate()
        {
            for (Int32 n = 0; n < SPs.Length; n++)
            {
                ModuleDeployableSolarPanel SP = SPs[n];

                if (SP?.deployState == ModuleDeployablePart.DeployState.EXTENDED)
                {
                    // Update the name
                    trackingBodyName = SP.trackingBody.bodyDisplayName.Replace("^N", "");

                    // Update the guiName for SwitchAOAMode
                    Events["SwitchAoaMode"].guiName = _relativeSunAoa ? button_AbsoluteExposure : button_RelativeExposure;//Use absolute exposure//Use relative exposure
                }
            }
        }

        [KSPEvent(active = true, guiActive = false, guiName = "#Kopernicus_UI_SelectBody")]//Select Tracking Body
        public void ManualTracking()
        {
            // Assemble the buttons
            Int32 stars = KopernicusStar.Stars.Count;
            DialogGUIBase[] options = new DialogGUIBase[stars + 1];
            options[0] = new DialogGUIButton(button_Auto, () => { _manualTracking = false; }, true);//Auto
            for (Int32 i = 0; i < stars; i++)
            {
                CelestialBody body = KopernicusStar.Stars[i].sun;
                options[i + 1] = new DialogGUIButton
                (
                    body.bodyDisplayName.Replace("^N", ""),
                    () =>
                    {
                        for (int n = SPs?.Length ?? 0; n > 0; n--)
                        {
                            ModuleDeployableSolarPanel SP = SPs[n - 1];
                            _manualTracking = true;
                            SP.trackingBody = body;
                            SP.GetTrackingBodyTransforms();
                        }
                    },
                    true
                );
            }

            PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog(
                "SelectTrackingBody",
                SelectBody_Msg,//Please select the Body you want to track with this Solar Panel.
                SelectBody,//Select Tracking Body
                UISkinManager.GetSkin("MainMenuSkin"),
                options), false, UISkinManager.GetSkin("MainMenuSkin"));
        }

        [KSPEvent(active = true, guiActive = true, guiName = "#Kopernicus_UI_RelativeExposure")]//Use relative exposure
        public void SwitchAoaMode()
        {
            _relativeSunAoa = !_relativeSunAoa;
        }

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                TimingManager.LateUpdateAdd(TimingManager.TimingStage.Early, EarlyLateUpdate);
                TimingManager.FixedUpdateAdd(TimingManager.TimingStage.FlightIntegrator, EarlyPostCalculateTracking);
                TimingManager.FixedUpdateAdd(TimingManager.TimingStage.Late, LatePostCalculateTracking);

                SPs = GetComponents<ModuleDeployableSolarPanel>();

                if (SPs.Any(p => p.isTracking))
                {
                    Fields["trackingBodyName"].guiActive = true;
                    Events["ManualTracking"].guiActive = true;
                }
            }

            base.OnStart(state);
        }

        public void OnDestroy()
        {
            TimingManager.LateUpdateRemove(TimingManager.TimingStage.Early, EarlyLateUpdate);
            TimingManager.FixedUpdateRemove(TimingManager.TimingStage.FlightIntegrator, EarlyPostCalculateTracking);
            TimingManager.FixedUpdateRemove(TimingManager.TimingStage.Late, LatePostCalculateTracking);
        }
    }
}
