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
        public static readonly GameCulture DEFAULT_GAME_CULTURE = GameCulture.English;

        private static readonly Dictionary<GameCulture, string> TERRARIA_WIKI = new Dictionary<GameCulture, string>()
        {
            { GameCulture.English, "http://terraria.gamepedia.com/index.php?search=%s" },
            { GameCulture.French, "http://terraria-fr.gamepedia.com/index.php?search=%s" }
        };

        /// <summary>
        /// Begins searching for something under the cursor.
        /// </summary>
        public static void SearchWiki()
        {
            if (ItemHover()) return;
            if (NPCHover()) return;
        }

        /// <summary>
        /// Tries to search for an item under the cursor.
        /// </summary>
        /// <returns>true if an item has been searched, false otherwise</returns>
        private static bool ItemHover()
        {
            if (Main.playerInventory && !string.IsNullOrWhiteSpace(Main.HoverItem.Name))
            {
                var itemName = string.Empty;
                if (Main.HoverItem.modItem != null)
                {
                    var mod = Main.HoverItem.modItem.mod;
                    if (QWiki.registeredMods.ContainsKey(mod))
                    {
                        DoSearch(QWiki.registeredMods[mod], ref itemName, () =>
                        {
                            itemName = Regex.Replace(Main.HoverItem.Name, @"\[.+\]", "").Trim();
                        });
                    }
                    else
                    {
                        ShowModMessage("item", itemName, mod);
                    }
                }
                else
                {
                    DoSearch(TERRARIA_WIKI, ref itemName, () =>
                    {
                        itemName = Main.HoverItem.Name;
                    });
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to search for a NPC under the cursor.
        /// </summary>
        /// <returns>true if a NPC has been searched, false otherwise</returns>
        private static bool NPCHover()
        {
            var npc = GetHoveringNPC();
            if (npc != null)
            {
                var npcName = string.Empty;
                if (npc.modNPC != null)
                {
                    var mod = npc.modNPC.mod;
                    if (QWiki.registeredMods.ContainsKey(mod))
                    {
                        DoSearch(QWiki.registeredMods[mod], ref npcName, () =>
                        {
                            npcName = npc.TypeName;
                        });
                    }
                    else
                    {
                        ShowModMessage("NPC", npc.TypeName, mod);
                    }
                }
                else
                {
                    DoSearch(TERRARIA_WIKI, ref npcName, () =>
                    {
                        npcName = npc.TypeName;
                    });
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Searches for the specified term.
        /// </summary>
        /// <param name="wiki">The wiki to search from</param>
        /// <param name="term">The term to search for</param>
        /// <param name="getTerm">The method to get the term</param>
        public static void DoSearch(Dictionary<GameCulture, string> wiki, ref string term, Action getTerm)
        {
            var searchUrl = string.Empty;

            if (GetInstance<ClientConfig>().UseActiveGameCulture && wiki.ContainsKey(LanguageManager.Instance.ActiveCulture))
            {
                getTerm();
                searchUrl = wiki[LanguageManager.Instance.ActiveCulture];
            }
            else
            {
                var oldGameCulture = LanguageManager.Instance.ActiveCulture;
                LanguageManager.Instance.SetLanguage(DEFAULT_GAME_CULTURE);
                getTerm();
                LanguageManager.Instance.SetLanguage(oldGameCulture);

                searchUrl = wiki[DEFAULT_GAME_CULTURE];
            }

            if (GetInstance<ClientConfig>().UseSteamOverlay && SteamAPI.IsSteamRunning() && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage(searchUrl.Replace("%s", term));
            }
            else
            {
                Process.Start(searchUrl.Replace("%s", term));
            }
        }

        /// <summary>
        /// Gets the hovered NPC.
        /// </summary>
        /// <returns>the NPC if one is being hovered, null otherwise</returns>
        private static NPC GetHoveringNPC()
        {
            for (var i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y))
                {
                    return Main.npc[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Display an error message if the mod has not been registered.
        /// </summary>
        /// <param name="type">The type of term that was being searched</param>
        /// <param name="term">The term that was being searched</param>
        /// <param name="mod">The mod that is not registered</param>
        private static void ShowModMessage(string type, string term, Mod mod)
        {
            Main.NewText($"Cannot search for {term}, because it is a modded {type} from {mod.DisplayName}, which has not been registered.");
        }
    }
}
