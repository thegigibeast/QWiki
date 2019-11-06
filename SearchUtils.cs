using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            { GameCulture.Chinese, "https://terraria-zh.gamepedia.com/index.php?search=%s" },
            { GameCulture.English, "https://terraria.gamepedia.com/index.php?search=%s" },
            { GameCulture.French, "https://terraria-fr.gamepedia.com/index.php?search=%s" },
            { GameCulture.German, "https://terraria-de.gamepedia.com/index.php?search=%s" },
            //{ GameCulture.Italian, "https://terraria-zh.gamepedia.com/index.php?search=%s" },
            { GameCulture.Polish, "https://terraria-pl.gamepedia.com/index.php?search=%s" },
            { GameCulture.Portuguese, "https://terraria-pt.gamepedia.com/index.php?search=%s" },
            { GameCulture.Russian, "https://terraria-ru.gamepedia.com/index.php?search=%s" },
            //{ GameCulture.Spanish, "https://terraria.fandom.com/es/wiki/Especial:Buscar?query=%s" }
        };

        public static HashSet<Item> defaultTileItems = new HashSet<Item>();
        public static HashSet<Item> defaultWallItems = new HashSet<Item>();

        public static Dictionary<Mod, HashSet<Item>> modTileItems = new Dictionary<Mod, HashSet<Item>>();
        public static Dictionary<Mod, HashSet<Item>> modWallItems = new Dictionary<Mod, HashSet<Item>>();

        /// <summary>
        /// Begins searching for something under the cursor.
        /// </summary>
        public static void SearchWiki()
        {
            if (ItemHover()) return;
            if (NPCHover()) return;
            if (TileHover()) return;
        }

        /// <summary>
        /// Tries to search for an item under the cursor.
        /// </summary>
        /// <returns>true if an item has been searched, false otherwise</returns>
        private static bool ItemHover()
        {
            var item = GetHoveringItem();
            if (item != null)
            {
                var itemName = string.Empty;
                if (IsModItem(item, out var mod))
                {
                    if (QWiki.registeredMods.ContainsKey(mod))
                    {
                        DoSearch(QWiki.registeredMods[mod], ref itemName, () =>
                        {
                            itemName = Regex.Replace(item.Name, @"\[.+\]", "").Trim();
                        });
                    }
                    else
                    {
                        ShowErrorMessage("item", item.Name, mod);
                    }
                }
                else
                {
                    DoSearch(TERRARIA_WIKI, ref itemName, () =>
                    {
                        itemName = item.Name;
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
                if (IsModNPC(npc, out var mod))
                {
                    if (QWiki.registeredMods.ContainsKey(mod))
                    {
                        DoSearch(QWiki.registeredMods[mod], ref npcName, () =>
                        {
                            npcName = npc.TypeName;
                        });
                    }
                    else
                    {
                        ShowErrorMessage("NPC", npc.TypeName, mod);
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
        /// Tries to search for a tile under the cursor.
        /// </summary>
        /// <returns>true if a tile has been searched, false otherwise</returns>
        private static bool TileHover()
        {
            Item item = null;
            var active = false;

            var tile = GetHoveringTile();
            if (tile != null)
            {
                active = tile.active();

                // Search for a tile first
                if (active)
                {
                    item = defaultTileItems.FirstOrDefault(x => x.createTile == tile.type);

                    if (item == null)
                    {
                        foreach (var set in modTileItems.Values)
                        {
                            item = set.FirstOrDefault(m => m.createTile == tile.type);
                            if (item != null) break;
                        }
                    }
                }
                // Search for a liquid second
                else if (tile.liquid > 0)
                {
                    LiquidHover(tile);
                    return true;
                }
                // Search for a wall third
                else if (tile.wall > 0)
                {
                    item = defaultWallItems.FirstOrDefault(i => i.createWall == tile.wall);

                    if (item == null)
                    {
                        foreach (var set in modWallItems.Values)
                        {
                            item = set.FirstOrDefault(x => x.createWall == tile.wall);
                            if (item != null) break;
                        }
                    }
                }

                if (item != null)
                {
                    var itemName = string.Empty;
                    if (IsModItem(item, out var mod))
                    {
                        if (QWiki.registeredMods.ContainsKey(mod))
                        {
                            DoSearch(QWiki.registeredMods[mod], ref itemName, () =>
                            {
                                itemName = item.Name;
                            });
                        }
                        else
                        {
                            ShowErrorMessage("item", item.Name, item.modItem.mod);
                        }
                    }
                    else
                    {
                        DoSearch(TERRARIA_WIKI, ref itemName, () =>
                        {
                            itemName = item.Name;
                        });
                    }
                }
                else if (active || (!active && tile.wall > 0))
                {
                    ShowErrorMessage(tile);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to search for a liquid under the cursor.
        /// </summary>
        /// <param name="tile">The liquid tile</param>
        private static void LiquidHover(Tile tile)
        {
            var liquid = tile.liquidType();
            var tileName = string.Empty;

            DoSearch(TERRARIA_WIKI, ref tileName, () =>
            {
                switch (liquid)
                {
                    case Tile.Liquid_Water:
                        tileName = Language.GetTextValue("Mods.QWiki.LiquidName.Water");
                        break;
                    case Tile.Liquid_Lava:
                        tileName = Language.GetTextValue("Mods.QWiki.LiquidName.Lava");
                        break;
                    case Tile.Liquid_Honey:
                        tileName = Language.GetTextValue("Mods.QWiki.LiquidName.Honey");
                        break;
                }
            });
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
        /// Gets the hovered item.
        /// </summary>
        /// <returns>the item if one is being hovered, null otherwise</returns>
        private static Item GetHoveringItem()
        {
            // Item in inventory
            if (!string.IsNullOrWhiteSpace(Main.HoverItem.Name))
            {
                return Main.HoverItem;
            }

            // Item in world
            for (var i = 0; i < Main.item.Length; i++)
            {
                if (Main.item[i].Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y))
                {
                    return Main.item[i];
                }
            }

            return null;
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
        /// Gets the hovered tile.
        /// </summary>
        /// <returns>the tile if one is being hovered, null otherwise</returns>
        private static Tile GetHoveringTile()
        {
            int tileTargetX = (int)((Main.mouseX + Main.screenPosition.X) / 16f);
            int tileTargetY = (int)((Main.mouseY + Main.screenPosition.Y) / 16f);

            if (Main.LocalPlayer.gravDir == -1f)
            {
                tileTargetY = (int)((Main.screenPosition.Y + Main.screenHeight - Main.mouseY) / 16f);
            }

            var hoverTile = new Vector2(tileTargetX, tileTargetY);

            if ((hoverTile.X > 0) && (hoverTile.Y > 0) && (hoverTile.X < Main.tile.GetLength(0)) &&
               (hoverTile.Y < Main.tile.GetLength(1)))
            {
                // get the tile under the cursor
                return Main.tile[(int)hoverTile.X, (int)hoverTile.Y];
            }

            return null;
        }

        /// <summary>
        /// Display an error message if the mod has not been registered.
        /// </summary>
        /// <param name="type">The type of term that was being searched</param>
        /// <param name="term">The term that was being searched</param>
        /// <param name="mod">The mod that is not registered</param>
        private static void ShowErrorMessage(string type, string term, Mod mod)
        {
            if (GetInstance<ClientConfig>().ShowErrorMessages)
            {
                Main.NewText($"{GetInstance<QWiki>().DisplayName}: Cannot search for {term}, because it is a modded {type} from {mod.DisplayName}, which has not been registered.");
            }
        }

        private static void ShowErrorMessage(Tile tile)
        {
            if (GetInstance<ClientConfig>().ShowErrorMessages)
            {
                Main.NewText($"{GetInstance<QWiki>().DisplayName}: Cannot search for this tile (ID: {(tile.active() ? tile.type : tile.wall)}) because no item can place it.");
            }
        }

        public static bool IsModItem(Item item, out Mod mod)
        {
            mod = null;

            if (item.modItem != null)
            {
                mod = item.modItem.mod;
                return true;
            }

            return false;
        }

        private static bool IsModNPC(NPC npc, out Mod mod)
        {
            mod = null;

            if (npc.modNPC != null)
            {
                mod = npc.modNPC.mod;
                return true;
            }

            return false;
        }
    }
}
