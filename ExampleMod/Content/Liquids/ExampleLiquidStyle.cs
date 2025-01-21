using ExampleMod.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace ExampleMod.Content.Liquids
{
	internal class ExampleLiquidStyle : ModLiquidStyle
	{
		public override int Liquid => ModContent.LiquidType<ExampleLiquid>();

		public override bool IsLiquidActive() {
			return ModContent.GetInstance<ExampleBiomeTileCount>().exampleBlockCount >= 40;
		}
	}
}
