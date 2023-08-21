using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Content.Liquids
{
	public class ExampleLiquid : ModLiquid
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			WaterfallLength = 15;
			DefaultOpacity = 1;
		}

		public override void OnItemCollide(Item item) {
			
		}

		public override bool HasWetItemPhysics(Item item) {
			return true;
		}

		public override void WetItemPhysics(Item item, Vector2 velocity, ref float gravity, ref float maxFallSpeed, ref Vector2 wetVelocity) {
			wetVelocity = velocity;
			gravity = -1f;
			maxFallSpeed = 5f;
		}
	}
}
