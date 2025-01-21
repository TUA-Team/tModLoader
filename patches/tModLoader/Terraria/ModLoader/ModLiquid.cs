using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.Localization;

namespace Terraria.ModLoader;

public abstract class ModLiquid : ModBlockType
{
	internal class DefaultLiquidStyle : ModLiquidStyle
	{
		public override bool IsLiquidActive() => true;

		internal DefaultLiquidStyle(string defaultTextureName)
		{
			DefaultTextureName = defaultTextureName;
		}

	}


	internal static VanillaWater Water = new VanillaWater();
	internal static VanillaLava Lava = new VanillaLava();
	internal static VanillaHoney Honey = new VanillaHoney();
	internal static VanillaShimmer Shimmer = new VanillaShimmer();


	internal List<LiquidLoader.LiquidTextures> Textures = new List<LiquidLoader.LiquidTextures>();
	internal int LiquidStyleCount = 0;
	internal ModLiquidStyle[] LiquidStyle = new ModLiquidStyle[0];

	internal virtual bool UseDefaultWaterStyle => true;

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

	public string WaterfallTexture {
		set => LiquidLoader._LiquidTextures[Type][0].WaterfallTexture = ModContent.Request<Texture2D>(value);
	}

	public override string LocalizationCategory => "Liquids";

	// public abstract string LiquidFallTexture { get; }
	// public abstract string LiquidCornerTexture { get; }

	protected sealed override void Register()
	{
		Type = (ushort)LiquidLoader.ReserveLiquidID();

		ModTypeLookup<ModLiquid>.Register(this);
		LiquidLoader.liquids.Add(this);

		ModLiquidStyle liquidStyle = new DefaultLiquidStyle(this.Texture);

		Array.Resize(ref Main.LiquidAlpha, Type+1);
		Main.LiquidAlpha[Type] = new float[1];

		if (UseDefaultWaterStyle)
			RegisterLiquidStyle(liquidStyle, init: false);
	}

	internal int RegisterLiquidStyle(ModLiquidStyle liquidStyle, bool init = false)
	{
		Array.Resize(ref LiquidStyle, ++LiquidStyleCount);
		if (!init)
			Array.Resize(ref Main.LiquidAlpha[liquidStyle.Liquid], LiquidStyleCount);
		LiquidStyle[LiquidStyleCount - 1] = liquidStyle;
		return LiquidStyleCount - 1;
	}

	public sealed override void SetupContent()
	{
		TextureAssets.Liquid[15 + Type - LiquidID.Count] = ModContent.Request<Texture2D>(Texture);
		LiquidRenderer.Instance._moddedLiquidTextures[Type - LiquidID.Count] = ModContent.Request<Texture2D>(Texture);
		LiquidLoader._LiquidTextures[Type] = new List<LiquidLoader.LiquidTextures>();
		LiquidLoader._LiquidTextures[Type].Add(new LiquidLoader.LiquidTextures());
		LiquidLoader._LiquidTextures[Type][0].MainTexture = ModContent.Request<Texture2D>(Texture);
		// Setting default waterfall texture to water by default
		WaterfallTexture = Texture + "_waterfall";

		SetStaticDefaults();

		LiquidID.Search.Add(FullName, Type);
	}

	public override void SetStaticDefaults()
	{
		WaterfallLength = 10;
		DefaultOpacity = 0.6f;
	}

	public virtual void OnEntityCollision(Entity entity, CollisionType collisionType)
	{

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


	public virtual void PreUpdate(int x, int y)
	{
		
	}

	public virtual bool Update(Liquid liquid, int x, int y, Tile left, Tile right, Tile up, Tile down)
	{
		return true;
	}

	public virtual void Merge(int otherLiquid, bool[] liquidNearby, ref int liquidMergeTileType, ref int liquidMergeType)
	{

	}
}