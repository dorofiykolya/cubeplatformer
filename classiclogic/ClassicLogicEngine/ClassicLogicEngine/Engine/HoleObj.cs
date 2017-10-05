namespace ClassicLogic.Engine
{
  public class HoleObj
  {
    public Sprite sprite;
    public int digLimit;
    public Action action;
    public Position pos = new Position();
    public int[] shapeFrame;
    public int curFrameIdx;

    public HoleObj(Tile[][] map, EngineState state, SpriteSheet spriteSheet)
    {
      sprite = new Sprite(spriteSheet);
    }

    public void removeFromScene()
    {

    }

    public void addToScene()
    {

    }
  }
}