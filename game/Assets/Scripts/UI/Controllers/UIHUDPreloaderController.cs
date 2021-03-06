﻿using Game.Components;
using Game.UI.Controllers;
using UnityEngine;

namespace Game.Controllers
{
  public class UIHUDPreloaderController : UIHUDController
  {
    private GamePreloaderComponent _preloader;

    protected override void OnInitialize()
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
        Object.Destroy(_preloader.gameObject);
        _preloader = null;
      }
    }

    private void Open()
    {
      if (_preloader == null)
      {
        _preloader = Object.Instantiate(Context.Providers.Preloader.GetPrefab());
        GameObject.DontDestroyOnLoad(_preloader.gameObject);
      }
      else
      {
        _preloader.Show();
      }
    }

    private void Close()
    {
      if (_preloader != null)
      {
        _preloader.Hide();
      }
    }
  }
}