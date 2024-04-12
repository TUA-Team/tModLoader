using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.Localization;

namespace Terraria.ModLoader;

public abstract class ModLiquid : ModBlockType
{
	public int WaterfallLength {
		get => LiquidRenderer.WATERFALL_LENGTH[Type];
		set {
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(value), "Waterfall Length must not be inferior to 0.");
			LiquidRenderer.WATERFALL_LENGTH[Type] = value;
		}
	}

	public float DefaultOpacity {
		get => LiquidRenderer.DEFAULT_OPACITY[Type];
		set {
			if (value < 0 || value > 1)
				throw new ArgumentOutOfRangeException(nameof(value), "Default opacity must be between 0 and 1.");
			LiquidRenderer.DEFAULT_OPACITY[Type] = value;
		}
	}

	public byte WaveMaskStrength {
		get => LiquidRenderer.WAVE_MASK_STRENGTH[Type + 1];
		set => LiquidRenderer.WAVE_MASK_STRENGTH[Type + 1] = value;
	}
	
	public byte ViscosityMask {
		get => LiquidRenderer.VISCOSITY_MASK[Type + 1];
		set => LiquidRenderer.VISCOSITY_MASK[Type + 1] = value;
	}

	public int Delay {
		get => LiquidLoader.liquidProperties[Type].FallDelay;
		set => LiquidLoader.liquidProperties[Type].FallDelay = value;
	}

	public bool HellEvaporation {
		get => LiquidLoader.liquidProperties[Type].HellEvaporation;
		set => LiquidLoader.liquidProperties[Type].HellEvaporation = value;
	}

	public override string LocalizationCategory => "Liquids";

	protected sealed override void Register()
	{
		Type = (ushort)LiquidLoader.ReserveLiquidID();

		ModTypeLookup<ModLiquid>.Register(this);
		LiquidLoader.liquids.Add(this);
	}

	public sealed override void SetupContent()
	{
		TextureAssets.Liquid[15 + Type - LiquidID.Count] = ModContent.Request<Texture2D>(Texture);

		SetStaticDefaults();

		LiquidID.Search.Add(FullName, Type);
	}

	public override void SetStaticDefaults()
	{
		WaterfallLength = 10;
		DefaultOpacity = 0.6f;
	}

	/// <summary>
	/// Adds an entry to the minimap for this liquid with the given color and display name. This should be called in SetStaticDefaults.
	/// <br/> For a typical liquid that has a map display name, use <see cref="ModBlockType.CreateMapEntryName"/> as the name parameter for a default key using the pattern "Mods.{ModName}.Liquids.{ContentName}.MapEntry".
	/// <br/> If a liquid will be using multiple map entries, it is suggested to use <c>this.GetLocalization("CustomMapEntryName")</c>. Modders can also re-use the display name localization of items, such as <c>ModContent.GetInstance&lt;ItemThatPlacesThisStyle&gt;().DisplayName</c>. 
	/// <br/><br/> Multiple map entries are suitable for liquids that need a different color or hover text for different liquid styles. Vanilla code uses this mostly only for chest and dresser tiles. Map entries will be given a corresponding map option value, counting from 0, according to the order in which they are added. Map option values don't necessarily correspond to tile styles.
	/// <br/> <see cref="ModBlockType.GetMapOption"/> will be used to choose which map entry is used for a given coordinate.
	/// <br/><br/> Vanilla map entries for most furniture liquids tend to be fairly generic, opting to use a single map entry to show "Table" for all styles of tables instead of the style-specific text such as "Wooden Table", "Honey Table", etc. To use these existing localizations, use the <see cref="Language.GetText(string)"/> method with the appropriate key, such as "MapObject.Chair", "MapObject.Door", "ItemName.WorkBench", etc. Consult the source code or ExampleMod to find the existing localization keys for common furniture types.
	/// </summary>
	public void AddMapEntry(Color color, LocalizedText name = null)
	{
		if (!MapLoader.initialized) {
			MapEntry entry = new MapEntry(color, name);
			if (!MapLoader.liquidEntries.Keys.Contains(Type)) {
				MapLoader.liquidEntries[Type] = new List<MapEntry>();
			}
			MapLoader.liquidEntries[Type].Add(entry);
		}
	}

	/// <summary>
	/// <inheritdoc cref="AddMapEntry(Color, LocalizedText)"/>
	/// <br/><br/> <b>Overload specific:</b> This overload has an additional <paramref name="nameFunc"/> parameter. This function will be used to dynamically adjust the hover text. The parameters for the function are the default display name, x-coordinate, and y-coordinate. This function is most typically used for chests and dressers to show the current chest name, if assigned, instead of the default chest name. <see href="https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Content/Tiles/Furniture/ExampleChest.cs">ExampleMod's ExampleChest</see> is one example of this functionality.
	/// </summary>
	public void AddMapEntry(Color color, LocalizedText name, Func<string, int, int, string> nameFunc)
	{
		if (!MapLoader.initialized) {
			MapEntry entry = new MapEntry(color, name, nameFunc);
			if (!MapLoader.liquidEntries.Keys.Contains(Type)) {
				MapLoader.liquidEntries[Type] = new List<MapEntry>();
			}
			MapLoader.liquidEntries[Type].Add(entry);
		}
	}

	/// <summary>
	/// Called at the very start of Liquid.Update before liquid merges or liquid movement.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="liquid"></param>
	/// <param name="thisTile"></param>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="up"></param>
	/// <param name="down"></param>
	public virtual void PreUpdate(int x, int y, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down) { }

	/// <summary>
	/// Called just after liquid merge checks, and just before liquid movement checks.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="liquid"></param>
	/// <param name="thisTile"></param>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="up"></param>
	/// <param name="down"></param>
	/// <returns></returns>
	public virtual bool Update(int x, int y, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down) => true;

	/// <summary>
	/// Called at the very end up Liquid.Update after merge and movement checks.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="liquid"></param>
	/// <param name="thisTile"></param>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="up"></param>
	/// <param name="down"></param>
	public virtual void PostUpdate(int x, int y, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down) { }

	/// <summary>
	/// Return false to prevent liquids from merging.  Return true by default.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="tile"></param>
	/// <param name="x2"></param>
	/// <param name="y2"></param>
	/// <param name="tile2"></param>
	/// <returns></returns>
	public virtual bool AllowMergeLiquids(int x, int y, Tile tile, int x2, int y2, Tile tile2) => true;

	/// <summary>
	/// Allows you to change the resulting tile created when liquids merge together.
	/// </summary>
	/// <param name="x">x tile coordinate of the tile being merged onto.</param>
	/// <param name="y">y tile coordinate of the tile being merged onto.</param>
	/// <param name="type">Liquid type at Main.tile[x, y]</param>
	/// <param name="liquidNearby">Use liquidNearby[LiquidID] to check if that liquid is touching the tile being merged onto.</param>
	/// <param name="liquidMergeTileType">The tile type being created by the merge at Main.tile[x, y].</param>
	/// <param name="liquidMergeType">The liquid type that is being used to determine the created tile type.  (Changing only this will do nothing since the created tile has already been determined.)</param>
	public virtual void GetLiquidMergeTypes(int x, int y, int otherLiquid, bool[] liquidNearby, ref int liquidMergeTileType, ref int liquidMergeType, LiquidMerge liquidMerge) { }

	public virtual void OnMerge(LiquidMerge liquidMerge, Dictionary<int, int> consumedLiquids) { }
	public virtual bool PreventMerge(LiquidMerge liquidMerge) => false;
	public virtual bool ShouldDeleteLiquid(LiquidMerge liquidMerge) => false;
}