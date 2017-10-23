namespace ClassicLogic.Engine
{
  public class Sprite
  {
    private Shape _current;

    public int CurrentFrame;
    public int CurrentFrameId;
    public int CurrentFrameTime;

    public Position Position = new Position();

    public void GotoAndStop(Shape shape)
    {
      //if (_current != shape)
      {
        _current = shape;
        CurrentFrame = 0;
      }
    }

    public void GotoAndStop(int frame)
    {
      CurrentFrame = frame;
    }

    public void GotoAndPlay(Shape shape)
    {
      //if (_current != shape)
      {
        _current = shape;
        CurrentFrame = 0;
      }
    }
  }
}