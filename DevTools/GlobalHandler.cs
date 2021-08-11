// -----------------------------------------------------------------------
// <copyright file="GlobalHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;

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
            Exiled.Events.Handlers.Server.SendingConsoleCommand += this.Handle<Exiled.Events.EventArgs.SendingConsoleCommandEventArgs>((ev) => this.Server_SendingConsoleCommand(ev));
            Exiled.Events.Handlers.Player.Banning += this.Handle<Exiled.Events.EventArgs.BanningEventArgs>((ev) => this.Player_Banning(ev));
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.SendingConsoleCommand -= this.Handle<Exiled.Events.EventArgs.SendingConsoleCommandEventArgs>((ev) => this.Server_SendingConsoleCommand(ev));
            Exiled.Events.Handlers.Player.Banning -= this.Handle<Exiled.Events.EventArgs.BanningEventArgs>((ev) => this.Player_Banning(ev));
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

        private void Server_SendingConsoleCommand(Exiled.Events.EventArgs.SendingConsoleCommandEventArgs ev)
        {
            if (ev.Player?.GroupName != "dev")
                return;
            if (ev.Name == "dur")
            {
                ev.Player.Inventory.items.ModifyDuration(ev.Player.CurrentItemIndex, float.Parse(ev.Arguments[0]));
                ev.IsAllowed = true;
                ev.ReturnMessage = "Done";
            }
            else if (ev.Name == "light")
            {
                ev.Player.CurrentRoom.SetLightIntensity(float.Parse(ev.Arguments[0]));
                ev.IsAllowed = true;
                ev.ReturnMessage = "Done";
            }
        }
    }
}
