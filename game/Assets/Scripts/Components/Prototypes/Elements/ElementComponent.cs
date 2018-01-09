using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Prototypes.Components;
using Game.Components.Prototypes;
using UnityEngine;

namespace Game.Prototypes.Elements
{
  public class ElementComponent : MonoBehaviour
  {
    public ElementEntity Element;

    private SphereCollider _collider;

    private void Awake()
    {
      _collider = GetComponent<SphereCollider>();
    }

    private void OnCollisionStay(Collision collision)
    {
      var material = collision.gameObject.GetComponent<MaterialComponent>();
      if (material)
      {
        MaterialStateRules.Apply(material.Material, Element);
      }
      else
      {
        var element = collision.gameObject.GetComponent<ElementComponent>();
        if (element && element != this)
        {
          ElementStateRules.Apply(Element, element.Element);
        }
      }
    }

    private void OnCollisionEnter(Collision collision)
    {
      var material = collision.gameObject.GetComponent<MaterialComponent>();
      if (material)
      {
        MaterialStateRules.Apply(material.Material, Element);
      }
      else
      {
        var element = collision.gameObject.GetComponent<ElementComponent>();
        if (element && element != this)
        {
          ElementStateRules.Apply(Element, element.Element);
        }
      }
    }
  }
}
