using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ModLoader.Core;
using static Terraria.Graphics.Capture.CaptureBiome.Sets;

namespace Terraria.ModLoader;

public enum CollisionType
{
	Enter,
	Stay,
	Exit
}

public static class LiquidLoader
{
	internal class LiquidTextures
	{
		/// <summary>
		/// Used for water style to map from the previous id it had
		/// </summary>
		public int Index;
		/// <summary>
		/// Main texture for the liquid
		/// </summary>
		public Asset<Texture2D> MainTexture;
		/// <summary>
		/// Texture used for corner
		/// </summary>
		public Asset<Texture2D> CornerTexture;
		/// <summary>
		/// Texture for the liquid waterfall
		/// </summary>
		public Asset<Texture2D> WaterfallTexture;
	}

	internal struct LiquidProperties
	{
		public int FallDelay;
		public bool NoStandardUpdate;
		public bool HellEvaporation;
		public float SpeedModifier;
	}

	internal static List<LiquidLoader.LiquidTextures>[] _LiquidTextures = new List<LiquidTextures>[4];

	private static int nextLiquid = LiquidID.Count;
	private static bool loaded = false;

	internal static readonly List<ModLiquid> liquids = new List<ModLiquid>() {
		ModLiquid.Water,
		ModLiquid.Lava,
		ModLiquid.Honey,
		ModLiquid.Shimmer
	};

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

	internal static void ResizeArrays(bool unloading = false)
	{
		//Texture
		Array.Resize(ref TextureAssets.Liquid, 15 + nextLiquid - LiquidID.Count);
		Array.Resize(ref LiquidRenderer.Instance._moddedLiquidTextures, nextLiquid - LiquidID.Count);
		Array.Resize(ref LiquidLoader._LiquidTextures, nextLiquid);

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
		Array.Resize(ref Collision.Liquid, nextLiquid + 1);
		Array.Resize(ref Main.LiquidStyle, nextLiquid);
		

		if (!unloading) {
			loaded = true;
		}
	}

	internal static void Unload()
	{
		loaded = false;
		nextLiquid = LiquidID.Count;
		liquids.RemoveRange(LiquidID.Count, liquids.Count - LiquidID.Count);
	}

	public static void ModifyLight(int type, int i, int j, ref float r, ref float g, ref float b)
	{
		GetLiquid(type)?.ModifyLight(i, j, ref r, ref g, ref b);
	}

	public static void PreUpdate(int type, int x, int y)
	{
		GetLiquid(type)?.PreUpdate(x, y);
	}

	public static bool Update(int type, Liquid liquid, int x, int y, Tile left, Tile right, Tile up, Tile down)
	{
		return GetLiquid(type)?.Update(liquid, x, y, left, right, up, down) ?? true;
	}

	public static void Merge(int type, bool[] liquidNearby, ref int liquidMergeTileType, ref int liquidMergeType)
	{
		GetLiquid(type)?.Merge(type, liquidNearby, ref liquidMergeTileType, ref liquidMergeType);
	}

	public static void OnEntityCollision(int type, Entity entity, CollisionType collisionType)
	{
		GetLiquid(type)?.OnEntityCollision(entity, collisionType);
	}

	public static void DrawWaterfall(WaterfallManager waterfallManager)
	{
		for (int i = 0; i < liquids.Count; i++) {
			for (int j = 0; j < liquids[i].LiquidStyle.Length; j++) {
				if (Main.LiquidAlpha[i][j] > 0f) {
					waterfallManager.DrawWaterfall(liquids[i].LiquidStyle[j].Slot, Main.LiquidAlpha[i][j], i);
				}
			}
		}
	}

	static LiquidLoader()
	{
		SetupVanillaLiquidProperties();
	}

	private static void SetupVanillaLiquidProperties() =>
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

	internal static void SetupVanillaLiquidArrays()
	{
		for (int i = 0; i < _LiquidTextures.Length; i++) {
			_LiquidTextures[i] = new List<LiquidTextures>();
		}
		
		// Texture is taking with the index of waterfall style
		for (int i = 0; i < 26; i++) {
			_LiquidTextures[0].Add(null);
			if (i == 1 || i == 14 || i == 25) continue;
			Asset<Texture2D> liquidTextureAsset;
			Asset<Texture2D> cornerTextureAsset;
			Asset<Texture2D> waterfallTexture;
			if (i >= 13 && i != 24) {
				liquidTextureAsset = LiquidRenderer.Instance._liquidTextures[0]; // Use default texture for gemstone/rgb waterfall
				cornerTextureAsset = TextureAssets.LiquidSlope[0];
				waterfallTexture = Main.instance.waterfallManager.waterfallTexture[0];
			}
			else {
				if (i == 24) {
					liquidTextureAsset = LiquidRenderer.Instance._liquidTextures[13]; // Ocean liquid texture
					cornerTextureAsset = TextureAssets.LiquidSlope[13];
					waterfallTexture = Main.instance.waterfallManager.waterfallTexture[13];
				}
				else {
					liquidTextureAsset = TextureAssets.Liquid[i];
					cornerTextureAsset = TextureAssets.LiquidSlope[i];
					waterfallTexture = Main.instance.waterfallManager.waterfallTexture[i];
				}
			}

			_LiquidTextures[0][i] = new LiquidTextures() {
				Index = i,
				MainTexture = liquidTextureAsset,
				CornerTexture = cornerTextureAsset,
				WaterfallTexture = waterfallTexture
			};
		}

		_LiquidTextures[1].Add(new LiquidTextures() {
			MainTexture = LiquidRenderer.Instance._liquidTextures[1],
			CornerTexture = TextureAssets.LiquidSlope[1],
			WaterfallTexture = Main.instance.waterfallManager.waterfallTexture[1]
		});

		_LiquidTextures[2].Add(new LiquidTextures() {
			MainTexture = LiquidRenderer.Instance._liquidTextures[11],
			CornerTexture = TextureAssets.LiquidSlope[11],
			WaterfallTexture = Main.instance.waterfallManager.waterfallTexture[14]
		});

		_LiquidTextures[3].Add(new LiquidTextures() {
			MainTexture = LiquidRenderer.Instance._liquidTextures[14],
			CornerTexture = TextureAssets.LiquidSlope[14],
			WaterfallTexture = Main.instance.waterfallManager.waterfallTexture[25]
		});
	}
}
