using System;
using System.Collections.Generic;
using ClassicLogic.Engine;
using Action = ClassicLogic.Engine.Action;

namespace ClassicLogic.Utils
{
  public class LevelParser
  {
    public static LevelMap Parse(LevelReader levelMapReader, int maxGuard)
    {
      //(1) create empty map[x][y] array;
      var map = new LevelMap();
      map.MaxGuard = maxGuard;

      var mapGuards = 0;
      var mapTeleports = 0;

      levelMapReader.Reset();

      while (levelMapReader.MoveNext())
      {
        if (levelMapReader.Token.Type == TileType.GUARD_T) mapGuards++;
        else if (levelMapReader.Token.Type == TileType.TELEPORT_T)
        {
          mapTeleports++;
        }
      }

      if (!(mapTeleports == 0 || mapTeleports == 2)) throw new ArgumentException("not valid count of teleports");

      //(2) draw map
      var y = 0;
      var x = 0;

      map.Teleports = new Tile[mapTeleports];

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
              tile.Base = TileType.BLOCK_T;
              tile.Act = TileType.BLOCK_T;
              //tile.bitmap.name = "brick";
              break;
            case TileType.SOLID_T: //Solid Brick
              tile.Base = TileType.SOLID_T;
              tile.Act = TileType.SOLID_T;
              //tile.bitmap.name = "solid";
              break;
            case TileType.LADDR_T: //Ladder
              tile.Base = TileType.LADDR_T;
              tile.Act = TileType.LADDR_T;
              //tile.bitmap.name = "ladder";
              break;
            case TileType.BAR_T: //Line of rope
              tile.Base = TileType.BAR_T;
              tile.Act = TileType.BAR_T;
              //tile.bitmap.name = "rope";
              break;
            case TileType.TRAP_T: //False brick
              tile.Base = TileType.TRAP_T; //behavior same as empty
              tile.Act = TileType.TRAP_T;
              //tile.bitmap.name = "brick";
              break;
            case TileType.HLADR_T: //Ladder appears at end of level
              tile.Base = TileType.HLADR_T; //behavior same as empty before end of level
              tile.Act = TileType.EMPTY_T; //behavior same as empty before end of level

              //tile.bitmap.name = "ladder";
              //tile.bitmap.setAlpha(0); //hide the laddr
              break;
            case TileType.GOLD_T: //Gold chest
              tile.Base = TileType.GOLD_T; //keep gold on base map
              tile.Act = TileType.EMPTY_T;
              //tile.bitmap.name = "gold";
              map.GoldCount += 1;
              break;
            case TileType.GUARD_T: //Guard
              tile.Base = TileType.EMPTY_T;
              tile.Act = TileType.GUARD_T;
              //tile.bitmap = null;
              if ((--mapGuards) >= maxGuard)
              {
                tile.Act = TileType.EMPTY_T; //too many guards, set this tile as empty
              }
              else
              {
                map.GuardCount += 1;
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
              tile.Base = TileType.EMPTY_T;
              tile.Act = TileType.RUNNER_T;
              //tile.bitmap = null;
              tile.Action = Action.Unknown;
              if (map.Runner != null)
              {
                tile.Act = TileType.EMPTY_T; //too many runner, set this tile as empty
              }
              else
              {
                map.Runner = tile;

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
              tile.Base = TileType.EMPTY_T;
              tile.Act = TileType.EMPTY_T;
              //tile.bitmap = null;
              break;
            case TileType.FINISH_T:
              tile.Base = TileType.FINISH_T;
              tile.Act = TileType.EMPTY_T;
              break;
            case TileType.TELEPORT_T:
              tile.Base = TileType.TELEPORT_T;
              tile.Act = TileType.TELEPORT_T;
              var index = map.Teleports[0] == null ? 0 : 1;
              map.Teleports[index] = tile;
              tile.Index = index;
              break;
            default:
              throw new ArgumentException("not valid level tile type");
          }

          x++;
        }
      }

      Assert.IsTrue(mapGuards == 0, "Error: mapCuardCount design error !");

      return map;
    }
  }
}
