using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.Liquid;
using Terraria.GameContent;
using Terraria.ID;

namespace Terraria.ModLoader;
public abstract class ModLiquidStyle : ModTexturedType
{
	internal int _liquid = LiquidID.Water;

	public virtual int Liquid => _liquid;
	public int Slot { get; internal set; }

	public virtual string BlockTexture => Texture + "_block";
	public virtual string SlopeTexture => Texture + "_slope";
	public virtual string WaterfallTexture => Texture + "_waterfall";

	internal string DefaultTextureName;

	private Asset<Texture2D> _textureAsset;
	private Asset<Texture2D> _blockTextureAsset;
	private Asset<Texture2D> _slopeTexture2D;
	private Asset<Texture2D> _waterfallTexture2D;

	internal Asset<Texture2D> TextureAsset => _textureAsset ??= ModContent.Request<Texture2D>(Texture);

	internal Asset<Texture2D> BlockAsset => _blockTextureAsset ??= ModContent.Request<Texture2D>(BlockTexture);

	internal Asset<Texture2D> SlopeAsset => _slopeTexture2D ??= ModContent.Request<Texture2D>(SlopeTexture);

	internal Asset<Texture2D> WaterfallAsset => _waterfallTexture2D ??= ModContent.Request<Texture2D>(WaterfallTexture);

	public override string Texture => (DefaultTextureName != null) ? DefaultTextureName : (GetType().Namespace + "." + Name).Replace('.', '/');

	/// <summary>
	/// Allow to only color the waterfall, like the gemspark block slope do
	/// </summary>
	public bool WaterfallOnly;


	protected override void Register()
	{
		LoaderManager.Get<LiquidStyleLoader>().Register(this);
		Slot = LoaderManager.Get<LiquidStyleLoader>().RegisterLiquidStyle(Liquid, this);
	} 


	public sealed override void SetupContent()
	{
		LiquidRenderer.Instance._liquidTextures[Slot] = ModContent.Request<Texture2D>(Texture);

		SetStaticDefaults();

		TextureAssets.Liquid[Slot] = ModContent.Request<Texture2D>(BlockTexture);
		if (Mod.TModLoaderVersion < new System.Version(2023, 6, 24)) {
			TextureAssets.LiquidSlope[Slot] = ModContent.Request<Texture2D>(BlockTexture); // TODO: Remove workaround
		}
		else {
			TextureAssets.LiquidSlope[Slot] = ModContent.Request<Texture2D>(SlopeTexture);
		}
	}

	public virtual bool IsLiquidActive() { return false; }
}
