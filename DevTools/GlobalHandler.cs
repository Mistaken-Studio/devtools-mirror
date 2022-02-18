// -----------------------------------------------------------------------
// <copyright file="GlobalHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord_Webhook;
using Exiled.API.Features;
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
            this.CallDelayed(2, () => Exiled.Events.Handlers.Player.Banning += this.Player_Banning, "SlowRegister");
            API.Diagnostics.MasterHandler.OnErrorCatched += this.MasterHandler_OnErrorCatched;
            API.Diagnostics.MasterHandler.OnUnityCatchedException += this.MasterHandler_OnUnityCatchedException;
            Exiled.Events.Handlers.Server.RoundStarted += this.IniTPSCounter;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.Banning -= this.Player_Banning;
            API.Diagnostics.MasterHandler.OnErrorCatched -= this.MasterHandler_OnErrorCatched;
            API.Diagnostics.MasterHandler.OnUnityCatchedException -= this.MasterHandler_OnUnityCatchedException;
            Exiled.Events.Handlers.Server.RoundStarted -= this.IniTPSCounter;
        }

        internal static AdminToys.PrimitiveObjectToy GetPrimitiveObject()
        {
            return API.MapPlus.SpawnPrimitive(UnityEngine.PrimitiveType.Sphere, new GameObject().transform, Color.red, true);
        }

        internal static AdminToys.LightSourceToy GetLightSourceObject()
        {
            return API.MapPlus.SpawnLight(new GameObject().transform, Color.red, 1, 1, false, true);
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

        private string ExceptionToString(System.Exception ex)
        {
            string stackTrace = ex.StackTrace;
            stackTrace = Regex.Replace(stackTrace, "(\\s*\\[0x\\d*\\] in <.*>:\\d*)|(\\s*\\[0x\\d*\\]\\s*at \\(wrapper managed - to - native\\))|(\\s*at \\(wrapper managed-to-native\\))", string.Empty);
            var index = stackTrace.IndexOf("System.Reflection.MonoMethod.InternalInvoke");
            if (index != -1)
                stackTrace = stackTrace.Substring(0, index);

            string message = ex.Message;
            string tor = !string.IsNullOrEmpty(message) ? (ex.GetType().Name + ": " + message) : ex.GetType().Name;
            if (ex.InnerException != null)
                tor += " ---> " + this.ExceptionToString(ex.InnerException) + Environment.NewLine + "   --- End of inner exception stack trace ---";

            if (stackTrace != null)
                tor += Environment.NewLine + stackTrace;

            return tor;
        }

        private void MasterHandler_OnUnityCatchedException(string message, string stackTrace)
        {
            if (string.IsNullOrWhiteSpace(PluginHandler.Instance.Config.WebhookLink))
                return;

            this.Send(new Webhook(PluginHandler.Instance.Config.WebhookLink)
                .AddMessage(msg => msg
                    .WithAvatar(string.IsNullOrWhiteSpace(PluginHandler.Instance.Config.WebhookAvatar) ? null : PluginHandler.Instance.Config.WebhookAvatar)
                    .WithUsername(string.IsNullOrWhiteSpace(PluginHandler.Instance.Config.WebhookUsername) ? null : PluginHandler.Instance.Config.WebhookUsername)
                    .WithContent(string.Concat(
                            $"[❗] ",
                            $"[`{Server.Port}`] ",
                            $"[`{DateTime.Now:HH:mm:ss}`] ",
                            $"Uncached Exception (<@356174382655209483>)"))
                    .WithEmbed(embed => embed
                        .WithColor(255, 0, 0)
                        .WithDescription($"```{message}\n{stackTrace}```"))));
        }

        private void MasterHandler_OnErrorCatched(System.Exception ex, string method)
        {
            if (string.IsNullOrWhiteSpace(PluginHandler.Instance.Config.WebhookLink))
                return;

            this.Send(new Webhook(PluginHandler.Instance.Config.WebhookLink)
                .AddMessage(msg => msg
                    .WithAvatar(string.IsNullOrWhiteSpace(PluginHandler.Instance.Config.WebhookAvatar) ? null : PluginHandler.Instance.Config.WebhookAvatar)
                    .WithUsername(string.IsNullOrWhiteSpace(PluginHandler.Instance.Config.WebhookUsername) ? null : PluginHandler.Instance.Config.WebhookUsername)
                    .WithContent(string.Concat(
                            $"[❗] ",
                            $"[`{Server.Port}`] ",
                            $"[`{DateTime.Now:HH:mm:ss}`] ",
                            $"Exception by `{method}` (<@356174382655209483>)"))
                    .WithEmbed(embed => embed
                        .WithColor(255, 0, 0)
                        .WithDescription($"```{this.ExceptionToString(ex)}```"))));
        }

        private async void Send(Webhook webhook)
        {
            var result = await webhook.Send();

            if (string.IsNullOrWhiteSpace(result))
                return;

            if (result.StartsWith("System.Net.WebException: The remote server returned an error: (429) Too Many Requests."))
            {
                this.Log.Warn("[Webhook] Failed to send error hook, I was too fast (429)");
                _ = Task.Run(() =>
                {
                    Task.Delay(6000).Wait();
                    this.Send(webhook);
                });
            }
            else
                this.Log.Error($"[Webhook] {result}");
        }

        private void IniTPSCounter()
        {
            Server.Host.GameObject.AddComponent<TPSCounter>();
        }
    }
}
