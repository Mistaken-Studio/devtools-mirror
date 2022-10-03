// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace Mistaken.DevTools
{
    internal class PluginHandler : Plugin<Config>
    {
        public override string Author => "Mistaken Devs";

        public override string Name => "DevTools";

        public override string Prefix => "MDEVTOOLS";

        public override PluginPriority Priority => PluginPriority.Higher - 1;

        public override Version RequiredExiledVersion => new (5, 2, 2);

        public override void OnEnabled()
        {
            Instance = this;

            // Exiled.Events.Events.DisabledPatchesHashSet.Add(typeof(RadioItem).GetMethod(nameof(RadioItem.Update), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static));
            // Exiled.Events.Events.Instance.ReloadDisabledPatches();
            var harmony = new HarmonyLib.Harmony("com.mistaken.devtools");
            harmony.PatchAll();

            new GlobalHandler(this);

            API.Diagnostics.Module.OnEnable(this);

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            API.Diagnostics.Module.OnDisable(this);

            base.OnDisabled();
        }

        internal static PluginHandler Instance { get; private set; }
    }
}
