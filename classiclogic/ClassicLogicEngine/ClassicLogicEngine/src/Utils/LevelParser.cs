﻿using ClassicLogic.Engine;

namespace ClassicLogic.Utils
{
  public class LevelParser
  {
    public static LevelMap Parse(LevelReader levelMapReader, int maxGuard)
    {
      //(1) create empty map[x][y] array;
      var map = new LevelMap();
      map.maxGuard = maxGuard;

      var mapGuards = 0;

      levelMapReader.Reset();

      while (levelMapReader.MoveNext())
      {
        if (levelMapReader.Token.Type == TileType.GUARD_T) mapGuards++;
      }

      //(2) draw map
      var y = 0;
      var x = 0;

      levelMapReader.Reset();
      while (levelMapReader.MoveNext())
      {
        if (levelMapReader.Token.TokenType == LevelTokenType.EndLine)
        {
          y++;
          x = 0;
        }
        else
        {
          var id = levelMapReader.Token.Type;
          var tile = map[x][y];

          switch (id)
          {
            case TileType.BLOCK_T: //Normal Brick
              tile.@base = TileType.BLOCK_T;
              tile.act = TileType.BLOCK_T;
              //tile.bitmap.name = "brick";
              break;
            case TileType.SOLID_T: //Solid Brick
              tile.@base = TileType.SOLID_T;
              tile.act = TileType.SOLID_T;
              //tile.bitmap.name = "solid";
              break;
            case TileType.LADDR_T: //Ladder
              tile.@base = TileType.LADDR_T;
              tile.act = TileType.LADDR_T;
              //tile.bitmap.name = "ladder";
              break;
            case TileType.BAR_T: //Line of rope
              tile.@base = TileType.BAR_T;
              tile.act = TileType.BAR_T;
              //tile.bitmap.name = "rope";
              break;
            case TileType.TRAP_T: //False brick
              tile.@base = TileType.TRAP_T; //behavior same as empty
              tile.act = TileType.TRAP_T;
              //tile.bitmap.name = "brick";
              break;
            case TileType.HLADR_T: //Ladder appears at end of level
              tile.@base = TileType.HLADR_T; //behavior same as empty before end of level
              tile.act = TileType.EMPTY_T; //behavior same as empty before end of level

              //tile.bitmap.name = "ladder";
              //tile.bitmap.setAlpha(0); //hide the laddr
              break;
            case TileType.GOLD_T: //Gold chest
              tile.@base = TileType.GOLD_T; //keep gold on base map
              tile.act = TileType.EMPTY_T;
              //tile.bitmap.name = "gold";
              map.goldCount += 1;
              break;
            case TileType.GUARD_T: //Guard
              tile.@base = TileType.EMPTY_T;
              tile.act = TileType.GUARD_T;
              //tile.bitmap = null;
              if ((--mapGuards) >= maxGuard)
              {
                tile.act = TileType.EMPTY_T; //too many guards, set this tile as empty
              }
              else
              {
                map.guardCount += 1;
              }
              /*
              curTile = new createjs.Sprite(guardData, "runLeft");
              guard[guardCount] = {
              sprite: curTile,
                                    pos: { x: x, y: y, xOffset: 0, yOffset: 0}, 
                                    action: ACT_STOP,
                                    shape: "runLeft",
                                    lastLeftRight: "ACT_LEFT",
                                    hasGold: 0
                                };
              guardCount++;
              curTile.stop();*/
              break;
            case TileType.RUNNER_T: //Player
              tile.@base = TileType.EMPTY_T;
              tile.act = TileType.RUNNER_T;
              //tile.bitmap = null;
              tile.action = Action.ACT_UNKNOWN;
              if (map.runner != null)
              {
                tile.act = TileType.EMPTY_T; //too many runner, set this tile as empty
              }
              else
              {
                map.runner = tile;

                /*runner = { };
                curTile = runner.sprite = new createjs.Sprite(runnerData, "runRight");
                runner.pos = { x: x, y: y, xOffset: 0, yOffset: 0};
                runner.action = ACT_UNKNOWN;
                runner.shape = "runRight";
                runner.lastLeftRight = "ACT_RIGHT";
                curTile.stop();
                */
              }

              break;
            case TileType.EMPTY_T: //empty
              tile.@base = TileType.EMPTY_T;
              tile.act = TileType.EMPTY_T;
              //tile.bitmap = null;
              break;
            default:
              throw new System.ArgumentException();
          }
          
          x++;
        }
      }

      Assert.IsTrue(mapGuards == 0, "Error: mapCuardCount design error !");

      return map;
    }
  }
}