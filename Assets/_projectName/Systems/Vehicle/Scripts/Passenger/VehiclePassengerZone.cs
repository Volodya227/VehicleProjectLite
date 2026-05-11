using System.Collections.Generic;
using UnityEngine;
namespace Systems.Vehicle.Passenger
{
    public class VehiclePassengerZone
    {
        private readonly int _id;
        private readonly bool _moveable;
        private List<VehicleChair> _chairs;
        private List<IVehiclePassenger> _passengers;
        private List<Visual.VehicleVisualDoor> _doors;
        public int Id => _id;
        public bool Moveable => _moveable;
        public int ChairCount => _chairs.Count;
        public int DoorCount => _doors.Count;
        public int PassengerCount => _passengers.Count;
        public VehicleChair Chair(int i) => _chairs[i];
        public IVehiclePassenger Passenger(int i) => _passengers[i];
        public Visual.VehicleVisualDoor Door(int i) => _doors[i];
        public VehiclePassengerZone(int id,bool moveable, int passengers, List<Visual.VehicleVisualDoor> doors, List<VehicleChair> chairs)
        {
            _id = id;
            _moveable = moveable;
            _passengers = new List<IVehiclePassenger>();
            for (int i = 0; i < passengers; i++) { _passengers.Add(null); }
            //Debug.Log(_passengers.Count);
            _chairs = chairs;
            _doors = doors;
        }
        //passenger
        public bool IsFreeForPassenger() {
            bool result = false;
            for (int i = 0; i < PassengerCount; i++)
            {
                if (_passengers[i] == null)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        public bool IsPassengerPresent(IVehiclePassenger passenger) {
            bool result = false;
            for(int i = 0; i < PassengerCount; i++)
            {
                if (_passengers[i]==passenger)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        public int GetPassengerIndex(IVehiclePassenger passenger)
        {
            int result = -1;
            for (int i = 0; i < PassengerCount; i++)
            {
                if (_passengers[i] == passenger)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
        public void SetNewPassenger(IVehiclePassenger passenger)
        {
            for (int i = 0; i < PassengerCount; i++)
            {
                if (_passengers[i] == null)
                {
                    _passengers[i] = passenger;
                    break;
                }
            }
        }
        public void ForgetPassenger(IVehiclePassenger passenger)
        {
            for (int i = 0; i < PassengerCount; i++)
            {
                if (_passengers[i] == passenger)
                {
                    _passengers[i] = null;
                    break;
                }
            }
        }
        //doors
        public int GetDoorIndex(Collider door)
        {
            int index = -1;
            for (int i = 0; i < DoorCount; i++) {
                if (_doors[i].Door == door) {index = i;break; }
            }
            //TODO find index; -1 - this door has not in this zone
            return index;
        }
        //chairs
        public int PassengerInChair(IVehiclePassenger passenger)
        {
            int result = -1;
            for (int i = 0; i < ChairCount; i++) {
                if (_chairs[i].IsBusyBy(passenger))
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
        public int ChairIndex(Collider chair)
        {
            int result = -1;
            for (int i = 0; i < ChairCount; i++)
            {
                if (_chairs[i].Collider == chair)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
        public VehicleChair GetChairFree()
        {
            VehicleChair result = null;
            for (int i = 0; i < ChairCount; i++)
            {
                if (_chairs[i].IsFree)
                {
                    result = _chairs[i];
                    break;
                }
            }
            return result;
        }
        public VehicleChair getChairByPassenger(IVehiclePassenger passenger)
        {
            VehicleChair result = null;
            for (int i = 0; i < ChairCount; i++)
            {
                if (_chairs[i].IsBusyBy(passenger))
                {
                    result = _chairs[i];
                    break;
                }
            }
            return result;
        }
    }
}