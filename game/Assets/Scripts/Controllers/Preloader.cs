using System;
using System.Collections.Generic;
using Utils;

namespace Game
{
  public class Preloader
  {
    private readonly Signal _onOpen;
    private readonly Signal _onClose;
    private int _opened;
    private HashSet<Lifetime> _lifetimes;

    public Preloader(Lifetime lifetime)
    {
      _lifetimes = new HashSet<Lifetime>();
      _onOpen = new Signal(lifetime);
      _onClose = new Signal(lifetime);

      lifetime.AddAction(() => _lifetimes.Clear());
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
      if (_lifetimes.Add(lifetime))
      {
        lifetime.AddAction(CloseInternal);
        OpenInternal();
      }
    }

    private void OpenInternal()
    {
      _opened++;
      if (_opened == 1)
      {
        _onOpen.Fire();
      }
    }

    private void CloseInternal()
    {
      if (_opened > 0)
      {
        _opened--;
        if (_opened == 0)
        {
          _onClose.Fire();
        }
      }
    }
  }
}