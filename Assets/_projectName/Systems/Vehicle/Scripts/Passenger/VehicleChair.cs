using UnityEngine;
namespace Systems.Vehicle.Passenger
{
    public class VehicleChair
    {
        private bool _single;
        private Collider _collider;
        private bool _driver;
        private IVehiclePassenger _passenger;
        private Transform _outPoint;
        private Transform _firstPersonView;

        public bool IsFree => _passenger == null;
        public bool Single => _single;
        public bool Driver => _driver;
        public Collider Collider => _collider;
        public Transform OutPoint => _outPoint;
        public Transform FirstPersonView => _firstPersonView;

        public VehicleChair(bool driver, bool single, Collider collider, Transform outPosition, Transform firstPersonView)
        {
            _driver = driver;
            _single = single;
            _collider = collider;
            _outPoint = outPosition;
            _firstPersonView = firstPersonView;
        }
        public void Assign(IVehiclePassenger passenger)
        {
            _passenger = passenger;
        }
        public void Leave()
        {
            _passenger = null;
        }
        public bool IsBusyBy(IVehiclePassenger passenger)
        {
            return _passenger == passenger;
        }
    }
}