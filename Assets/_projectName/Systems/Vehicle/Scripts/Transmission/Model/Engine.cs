using UnityEngine;
namespace Systems.Vehicle.Transmission.Model
{
    [System.Serializable]
    public class Engine
    {
        private readonly Data.EngineData.TypeEngine _typeEngine;
        private Input.VehicleInput _input;
        private readonly TorqueConverter.TorqueConverterBase _clutch;
        private readonly ContainerData.VehicleContainerData _vehicleContainerData;
        private readonly ContainerData.VehicleEngineState _state;
        [SerializeField] private bool _working = false;
        public float deltaRPM = 0;
        private float _RPM = 0;
        public float GetRPM => _RPM;
        public float GetLoad => _load;
        [SerializeField] private float _friction;
        [SerializeField] private float _powerValue;
        [SerializeField] private float _accelerator;
        [SerializeField] private float _load;// != _accelerator
        private readonly int _postIdleRPM = 1500;
        private readonly int _idleRPM = 1000;
        private readonly int _idleHRPM = -120;
        private readonly int _stallRPM = 780;
        private readonly int _limitRPM = 8000;
        private readonly int _limitRPMCutLoad = 200;
        private readonly int _limitCutLoadFromRPM = 200;
        private float _limitRPMCutLoadTarget = 2200;
        private readonly float _loadTargetToRPM = .1f;
        private readonly float _inertia = .2f;
        //Power
        private readonly float _idlePower = 80;
        private readonly float _peakPower = 150;
        private readonly float _peakPowerEnd = 170;
        private readonly int _idlePowerRPM = 1000;
        private readonly int _peakPowerRPMStart = 5300;
        private readonly int _peakPowerRPMEnd = 5800;
        private readonly float _endPower = 80;
        private readonly int _endPowerRPM = 8000;
        //Friction
        private readonly float _extraFriction = 30;
        private readonly int _extraFrictionRPM = 30;
        private readonly float _idleFriction = 60;
        private readonly float _idleFrictionMax = 60;
        private readonly float _maxFriction = 120;
        public float torque;
        [Header("IldeControl")]
        [SerializeField] private bool _activeStarter = false;
        private readonly float _starterTorque = 550;
        private float _starterTorqueActive = 0;
        [Header("throttleControl")]
        public int _range = 1;// for inspector
        private readonly IdleSupport _idleSupport;
        private readonly float _idleThrottle;
        public float ClutchTorque;
        //Fuel
        private readonly float _maxFuelLose = 120;
        public float loseFuel = 0;
        public float timeWork;
        private readonly FuelSystem.FuelSystem _fuelSystem;
        private readonly int _upShiftRPMMin = 1100;
        private readonly int _upShiftRPMMax = 1600;
        private readonly int _downShiftRPMMin = 700;
        private readonly int _downShiftRPMMax = 1100;
        public float UpShiftRPM => _upShiftRPMMin + (_upShiftRPMMax - _upShiftRPMMin) * _load;
        public float DownShiftRPM => _downShiftRPMMin + (_downShiftRPMMax - _downShiftRPMMin) * _load;

        public Engine(Data.EngineData data, TorqueConverter.TorqueConverterBase clutch, ContainerData.VehicleContainerData vehicleContainerData, FuelSystem.FuelSystem fuelSystem)
        {
            _vehicleContainerData = vehicleContainerData;
            _state = _vehicleContainerData.EngineState;
            _clutch = clutch;
            _stallRPM = data.StallRPM;
            _idleRPM = data.IdleRPM;
            _postIdleRPM = data.PostIdleRPM;
            _idleHRPM = data.IdleHRPM;
            _limitRPM = data.LimitRPM;
            _inertia = data.Inertia;

            _idlePower = data.IdleTorque;
            _idlePowerRPM = data.IdleTorqueRPM;
            _peakPower = data.PeakTorque;
            _peakPowerEnd = data.PeakTorque;
            _peakPowerRPMStart = data.PeakTorqueRPMStart;
            _peakPowerRPMEnd = data.PeakTorqueRPMEnd;
            _endPower = data.TorqueEnd;
            _endPowerRPM = data.TorqueEndRPM;

            _extraFriction = data.ExtraFriction;
            _extraFrictionRPM = data.ExtraFrictionMaxRPM;
            _idleFriction = data.IdleFriction;
            _maxFriction = data.MaxFriction;
            _idleFrictionMax = data.IdleFrictionMax;

            _starterTorque = data.StarterTorqueActive;

            _idlePower += _idleFriction;
            _peakPower += FrictionByRPM(_peakPowerRPMStart);
            _peakPowerEnd += FrictionByRPM(_peakPowerRPMEnd);
            _endPower += FrictionByRPM(_endPowerRPM);

            _RPM = _idleRPM;
            _working = true;
            _idleThrottle = CurrentFriction() / Power();
            _idleSupport = new IdleSupport(false, _idleRPM - (_idleRPM + _idleHRPM), (_idleRPM + _idleHRPM), _idleThrottle + data.DeltaSupport, _idleThrottle);
            _range = (int)_idleSupport.RangeRPM;
            _limitCutLoadFromRPM = _limitRPM - (int)_idleSupport.RangeRPM;

            StallEngine();
            _RPM = 0;
            _starterTorqueActive = data.StarterTorqueActive;
            _vehicleContainerData.SetMaxRPM(_limitRPM);

            _maxFuelLose = data.MaxFuelLose;
            _fuelSystem = fuelSystem;

            _typeEngine = data.EngineType;
            _upShiftRPMMin = data.UpShiftRPMMin;
            _upShiftRPMMax = data.UpShiftRPMMax;
            _downShiftRPMMin = data.DownShiftRPMMin;
            _downShiftRPMMax = data.DownShiftRPMMax;
        }
        private void EnableEvent()
        {
            if (_input == null) return;
            _input.EventStartEngine += OnStarterPressed;
            _input.EventStopStarter += OnStarterReleased;
        }
        private void DisableEvent()
        {
            if (_input == null) return;
            _input.EventStartEngine -= OnStarterPressed;
            _input.EventStopStarter -= OnStarterReleased;
        }
        public void SetInput(Input.VehicleInput input)
        {
            DisableEvent();
            _input = input;
            EnableEvent();
        }
        private void OnStarterPressed()
        {
            if (!_working)
            {
                _activeStarter = true;
                _state.SetStarterActive(_activeStarter);
            }
            StallEngine();
        }
        private void OnStarterReleased()
        {
            _activeStarter = false;
            _state.SetStarterActive(_activeStarter);
        }
        private void CheckStarter()
        {
            if (_activeStarter && !_working && _RPM >= _stallRPM)
                StartEngine();
        }
        private void StartEngine()
        {
            _working = true;
            _state.SetActive(_working);
        }
        private void StallEngine()
        {
            _working = false;
            _state.SetActive(_working);
        }
        private void SetInputValue()
        {
            if (!_working)
            {
                _load = 0;
                return;
            }
            _accelerator = (_input == null) ? 0 : Mathf.Clamp01(_input.Accelerator);
            _load = _accelerator;
        }
        private void SetState()
        {
            _state.rpm = _working ? (int)_RPM : 0;
            _state.load = _load;
        }
        public void Update()
        {
            SetInputValue();
            _clutch.UpdateInput((_input == null) ? 1 : _input.Clutch);
            _load = GetIdleSupportThrottle();
            _vehicleContainerData.SetAccelerator(_load);
            _vehicleContainerData.SetClutch(_clutch.GetClutch);
            deltaRPM = _RPM - _clutch.GetRPMGearBoxShaft;
            if (_clutch.IsSynchronized)
            {
                _RPM = _clutch.GetRPMGearBoxShaft;
            }
            else
            {
                _clutch.SetTorqueEngine(Torque());
                _RPM += (Torque() + _clutch.SetTorqueToEngine(_RPM)) / _inertia * 2 * Time.fixedDeltaTime;
            }
            loseFuel += _maxFuelLose * Time.fixedDeltaTime * _load;
            if (_working)
            {
                if (!_fuelSystem.LoseFuel(_maxFuelLose * Time.fixedDeltaTime * _load))
                {
                    StallEngine();
                }
                timeWork += Time.fixedDeltaTime;
                if (_RPM < _stallRPM)
                {
                    StallEngine();
                }
            }
            else
            {
                CheckStarter();
            }
            _clutch.SetTorqueEngine(Torque());
            ClutchTorque = _clutch.GetTorqueFromEngine();
            _vehicleContainerData.ReWriteFuelUseable(_maxFuelLose * _load);
            SetState();
        }
        private float GetIdleSupportThrottle()
        {
            if (!_working)
                return 0;
            if (_RPM > _limitRPM)
                return 0;
            float t = (_load - _loadTargetToRPM) / (1f - _loadTargetToRPM);
            t = Mathf.Pow(Mathf.Clamp01(t), 2);
            _limitRPMCutLoadTarget = _limitCutLoadFromRPM + (_limitRPMCutLoad - _limitCutLoadFromRPM) * t;
            if (_RPM > _limitRPM - _limitRPMCutLoadTarget)
                return _load * (1 - (_RPM - _limitRPM + _limitRPMCutLoadTarget) / _limitRPMCutLoadTarget);
            return _idleSupport.Evaluate(_RPM, _load);
        }
        public float Torque()
        {
            _friction = CurrentFriction();
            _powerValue = Power() * _load;
            torque = _powerValue - _friction;
            _starterTorqueActive = (!_working && _activeStarter) ? _starterTorque : 0;

            return torque + _starterTorqueActive;
        }
        private float Power()
        {
            //if (!_working)
            //    return 0;
            if (_RPM < _idlePowerRPM)
                return _idlePower;
            if (_RPM < _peakPowerRPMStart)
                return _idlePower + (_RPM - _idlePowerRPM) / (_peakPowerRPMStart - _idlePowerRPM) * (_peakPower - _idlePower);
            if (_RPM < _peakPowerRPMEnd)
                return _peakPower + (_RPM - _peakPowerRPMStart) / (_peakPowerRPMEnd - _peakPowerRPMStart) * (_peakPowerEnd - _peakPower);
            if (_RPM < _limitRPM)
                return _endPower + (1 - (_RPM - _peakPowerRPMEnd) / (_endPowerRPM - _peakPowerRPMEnd)) * (_peakPowerEnd - _endPower);
            return 0;
        }
        private float StaticFriction()
        {
            if (_RPM < 15)
                return 0;
            if (_RPM < _extraFrictionRPM)
                return _idleFriction + _extraFriction;
            if (_RPM < _idleRPM)
                return _idleFriction;
            if (_RPM < _postIdleRPM)
                return _idleFriction + (_RPM - _idleRPM) / (_postIdleRPM - _idleRPM) * (_idleFrictionMax - _idleFriction);
            return _idleFrictionMax + (_RPM - _postIdleRPM) / (_limitRPM - _postIdleRPM) * (_maxFriction - _idleFrictionMax);
        }
        private float FrictionByRPM(float rpm)
        {
            if (rpm < _postIdleRPM)
                return _idleFriction + (rpm - _idleRPM) / (_postIdleRPM - _idleRPM) * (_idleFrictionMax - _idleFriction);
            return _idleFrictionMax + (rpm - _postIdleRPM) / (_limitRPM - _postIdleRPM) * (_maxFriction - _idleFrictionMax);
        }
        private float CurrentFriction() {
            float friction = StaticFriction();
            if (_typeEngine == Data.EngineData.TypeEngine.Diesel)
            {
                //TODO dynamic: brake jake for diesel
                //TODO event
            }
            else
            {
                if (_RPM > _idleFriction)
                {
                    if (_load < .001f)
                        friction *= 1.5f;
                }
            }
            return friction;
        }
    }
}