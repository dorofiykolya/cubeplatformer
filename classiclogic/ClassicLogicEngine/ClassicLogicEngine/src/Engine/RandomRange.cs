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
    private int _seed;

    public RandomRange(int minValue, int maxValue, int seedValue)
    {
      _random = new Random();

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

      Calculate();
    }

    public int Get()
    {
      if (_idx >= _items)
      {
        Calculate();
      }
      return _rndList[_idx++];
    }

    private int CalculateSeed()
    {
      var x = Math.Sin(_seed++) * 10000;
      return (int)(x - Math.Floor(x));
    }

    private void Calculate()
    {
      _rndList = new int[_items];
      for (var i = 0; i < _items; i++) _rndList[i] = _min + i;
      for (var i = 0; i < _items; i++)
      {
        int swapId;
        if (_seedValue > 0)
        {
          _seed = _seedValue;
          swapId = (CalculateSeed() * _items) | 0;
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
  }
}