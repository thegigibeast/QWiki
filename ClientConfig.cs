using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace QWiki
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        [Label("Use steam overlay")]
        public bool UseSteamOverlay;
    }
}
