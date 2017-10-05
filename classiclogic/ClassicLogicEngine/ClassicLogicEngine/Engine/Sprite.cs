using System.Collections.Generic;
using System.Linq;

namespace ClassicLogic.Engine
{
  public class Sprite
  {
    private readonly List<System.Action> _events = new List<System.Action>();
    private readonly SpriteSheet _spriteSheet;
    private bool _isPlay;
    private SheetAnimation _animation;
    private Shape _current;

    public int currentAnimationFrame;
    public int curFrameIdx;
    public int curFrameTime;

    public Position pos = new Position();

    public Sprite(SpriteSheet spriteSheet)
    {
      _spriteSheet = spriteSheet;
    }

    public void gotoAndStop(Shape shape)
    {
      stop();
      if (_current != shape)
      {
        _current = shape;
        _animation = _spriteSheet[shape];
        currentAnimationFrame = 0;
      }
    }

    public void gotoAndStop(int frame)
    {
      currentAnimationFrame = frame;
      stop();
    }

    public void removeAllEventListeners()
    {
      _events.Clear();
    }

    public void gotoAndPlay(Shape shape)
    {
      if (_current != shape)
      {
        _current = shape;
        _animation = _spriteSheet[shape];
        currentAnimationFrame = 0;
      }
      play();
    }

    public void setTransform(double x, double y)
    {

    }

    public void onAnimationEnded(System.Action action)
    {
      if (!_events.Contains(action))
      {
        _events.Add(action);
      }
    }

    public void play()
    {
      _isPlay = true;
    }

    public void stop()
    {
      _isPlay = false;
    }

    public void removeFromScene()
    {

    }

    public void tick(int ticks)
    {
      if (_isPlay)
      {
        currentAnimationFrame += ticks;
        while (currentAnimationFrame >= _animation.frames.Length)
        {
          currentAnimationFrame -= _animation.frames.Length;
          var frame = currentAnimationFrame % _animation.frames.Length;
          if (_animation.next != null)
          {
            gotoAndPlay(_animation.next.Value);
          }
          currentAnimationFrame = frame;
          _events.ToList().ForEach(a => a());
        }
      }
    }

    //to hide
    public void setAlpha(double value)
    {

    }
  }
}