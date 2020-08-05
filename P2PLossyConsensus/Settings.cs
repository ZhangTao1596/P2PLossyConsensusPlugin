using Microsoft.Extensions.Configuration;
using Neo.Network.P2P.Payloads;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Plugins
{
    internal class Settings
    {
        public double LossRate { get; }

        public static Settings Default { get; private set; }

        private Settings(IConfigurationSection section)
        {
            this.LossRate = GetValueOrDefault(section.GetSection("LossRate"), 0, p => double.Parse(p));
        }

        public T GetValueOrDefault<T>(IConfigurationSection section, T defaultValue, Func<string, T> selector)
        {
            if (section.Value == null) return defaultValue;
            return selector(section.Value);
        }

        public static void Load(IConfigurationSection section)
        {
            Default = new Settings(section);
        }
    }
}
