using UnityEngine;
namespace Systems.Vehicle.Visual
{
    [System.Serializable]
    public class ZonePassenger
    {
        [SerializeField] private bool _moveable = false;
        [SerializeField] private int _passengerCount;
        [SerializeField] private VehicleVisualDoor[] _doors;
        [SerializeField] private VehicleVisualChair[] _chairs;
        public bool Moveable => _moveable;
        public int PassengerCount => _passengerCount;
        public int DoorsCount => _doors.Length;
        public int ChairsCount => _chairs.Length;
        public VehicleVisualDoor getDoor(int i) => _doors[i];
        public VehicleVisualChair getChair(int i) => _chairs[i];
    }
}