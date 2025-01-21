using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Terraria.ModLoader;

[Autoload(Side = ModSide.Client)]
internal class LiquidStyleLoader : Loader<ModLiquidStyle>
{
	internal int RegisterLiquidStyle(int Liquid, ModLiquidStyle liquidStyle)
	{
		return LiquidLoader.liquids[Liquid].RegisterLiquidStyle(liquidStyle);
	}

	public void IsLiquidActive(int liquid)
	{
		foreach (ModLiquidStyle item in LiquidLoader.liquids[liquid].LiquidStyle) {
			bool flag = item.IsLiquidActive();
			if (flag) {
				Main.LiquidStyle[liquid] = item.Slot;
			}
		}
	}

	public void ChooseStyle(int liquid, out int style, out SceneEffectPriority priority)
	{
		int tst = Main.LocalPlayer.CurrentSceneEffect.waterStyle.value;
		style = -1;
		priority = SceneEffectPriority.None;

		if (tst >= VanillaCount) {
			style = tst;
			priority = Main.LocalPlayer.CurrentSceneEffect.waterStyle.priority;
		}
	}

	public void UpdateLiquidAlphas(int Liquid)
	{
		if (Main.waterStyle >= VanillaCount) {
			for (int k = 0; k < VanillaCount; k++) {
				if (k == 1 || k == 11) {
					continue;
				}
				Main.liquidAlpha[k] -= 0.2f;
				if (Main.liquidAlpha[k] < 0f) {
					Main.liquidAlpha[k] = 0f;
				}
			}
		}

		foreach (ModLiquidStyle liquidStyle in LiquidLoader.liquids[Liquid].LiquidStyle) {
			int type = liquidStyle.Slot;
			if (Main.LiquidStyle[Liquid] == type) {
				Main.LiquidAlpha[Liquid][type] += 0.2f;
				if (Main.LiquidAlpha[Liquid][type] > 1f) {
					Main.LiquidAlpha[Liquid][type] = 1f;
				}
			}
			else {
				Main.LiquidAlpha[Liquid][type] -= 0.2f;
				if (Main.LiquidAlpha[Liquid][type] < 0f) {
					Main.LiquidAlpha[Liquid][type] = 0f;
				}
			}
		}
	}
}
