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

        [DefaultValue(true)]
        [Label("Use active game culture")]
        public bool UseActiveGameCulture;

        [DefaultValue(false)]
        [Label("Show error messages")]
        public bool ShowErrorMessages;
    }
}
