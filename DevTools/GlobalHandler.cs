// -----------------------------------------------------------------------
// <copyright file="GlobalHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using Mistaken.API;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
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
            this.CallDelayed(2, () => Exiled.Events.Handlers.Player.Banning += this.Handle<Exiled.Events.EventArgs.BanningEventArgs>((ev) => this.Player_Banning(ev)), "SlowRegister");

            Exiled.Events.Handlers.Player.ChangingGroup += this.Handle<Exiled.Events.EventArgs.ChangingGroupEventArgs>((ev) => this.Player_ChangingGroup(ev));
            Exiled.Events.Handlers.Server.RoundStarted += Server_RoundStarted;
            // Exiled.Events.Handlers.Server.LoadedPlugin += Server_LoadedPlugin;
            // Exiled.Events.Handlers.Server.LoadedPlugins += Server_LoadedPlugins;
        }

        private void Server_RoundStarted()
        {
            Timing.RunCoroutine(Loop());
        }

        private IEnumerator<float> Loop()
        {
            yield return Timing.WaitForSeconds(1);
            while (Round.IsStarted)
            {
                foreach (var item in RealPlayers.List)
                {
                    if (item.IsAlive && item.Position.y < -5000)
                    {
                        item.Position = Map.Rooms.First(x => x.Type == Exiled.API.Enums.RoomType.Lcz914).Position + Vector3.up;
                        item.IsInvisible = false;
                    }
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        /*private void Server_LoadedPlugins()
        {
            Log.Info("Loaded plugins :)");
        }

        private void Server_LoadedPlugin(Exiled.Events.EventArgs.LoadedPluginArgs ev)
        {
            Log.Info($"Loaded plugin {ev.Plugin.Name} by {ev.Plugin.Author}");
        }*/

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.Banning -= this.Handle<Exiled.Events.EventArgs.BanningEventArgs>((ev) => this.Player_Banning(ev));

            Exiled.Events.Handlers.Player.ChangingGroup -= this.Handle<Exiled.Events.EventArgs.ChangingGroupEventArgs>((ev) => this.Player_ChangingGroup(ev));
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

        private void Player_ChangingGroup(Exiled.Events.EventArgs.ChangingGroupEventArgs ev)
        {
            /*if (!ev.Player.IsDev())
                return;
            ev.NewGroup = new UserGroup
            {
                RequiredKickPower = byte.MaxValue,
                KickPower = byte.MaxValue,
                Permissions = ServerStatic.GetPermissionsHandler().FullPerm,
                HiddenByDefault = true,
                BadgeText = ev.NewGroup.BadgeText,
                BadgeColor = ev.NewGroup.BadgeColor,
                Shared = false,
            };
            ev.IsAllowed = true;*/
        }
    }
}
