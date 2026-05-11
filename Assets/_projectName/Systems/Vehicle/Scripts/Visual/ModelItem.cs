using UnityEngine;
namespace Systems.Vehicle.Visual
{
    [System.Serializable]
    public class LightSystem
    {
        [Header("front")]
        [SerializeField] private Light left;
        [SerializeField] private Light right;
        [Header("front long")]
        [SerializeField] private Light leftL;
        [SerializeField] private Light rightL;
        public void SetActive(bool value1, bool value2)
        {
            if (left == null) return;
            left.gameObject.SetActive(value1);
            right.gameObject.SetActive(value1);
            if (leftL == null) return;
            leftL.gameObject.SetActive(value2);
            rightL.gameObject.SetActive(value2);
        }
    }
    public class ModelItem : MonoBehaviour
    {
        [field:SerializeField] public Trailer.Attach.Core.Attachment[] Attachments{ get; private set; }
        [Header("GearBox")]
        private GameObject _gearBoxVisual;
        private int _angleX;
        private int _angleZ;
        [SerializeField] private Transform _positionGearBox;
        [SerializeField] private Vector3 _centerOfMass;
        public Vector3 CenterOfMass=> _centerOfMass;
        public LightSystem LightSystem;
        [Header("CameraParentView")]
        [SerializeField] private GameObject _thirthView;
        public Transform ThirdView=>_thirthView.transform;
        [Header("zones")]
        [SerializeField] private ZonePassenger[] _zonePassenger;
        public int ZonePassengerCount => _zonePassenger.Length;
        public ZonePassenger GetZonePassenger(int i) => _zonePassenger[i];
        [Header("Mirror")]
        [SerializeField] private GameObject _leftMirrorView;
        [SerializeField] private GameObject _rightMirrorView;
        [SerializeField] private GameObject _centerMirrorView;
        public GameObject LeftMirrorView => _leftMirrorView;
        public GameObject RightMirrorView => _rightMirrorView;
        public GameObject CenterMirrorView => _rightMirrorView;
        public GameObject _leftMirror;
        public GameObject _rightMirror;
        public GameObject _centerMirror;
        [Header("steeringWheel")]
        [SerializeField] private GameObject _steeringWheel;
        [SerializeField] private float _steeringWheelMaxRotate = 0;
        [SerializeField] private bool _steeringWheelUseY;
        [SerializeField] private float _targetX = -150;
        public void SteerWheel(float steer)
        {
            if (_steeringWheel == null)
            {
                return;
            }
            if (_steeringWheelUseY)
                _steeringWheel.transform.localRotation = Quaternion.Euler(_targetX, steer * _steeringWheelMaxRotate, 0);
            else
                _steeringWheel.transform.localRotation = Quaternion.Euler(_targetX, 0, steer * _steeringWheelMaxRotate);
        }
        public void SetActiveMirrorView(bool active = false)
        {
            if (_leftMirrorView != null)
            {
                _leftMirrorView.SetActive(active);
            }
            if (_rightMirrorView != null)
            {
                _rightMirrorView.SetActive(active);
            }
            if (_centerMirrorView != null)
            {
                _centerMirrorView.SetActive(active);
            }
        }
        public void SetGearBoxValue(GameObject prefab, int x, int z)
        {
            _angleX = x;
            _angleZ = z;
            _gearBoxVisual = Instantiate(original:prefab, parent: _positionGearBox);
        }
        public void UpdateGearBox(int x, int z) {
            _gearBoxVisual.transform.localRotation = Quaternion.Euler(z * _angleZ, 0, -x * _angleX);
        }
    }
}
