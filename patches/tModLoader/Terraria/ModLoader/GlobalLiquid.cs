using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terraria.ModLoader;

/// <summary>
/// This class allows you to modify and use hooks for liquids, including vanilla items. Create an instance of an overriding class to use this.
/// </summary>
public abstract class GlobalLiquid : ModType
{
	protected sealed override void Register()
	{
		LiquidLoader.globalLiquids.Add(this);
	}

	public virtual void ModifyLight(int x, int y, int liquidType, ref float r, ref float g, ref float b) { }

	/// <summary>
	/// Called at the very start of Liquid.Update before liquid merges or liquid movement.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="liquidType"></param>
	/// <param name="liquid"></param>
	/// <param name="thisTile"></param>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="up"></param>
	/// <param name="down"></param>
	public virtual void PreUpdate(int x, int y, int liquidType, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down) { }

	/// <summary>
	/// Called just after liquid merge checks, and just before liquid movement checks.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="liquidType"></param>
	/// <param name="liquid"></param>
	/// <param name="thisTile"></param>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="up"></param>
	/// <param name="down"></param>
	/// <returns></returns>
	public virtual bool Update(int x, int y, int liquidType, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down) => true;

	/// <summary>
	/// Called at the very end up Liquid.Update after merge and movement checks.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="liquidType"></param>
	/// <param name="liquid"></param>
	/// <param name="thisTile"></param>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="up"></param>
	/// <param name="down"></param>
	public virtual void PostUpdate(int x, int y, int liquidType, Liquid liquid, Tile thisTile, Tile left, Tile right, Tile up, Tile down) { }

	/// <summary>
	/// Return false to prevent liquids from merging.  Return true by default.  This is called when checking if the liquid at Main.tile[x, y] is being updated.
	/// If true is returned, the liquid at Main.tile[x2, y2] will be added to Main.liquid to be updated.
	/// </summary>
	/// <param name="x">x tile coordinate of the tile being checked</param>
	/// <param name="y">y tile coordinate of the tile being checked</param>
	/// <param name="tile">Tile at Main.tile[x, y]</param>
	/// <param name="x2">x tile coordinate of the liquid trying to merge with this tile</param>
	/// <param name="y2">y tile coordinate of the liquid trying to merge with this tile</param>
	/// <param name="tile2">Tile at Main.tile[x2, y2]</param>
	/// <returns></returns>
	public virtual bool AllowMergeLiquids(int x, int y, Tile tile, int x2, int y2, Tile tile2) => true;

	/// <summary>
	/// Allows you to change the resulting tile created when liquids merge together.
	/// </summary>
	/// <param name="x">x tile coordinate of the tile being merged onto.</param>
	/// <param name="y">y tile coordinate of the tile being merged onto.</param>
	/// <param name="type">Liquid type at Main.tile[x, y]</param>
	/// <param name="liquidNearby">Use liquidNearby[LiquidID] to check if that liquid is touching the tile being merged onto.</param>
	/// <param name="liquidMergeTileType">The tile type being created by the merge at Main.tile[x, y].</param>
	/// <param name="liquidMergeType">The liquid type that is being used to determine the created tile type.  (Changing only this will do nothing since the created tile has already been determined.)</param>
	public virtual void GetLiquidMergeTypes(int x, int y, int type, bool[] liquidNearby, ref int liquidMergeTileType, ref int liquidMergeType, LiquidMerge liquidMerge) { }

	/// <summary>
	/// Called when a liquid merge is attempted and the total other liquids are less than 24 or the liquidMergeType is the same type as the liquidMerge.MergeTargetTile.LiquidType.
	/// Return false to prevent the liquid above from being deleted and continue with the merge instead.  Return true by default.
	/// </summary>
	/// <param name="liquidMerge"></param>
	/// <returns></returns>
	public virtual bool ShouldDeleteLiquid(LiquidMerge liquidMerge) => true;

	/// <summary>
	/// Called after ShouldDeleteLiquid just before the liquid merge occurs.
	/// Return true to prevent the merge from occurring.  Return true by default.
	/// </summary>
	/// <param name="liquidMerge"></param>
	/// <returns></returns>
	public virtual bool PreventMerge(LiquidMerge liquidMerge) => false;

	/// <summary>
	/// Called just after a merge occurs.  Tiles and liquids have already been updated.
	/// </summary>
	/// <param name="liquidMerge"></param>
	/// <param name="consumedLiquids">The (type, amount) of liquids used in the merge.</param>
	public virtual void OnMerge(LiquidMerge liquidMerge, Dictionary<int, int> consumedLiquids) { }

	/// <summary>
	/// x, y are the tile coordinates of the liquid that is trying to move.  
	/// xMove, yMove are the tile coordinates of the tile the liquid is trying to move to.  
	/// Return true to force the liquid to move.  
	/// Return null for vanilla behavior (same as canMoveVanilla).  
	/// Return false to prevent the liquid from moving.  
	/// </summary>
	/// <param name="x">x tile coordinate of the liquid</param>
	/// <param name="y">y tile coordinate of the liquid</param>
	/// <param name="xMove">x tile coordinate of the target being moved to</param>
	/// <param name="yMove">y tile coordinate of the target being moved to</param>
	/// <param name="canMoveLeftVanilla">Vanilla logic's determination of if the liquid should move or not.</param>
	/// <returns></returns>
	public virtual bool? CanMoveLeft(int x, int y, int xMove, int yMove, bool canMoveLeftVanilla) => null;

	/// <summary>
	/// x, y are the tile coordinates of the liquid that is trying to move.  
	/// xMove, yMove are the tile coordinates of the tile the liquid is trying to move to.  
	/// Return true to force the liquid to move.  
	/// Return null for vanilla behavior (same as canMoveVanilla).  
	/// Return false to prevent the liquid from moving.  
	/// </summary>
	/// <param name="x">x tile coordinate of the liquid</param>
	/// <param name="y">y tile coordinate of the liquid</param>
	/// <param name="xMove">x tile coordinate of the target being moved to</param>
	/// <param name="yMove">y tile coordinate of the target being moved to</param>
	/// <param name="canMoveRightVanilla">Vanilla logic's determination of if the liquid should move or not.</param>
	/// <returns></returns>
	public virtual bool? CanMoveRight(int x, int y, int xMove, int yMove, bool canMoveRightVanilla) => null;

	/// <summary>
	/// x, y are the tile coordinates of the liquid that is trying to move.  
	/// xMove, yMove are the tile coordinates of the tile the liquid is trying to move to.  
	/// Return true to force the liquid to move.  
	/// Return null for vanilla behavior (same as canMoveVanilla).  
	/// Return false to prevent the liquid from moving.  
	/// </summary>
	/// <param name="x">x tile coordinate of the liquid</param>
	/// <param name="y">y tile coordinate of the liquid</param>
	/// <param name="xMove">x tile coordinate of the target being moved to</param>
	/// <param name="yMove">y tile coordinate of the target being moved to</param>
	/// <param name="canMoveDownVanilla">Vanilla logic's determination of if the liquid should move or not.</param>
	/// <returns></returns>
	public virtual bool? CanMoveDown(int x, int y, int xMove, int yMove, bool canMoveDownVanilla) => null;

	/// <summary>
	/// Used to force or prevent liquids from being drawn at Main.tile[x, y].  This includes visual only liquid runoff from liquids.
	/// This method is called on every tile being drawn, not just tiles with liquids or liquid runoffs.
	/// </summary>
	/// <param name="x">x tile coordinate of the tile being checked</param>
	/// <param name="y">y tile coordinate of the tile being checked</param>
	/// <param name="shouldDrawVanilla">Vanilla logic's determination of if liquids should be drawn at the location if applicable.</param>
	/// <returns></returns>
	public virtual bool? ShouldDrawLiquids(int x, int y, bool shouldDrawVanilla) => null;
}
