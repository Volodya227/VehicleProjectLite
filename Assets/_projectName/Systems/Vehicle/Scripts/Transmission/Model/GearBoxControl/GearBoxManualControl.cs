using System.Collections.Generic;
using UnityEngine;
namespace Systems.Vehicle.Transmission.Model.GearBoxControl
{
    public class GearBoxManualControl : GearBoxControlBase
    {
        protected bool _isManual;
        private int _currentGear = 0;
        private readonly ContainerData.VehicleContainerData _vehicleContainerData;
        private readonly Vector2Int _positionXLimit;
        private readonly Vector2Int _positionZLimit;
        private readonly List<Vector2Int> _forwardGears = new();
        private readonly List<Vector2Int> _reverseGears = new();

        private int _newX;
        private int _newZ;
        private int _newGear;
        private int _newAbstaractGear;
        private int _currentAbstaractGear;
        public GearBoxManualControl(Data.GearBoxManualControlData config, ContainerData.VehicleContainerData vehicleContainerData)
        {
            _isManual = true;
            _vehicleContainerData = vehicleContainerData;
            _positionXLimit = config.XLimit;
            _positionZLimit = config.ZLimit;
            foreach (Vector2Int item in config.ForwardGearPositions) { _forwardGears.Add(item); }
            foreach (Vector2Int item in config.ReverseGearPositions) { _reverseGears.Add(item); }
            _currentGear = GetGear(_positionX, _positionZ, _isManual);
        }
        protected override void NewPosition(int x, int z)
        {
            _newX = Mathf.Clamp(x, _positionXLimit[0], _positionXLimit[1]);
            _newZ = Mathf.Clamp(z, _positionZLimit[0], _positionZLimit[1]);
            _newAbstaractGear = GetGear(_newX, _newZ);
            _newGear = GetGear(_newX, _newZ, _isManual);
            if (_newAbstaractGear != _currentAbstaractGear)
            {
                if (_newAbstaractGear != 0)
                {
                    if (_currentAbstaractGear != 0) return;
                }
                ActivateEventTrySetGear(_newGear);
                return;
            }
            UpdatePosition();
        }
        public override void UpdatePosition()
        {
            _positionX = _newX;
            _positionZ = _newZ;
            _currentGear = _newGear;
            _currentAbstaractGear = _newAbstaractGear;
            _vehicleContainerData.SetPositionGear(_positionX, _positionZ);
        }
        private int GetGear(int x, int z, bool manual = true)
        {
            Vector2Int item = new(x, z);
            for (int i = 0; i < _forwardGears.Count; i++)
            {
                if (_forwardGears[i] == item)
                    return i + 1;
            }
            for (int i = 0; i < _reverseGears.Count; i++)
            {
                if (_reverseGears[i] == item)
                    return -(i + 1);
            }
            if(manual)
                return 0;
            else
            {
                if (_currentGear == 0)
                    return 1;
                return _currentGear;
            }
        }
    }
}