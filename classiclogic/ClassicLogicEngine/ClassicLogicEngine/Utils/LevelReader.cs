using System;
namespace ClassicLogic.Utils
{
  public class LevelReader
  {
    readonly string _data;

    public LevelReader(string data)
    {
      _data = data;
    }

    public char this[int index]
    {
      get
      {
        return _data[index];
      }
    }
  }
}
