namespace Systems.Vehicle.Input
{
    public abstract class VehicleInput : TrailerInput
    {
        public event System.Action EventChangeDifferentials;
        public event System.Action EventChangeHandBrake;
        public event System.Action EventStartEngine;
        public event System.Action EventStopStarter;
        public event System.Action EventChangeSteeringWheelController;
        public event System.Action EventChangeSteeringWheelControllerRaw;
        public event System.Action<int> EventChangeSequentialGear;
        public event System.Action<int> EventChangeGear;
        public event System.Action EventChangeLowGear;
        public event System.Action<int, int> EventChangeShiftXZGear;
        public event System.Action<int> EventChangeShiftXGearDelta;
        public event System.Action<int> EventChangeShiftZGearDelta;
        private bool _rawSteering = false;
        private bool _active = false;
        protected float _steering = 0;
        protected float _clutch = 1;
        protected float _braking = 0;
        protected float _accelerator = 0;
        public bool Active => _active;
        public float Steering => _steering;
        public float Clutch => _clutch;
        public float Braking => _braking;
        public float Accelerator => _accelerator;
        public bool RawSteering => _rawSteering;
        public void SetAcceleratorFake(float value) => _accelerator = value;
        public void SetActive(bool value)
        {
            _active = value;
            if (!value)
            {
                _clutch = 1;
                _braking = 0;
                _accelerator = 0;
                _steering = 0;
            }
        }
        protected void AcivateEventChangeHandBrake()
        {
            EventChangeHandBrake?.Invoke();
        }
        protected void AcivateEventStartStarter()
        {
            EventStartEngine?.Invoke();
        }
        protected void AcivateEventStopStarter()
        {
            EventStopStarter?.Invoke();
        }
        protected void AcivateEventChangeSteeringWheelController()
        {
            EventChangeSteeringWheelController?.Invoke();
        }
        protected void ActivateEventChangeSequentialGear(int delta)
        {
            EventChangeSequentialGear?.Invoke(delta);
        }
        protected void ActivateEventChangeGear(int value)
        {
            EventChangeGear?.Invoke(value);
        }
        protected void ActivateEventChangeShiftXZGear(int x, int z)
        {
            EventChangeShiftXZGear?.Invoke(x, z);
        }
        protected void ActivateEventChangeShiftXGearDelta(int delta)
        {
            EventChangeShiftXGearDelta?.Invoke(delta);
        }
        protected void ActivateEventChangeShiftZGearDelta(int delta)
        {
            EventChangeShiftZGearDelta?.Invoke(delta);
        }
        protected void ActivateEventChangeLowGear()
        {
            EventChangeLowGear?.Invoke();
        }
        protected void SetRawSteering(bool value)
        {
            _rawSteering = value;
            if (_active) EventChangeSteeringWheelControllerRaw.Invoke();
        }
    }
    public class VehicleInputNullRef : VehicleInput
    {
        public VehicleInputNullRef()
        {
            _brakingTrailer = 0;
            _clutch = 1;
            _braking = 0;
            _accelerator = 0;
        }
    }
}