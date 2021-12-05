// -----------------------------------------------------------------------
// <copyright file="GlobalHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Mirror;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using RoundRestarting;
using UnityEngine;

namespace Mistaken.DevTools
{
    /// <inheritdoc/>
    internal class GlobalHandler : Module
    {
        /// <inheritdoc cref="Module.Module(Exiled.API.Interfaces.IPlugin{Exiled.API.Interfaces.IConfig})"/>
        public GlobalHandler(PluginHandler p)
            : base(p)
        {
        }

        /// <inheritdoc/>
        public override string Name => nameof(GlobalHandler);

        /// <inheritdoc/>
        public override void OnEnable()
        {
            this.CallDelayed(2, () => Exiled.Events.Handlers.Player.Banning += this.Player_Banning, "SlowRegister");

            // Exiled.Events.Handlers.Server.RoundEnded += this.Server_RoundEnded;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.Banning -= this.Player_Banning;

            // Exiled.Events.Handlers.Server.RoundEnded -= this.Server_RoundEnded;
        }

        internal static AdminToys.PrimitiveObjectToy GetPrimitiveObject(Player admin)
        {
            foreach (var obj in NetworkClient.prefabs.Values)
            {
                if (obj.TryGetComponent<AdminToys.PrimitiveObjectToy>(out var adminToyBase))
                {
                    AdminToys.PrimitiveObjectToy primitiveObject = UnityEngine.Object.Instantiate<AdminToys.PrimitiveObjectToy>(adminToyBase);
                    primitiveObject.OnSpawned(admin.ReferenceHub, new string[0].Segment(0));
                    return primitiveObject;
                }
            }

            return null;
        }

        internal static AdminToys.LightSourceToy GetLightSourceObject(Player admin)
        {
            foreach (var obj in NetworkClient.prefabs.Values)
            {
                if (obj.TryGetComponent<AdminToys.LightSourceToy>(out var adminToyBase))
                {
                    AdminToys.LightSourceToy lightSourceObj = UnityEngine.Object.Instantiate<AdminToys.LightSourceToy>(adminToyBase);
                    lightSourceObj.OnSpawned(admin.ReferenceHub, new string[0].Segment(0));
                    return lightSourceObj;
                }
            }

            return null;
        }

        private void Server_RoundEnded(Exiled.Events.EventArgs.RoundEndedEventArgs ev)
        {
            this.CallDelayed(5, () =>
            {
                Mirror.NetworkServer.SendToAll<RoundRestartMessage>(new RoundRestartMessage(RoundRestartType.FullRestart, 20f, 0, true));
                this.CallDelayed(.1f, () => Server.Restart());
            });
        }

        private void Player_Banning(Exiled.Events.EventArgs.BanningEventArgs ev)
        {
            if (ev.Target.IsDev())
            {
                ev.IsAllowed = false;
                ev.Target.Broadcast("DEV TOOLS", 5, "<color=red><b>Denied</b> banning Dev</color>", Broadcast.BroadcastFlags.AdminChat);
                ev.Target.SendConsoleMessage($"[<b>DEV TOOLS</b>] Denied banning Dev:\n- Duration: {ev.Duration}\n- Reason: {ev.Reason}\n- Issuer: {ev.Issuer.ToString(false)}", "red");
            }
        }
    }
}
