namespace ClassicLogic.Engine
{
  public class Guard
  {
    private readonly int _id;
    public Sprite sprite;
    public Position pos = new Position();
    public Action action = Action.ACT_STOP;
    public Action lastLeftRight = Action.ACT_LEFT;
    public int hasGold = 0;
    public Shape shape = Shape.runLeft;
    public int curFrameIdx;
    public int curFrameTime;
    public int[] shapeFrame;
    public Point holePos;

    public Guard(int x, int y, int id, SpriteSheet spriteSheet)
    {
      _id = id;
      pos.x = x;
      pos.y = y;
      pos.xOffset = pos.yOffset = 0;

      sprite = new Sprite(spriteSheet);
      sprite.gotoAndPlay(Shape.runRight);
    }

    public int Id { get { return _id; } }

    public void setTransform(int x, int y, double xOffset, double yOffset)
    {
      pos.x = x;
      pos.y = y;
      pos.xOffset = xOffset;
      pos.yOffset = yOffset;
    }
  }
}