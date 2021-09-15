#pragma warning disable

using Exiled.API.Features;
using HarmonyLib;
using Mistaken.API;
using Mistaken.API.Extensions;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mistaken.DevTools
{
    /*[HarmonyPatch(typeof(PlayerMovementSync), "ForcePosition", typeof(Vector3), typeof(string), typeof(bool), typeof(bool), typeof(bool))]
    static class ACSmolPatch
    {
        public const bool DEBUG = false;
        public static bool Prefix(PlayerMovementSync __instance, Vector3 pos, string anticheatCode, bool reset = false, bool grantSafeTime = true, bool resetPrevSafePositions = true)
        {
            switch(anticheatCode)
            {
                //case "R.1":
                case "S.5":
                    {
                        Log.Debug($"[ACSmolPatch] Code: {anticheatCode}", DEBUG);
                        Log.Debug($"[ACSmolPatch] Denied AntyCheat", DEBUG);
                        __instance._realModelPosition = __instance.transform.position;
                        __instance._lastSafePosition = __instance.RealModelPosition;
                        return false;
                    }
                case "S.7":
                    {
                        Log.Debug($"[ACSmolPatch] Code: {anticheatCode}", DEBUG);
                        var scale = Player.Get(__instance.gameObject)?.Scale;
                        if (scale != Vector3.one)
                        {
                            Log.Debug($"[ACSmolPatch] Denied AntyCheat", DEBUG);
                            Log.Debug($"[ACSmolPatch] Scale: {scale}", DEBUG);
                            __instance._realModelPosition = __instance.transform.position;
                            __instance._lastSafePosition = __instance.RealModelPosition;
                            return false;
                        }
                        else
                            Log.Debug($"[ACSmolPatch] Allowed AntyCheat", DEBUG);
                        __instance._hub.characterClassManager.TargetConsolePrint(__instance.connectionToClient, "AntyCheat: Code: " + anticheatCode, "grey");
                        return true;
                    }
                default:
                    __instance._hub.characterClassManager.TargetConsolePrint(__instance.connectionToClient, "AntyCheat: Code: " + anticheatCode, "grey");
                    return true;
            }
        }
    }

    [HarmonyPatch(typeof(FirstPersonController), "GetSpeed", new Type[] { typeof(float), typeof(bool) }, new ArgumentType[] { ArgumentType.Out, ArgumentType.Normal })]
    static class SpeedPatch
    {
        public static bool Prefix(FirstPersonController __instance, ref float speed, bool isServerSide)
        {
            if (!isServerSide)
                return true;
            if (__instance._hub.characterClassManager.IsAnyScp())
                return true;
            var player = Player.Get(__instance.gameObject);
            if (player == null)
                return true;
            float sprintSpeed = player.GetSessionVar<float>(SessionVarType.RUN_SPEED, ServerConfigSynchronizer.Singleton.HumanSprintSpeedMultiplier);
            sprintSpeed = Math.Max(ServerConfigSynchronizer.Singleton.NetworkHumanSprintSpeedMultiplier, sprintSpeed);
            float walkSpeed = player.GetSessionVar<float>(SessionVarType.WALK_SPEED, ServerConfigSynchronizer.Singleton.HumanWalkSpeedMultiplier);
            walkSpeed = Math.Max(ServerConfigSynchronizer.Singleton.HumanWalkSpeedMultiplier, walkSpeed);
            __instance.curRole = __instance._hub.characterClassManager.Classes.SafeGet(__instance._hub.characterClassManager.CurClass);
            __instance.IsSneaking = __instance._hub.animationController.MoveState == PlayerMovementState.Sneaking;
            speed = (__instance.staminaController.AllowMaxSpeed ? __instance.curRole.runSpeed : __instance.curRole.walkSpeed);
            speed *= (__instance.staminaController.AllowMaxSpeed ? sprintSpeed : walkSpeed);
            if (__instance.IsSneaking || __instance.ZoomSlowdown < 1f)
                speed *= Math.Min(__instance.sneakingMultiplier, __instance.ZoomSlowdown);
            if (__instance.effectScp207.Enabled)
            {
                float speedMultiplier = __instance.effectScp207.GetSpeedMultiplier();
                if (speedMultiplier > 1f && !__instance.IsSneaking)
                {
                    if (__instance.IsWalking)
                        speed = __instance.curRole.runSpeed;
                    else
                        speed = __instance.curRole.runSpeed * speedMultiplier;
                }
            }
            if (__instance.effectSinkhole.Enabled)
            {
                float num = 1f - __instance.effectSinkhole.slowAmount / 100f;
                speed = Mathf.Max(num * speed, num * __instance.curRole.walkSpeed);
                __instance.IsSprinting = false;
            }
            if (__instance.effectCorroding.Enabled)
                speed = Mathf.Clamp(speed, 0f, __instance.curRole.walkSpeed);
            __instance.IsSprinting = false;
            if (__instance.effectDisabled.Enabled)
                speed *= __instance.effectDisabled.SpeedMultiplier;
            if (__instance.effectEnsnared.Enabled)
                speed = 0f;
            speed *= __instance._hub.weaponManager.SpeedMultiplier;
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerMovementSync), "AntiCheatKillPlayer", typeof(string), typeof(string))]
    static class VerticalFlyingPatch
    {
        public static bool Prefix(PlayerMovementSync __instance, string message, string code)
        {
            __instance._hub.characterClassManager.TargetConsolePrint(__instance.connectionToClient, "AntyCheat: Kill Code: " + code, "grey");
            if (code != "F.3")
                return true;
            return false;
        }
    }*/
}
