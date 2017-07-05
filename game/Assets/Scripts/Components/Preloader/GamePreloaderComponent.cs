using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Extensions;

namespace Game.Components
{
  public class GamePreloaderComponent : MonoBehaviour
  {
    [Header("Fades")]
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Material _cubeMaterial;
    [SerializeField]
    private Text _text;
    [SerializeField]
    [Header("Time")]
    private float _showTime = 1f;
    [SerializeField]
    private float _hideTime = 1f;

    private Coroutine _showFade;
    private Coroutine _hideFade;

    public void Show()
    {
      if (_hideFade != null)
      {
        StopCoroutine(_hideFade);
        _hideFade = null;
      }
      if (_showFade == null)
      {
        if (!gameObject.activeSelf)
        {
          gameObject.SetActive(true);
        }
        _showFade = StartCoroutine(ShowFade());
      }
    }

    public void Hide()
    {
      if (gameObject.activeSelf)
      {
        if (_showFade != null)
        {
          StopCoroutine(_showFade);
          _showFade = null;
        }
        if (_hideFade == null)
        {
          _hideFade = StartCoroutine(HideFade());
        }
      }
    }

    private void OnEnable()
    {
      _image.color = _image.color.A(1);
      _cubeMaterial.color = _cubeMaterial.color.A(1);
      _text.color = _text.color.A(1);
    }

    private float _passedTime;

    private IEnumerator ShowFade()
    {
      _passedTime = 0;
      while (true)
      {
        _passedTime += Time.deltaTime;
        SetAlpha(_passedTime / _showTime, 1f);
        if (_passedTime >= _showTime)
        {
          yield break;
        }
        yield return null;
      }
    }

    private IEnumerator HideFade()
    {
      _passedTime = 0;
      while (true)
      {
        _passedTime += Time.deltaTime;
        SetAlpha(_passedTime / _hideTime, 0f);
        if (_passedTime >= _hideTime)
        {
          gameObject.SetActive(false);
          yield break;
        }
        yield return null;
      }
    }

    private void SetAlpha(float ratio, float alpha)
    {
      _image.color = _image.color.A(Mathf.Lerp(_image.color.a, alpha, ratio));
      _cubeMaterial.color = _cubeMaterial.color.A(Mathf.Lerp(_image.color.a, alpha, ratio));
      _text.color = _text.color.A(Mathf.Lerp(_image.color.a, alpha, ratio));
    }
  }
}