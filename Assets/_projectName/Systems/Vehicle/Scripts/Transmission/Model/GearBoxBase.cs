namespace Systems.Vehicle.Transmission.Model
{
    public abstract class GearBoxBase
    {
        public abstract int GetGear { get; }
        public abstract int GetMaxGear { get; }
        protected Input.VehicleInput _input;
        protected readonly ContainerData.VehicleContainerData _vehicleContainerData;
        protected readonly TorqueConverter.TorqueConverterBase _torqueConverter;
        private float _currentGearKoef;
        private readonly bool _hasLowRange = false;
        private readonly float _lowRangeRatio = 3;
        private bool _lowRange = false;
        public GearBoxBase(Data.VehicleData vehicleData, ContainerData.VehicleContainerData vehicleContainerData, TorqueConverter.TorqueConverterBase torqueConverter)
        {
            _vehicleContainerData = vehicleContainerData;
            _torqueConverter = torqueConverter;

            _hasLowRange = vehicleData.HasLowRange;
            _lowRangeRatio = vehicleData.LowRangeRatio;
        }
        private void EnableEvent()
        {
            if (_input == null) return;
            if (_hasLowRange)
            {
                _input.EventChangeLowGear += SetLowGear;
            }
        }
        private void DisableEvent()
        {
            if (_input == null) return;
            if (_hasLowRange)
            {
                _input.EventChangeLowGear -= SetLowGear;
            }
        }
        public void SetInput(Input.VehicleInput input)
        {
            DisableEvent();
            _input = input;
            EnableEvent();
        }
        public virtual bool TrySetGear(int newGear)
        {
            return false;
        }
        private void SetLowGear()
        {
            _lowRange = !_lowRange;
            _vehicleContainerData.SetLowGear(_lowRange);
            UpdateGearBox();
        }
        protected virtual void SetGear(int gear) { }
        protected void UpdateGearBox()
        {
            _currentGearKoef = GetKoef() * (_lowRange ? _lowRangeRatio : 1);
            _torqueConverter.SetGearKoef(_currentGearKoef);
        }
        protected virtual float GetKoef()
        {
            return 0;
        }
        public void SetTransmissionRPM(float RPM)
        {
            _torqueConverter.SetGearBoxShaftRPM(RPM * _currentGearKoef);
        }
        public float GetTorque()
        {
            return _torqueConverter.GetTorqueFromEngine() * _currentGearKoef;
        }

    }
}