// -----------------------------------------------------------------------
// <copyright file="GlobalHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Mistaken.API.Diagnostics;

namespace Mistaken.DevTools
{
    /// <inheritdoc/>
    public class GlobalHandler : Module
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
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.SendingConsoleCommand -= this.Handle<Exiled.Events.EventArgs.SendingConsoleCommandEventArgs>((ev) => this.Server_SendingConsoleCommand(ev));
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
            else if (ev.Name == "int")
            {
                ev.Player.CurrentRoom.SetLightIntensity(float.Parse(ev.Arguments[0]));
                ev.IsAllowed = true;
                ev.ReturnMessage = "Done";
            }
        }
    }
}
