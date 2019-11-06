# QWiki
Based on DrEinsteinium's tWiki mod and on abluescarab's WikiSearch mod, QWiki is a tModLoader mod that allows you to search the [Terraria Wiki](http://terraria.gamepedia.com/) for whatever is under the mouse cursor.

Configuration options available.

Press Q to search the wiki while hovering over any item, NPC, or tile.

Now has mod support! Included mods: Antiaris, Calamity, Dragon Ball Terraria, Elements Awoken, Exodus, GRealm, Enigma, Shadows of Abaddon, Spirit, Split and Thorium.

## Registering Your Mod
To register your mod, you must use the `Call()` method. Place the following code into your mod's main file (e.g. if your mod is MyMod, place this code into `MyMod.cs`; in other words, into the file that inherits from `Terraria.ModLoader.Mod`).

```csharp
public override void PostSetupContent() {
    Mod qWiki = ModLoader.GetMod("QWiki");

    if(qWiki != null) {
        qWiki.Call("RegisterMod", this, "http://mymod.gamepedia.com/index.php?search=%s");
    }
}
```

To register another language for your mod (if your wiki is available in different languages), make sure your mod is registered first, then add this line right under the initial call:

```csharp
qWiki.Call("RegisterGameCulture", this, GameCulture.French, "http://mymod-fr.gamepedia.com/index.php?search=%s");
```

Note that the Gamepedia link can be any wiki link with the search term replaced. Just go to your wiki, search for something that isn't a real page (like "testtest"), and replace the search term with "%s".

## Known Issues
* Some naturally-placed tiles cannot be searched because no item can place them
* Chests open the chest page instead of the individual page (because of the structure of the wiki)

## Credits
* [DrEinsteinium](https://forums.terraria.org/index.php?members/dreinsteinium.48502/) for tWiki mod
* [abluescarab](https://forums.terraria.org/index.php?members/abluescarab.63946/) for WikiSearch mod
