using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace QWiki
{
    public class QWiki : Mod
	{
        private const string WIKI_SEARCH_NAME = "Search Wiki";
        private const string WIKI_SEARCH_KEY = "Q";

        private static ModHotKey wikiSearchKey;

        public static Dictionary<Mod, Dictionary<GameCulture, string>> registeredMods;

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
                        RegisterMod((Mod)args[1], (GameCulture)args[2], (string)args[3]);
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
            registeredMods = new Dictionary<Mod, Dictionary<GameCulture, string>>();
        }

        public override void PostSetupContent()
        {
            #region SpiritMod
            var spiritMod = ModLoader.GetMod("SpiritMod");
            if (spiritMod != null)
            {
                RegisterMod(spiritMod, GameCulture.English, "http://spiritmod.gamepedia.com/index.php?search=%s");
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

        private void RegisterMod(Mod mod, GameCulture gameCulture, string searchUrl)
        {
            // Check to see if the mod parameter has a value
            if (mod != null && !string.IsNullOrWhiteSpace(searchUrl))
            {
                // Check to see if the mod has already been registered
                if (!registeredMods.ContainsKey(mod))
                {
                    registeredMods.Add(mod, new Dictionary<GameCulture, string>());
                }

                registeredMods[mod][gameCulture] = searchUrl;

                // Check to see if we don't have a default culture entry for this mod
                if (!registeredMods[mod].ContainsKey(SearchUtils.DEFAULT_GAME_CULTURE))
                {
                    registeredMods[mod][SearchUtils.DEFAULT_GAME_CULTURE] = searchUrl;
                }

                Logger.Info($"[{DateTime.Now}] {Name}: Successfully registered {mod.Name}");
            }
        }
    }
}