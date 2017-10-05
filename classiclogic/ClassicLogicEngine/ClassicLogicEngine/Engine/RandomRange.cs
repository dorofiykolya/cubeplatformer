using System;

namespace ClassicLogic.Engine
{
  public class RandomRange
  {
    private readonly Random _random;
    private readonly int _items;
    private readonly int _min;
    private readonly int _seedValue;
    private int[] _rndList;
    private int _idx;
    private int _reset;
    private int _seed;

    public RandomRange(int minValue, int maxValue, int seedValue)
    {
      _random = new Random();

      _reset = 0;
      _min = minValue;
      var max = maxValue;
      _seedValue = seedValue;
      if (_min > max)
      {
        var tmp = _min;
        _min = max;
        max = tmp;
      }
      _items = max - _min + 1;

      rndStart();
    }

    public int rndReset()
    {
      return _reset;
    }

    private int seedRandom()
    {
      var x = Math.Sin(_seed++) * 10000;
      return (int)(x - Math.Floor(x));
    }

    private void rndStart()
    {
      _rndList = new int[_items];
      for (var i = 0; i < _items; i++) _rndList[i] = _min + i;
      for (var i = 0; i < _items; i++)
      {
        int swapId;
        if (_seedValue > 0)
        {
          _seed = _seedValue;
          swapId = (seedRandom() * _items) | 0;
        }
        else
        {
          swapId = (int)(_random.NextDouble() * _items);
        }
        var tmp = _rndList[i];
        _rndList[i] = _rndList[swapId];
        _rndList[swapId] = tmp;
      }
      _idx = 0;
    }

    public int get()
    {
      if (_idx >= _items)
      {
        rndStart();
        _reset = 1;
      }
      else
      {
        _reset = 0;
      }
      return _rndList[_idx++];
    }
  }
}