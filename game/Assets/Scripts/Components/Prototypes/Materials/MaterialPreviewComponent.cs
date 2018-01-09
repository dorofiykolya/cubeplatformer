using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Prototypes;
using Game.Prototypes.Components;
using Game.Components.Prototypes;
using UnityEngine;

namespace Assets.Scripts
{
  [RequireComponent(typeof(Renderer))]
  public class MaterialPreviewComponent : MonoBehaviour
  {
    public ParticleSystem BurnEffect;
    public Material BurnMaterial;
    public Material DryMaterial;
    public Material WetMaterial;
    public Material BurntMaterial;
    public Material FreezeMaterial;
    public Material ElectricutyMaterial;
    public Material FriedMaterial;

    private MaterialEntity _material;
    private MaterialState _state;
    private Renderer _renderer;

    private void Awake()
    {
      _material = GetComponent<MaterialComponent>().Material;
      _renderer = GetComponent<Renderer>();
      _state = _material.State;
      ApplyState(_state);
    }

    private void Update()
    {
      if (_material.State != _state)
      {
        _state = _material.State;
        ApplyState(_state);
      }
    }

    private void ApplyState(MaterialState state)
    {
      if (state == MaterialState.Burn)
      {
        BurnEffect.Play(true);
        _renderer.material = BurnMaterial;
      }
      else
      {
        BurnEffect.Stop();
        switch (state)
        {
          case MaterialState.Burn:
            _renderer.material = BurnMaterial;
            break;
          case MaterialState.Burnt:
            _renderer.material = BurntMaterial;
            break;
          case MaterialState.Dry:
            _renderer.material = DryMaterial;
            break;
          case MaterialState.Electricity:
            _renderer.material = ElectricutyMaterial;
            break;
          case MaterialState.Freeze:
            _renderer.material = FreezeMaterial;
            break;
          case MaterialState.Wet:
            _renderer.material = WetMaterial;
            break;
          case MaterialState.Fried:
            _renderer.material = FriedMaterial;
            break;
        }
      }
    }
  }
}
