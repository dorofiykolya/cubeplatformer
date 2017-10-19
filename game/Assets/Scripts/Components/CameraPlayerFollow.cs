using Cinemachine;
using UnityEngine;

namespace Game.Components
{
  public class CameraPlayerFollow : MonoBehaviour
  {
    [SerializeField]
    private CinemachineVirtualCamera _camera;

    private void Awake()
    {
      if (_camera == null)
      {
        _camera = GetComponent<CinemachineVirtualCamera>();
      }
    }

    private void Update()
    {
      var player = GameObject.FindGameObjectWithTag("Player");
      if (player != null)
      {
        _camera.Follow = player.transform;
      }
    }
  }
}
