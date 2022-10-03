// -----------------------------------------------------------------------
// <copyright file="TPSCounter.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Discord_Webhook;
using Exiled.API.Features;
using UnityEngine;

namespace Mistaken.DevTools
{
    internal class TPSCounter : MonoBehaviour
    {
        private readonly List<(int Ticks, double Second)> lastTPS = new ();
        private readonly ConcurrentQueue<double> longTicks = new ();
        private int ticks = 0;

        private void Start()
        {
            this.StartCoroutine(this.CountTicks());
            this.StartCoroutine(this.SubmitTicks());
            this.StartCoroutine(this.HandleLongTicks());
        }

        private void LateUpdate()
        {
            this.ticks++;
        }

        private IEnumerator CountTicks()
        {
            DateTime tickStart;
            int thisTicks;
            double delta;
            do
            {
                tickStart = DateTime.Now;
                yield return new WaitForSeconds(1);
                thisTicks = this.ticks;
                this.ticks = 0;
                var second = (DateTime.Now - tickStart).TotalSeconds;
                this.lastTPS.Add((thisTicks, second));
                delta = Time.deltaTime;
                if (delta >= 0.1f)
                    this.longTicks.Enqueue(delta);
            }
            while (Round.IsStarted);
        }

        private IEnumerator SubmitTicks()
        {
            while (true)
            {
                yield return new WaitForSeconds(60);

                var tpsArr = this.lastTPS.Count > 60 ? this.lastTPS.Skip(this.lastTPS.Count - 60).ToArray() : this.lastTPS.ToArray();
                var tps = Math.Round(tpsArr.Average(x => x.Ticks), 2);
                var second = Math.Round(tpsArr.Average(x => x.Second), 3);
                var realAvgTPS = Math.Round(tpsArr.Sum(x => x.Ticks) / tpsArr.Sum(x => x.Second), 2);
                bool warn = false;
                if (tps < 30)
                    warn = true;

                if (second > 1.1f)
                    warn = true;

                if (realAvgTPS < 30)
                    warn = true;

                if (!warn)
                    continue;

                new Webhook(PluginHandler.Instance.Config.WebhookLink)
                    .AddMessage((msg) =>
                        msg
                        .WithAvatar(string.IsNullOrWhiteSpace(PluginHandler.Instance.Config.WebhookAvatar) ? null : PluginHandler.Instance.Config.WebhookAvatar)
                        .WithUsername(string.IsNullOrWhiteSpace(PluginHandler.Instance.Config.WebhookUsername) ? null : PluginHandler.Instance.Config.WebhookUsername)
                        .WithContent(string.Concat(
                            $"[{(warn ? "⚠" : "❕")}] ",
                            $"[`{Server.Port}`] ",
                            $"[`{DateTime.Now:HH:mm:ss}`] ",
                            $"[`{Round.ElapsedTime.Minutes:00}:{Round.ElapsedTime.Seconds:00}`] ",
                            $"TPS: `{tps:00.00}`, ",
                            $"Real Avg TPS: `{realAvgTPS:00.00}`, ",
                            $"`{second:0.000}s/1s`"))).Send();
            }
        }

        private IEnumerator HandleLongTicks()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (this.longTicks.TryDequeue(out double delta))
                {
                    new Webhook(PluginHandler.Instance.Config.WebhookLink)
                        .AddMessage((msg) =>
                            msg
                            .WithAvatar(string.IsNullOrWhiteSpace(PluginHandler.Instance.Config.WebhookAvatar) ? null : PluginHandler.Instance.Config.WebhookAvatar)
                            .WithUsername(string.IsNullOrWhiteSpace(PluginHandler.Instance.Config.WebhookUsername) ? null : PluginHandler.Instance.Config.WebhookUsername)
                            .WithContent($"[⚠] [`{Server.Port}`] [`{DateTime.Now:HH:mm:ss}`] [`{Round.ElapsedTime.Minutes:00}:{Round.ElapsedTime.Seconds:00}`] One of ticks took: `{delta:0.000}s`")).Send();
                }
            }
        }
    }
}
