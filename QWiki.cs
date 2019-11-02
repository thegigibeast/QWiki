using System;
using System.Collections.Generic;
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
                    case "RegisterGameCulture":
                        RegisterGameCulture((Mod)args[1], (GameCulture)args[2], (string)args[3]);
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
            #region Antiaris
            var antiaris = ModLoader.GetMod("Antiaris");
            if (antiaris != null)
            {
                RegisterMod(antiaris, "https://antiaris.gamepedia.com/index.php?search=%s");
            }
            #endregion
            #region CalamityMod
            var calamityMod = ModLoader.GetMod("CalamityMod");
            if (calamityMod != null)
            {
                RegisterMod(calamityMod, "https://calamitymod.gamepedia.com/index.php?search=%s");
            }
            #endregion
            #region DBZMOD
            var dbzmod = ModLoader.GetMod("DBZMOD");
            if (dbzmod != null)
            {
                RegisterMod(dbzmod, "https://dbtmod.gamepedia.com/index.php?search=%s");
            }
            #endregion
            #region ElementsAwoken
            var elementsAwoken = ModLoader.GetMod("ElementsAwoken");
            if (elementsAwoken != null)
            {
                RegisterMod(elementsAwoken, "https://elementsawoken.gamepedia.com/index.php?search=%s");
            }
            #endregion
            #region Laugicality
            var laugicality = ModLoader.GetMod("Laugicality");
            if (laugicality != null)
            {
                RegisterMod(laugicality, "https://enigmamod.gamepedia.com/index.php?search=%s");
            }
            #endregion
            #region SacredTools
            var sacredTools = ModLoader.GetMod("SacredTools");
            if (sacredTools != null)
            {
                RegisterMod(sacredTools, "https://shadowsofabaddon.gamepedia.com/index.php?search=%s");
            }
            #endregion
            #region SpiritMod
            var spiritMod = ModLoader.GetMod("SpiritMod");
            if (spiritMod != null)
            {
                RegisterMod(spiritMod, "https://spiritmod.gamepedia.com/index.php?search=%s");
            }
            #endregion
            #region ThoriumMod
            var thoriumMod = ModLoader.GetMod("ThoriumMod");
            if (thoriumMod != null)
            {
                RegisterMod(thoriumMod, "https://thoriummod.gamepedia.com/index.php?search=%s");
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

        /// <summary>
        /// Register a mod with default GameCulture.
        /// </summary>
        private void RegisterMod(Mod mod, string searchUrl)
        {
            // Create the entry if the mod has not already been registered
            if (!registeredMods.ContainsKey(mod))
            {
                registeredMods.Add(mod, new Dictionary<GameCulture, string>());
            }

            // Set default GameCulture search url
            registeredMods[mod][SearchUtils.DEFAULT_GAME_CULTURE] = searchUrl;

            Logger.Info($"[{DateTime.Now}] {Name}: Successfully registered {mod.DisplayName} with default GameCulture ({SearchUtils.DEFAULT_GAME_CULTURE.Name})");
        }

        /// <summary>
        /// Register a search URL for a specific GameCulture.
        /// </summary>
        private void RegisterGameCulture(Mod mod, GameCulture gameCulture, string searchUrl)
        {
            // Allow creating an entry only if the mod has been registered with the default GameCulture first
            if (!registeredMods.ContainsKey(mod))
            {
                Logger.Error($"[{DateTime.Now}] {Name} Call Error: {mod.DisplayName} has not been registered with default game culture yet, please call the RegisterMod message first.");
                return;
            }

            // Register an entry for this GameCulture
            registeredMods[mod][gameCulture] = searchUrl;

            Logger.Info($"[{DateTime.Now}] {Name}: Successfully registered {mod.DisplayName} with specific GameCulture ({gameCulture.Name})");
        }
    }
}