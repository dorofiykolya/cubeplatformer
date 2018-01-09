using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Prototypes.Elements
{
  public class ElementPreviewComponent : MonoBehaviour
  {
    private ElementEntity _element;

    private void Awake()
    {
      _element = GetComponent<ElementComponent>().Element;
    }
  }
}
