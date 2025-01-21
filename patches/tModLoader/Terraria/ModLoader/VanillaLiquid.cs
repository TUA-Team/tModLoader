using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.VanillaWater;

namespace Terraria.ModLoader;
[Autoload(false)]
internal class VanillaWater : ModLiquid
{
	public override string Texture => "Terraria/Images/Liquid_0";

	internal override bool UseDefaultWaterStyle => false;

	public VanillaWater()
	{
		RegisterLiquidStyle(new DefaultWaterStyle(), init: true);
		DefaultOpacity = 0.2f;
	}

	public override void SetStaticDefaults()
	{
		TileIO.Liquids.vanillaTypes.Add(Type);
	}

	[Autoload(false)]
	internal class DefaultWaterStyle : ModLiquidStyle
	{
		public override string Texture => "Terraria/Images/Liquid_0";
		public override string BlockTexture { get; }
		public override string SlopeTexture => "Terraria/Images/LiquidSlope_0";
		public override string WaterfallTexture => "Terraria/Images/Waterfall_0";

		public override bool IsLiquidActive() => true;
	}
}

[Autoload(false)]
internal class VanillaLava : ModLiquid
{

	public override string Texture => "Terraria/Images/Liquid_1";

	internal override bool UseDefaultWaterStyle => false;

	public VanillaLava()
	{
		RegisterLiquidStyle(new DefaultLavaStyle(), init: true);
	}


	public override void SetStaticDefaults()
	{
		TileIO.Liquids.vanillaTypes.Add(Type);
	}

	[Autoload(false)]
	internal class DefaultLavaStyle : ModLiquidStyle
	{
		public override string Texture => "Terraria/Images/Liquid_1";
		public override string BlockTexture { get; }
		public override string SlopeTexture => "Terraria/LiquidSlope_1";
		public override string WaterfallTexture => "Terraria/Images/Waterfall_1";

		public override bool IsLiquidActive() => true;
	}
}

[Autoload(false)]
internal class VanillaHoney : ModLiquid
{
	public override string Texture => "Terraria/Images/Liquid_11";

	internal override bool UseDefaultWaterStyle => false;

	public VanillaHoney()
	{
		RegisterLiquidStyle(new DefaultHoneyStyle(), init: true);
	}

	public override void SetStaticDefaults()
	{
		TileIO.Liquids.vanillaTypes.Add(Type);
	}

	[Autoload(false)]
	internal class DefaultHoneyStyle : ModLiquidStyle
	{
		public override string Texture => "Terraria/Images/Liquid_11";
		public override string BlockTexture { get; }
		public override string SlopeTexture => "Terraria/Images/LiquidSlope_11";
		public override string WaterfallTexture => "Terraria/Images/Waterfall_14";

		public override bool IsLiquidActive() => true;
	}
}

[Autoload(false)]
internal class VanillaShimmer : ModLiquid
{
	public override string Texture => "Terraria/Images/Liquid_14";

	internal override bool UseDefaultWaterStyle => false;

	public VanillaShimmer()
	{
		RegisterLiquidStyle(new DefaultShimmerStyle(), init: true);
	}

	public override void SetStaticDefaults()
	{
		TileIO.Liquids.vanillaTypes.Add(Type);
	}

	[Autoload(false)]
	internal class DefaultShimmerStyle : ModLiquidStyle
	{
		public override string Texture => "Terraria/Images/Liquid_14";
		public override string BlockTexture { get; }
		public override string SlopeTexture => "Terraria/Images/LiquidSlope_14";
		public override string WaterfallTexture => "Terraria/Images/Waterfall_14";

		public override bool IsLiquidActive() => true;
	}
}