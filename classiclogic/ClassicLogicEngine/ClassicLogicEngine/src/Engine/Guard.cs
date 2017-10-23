namespace ClassicLogic.Engine
{
  public class Guard
  {
    private readonly int _id;
    public Sprite Sprite;
    public Position Position = new Position();
    public Action Action = Action.Stop;
    public Action LastLeftRight = Action.Left;
    public int HasGold = 0;
    public Shape Shape = Shape.RunLeft;
    public int CurFrameIdx;
    public int CurFrameTime;
    public int[] ShapeFrame;
    public Point HolePos;

    public Guard(int x, int y, int id)
    {
      _id = id;
      Position.X = x;
      Position.Y = y;
      Position.XOffset = Position.YOffset = 0;

      Sprite = new Sprite();
      Sprite.GotoAndPlay(Shape.RunRight);
    }

    public int Id { get { return _id; } }

    public void SetTransform(int x, int y, double xOffset, double yOffset)
    {
      Position.X = x;
      Position.Y = y;
      Position.XOffset = xOffset;
      Position.YOffset = yOffset;
    }
  }
}