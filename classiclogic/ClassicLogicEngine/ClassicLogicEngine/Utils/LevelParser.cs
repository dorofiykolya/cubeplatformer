using ClassicLogic.Engine;

namespace ClassicLogic.Utils
{
  public class LevelParser
  {
    public static LevelMap Parse(LevelReader levelMap, int maxGuard)
    {
      var index = 0;

      //(1) create empty map[x][y] array;
      var map = new LevelMap();
      map.maxGuard = maxGuard;

      var mapGuards = 0;

      for (var x = 0; x < Constants.NO_OF_TILES_X; x++)
      {
        for (var y = 0; y < Constants.NO_OF_TILES_Y; y++)
        {
          if (levelMap[index++] == '0') mapGuards++;
        }
      }

      //(2) draw map
      index = 0;
      for (var y = 0; y < Constants.NO_OF_TILES_Y; y++)
      {
        for (var x = 0; x < Constants.NO_OF_TILES_X; x++)
        {
          var id = levelMap[index++];
          var tile = map[x][y];

          switch (id)
          {
            case '#': //Normal Brick
              tile.@base = TileType.BLOCK_T;
              tile.act = TileType.BLOCK_T;
              //tile.bitmap.name = "brick";
              break;
            case '@': //Solid Brick
              tile.@base = TileType.SOLID_T;
              tile.act = TileType.SOLID_T;
              //tile.bitmap.name = "solid";
              break;
            case 'H': //Ladder
              tile.@base = TileType.LADDR_T;
              tile.act = TileType.LADDR_T;
              //tile.bitmap.name = "ladder";
              break;
            case '-': //Line of rope
              tile.@base = TileType.BAR_T;
              tile.act = TileType.BAR_T;
              //tile.bitmap.name = "rope";
              break;
            case 'X': //False brick
              tile.@base = TileType.TRAP_T; //behavior same as empty
              tile.act = TileType.TRAP_T;
              //tile.bitmap.name = "brick";
              break;
            case 'S': //Ladder appears at end of level
              tile.@base = TileType.HLADR_T; //behavior same as empty before end of level
              tile.act = TileType.EMPTY_T; //behavior same as empty before end of level

              //tile.bitmap.name = "ladder";
              //tile.bitmap.setAlpha(0); //hide the laddr
              break;
            case '$': //Gold chest
              tile.@base = TileType.GOLD_T; //keep gold on base map
              tile.act = TileType.EMPTY_T;
              //tile.bitmap.name = "gold";
              map.goldCount += 1;
              break;
            case '0': //Guard
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
            case '&': //Player
              tile.@base = TileType.EMPTY_T;
              tile.act = TileType.RUNNER_T;
              //tile.bitmap = null;
              tile.action = Action.ACT_UNKNOWN;
              if (map.runner != null)
              {
                map[x][y].act = TileType.EMPTY_T; //too many runner, set this tile as empty
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
            case ' ': //empty
              tile.@base = TileType.EMPTY_T;
              tile.act = TileType.EMPTY_T;
              //tile.bitmap = null;
              break;
            default:
              throw new System.ArgumentException();
          }
        }
      }

      Assert.IsTrue(mapGuards == 0, "Error: mapCuardCount design error !");

      return map;
    }
  }
}
