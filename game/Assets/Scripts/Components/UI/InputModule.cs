using System;
using UnityEngine.EventSystems;
using Utils;

namespace Game.Components
{
  public class InputModule : StandaloneInputModule
  {
    private Signal _onPreProcess;
    private Signal _onPostProcess;
    private Lifetime.Definition _definition;

    protected override void OnDestroy()
    {
      if (_definition != null)
      {
        var def = _definition;
        _definition = null;
        def.Terminate();
      }
      base.OnDestroy();
    }

    public override void Process()
    {
      InitializeSignals();
      _onPreProcess.Fire();
      base.Process();
      _onPostProcess.Fire();
    }

    public void SubscribeOnPreProcess(Lifetime lifetime, Action listener)
    {
      InitializeSignals();
      _onPreProcess.Subscribe(lifetime, listener);
    }

    public void SubscribeOnPostProcess(Lifetime lifetime, Action listener)
    {
      InitializeSignals();
      _onPostProcess.Subscribe(lifetime, listener);
    }

    private void InitializeSignals()
    {
      if (_definition == null)
      {
        _definition = Lifetime.Define(Lifetime.Eternal);
        _onPostProcess = new Signal(_definition.Lifetime);
        _onPreProcess = new Signal(_definition.Lifetime);
      }
    }
  }
}
