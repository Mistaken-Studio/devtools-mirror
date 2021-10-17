// -----------------------------------------------------------------------
// <copyright file="ACSmolPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable

using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs;
using HarmonyLib;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Flashlight;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.Radio;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables;
using Mistaken.API;
using Mistaken.API.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mistaken.DevTools
{
    #region Hide
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


    /*[HarmonyPatch(typeof(RadioItem), nameof(RadioItem.Update))]
    static class MemePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = new List<CodeInstruction>();
            newInstructions.AddRange(instructions);
            int num = -4;
            int index = newInstructions.FindIndex((CodeInstruction instruction) => instruction.opcode == OpCodes.Ldloc_0) + num;
            Label label = newInstructions[newInstructions.Count - 1].labels[0];
            LocalBuilder localBuilder = generator.DeclareLocal(typeof(UsingRadioBatteryEventArgs));
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0, null),
                new CodeInstruction(OpCodes.Ldarg_0, null),
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(RadioItem), "Owner")),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.API.Features.Player), "Get", new Type[]
                {
                    typeof(ReferenceHub)
                }, null)),
                new CodeInstruction(OpCodes.Ldloc_0, null),
                new CodeInstruction(OpCodes.Ldc_I4_1, null),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UsingRadioBatteryEventArgs), null)[0]),
                new CodeInstruction(OpCodes.Dup, null),
                new CodeInstruction(OpCodes.Dup, null),
                new CodeInstruction(OpCodes.Stloc_S, localBuilder.LocalIndex),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.Events.Handlers.Player), "OnUsingRadioBattery", null, null)),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UsingRadioBatteryEventArgs), "IsAllowed")),
                new CodeInstruction(OpCodes.Brfalse_S, label),
                new CodeInstruction(OpCodes.Ldloc_S, localBuilder.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UsingRadioBatteryEventArgs), "Drain")),
                new CodeInstruction(OpCodes.Stloc_0, null),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Exception), null)[0]),
                new CodeInstruction(OpCodes.Throw, null),
            });
            int num2;
            for (int z = 0; z < newInstructions.Count; z = num2 + 1)
            {
                yield return newInstructions[z];
                num2 = z;
            }
            yield break;
        }
    }*/
    #endregion
    
    [HarmonyPatch(typeof(Item), nameof(Item.Get))]
    internal static class Itempatch
    {
        private static FieldInfo tmp = typeof(Item).GetField("BaseToItem", BindingFlags.NonPublic | BindingFlags.Static);
        private static Dictionary<ItemBase, Item> BaseToItem => (Dictionary<ItemBase, Item>)tmp.GetValue(null);
        public static bool Prefix(ItemBase itemBase, ref Item __result)
        {
            if (itemBase == null)
            {
                Log.SendRaw($"[Item.Debug] ItemBase is null", ConsoleColor.Cyan);
                __result = null;
                return false;
            }
            if (BaseToItem.TryGetValue(itemBase, out Item result))
            {
                if(result?.Type == ItemType.Radio && result?.GetType() == typeof(Exiled.API.Features.Items.Radio))
                {
                    __result = result;
                    return false;
                }

                Log.SendRaw($"[Item.Debug] ItemBase ({result?.Type}) is {result?.GetType().FullName}", ConsoleColor.Cyan);
                __result = result;
                return false;
            }
            InventorySystem.Items.Firearms.Firearm firearm = itemBase as InventorySystem.Items.Firearms.Firearm;
            if (firearm != null)
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be Firearm", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.Firearm(firearm);
                return false;
            }
            KeycardItem keycardItem = itemBase as KeycardItem;
            if (keycardItem != null)
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be Keycard", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.Keycard(keycardItem);
                return false;
            }
            UsableItem usableItem = itemBase as UsableItem;
            if (usableItem != null)
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be Usable", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.Usable(usableItem);
                return false;
            }
            RadioItem radioItem = itemBase as RadioItem;
            if (radioItem != null)
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be Radio", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.Radio(radioItem);
                return false;
            }
            MicroHIDItem microHIDItem = itemBase as MicroHIDItem;
            if (microHIDItem != null)
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be MicroHid", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.MicroHid(microHIDItem);
                return false;
            }
            BodyArmor bodyArmor = itemBase as BodyArmor;
            if (bodyArmor != null)
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be Armor", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.Armor(bodyArmor);
                return false;
            }
            AmmoItem ammoItem = itemBase as AmmoItem;
            if (ammoItem != null)
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be Ammo", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.Ammo(ammoItem);
                return false;
            }
            FlashlightItem flashlightItem = itemBase as FlashlightItem;
            if (flashlightItem != null)
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be Flashlight", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.Flashlight(flashlightItem);
                return false;
            }
            ThrowableItem throwableItem = itemBase as ThrowableItem;
            if (throwableItem == null)
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be Item", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.Item(itemBase);
                return false;
            }
            ThrownProjectile projectile = throwableItem.Projectile;
            if (projectile is FlashbangGrenade)
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be FlashGrenade", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.FlashGrenade(throwableItem);
                return false;
            }
            if (!(projectile is ExplosionGrenade))
            {
                Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be Throwable", ConsoleColor.Cyan);
                __result = new Exiled.API.Features.Items.Throwable(throwableItem);
                return false;
            }
            Log.SendRaw($"[Item.Debug] ItemBase ({itemBase.ItemTypeId}) will be ExplosiveGrenade", ConsoleColor.Cyan);
            __result = new Exiled.API.Features.Items.ExplosiveGrenade(throwableItem);
            return false;
        }
    }

    [HarmonyPatch(typeof(Item), MethodType.Constructor, typeof(ItemType))]
    internal static class ItemConstructirPatch
    {
        public static bool Prefix(ItemType type)
        {
            switch(type)
            {
                case ItemType.Flashlight:
                case ItemType.Coin:
                    return true;
                default:
                    Log.SendRaw("[ItemConstructor] DIE :) | " + type, ConsoleColor.Red);
                    throw new ArgumentOutOfRangeException("Incorrect item type: " + type);
            }
        }
    }

    [HarmonyPatch(typeof(Item), MethodType.Constructor, typeof(ItemBase))]
    internal static class ItemConstructirPatch2
    {
        public static bool Prefix(Item __instance, ItemBase itemBase)
        {
            if (__instance.GetType() != typeof(Item))
                return true;
            switch (itemBase.ItemTypeId)
            {
                case ItemType.Flashlight:
                case ItemType.Coin:
                    return true;
                default:
                    Log.SendRaw("[ItemConstructor2] DIE :) | " + itemBase.ItemTypeId, ConsoleColor.Red);
                    throw new ArgumentOutOfRangeException("(2) Incorrect item type: " + itemBase.ItemTypeId);
            }
        }
    }
}
