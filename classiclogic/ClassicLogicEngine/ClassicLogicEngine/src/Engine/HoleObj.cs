namespace ClassicLogic.Engine
{
  public class HoleObj
  {
    public Sprite Sprite;
    public int DigLimit;
    public Action Action;
    public Position Position = new Position();
    public int[] ShapeFrame;
    public int CurFrameIdx;

    public HoleObj(Tile[][] map, EngineState state)
    {
      Sprite = new Sprite();
    }

    public void RemoveFromScene()
    {

    }

    public void AddToScene()
    {

    }
  }
}