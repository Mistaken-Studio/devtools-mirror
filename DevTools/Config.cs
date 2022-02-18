// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Mistaken.Updater.Config;

namespace Mistaken.DevTools
{
    /// <inheritdoc/>
    public class Config : IAutoUpdatableConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug should be displayed.
        /// </summary>
        [Description("If true then debug will be displayed")]
        public bool VerbouseOutput { get; set; }

        /// <summary>
        /// Gets or sets webhook Diagnostics Link.
        /// </summary>
        public string WebhookLink { get; set; } = null;

        /// <summary>
        /// Gets or sets webhook Diagnostics Username.
        /// </summary>
        public string WebhookUsername { get; set; } = null;

        /// <summary>
        /// Gets or sets webhook Diagnostics Avatar.
        /// </summary>
        public string WebhookAvatar { get; set; } = null;

        /// <inheritdoc/>
        [Description("Auto Update Settings")]
        public System.Collections.Generic.Dictionary<string, string> AutoUpdateConfig { get; set; } = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Url", "https://git.mistaken.pl/api/v4/projects/23" },
            { "Token", string.Empty },
            { "Type", "GITLAB" },
            { "VerbouseOutput", "false" },
        };
    }
}
