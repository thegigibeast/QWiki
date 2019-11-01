using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace QWiki
{
    public static class SearchUtils
    {
        public static GameCulture DEFAULT_GAME_CULTURE = GameCulture.English;
        public static Dictionary<GameCulture, string> TERRARIA_WIKI = new Dictionary<GameCulture, string>()
        {
            { GameCulture.Chinese, "http://terraria-zh.gamepedia.com/index.php?search=%s" },
            { GameCulture.English, "http://terraria.gamepedia.com/index.php?search=%s" },
            { GameCulture.French, "http://terraria-fr.gamepedia.com/index.php?search=%s" }
        };

        /// <summary>
        /// Begin searching for something under the mouse cursor.
        /// </summary>
        public static void SearchWiki()
        {
            if (ItemHover()) return;
        }

        /// <summary>
        /// Checks for an item under the mouse cursor.
        /// </summary>
        /// <returns>whether an item is under the mouse cursor</returns>
        private static bool ItemHover()
        {
            // Checks if the inventory is open
            if (Main.playerInventory)
            {
                // Checks if the mouse is over an item
                if (!string.IsNullOrWhiteSpace(Main.HoverItem.Name))
                {
                    var itemName = Main.HoverItem.Name;

                    // Checks if the hovered item is a mod item
                    if (Main.HoverItem.modItem != null)
                    {
                        var mod = Main.HoverItem.modItem.mod;
                        itemName = Regex.Replace(itemName, @"\[.+\]", "").Trim();

                        // Checks if the mod is registered
                        if (QWiki.registeredMods.ContainsKey(mod))
                        {
                            // Check to see if we can use the active culture and if the mod has a search URL registered for this culture
                            if (GetInstance<ClientConfig>().UseActiveGameCulture && QWiki.registeredMods[mod].ContainsKey(LanguageManager.Instance.ActiveCulture))
                            {
                                DoSearch(QWiki.registeredMods[mod][LanguageManager.Instance.ActiveCulture], itemName);
                            }
                            else
                            {
                                ActionWithDefaultGameCulture(() =>
                                {
                                    // We get the item name again to get it in the default game culture
                                    itemName = Main.HoverItem.Name;
                                    itemName = Regex.Replace(itemName, @"\[.+\]", "").Trim();
                                    DoSearch(QWiki.registeredMods[mod][DEFAULT_GAME_CULTURE], itemName);
                                });
                            }
                        }
                        else
                        {
                            ShowModMessage("item", itemName, mod);
                        }
                    }
                    else
                    {
                        // Check if we can use the active culture
                        if (GetInstance<ClientConfig>().UseActiveGameCulture)
                        {
                            DoSearch(TERRARIA_WIKI[LanguageManager.Instance.ActiveCulture], itemName);
                        }
                        else
                        {
                            ActionWithDefaultGameCulture(() =>
                            {
                                // We get the item name again to get it in the default game culture
                                itemName = Main.HoverItem.Name;
                                DoSearch(TERRARIA_WIKI[DEFAULT_GAME_CULTURE], itemName);
                            });
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private static void DoSearch(string url, string term)
        {
            // Checks if Steam overlay option is enabled, if Steam is running and if Steam overlay is enabled
            if (GetInstance<ClientConfig>().UseSteamOverlay && SteamAPI.IsSteamRunning() && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage(url.Replace("%s", term));
            }
            else
            {
                Process.Start(url.Replace("%s", term));
            }
        }

        private static void ShowModMessage(string type, string name, Mod mod)
        {
            Main.NewText($"Cannot search for {name}, because it is a modded {type} from {mod.DisplayName}.");
        }

        /// <summary>
        /// Temporarily changes the game culture to the default game culture to execute an action.
        /// </summary>
        private static void ActionWithDefaultGameCulture(Action action)
        {
            var oldGameCulture = LanguageManager.Instance.ActiveCulture;
            LanguageManager.Instance.SetLanguage(DEFAULT_GAME_CULTURE);
            action();
            LanguageManager.Instance.SetLanguage(oldGameCulture);
        }
    }
}
