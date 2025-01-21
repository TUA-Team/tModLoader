using ExampleMod.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Liquids
{
	internal class ExampleLavaStyle : ModLiquidStyle
	{
		public override int Liquid => LiquidID.Lava;

		public override bool IsLiquidActive() {
			return ModContent.GetInstance<ExampleBiomeTileCount>().exampleBlockCount >= 40;
		}
	}
}
