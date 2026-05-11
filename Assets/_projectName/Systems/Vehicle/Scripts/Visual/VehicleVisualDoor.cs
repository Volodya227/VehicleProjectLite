using UnityEngine;
namespace Systems.Vehicle.Visual
{
    [System.Serializable]
    public class VehicleVisualDoor
    {
        [SerializeField] private Transform _exitPosition;
        [SerializeField] private Collider _door;
        [SerializeField] private Transform _enterPosition;
        public Collider Door => _door;
        public Transform ExitPoint=>_exitPosition;
        public Transform EnterPoint=>_enterPosition;
    }
}