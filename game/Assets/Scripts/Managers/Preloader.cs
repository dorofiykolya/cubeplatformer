using System;
using Utils;

namespace Game
{
  public class Preloader
  {
    private readonly Signal _onOpen;
    private readonly Signal _onClose;
    private int _opened;

    public Preloader(Lifetime lifetime)
    {
      _onOpen = new Signal(lifetime);
      _onClose = new Signal(lifetime);
    }

    public bool Opened
    {
      get { return _opened > 0; }
    }

    public void SubscribeOnOpen(Lifetime lifetime, Action listener)
    {
      _onOpen.Subscribe(lifetime, listener);
    }

    public void SubscribeOnClose(Lifetime lifetime, Action listener)
    {
      _onClose.Subscribe(lifetime, listener);
    }

    public void Open(Lifetime lifetime)
    {
      lifetime.AddAction(CloseInternal);
      OpenInternal();
    }

    private void OpenInternal()
    {
      if (_opened == 0)
      {

      }
      _opened++;
    }

    private void CloseInternal()
    {
      if (_opened > 0)
      {
        _opened--;
        if (_opened == 0)
        {

        }
      }
    }
  }
}