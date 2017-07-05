using Game.Components;
using UnityEngine;

namespace Game.Views.Controllers
{
  public class ViewPreloaderController : ViewController
  {
    private GamePreloaderComponent _preloader;

    protected override void Initialize()
    {
      if (Context.Preloader.Opened)
      {
        Open();
      }
      Context.Preloader.SubscribeOnClose(Lifetime, Close);
      Context.Preloader.SubscribeOnOpen(Lifetime, Open);
    }

    protected override void OnDispose()
    {
      if (_preloader != null)
      {
        GameObject.Destroy(_preloader.gameObject);
        _preloader = null;
      }
    }

    private void Open()
    {
      if (_preloader == null)
      {
        _preloader = GameObject.Instantiate(Context.Providers.Preloader.GetPrefab());
      }
      else
      {
        _preloader.gameObject.SetActive(true);
      }
    }

    private void Close()
    {
      if (_preloader != null)
      {
        _preloader.gameObject.SetActive(false);
      }
    }
  }
}