using System;
using System.Collections.Generic;
namespace ClassicLogic.Engine
{
  public class LevelMap
  {
    private readonly List<Column> _columns;

    public int guardCount;
    public int goldCount;
    public int maxGuard;
    public Tile runner;

    public LevelMap()
    {
      _columns = new List<Column>();
    }

    public Tile[][] ToArray()
    {
      var result = new Tile[_columns.Count][];

      for (var i = 0; i < _columns.Count; i++)
      {
        result[i] = _columns[i].ToArray();
      }

      return result;
    }

    public Column this[int x]
    {
      get 
      {
        while(_columns.Count <= x)
        {
          _columns.Add(new Column());
        }
        return _columns[x]; 
      }
    }

    public class Column
    {
      private readonly List<Tile> _tiles;

      public Column()
      {
        _tiles = new List<Tile>();
      }

      public Tile this[int y]
      {
        get 
        {
          while(_tiles.Count <= y)
          {
            _tiles.Add(new Tile());
          }
          return _tiles[y]; 
        }
      }

      public Tile[] ToArray()
      {
        return _tiles.ToArray();
      }
    }
}
