namespace Systems.Vehicle.ContainerData
{
    public class VehicleEngineState
    {
        public event System.Action<bool> EventStarterActive;
        public event System.Action<bool> EventActive;
        public int rpm;
        public float load;
        public void SetStarterActive(bool value)
        {
            EventStarterActive?.Invoke(value);
        }
        public void SetActive(bool value) {
            EventActive?.Invoke(value);
        }
    }
    public class VehicleContainerData : IVehicleContainerData
    {
        public event System.Action<float> EventSteeringChange;
        public event System.Action EventPositionGear;
        public event System.Action EventChangeLowGear;
        public event System.Action EventChangeFuelUseable;
        public event System.Action EventChangeHandBrake;
        public VehicleEngineState EngineState { get; private set; }
        private int _speed = 0;
        private int _maxRPM;
        private int _distance;
        private string _gear;
        private bool _lowGear;
        private float _clutch;
        private float _brake;
        private float _accelerator;
        private float _fuel;
        private float _fuelUseable;
        private float _maxFuel;
        private int _gearBoxVisualX;
        private int _gearBoxVisualZ;
        private bool _handBrake;
        //private float _UseFuelPerHour;
        private readonly AxleDataVehicle[] _axleData;
        public float Speed => _speed;
        public int RPM => EngineState.rpm;
        public int MaxRPM => _maxRPM;
        public string Gear => _gear;
        public int Distance => _distance;
        public float Clutch => _clutch;
        public float Brake => _brake;
        public float Accelerator => _accelerator;
        public float MaxFuel => _maxFuel;
        public float Fuel => _fuel;
        public float FuelUseable => _fuelUseable;
        public bool LowGear => _lowGear;
        public VehicleContainerData(int axleCount = 2)
        {
            //_UseFuelPerHour = useFuelPerHour;
            _axleData = new AxleDataVehicle[axleCount];
            for (int i = 0; i < axleCount; i++) {
                _axleData[i] = new AxleDataVehicle();
            }
            EngineState = new VehicleEngineState();
        }
        public int FuelInt()
        {
            int value = (int)_fuel;
            if (_fuel > value + .001f) value++;
            return value;
        }
        public float FuelFill => _fuel / _maxFuel;
        public AxleDataVehicle GetAxleData(int i)
        {
            return _axleData[i];
        }
        public int GearBoxVisualX => _gearBoxVisualX;
        public int GearBoxVisualZ => _gearBoxVisualZ;

        public int AxleDataCount => _axleData.Length;

        public bool HandBrake => _handBrake;

        public void SetSpeed(int speed) => _speed = speed;
        public void SetDistance(int value) => _distance = value;
        public void SetMaxRPM(int maxRpm) => _maxRPM = maxRpm;
        public void SetGear(string gear)
        {
            _gear = gear;
            EventPositionGear?.Invoke();
        }
        public void SetClutch(float clutch) => _clutch = clutch;
        public void SetBrake(float brake) => _brake = brake;
        public void SetAccelerator(float accelerator) => _accelerator = accelerator;
        public void SetFuel(float value) => _fuel = value;
        public void SetMaxFuel(float value) => _maxFuel = value;
        public void SetSteering(float steer)
        {
            EventSteeringChange?.Invoke(steer);
        }
        public void SetPositionGear(int x, int z)
        {
            _gearBoxVisualX = x;
            _gearBoxVisualZ = z;
            EventPositionGear?.Invoke();
        }
        public void SetLowGear(bool value)
        {
            _lowGear = value;
            EventChangeLowGear.Invoke();
        }
        public void ReWriteFuelUseable(float useFuel)
        {
            float value = (_speed == 0) ? 0 : 3600 * useFuel / _speed * 100;
            if (_fuelUseable != value)
            {
                _fuelUseable = value;
                EventChangeFuelUseable?.Invoke();
            }
        }
        public void SetHandBrake(bool value) {
            _handBrake = value;
            EventChangeHandBrake?.Invoke();
        }
    }
}