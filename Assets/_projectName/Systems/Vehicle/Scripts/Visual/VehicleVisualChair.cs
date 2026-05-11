using UnityEngine;
namespace Systems.Vehicle.Visual
{
    [System.Serializable]
    public class VehicleVisualChair
    {
        [SerializeField] private Transform _seatPoint;
        [SerializeField] private Transform _outPoint;
        [SerializeField] private Transform _firstPersonView;
        [SerializeField] private Collider _interactCollider;
        [SerializeField] private bool _driver;
        public Transform SeatPoint => _seatPoint;
        public Transform OutPoint => _outPoint;
        public Transform FirstPersonView => _firstPersonView;
        public Collider Collider => _interactCollider;
        public bool Driver => _driver;
    }
}