using UnityEngine;

namespace BitBenderGames
{
    [RequireComponent(typeof(MobileTouchCamera))]
    public class MobileCameraAngle : MonoBehaviour
    {
        [SerializeField]
        private MobileTouchCamera _touchCamera;
        [SerializeField]
        private Axes _zoomAxe;

        [SerializeField]
        private Axes _rotateAxe;
        [SerializeField]
        private float _rotateMin;
        [SerializeField]
        private float _rotateMax;

        private Transform _transform;

        private void Awake()
        {
            if (_touchCamera == null) _touchCamera = GetComponent<MobileTouchCamera>();
            _transform = GetComponent<Transform>();
        }

        public void Update()
        {
            if (_touchCamera.PerspectiveZoomMode == PerspectiveZoomMode.TRANSLATION)
            {
                var currZoom = 0f;
                var minZoom = _touchCamera.CamZoomMin;
                var maxZoom = _touchCamera.CamZoomMax;
                switch (_zoomAxe)
                {
                    case Axes.X:
                        currZoom = _transform.localPosition.x;
                        break;
                    case Axes.Y:
                        currZoom = _transform.localPosition.y;
                        break;
                    case Axes.Z:
                        currZoom = _transform.localPosition.z;
                        break;
                }
                var clamped = Mathf.Clamp(currZoom, minZoom, maxZoom) - minZoom;
                var clampedMax = maxZoom - minZoom;
                var ratio = clamped / clampedMax;

                var axeRotation = Mathf.Lerp(_rotateMin, _rotateMax, ratio);
                var rotation = _transform.localRotation;
                var euler = rotation.eulerAngles;
                switch (_rotateAxe)
                {
                    case Axes.X:
                        euler.x = axeRotation;
                        break;
                    case Axes.Y:
                        euler.y = axeRotation;
                        break;
                    case Axes.Z:
                        euler.z = axeRotation;
                        break;
                }
                rotation.eulerAngles = euler;
                _transform.localRotation = rotation;
            }
        }

        public enum Axes
        {
            X,
            Y,
            Z
        }
    }
}
