using ExampleMod.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Common.GlobalLiquids
{
	//This class prevents liquids from touching or moving past ExampleBlock tiles to show off how the movement of liquids can be controlled with GlobalLiquid.
	//The hooks can also force liquids to move if desired by returning true in the CanMove hooks, but that isn't shown off here.
	public class ExampleGlobalLiquid : GlobalLiquid
	{
		public override bool? CanMoveLeft(int x, int y, int xMove, int yMove, bool canMoveVanilla) {
			Tile leftTile = Main.tile[xMove - 1, yMove];
			int exampleBlock = ModContent.TileType<ExampleBlock>();
			if (leftTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing into a tile touching the right side of an ExampleBlock tile.

			Tile upTile = Main.tile[xMove, yMove - 1];
			if (upTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing into a tile touching the bottom side of an ExampleBlock tile.

			Tile downTile = Main.tile[xMove, yMove + 1];
			if (downTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing into a tile touching the top side of an ExampleBlock tile.

			Tile downLeftTile = Main.tile[xMove - 1, yMove + 1];
			if (downLeftTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing into a tile touching the top right side of an ExampleBlock tile.

			Tile downRightTile = Main.tile[xMove + 1, yMove + 1];
			if (downRightTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing into a tile touching the top left side of an ExampleBlock tile.

			return null;
		}

		public override bool? CanMoveRight(int x, int y, int xMove, int yMove, bool canMoveVanilla) {
			Tile rightTile = Main.tile[xMove + 1, yMove];
			int exampleBlock = ModContent.TileType<ExampleBlock>();
			if (rightTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing into a tile touching the left side of an ExampleBlock tile.

			Tile upTile = Main.tile[xMove, yMove - 1];
			if (upTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing into a tile touching the bottom side of an ExampleBlock tile.

			Tile downTile = Main.tile[xMove, yMove + 1];
			if (downTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing into a tile touching the top side of an ExampleBlock tile.

			Tile downLeftTile = Main.tile[xMove - 1, yMove + 1];
			if (downLeftTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing into a tile touching the top right side of an ExampleBlock tile.

			Tile downRightTile = Main.tile[xMove + 1, yMove + 1];
			if (downRightTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing into a tile touching the top left side of an ExampleBlock tile.

			return null;
		}

		public override bool? CanMoveDown(int x, int y, int xMove, int yMove, bool canMoveVanilla) {
			Tile downTile = Main.tile[xMove, yMove + 1];
			int exampleBlock = ModContent.TileType<ExampleBlock>();
			if (downTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing down onto the top side of an ExampleBlock tile.

			Tile leftTile = Main.tile[xMove - 1, yMove];
			if (leftTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing down into a tile that is touching the right side of an ExampleBlock tile.

			Tile rightTile = Main.tile[xMove + 1, yMove];
			if (rightTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing down into a tile that is touching the left side of an ExampleBlock tile.

			Tile downLeftTile = Main.tile[xMove - 1, yMove + 1];
			if (downLeftTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing down into a tile that is touching the top right side of an ExampleBlock tile.

			Tile downRightTile = Main.tile[xMove + 1, yMove + 1];
			if (downRightTile.TileType == exampleBlock)
				return false;//Prevents liquids from flowing down into a tile that is touching the top left side of an ExampleBlock tile.

			return null;
		}

		public override bool? ShouldDrawLiquids(int x, int y, bool shouldDrawVanilla) {
			Tile tile = Main.tile[x, y];
			if (tile.LiquidAmount > 0)
				return null;

			Tile leftTile = Main.tile[x - 1, y];
			int exampleBlock = ModContent.TileType<ExampleBlock>();
			if (leftTile.TileType == exampleBlock)
				return false;//Prevents liquids from drawing to the right of an ExampleBlock tile.

			Tile rightTile = Main.tile[x + 1, y];
			if (rightTile.TileType == exampleBlock)
				return false;//Prevents liquids from drawing to the left of an ExampleBlock tile.
			
			Tile downTile = Main.tile[x, y + 1];
			if (downTile.TileType == exampleBlock)
				return false;//Prevents liquids from drawing above an ExampleBlock tile.

			Tile upTile = Main.tile[x, y - 1];
			if (upTile.TileType == exampleBlock)
				return false;//Prevents liquids from drawing below an ExampleBlock tile.

			Tile upLeftTile = Main.tile[x - 1, y - 1];
			if (upLeftTile.TileType == exampleBlock)
				return false;//Prevents liquids from drawing below and to the right of an ExampleBlock tile.

			Tile upRightTile = Main.tile[x + 1, y - 1];
			if (upRightTile.TileType == exampleBlock)
				return false;//Prevents liquids from drawing below and to the left of an ExampleBlock tile.

			Tile downRightTile = Main.tile[x + 1, y + 1];
			if (downRightTile.TileType == exampleBlock)
				return false;//Prevents liquids from drawing above and to the left of an ExampleBlock tile.

			Tile downLeftTile = Main.tile[x - 1, y + 1];
			if (downLeftTile.TileType == exampleBlock)
				return false;//Prevents liquids from drawing above and to the right of an ExampleBlock tile.

			return null;
		}

		public override bool AllowMergeLiquids(int x, int y, Tile tile, int x2, int y2, Tile tile2) {
			return ExampleBlockLiquidInteractionsAllowed(x, y);
		}
		public override bool ShouldDeleteLiquid(LiquidMerge liquidMerge) {
			return ExampleBlockLiquidInteractionsAllowed(liquidMerge.X, liquidMerge.Y);
		}
		private bool ExampleBlockLiquidInteractionsAllowed(int x, int y) {
			//If the liquid has an ExampleBlock on both sides of the body of liquid, prevent merging.
			int exampleBlock = ModContent.TileType<ExampleBlock>();

			//Move to the farthest connected liquid to the left.
			int leftX = x - 1;
			for (; leftX > 0; leftX--) {
				Tile left = Main.tile[leftX - 1, y];
				if (left.LiquidAmount <= 0 && left.HasTile)
					break;
			}

			//Check if there is an ExampleBlock within 2 blocks of the liquid, or if it's at the edge of the world.
			if (leftX < 0)
				return true;

			Tile left1 = Main.tile[leftX, y];
			if (!left1.HasTile || left1.TileType != exampleBlock) {
				if (leftX < 1)
					return true;

				Tile left2 = Main.tile[leftX - 1, y];
				if (!left2.HasTile || left2.TileType != exampleBlock)
					return true;
			}

			//Move to the farthest connected liquid to the right.
			int rightX = x + 1;
			for (; rightX < Main.maxTilesX; rightX++) {
				Tile right = Main.tile[rightX + 1, y];
				if (right.LiquidAmount <= 0 && right.HasTile)
					break;
			}

			//Check if there is an ExampleBlock within 2 blocks of the liquid, or if it's at the edge of the world.
			if (rightX >= Main.maxTilesX)
				return true;

			Tile right1 = Main.tile[rightX, y];
			if (!right1.HasTile || right1.TileType != exampleBlock) {
				if (rightX >= Main.maxTilesX - 1)
					return true;

				Tile right2 = Main.tile[rightX + 1, y];
				if (!right2.HasTile || right2.TileType != exampleBlock)
					return true;
			}

			return false;
		}
	}

	public class LiquidGlobalTileHelper : GlobalTile {
		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem) {
			if (fail || effectOnly)
				return;

			if (type == ModContent.TileType<ExampleBlock>()) {
				//AddWater causes the liquid at this location to check if it can flow during the next liquid update.
				//If this is not done correctly, the liquid won't move when the example block is broken because vanilla only calls AddWater
				//	for adjacent blocks when they are broken, and ExampleBlock is preventing the liquid from moving from 1 block away.
				// x u u u x
				// l x x x r
				// l x b x r
				// l x x x r
				// x d d d x

				//left
				int leftX2 = i - 2;
				if (leftX2 >= 0)
					Liquid.AddWater(leftX2, j);

				//left2 up 1
				int upY1 = j - 1;
				if (leftX2 >= 0 && upY1 >= 0)
					Liquid.AddWater(leftX2, upY1);

				//up2 left 1
				int upY2 = j - 2;
				int leftX1 = i - 1;
				if (leftX1 >= 0 && upY2 >= 0)
					Liquid.AddWater(leftX2, upY2);

				//up
				if (upY2 >= 0)
					Liquid.AddWater(i, upY2);

				//up2 right 1
				int rightX1 = i + 1;
				if (rightX1 < Main.maxTilesX && upY2 >= 0)
					Liquid.AddWater(rightX1, upY2);

				//right2 up 1
				int rightX2 = i + 2;
				if (rightX2 < Main.maxTilesX && upY1 >= 0)
					Liquid.AddWater(rightX2, upY1);

				//right
				if (rightX2 < Main.maxTilesX)
					Liquid.AddWater(rightX2, j);

				//right2 down 1
				int downY1 = j + 1;
				if (rightX2 < Main.maxTilesX && downY1 < Main.maxTilesY)
					Liquid.AddWater(rightX2, downY1);

				//down2 right 1
				int downY2 = j + 2;
				if (rightX1 < Main.maxTilesX && downY2 < Main.maxTilesY)
					Liquid.AddWater(rightX1, downY2);

				//down
				if (downY2 < Main.maxTilesY)
					Liquid.AddWater(i, downY2);

				//down2 left 1
				if (leftX1 >= 0 && downY2 < Main.maxTilesY)
					Liquid.AddWater(leftX1, downY2);

				//left2 down 1
				if (leftX2 >= 0 && downY1 < Main.maxTilesY)
					Liquid.AddWater(leftX2, downY1);
			}
		}
	}
}
