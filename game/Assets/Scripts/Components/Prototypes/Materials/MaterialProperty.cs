using System;
using System.Collections.Generic;
using Game.Prototypes.Elements;
using UnityEngine;

namespace Game.Components.Prototypes
{
  [Serializable]
  public class MaterialProperty : ScriptableObject
  {
    private Dictionary<ElementType, MaterialResistance> _cacheResistance;
    private Dictionary<ElementType, MaterialConductivity> _cacheConductivity;

    public float Density;
    public MaterialResistance[] Resistances;
    public MaterialConductivity[] Conductivities;

    public MaterialConductivity GetConductivity(ElementType elementType)
    {
      if (_cacheConductivity == null)
      {
        _cacheConductivity = new Dictionary<ElementType, MaterialConductivity>();
        if (Resistances != null)
        {
          foreach (var conductivity in Conductivities)
          {
            _cacheConductivity[conductivity.ElementType] = conductivity;
          }
        }
      }
      MaterialConductivity data;
      _cacheConductivity.TryGetValue(elementType, out data);
      return data;
    }

    public MaterialResistance GetResistance(ElementType elementType)
    {
      if (_cacheResistance == null)
      {
        _cacheResistance = new Dictionary<ElementType, MaterialResistance>();
        if (Resistances != null)
        {
          foreach (var resistance in Resistances)
          {
            _cacheResistance[resistance.ElementType] = resistance;
          }
        }
      }
      MaterialResistance data;
      _cacheResistance.TryGetValue(elementType, out data);
      return data;
    }
  }

  [Serializable]
  public class MaterialResistance
  {
    public ElementType ElementType;
    public float Value;
  }

  [Serializable]
  public class MaterialConductivity
  {
    public ElementType ElementType;
    public float Value;
  }
}
