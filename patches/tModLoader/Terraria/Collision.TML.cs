using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Terraria;

public partial class Collision
{
	internal static bool[] _liquids = new bool[LiquidLoader.LiquidCount];
	public static bool[] Liquids => _liquids;
}
