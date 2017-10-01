using System;
using LodeRunnerGame;
namespace ClassicLogic.Engine
{
  public class Tile
  {
    public Type act;
    public Type @base;
    public Action action;

    public Bitmap bitmap = new Bitmap();
    public Sprite sprite = new Sprite();

    public Tile()
    {
    }
  }
}
