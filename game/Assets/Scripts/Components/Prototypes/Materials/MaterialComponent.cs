using Game.Components.Prototypes;
using UnityEngine;

namespace Game.Prototypes.Components
{
  public class MaterialComponent : MonoBehaviour
  {
    public MaterialEntity Material;

    private void Update()
    {
      Material.Update();
    }
  }
}
