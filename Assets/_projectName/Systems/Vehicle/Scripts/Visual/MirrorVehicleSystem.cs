using UnityEngine;
namespace Systems.Vehicle.Visual.Mirror
{
    public class MirrorVehicleSystem : MonoBehaviour
    {
        [SerializeField] private Camera _left;
        [SerializeField] private Camera _right;
        [SerializeField] private Camera _center;
        private void Start()
        {
            OffMirror();
        }
        public void OffMirror()
        {
            _left.transform.parent = transform;
            _left.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _right.transform.parent = transform;
            _right.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _center.transform.parent = transform;
            _center.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _left.gameObject.SetActive(false);
            _right.gameObject.SetActive(false);
            _center.gameObject.SetActive(false);
        }
        public void OnMirror(GameObject left = null, GameObject right = null, GameObject center = null)
        {
            if (left != null)
            {
                _left.transform.parent = left.transform;
                _left.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _left.gameObject.SetActive(true);
            }
            if (right != null)
            {
                _right.transform.parent = right.transform;
                _right.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _right.gameObject.SetActive(true);
            }
            if (center != null)
            {
                _center.transform.parent = center.transform;
                _center.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _center.gameObject.SetActive(true);
            }
        }
    }
}