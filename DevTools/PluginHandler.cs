// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using InventorySystem.Items.Radio;

namespace Mistaken.DevTools
{
    /// <inheritdoc/>
    public class PluginHandler : Plugin<Config>
    {
        /// <inheritdoc/>
        public override string Author => "Mistaken Devs";

        /// <inheritdoc/>
        public override string Name => "DevTools";

        /// <inheritdoc/>
        public override string Prefix => "MDEVTOOLS";

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Higher - 1;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(4, 0, 0);

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            API.Diagnostics.Module.OnDisable(this);

            base.OnDisabled();
        }

        internal static PluginHandler Instance { get; private set; }
    }
}
