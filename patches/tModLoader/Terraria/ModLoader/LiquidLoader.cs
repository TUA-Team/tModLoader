using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ModLoader.Core;

namespace Terraria.ModLoader;
/// <summary>
/// This serves as the central class from which liquid-related functions are carried out.
/// </summary>
public static class LiquidLoader
{
	internal struct LiquidProperties
	{
		public int FallDelay;
		public bool NoStandardUpdate;
		public bool HellEvaporation;
	}

	private static int nextLiquid = LiquidID.Count;
	private static bool loaded = false;

	internal static readonly IList<ModLiquid> liquids = new List<ModLiquid>();

	internal static LiquidProperties[] liquidProperties;

	public static int LiquidCount => nextLiquid;

	public static ModLiquid GetLiquid(int type)
	{
		return type >= LiquidID.Count && type < LiquidCount ? liquids[type - LiquidID.Count] : null;
	}

	internal static int ReserveLiquidID()
	{
		if (ModNet.AllowVanillaClients) 
			throw new Exception("Adding liquid breaks vanilla client compatibility");
		int reserveId = nextLiquid;
		nextLiquid++;
		return reserveId;
	}

	internal static readonly IList<GlobalLiquid> globalLiquids = new List<GlobalLiquid>();
	internal static void ResizeArrays(bool unloading = false)
	{
		//Texture
		Array.Resize(ref TextureAssets.Liquid, 15 + nextLiquid - LiquidID.Count);

		//Sets
		LoaderUtils.ResetStaticMembers(typeof(LiquidID));

		//Etc
		Array.Resize(ref LiquidRenderer.WATERFALL_LENGTH, nextLiquid);
		Array.Resize(ref LiquidRenderer.DEFAULT_OPACITY, nextLiquid);
		Array.Resize(ref LiquidRenderer.WAVE_MASK_STRENGTH, nextLiquid + 1);
		Array.Resize(ref LiquidRenderer.VISCOSITY_MASK, nextLiquid + 1);
		Array.Resize(ref LiquidLoader.liquidProperties, nextLiquid);
		Array.Resize(ref Main.SceneMetrics._liquidCounts, nextLiquid);
		Array.Resize(ref Main.PylonSystem._sceneMetrics._liquidCounts, nextLiquid);

		//Hooks
		ModLoader.BuildGlobalHook(ref HookModifyLight, globalLiquids, g => g.ModifyLight);
		ModLoader.BuildGlobalHook(ref HookLiquidPreUpdate, globalLiquids, g => g.PreUpdate);
		ModLoader.BuildGlobalHook(ref HookLiquidUpdate, globalLiquids, g => g.Update);
		ModLoader.BuildGlobalHook(ref HookLiquidPostUpdate, globalLiquids, g => g.PostUpdate);
		ModLoader.BuildGlobalHook(ref HookAllowMergeLiquids, globalLiquids, g => g.AllowMergeLiquids);
		ModLoader.BuildGlobalHook(ref HookGetLiquidMergeTypes, globalLiquids, g => g.GetLiquidMergeTypes);
		ModLoader.BuildGlobalHook(ref HookShouldDeleteLiquids, globalLiquids, g => g.ShouldDeleteLiquid);
		ModLoader.BuildGlobalHook(ref HookPreventMerge, globalLiquids, g => g.PreventMerge);
		ModLoader.BuildGlobalHook(ref HookOnMerge, globalLiquids, g => g.OnMerge);
		ModLoader.BuildGlobalHook(ref HookCanMoveLeft, globalLiquids, g => g.CanMoveLeft);
		ModLoader.BuildGlobalHook(ref HookCanMoveRight, globalLiquids, g => g.CanMoveRight);
		ModLoader.BuildGlobalHook(ref HookCanMoveDown, globalLiquids, g => g.CanMoveDown);
		ModLoader.BuildGlobalHook(ref HookShouldDrawLiquids, globalLiquids, g => g.ShouldDrawLiquids);

		if (!unloading) {
			loaded = true;
		}
	}

	internal static void Unload()
	{
		loaded = false;
		nextLiquid = LiquidID.Count;
		liquids.Clear();
	}

	private delegate void DelegateModifyLight(int x, int y, int liquidType, ref float r, ref float g, ref float b);
	private static DelegateModifyLight[] HookModifyLight;
	public static void ModifyLight(int x, int y, int liquidType, ref float r, ref float g, ref float b)
	{
		GetLiquid(liquidType)?.ModifyLight(x, y, ref r, ref g, ref b);

		foreach (var hook in HookModifyLight) {
			hook(x, y, liquidType, ref r, ref g, ref b);
		}
	}

	private delegate void DelegateLiquidPreUpdate(int x, int y, int liquidType, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down);
	private static DelegateLiquidPreUpdate[] HookLiquidPreUpdate;
	public static void PreUpdate(int x, int y, int liquidType, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down)
	{
		GetLiquid(liquidType)?.PreUpdate(x, y, liquid, thisTile, left, right, up, down);

		foreach (var hook in HookLiquidPreUpdate) {
			hook(x, y, liquidType, liquid, thisTile, left, right, up, down);
		}
	}

	private delegate bool DelegateLiquidUpdate(int x, int y, int liquidType, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down);
	private static DelegateLiquidUpdate[] HookLiquidUpdate;
	public static bool Update(int x, int y, int liquidType, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down)
	{
		bool result = GetLiquid(liquidType)?.Update(x, y, liquid, thisTile, left, right, up, down) ?? true;

		foreach (var hook in HookLiquidUpdate) {
			if (!hook(x, y, liquidType, liquid, thisTile, left, right, up, down))
				result = false;
		}

		return result;
	}

	private delegate void DelegateLiquidPostUpdate(int x, int y, int liquidType, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down);
	private static DelegateLiquidPostUpdate[] HookLiquidPostUpdate;
	public static void PostUpdate(int x, int y, int liquidType, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down)
	{
		GetLiquid(liquidType)?.PostUpdate(x, y, liquid, thisTile, left, right, up, down);

		foreach (var hook in HookLiquidPostUpdate) {
			hook(x, y, liquidType, liquid, thisTile, left, right, up, down);
		}
	}

	private delegate bool DelegateAllowMergeLiquids(int x, int y, Tile tile, int x2, int y2, Tile tile2);
	private static DelegateAllowMergeLiquids[] HookAllowMergeLiquids;
	public static bool AllowMergeLiquids(int x, int y, Tile tile, int x2, int y2, Tile tile2)
	{
		//AllowMergeLiquids is only called when there is a liquid at Main.tile[x, y] and Main.tile[x2, y2] and they will always be different types.
		if (GetLiquid(tile.LiquidType)?.AllowMergeLiquids(x, y, tile, x2, y2, tile2) == false)
			return false;

		if (GetLiquid(tile2.LiquidType)?.AllowMergeLiquids(x, y, tile, x2, y2, tile2) == false)
			return false;

		foreach (var hook in HookAllowMergeLiquids) {
			bool? shouldMerge = hook(x, y, tile, x2, y2, tile2);
			if (shouldMerge.HasValue) {
				if (!shouldMerge.Value)
					return false;
			}
		}

		return true;
	}

	private delegate void DelegateGetLiquidMergeTypes(int x, int y, int type, bool[] liquidNearby, ref int liquidMergeTileType, ref int liquidMergeType, LiquidMerge liquidMerge);
	private static DelegateGetLiquidMergeTypes[] HookGetLiquidMergeTypes;
	public static void GetLiquidMergeTypes(int x, int y, int liquidType, bool[] liquidsNearby, ref int liquidMergeTileType, ref int liquidMergeType, LiquidMerge liquidMerge)
	{
		SortedSet<int> liquidsToCheck = new() { liquidType };
		for (int i = 0; i < liquidsNearby.Length; i++) {
			if (liquidsNearby[i])
				liquidsToCheck.Add(i);
		}

		foreach (int liquidToCheck in liquidsToCheck) {
			GetLiquid(liquidToCheck)?.GetLiquidMergeTypes(x, y, liquidType, liquidsNearby, ref liquidMergeTileType, ref liquidMergeType, liquidMerge);
		}

		foreach (var hook in HookGetLiquidMergeTypes) {
			hook(x, y, liquidType, liquidsNearby, ref liquidMergeTileType, ref liquidMergeType, liquidMerge);
		}
	}

	private delegate bool DelegateShouldDeleteLiquids(LiquidMerge liquidMerge);
	private static DelegateShouldDeleteLiquids[] HookShouldDeleteLiquids;
	public static bool ShouldDeleteLiquid(LiquidMerge liquidMerge)
	{
		SortedSet<int> liquidsToCheck = new();
		foreach (LiquidMergeIngredient liquidMergeIngredient in liquidMerge.LiquidMergeIngredients) {
			liquidsToCheck.Add(liquidMergeIngredient.LiquidType);
		}

		foreach (int liquidToCheck in liquidsToCheck) {
			if (GetLiquid(liquidToCheck)?.ShouldDeleteLiquid(liquidMerge) == false)
				return false;
		}

		foreach (var hook in HookShouldDeleteLiquids) {
			if (!hook(liquidMerge))
				return false;
		}

		return true;
	}

	private delegate bool DelegatePreventMerge(LiquidMerge liquidMerge);
	private static DelegatePreventMerge[] HookPreventMerge;
	public static bool PreventMerge(LiquidMerge liquidMerge)
	{
		if (GetLiquid(liquidMerge.MergeTargetTile.LiquidType)?.PreventMerge(liquidMerge) ?? false)
			return true;

		foreach (var hook in HookPreventMerge) {
			if (hook(liquidMerge))
				return true;
		}

		return false;
	}

	private delegate void DelegateOnMerge(LiquidMerge liquidMerge, Dictionary<int, int> consumedLiquids);
	private static DelegateOnMerge[] HookOnMerge;
	public static void OnMerge(LiquidMerge liquidMerge, Dictionary<int, int> consumedLiquids)
	{
		GetLiquid(liquidMerge.MergeTargetTile.LiquidType)?.OnMerge(liquidMerge, consumedLiquids);

		foreach (var hook in HookOnMerge) {
			hook(liquidMerge, consumedLiquids);
		}
	}

	static LiquidLoader()
	{
		liquidProperties = new LiquidProperties[] {
			// Water
			new LiquidProperties() {
				FallDelay = 0,
				HellEvaporation = true
			},
			// Lava
			new LiquidProperties() {
				FallDelay = 5
			},
			// Honey
			new LiquidProperties() {
				FallDelay = 10
			},
			//Shimmer
			new LiquidProperties() {
				FallDelay = 0
			}};
	}

	private delegate bool? DelegateCanMove(int x, int y, int xMove, int yMove, bool canMoveLeftVanilla);
	private static DelegateCanMove[] HookCanMoveLeft;
	public static bool CanMoveLeft(int x, int y, int xMove, int yMove, bool canMoveLeftVanilla)
	{
		bool? result = null;

		foreach (var hook in HookCanMoveLeft) {
			bool? move = hook(x, y, xMove, yMove, canMoveLeftVanilla);
			if (move.HasValue) {
				result = move;
				if (!move.Value)
					return false;
			}
		}

		return result ?? canMoveLeftVanilla;
	}

	private delegate bool? DelegateCanMoveRight(int x, int y, int xMove, int yMove, bool canMoveRightVanilla);
	private static DelegateCanMove[] HookCanMoveRight;
	public static bool CanMoveRight(int x, int y, int xMove, int yMove, bool canMoveRightVanilla)
	{
		bool? result = null;

		foreach (var hook in HookCanMoveRight) {
			bool? move = hook(x, y, xMove, yMove, canMoveRightVanilla);
			if (move.HasValue) {
				result = move;
				if (!move.Value)
					return false;
			}
		}

		return result ?? canMoveRightVanilla;
	}

	private delegate bool? DelegateCanMoveDown(int x, int y, int xMove, int yMove, bool canMoveDownVanilla);
	private static DelegateCanMove[] HookCanMoveDown;
	public static bool CanMoveDown(int x, int y, int xMove, int yMove, bool canMoveDownVanilla)
	{
		bool? result = null;

		foreach (var hook in HookCanMoveDown) {
			bool? move = hook(x, y, xMove, yMove, canMoveDownVanilla);
			if (move.HasValue) {
				result = move;
				if (!move.Value)
					return false;
			}
		}

		return result ?? canMoveDownVanilla;
	}

	private delegate bool? DelegateShouldDrawLiquids(int x, int y, bool shouldDrawVanilla);
	private static DelegateShouldDrawLiquids[] HookShouldDrawLiquids;
	public static bool ShouldDrawLiquids(int x, int y, bool shouldDrawVanilla)
	{
		bool? result = null;

		foreach (var hook in HookShouldDrawLiquids) {
			bool? draw = hook(x, y, shouldDrawVanilla);
			if (draw.HasValue) {
				result = draw;
				if (!draw.Value)
					return false;
			}
		}

		return result ?? shouldDrawVanilla;
	}
}