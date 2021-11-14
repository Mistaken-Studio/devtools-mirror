// -----------------------------------------------------------------------
// <copyright file="GlobalHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
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
            this.CallDelayed(2, () => Exiled.Events.Handlers.Player.Banning += this.Player_Banning, "SlowRegister");

            // Exiled.Events.Handlers.Server.RoundEnded += this.Server_RoundEnded;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.Banning -= this.Player_Banning;

            // Exiled.Events.Handlers.Server.RoundEnded -= this.Server_RoundEnded;
        }

        private void Server_RoundEnded(Exiled.Events.EventArgs.RoundEndedEventArgs ev)
        {
            this.CallDelayed(5, () =>
            {
                PlayerStats._singleton.RpcRoundrestart(20, true);
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
