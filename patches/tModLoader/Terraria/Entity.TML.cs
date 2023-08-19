using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Terraria;
partial class Entity
{
	public bool[] LiquidsWet { get; } = new bool[LiquidLoader.LiquidCount];
}
