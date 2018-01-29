using System;
using System.Collections;
using UnityEngine;

namespace Game.Components.MainMenu
{
  public class MainMenuTVComponent : MonoBehaviour
  {
    public enum State
    {
      None,
      In,
      Out
    }

    public MainMenuTVSceneComponent PreviewScene;
    public MainMenuPreviewComponent Preview;

    public Renderer Renderer;
    public float InTime = 1f;
    public float OutTime = 0.1f;
    public float MaxEmission = 1f;

    private Material _material;
    private float _passedTime;
    private float _ratio;
    private State _state;
    private Coroutine _coroutine;

    public bool Activated { get; private set; }

    public void Activate()
    {
      if (!Activated)
      {
        Activated = true;
        PreviewScene.Activate(Preview);
        _state = State.In;
        Animate();
      }
    }

    public void Deactivate()
    {
      Activated = false;
      PreviewScene.Deactivate(Preview);
      _state = State.Out;
      Animate();
    }

    private void Animate()
    {
      if (_coroutine != null)
      {
        StopCoroutine(_coroutine);
      }
      _coroutine = StartCoroutine(AnimateCoroutine());
    }

    private IEnumerator AnimateCoroutine()
    {
      float totalTime;
      if (_state == State.In) totalTime = InTime;
      else if (_state == State.Out) totalTime = OutTime;
      else yield break;

      _passedTime = _ratio * totalTime;
      _ratio = _passedTime / totalTime;
      UpdateMaterial(_ratio);

      while (true)
      {
        yield return null;
        if (_state == State.In)
        {
          _passedTime += Time.deltaTime;
          _ratio = Math.Max(0, Math.Min(1, _passedTime / totalTime));
          UpdateMaterial(_ratio);
          if (_ratio >= 1)
          {
            _coroutine = null;
            yield break;
          }
        }
        else if (_state == State.Out)
        {
          _passedTime -= Time.deltaTime;
          if (_passedTime < 0) _passedTime = 0;
          _ratio = Mathf.Max(0, Math.Min(1, _passedTime / totalTime));
          UpdateMaterial(_ratio);
          if (_ratio <= 0)
          {
            _coroutine = null;
            yield break;
          }
        }
        else
        {
          yield break;
        }
      }
    }

    private void UpdateMaterial(float ratio)
    {
      var value = ratio * MaxEmission;
      Material.SetVector("_EmissionColor", new Vector4(value, value, value, 1));
    }

    private Material Material
    {
      get
      {
        return _material ?? (_material = Renderer.material);
      }
    }
  }
}
