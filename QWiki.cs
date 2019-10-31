using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace QWiki
{
    public class QWiki : Mod
	{
        private const string WIKI_SEARCH_NAME = "Search Wiki";
        private const string WIKI_SEARCH_KEY = "Q";

        private static ModHotKey wikiSearchKey;

        public static Dictionary<Mod, string> registeredMods;

		public QWiki()
		{
		}

        public override object Call(params object[] args)
        {
            try
            {
                var message = (string)args[0];
                switch (message)
                {
                    default:
                        Logger.Error($"[{DateTime.Now}] {Name} Call Error: Unknown Message: {message}");
                        break;
                    case "RegisterMod":
                        RegisterMod((Mod)args[1], (string)args[2]);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"[{DateTime.Now}] {Name} Call Error: {e.StackTrace}{e.Message}");
            }

            return null;
        }

        public override void Load()
        {
            wikiSearchKey = RegisterHotKey(WIKI_SEARCH_NAME, WIKI_SEARCH_KEY);
            registeredMods = new Dictionary<Mod, string>();
        }

        public override void PostSetupContent()
        {
            #region SpiritMod
            var spiritMod = ModLoader.GetMod("SpiritMod");
            if (spiritMod != null)
            {
                RegisterMod(spiritMod, "http://spiritmod.gamepedia.com/index.php?search=%s");
            }
            #endregion
        }

        public override void PostUpdateInput()
        {
            if (wikiSearchKey != null && wikiSearchKey.JustPressed)
            {
                SearchUtils.SearchWiki();
            }
        }

        public override void Unload()
        {
            wikiSearchKey = null;
            registeredMods = null;
        }

        private void RegisterMod(Mod mod, string searchUrl)
        {
            if (mod != null && !string.IsNullOrWhiteSpace(searchUrl))
            {
                registeredMods.Add(mod, searchUrl);
                Logger.Info($"[{DateTime.Now}] {Name}: Successfully registered {mod.Name}");
            }
        }
    }
}