using Steamworks;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace QWiki
{
    public static class SearchUtils
    {
        public const string TERRARIA_WIKI = "http://terraria.gamepedia.com/index.php?search=%s";

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
                            DoSearch(QWiki.registeredMods[mod], itemName);
                        }
                        else
                        {
                            ShowModMessage("item", itemName, mod.DisplayName);
                        }
                    }
                    else
                    {
                        DoSearch(TERRARIA_WIKI, itemName);
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

        private static void ShowModMessage(string type, string name, string mod)
        {
            Main.NewText($"Cannot search for {name}, because it is a modded {type} from {mod}.");
        }
    }
}
