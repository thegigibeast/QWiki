using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace QWiki
{
    public class QWikiItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            base.SetDefaults(item);

            // Add the item to the list of tiles
            if (item.createTile > -1)
            {
                var modItem = item.modItem;
                if (modItem != null)
                {
                    var mod = modItem.mod;
                    if (!SearchUtils.modTileItems.ContainsKey(mod))
                    {
                        SearchUtils.modTileItems[mod] = new HashSet<Item>();
                    }

                    SearchUtils.modTileItems[mod].Add(item);
                }
                else
                {
                    SearchUtils.defaultTileItems.Add(item);
                }
            }

            // Add the item to the list of walls
            if (item.createWall > 0)
            {
                var modItem = item.modItem;
                if (modItem != null)
                {
                    var mod = modItem.mod;
                    if (!SearchUtils.modWallItems.ContainsKey(mod))
                    {
                        SearchUtils.modWallItems[mod] = new HashSet<Item>();
                    }

                    SearchUtils.modWallItems[mod].Add(item);
                }
                else
                {
                    SearchUtils.defaultWallItems.Add(item);
                }
            }
        }
    }
}
