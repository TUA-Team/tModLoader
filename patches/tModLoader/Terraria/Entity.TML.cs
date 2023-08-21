using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terraria;
partial class Entity
{
	/// <summary>
	/// An array containing whether this entity is inside a specific liquid.
	/// </summary>
	public bool[] LiquidsWet { get; } = new bool[LiquidLoader.LiquidCount];

	/// <summary>
	/// The id of the liquid this entity is currently inside of, or -1 if it is not inside any liquid.
	/// </summary>
	public int LiquidWetId {
		get {
			int id = Array.IndexOf(LiquidsWet, true);
			if (id == -1 && wet)
				return LiquidID.Water;
			return id;
		}
	}
}
