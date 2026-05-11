using UnityEngine;
namespace Systems.Vehicle.Transmission.View
{
    [System.Serializable]
    public class TransmissionView
    {
        public float WheelRPM;
        public float TORque;
        private float _brake;
        private float _handBrakeValue = 0;
        [SerializeField] private float _targetRPM;
        [SerializeField] private float _lockStrength;
        private Input.VehicleInput _input = null;
        private readonly ContainerData.VehicleContainerData _containerData;
        private readonly int _frontAxles;
        private readonly float _maxSteeringAngle = 30;
        [SerializeField] private float _currentSteerAngle = 0;
        [SerializeField] private float _lastSteerAngle = 0;
        [SerializeField] private float _radiusSteering;
        private float _wheelBase;
        private float _positionZRearAxles;
        [SerializeField] private AxleInfo[] _axleInfos;
        private bool _isAWD = false;
        private bool _is4WD = false;
        private bool _isSteeringWheelCenter = false;
        private bool _isRawInputMode = false;
        private bool _lockCenterDifferential;
        private float _steeringSensitivity = 1;
        private readonly bool _hasLockDifferential = false;
        private readonly int _sleepLockDifferentialRPM;
        private readonly int _highTorqueLock;
        private readonly Data.TypeWheel _typeWheel;
        private readonly Data.WheelSetupData _wheelSetupData;
        private readonly Data.VehicleData _vehicleData;
        public int AxleCount => _axleInfos.Length;
        public float CurrentSteerAngle => _currentSteerAngle / _maxSteeringAngle;
        private void EnableEvent()
        {
            if (_input == null) return;
            _input.EventChangeHandBrake += ChangeHandBrake;
            _input.EventChangeSteeringWheelController += ChangeSteeringController;
            _input.EventChangeSteeringWheelControllerRaw += ChangeSteeringControllerRaw;
            _isRawInputMode = _input.RawSteering;
        }
        private void DisableEvent()
        {
            if (_input == null) return;
            _input.EventChangeHandBrake -= ChangeHandBrake;
            _input.EventChangeSteeringWheelControllerRaw -= ChangeSteeringControllerRaw;
            _input.EventChangeSteeringWheelController -= ChangeSteeringController;
            _isRawInputMode = false;
            _isSteeringWheelCenter = false;
        }
        public TransmissionView(Data.VehicleData vehicleData, GameObject prefabAxleCollider, GameObject prefabAxleController, Rigidbody body, ContainerData.VehicleContainerData containerData)
        {
            _vehicleData = vehicleData;
            _maxSteeringAngle = vehicleData.MaxSteering;
            _axleInfos = new AxleInfo[vehicleData.AxleCount];
            _wheelSetupData = vehicleData.GetWheelSetupData();
            _frontAxles = _wheelSetupData.FrontAxleCount;
            _typeWheel = _wheelSetupData.typeWheel;
            _containerData = containerData;
            _steeringSensitivity = vehicleData.SteeringWheelSensitivity;

            for (int i = 0; i < vehicleData.AxleCount; i++)
            {
                _axleInfos[i] = new AxleInfo(body, _containerData.GetAxleData(i), vehicleData.GetAxle(i), vehicleData.WheelData, prefabAxleCollider, prefabAxleController, _containerData);
                _axleInfos[i].SetAxlePrefab(_wheelSetupData, i);
            }
            InitializeTypeTransmission();
            CalculateWheelBase();
            _sleepLockDifferentialRPM = vehicleData.SleepLockDifferentialRPM;
            _highTorqueLock = vehicleData.HighTorqueLock;
            SetHandBrake(1);
            LockingCenterDifferential();
        }
        private void CalculateWheelBase()
        {
            float count = 0;
            _positionZRearAxles = 0;
            foreach (AxleInfo axle in _axleInfos)
            {
                if (!axle.Steering)
                {
                    _positionZRearAxles += axle.Z;
                    count++;
                }
            }

            _positionZRearAxles /= count;

            foreach (AxleInfo axle in _axleInfos)
            {
                if (axle.Steering)
                {
                    float length = axle.Z - _positionZRearAxles;
                    axle.SetWidthBase(length);
                    if (_wheelBase < length)
                    {
                        _wheelBase = length;
                    }
                }
            }
        }
        private void LockingCenterDifferential()
        {
            UnlockingCenterDifferential();
            _lockCenterDifferential = true;
        }
        private void UnlockingCenterDifferential()
        {
            _lockCenterDifferential = false;
        }
        private void ChangeLockDifferential()
        {
            if (_lockCenterDifferential)
                UnlockingCenterDifferential();
            else
                LockingCenterDifferential();
        }
        public void SetInput(Input.VehicleInput input)
        {
            DisableEvent();
            _input = input;
            EnableEvent();
            foreach (AxleInfo axle in _axleInfos) { axle.SetInput(_input); }
        }
        public void Update(float torque)
        {
            if (_typeWheel == Data.TypeWheel.Collider)
            {
                AxleUpdate();
            }
            UpdateTraction();
            SteerAngle();
            SetTorque(torque);
            WheelRPM = GetRPM();
        }
        private void UpdateTraction()
        {
            for (int i = 0; i < _axleInfos.Length; i++)
            {
                ContainerData.AxleDataVehicle axleDataVehicle = _containerData.GetAxleData(i);
                _axleInfos[i].ApplyTraction();
                axleDataVehicle.WheelGripLeft = _axleInfos[i].Left.Traction;
                axleDataVehicle.WheelGripRight = _axleInfos[i].Right.Traction;
            }
        }
        private void SteeringAngle(float steer = 0)
        {
            if (_isRawInputMode)
            {
                _currentSteerAngle = steer;
                return;
            }
            if (_isSteeringWheelCenter)
            {
                float steers = Time.fixedDeltaTime * _steeringSensitivity * (steer == 0 ? .2f : 1);
                if (Mathf.Abs(steer - _currentSteerAngle) <= steers)
                    _currentSteerAngle = steer;
                else
                {
                    if (steer > _currentSteerAngle)
                        _currentSteerAngle += steers;
                    else
                        _currentSteerAngle -= steers;
                }
                _currentSteerAngle = Mathf.Clamp(_currentSteerAngle, -1, 1);
                return;
            }
            _currentSteerAngle = Mathf.Clamp(_currentSteerAngle + steer * Time.fixedDeltaTime * _steeringSensitivity, -1, 1);
        }
        private void SteerAngle()
        {
            SteeringAngle((_input == null) ? 0 : _input.Steering);
            if (_currentSteerAngle == _lastSteerAngle) { return; }
            _lastSteerAngle = _currentSteerAngle;
            _containerData.SetSteering(_currentSteerAngle);
            if (_currentSteerAngle == 0) _radiusSteering = 0;
            else
            {
                _radiusSteering = _wheelBase / Mathf.Tan(_currentSteerAngle * _maxSteeringAngle * Mathf.Deg2Rad);
            }
            foreach (AxleInfo axleInfo in _axleInfos)
            {
                if (axleInfo.Steering) axleInfo.SteerAngle(_radiusSteering);
            }
        }
        private void ChangeSteeringController()
        {
            _isSteeringWheelCenter = !_isSteeringWheelCenter;
        }
        private void ChangeSteeringControllerRaw()
        {
            _isRawInputMode = _input.RawSteering;
        }
        private void SetTorque(float torque)
        {
            TORque = torque;
            _brake = (_input == null) ? 0 : Mathf.Pow(_input.Braking, 1.5f);
            if (_lockCenterDifferential)
            {
                _targetRPM = 0;
                int countAxles = 0;
                foreach (AxleInfo axle in _axleInfos)
                {
                    if (axle.IsActive)
                    {
                        _targetRPM += axle.GetRpm();
                        countAxles++;
                    }
                }
                _targetRPM /= countAxles;// count!=0 => sum koefTransmissionAxle = 1
                _lockStrength = (Mathf.Abs(_targetRPM) < _sleepLockDifferentialRPM) ? 0 : _highTorqueLock;
                foreach (AxleInfo axle in _axleInfos)
                {
                    axle.SetTorque(axle.IsActive ? (torque - (axle.GetRpm() - _targetRPM) * _lockStrength) : 0, _brake, _handBrakeValue);
                }
            }
            else
            {
                foreach (AxleInfo axle in _axleInfos)
                {
                    axle.SetTorque(axle.IsActive ? torque : 0, _brake, _handBrakeValue);
                }
            }
        }
        public float GetRPM()
        {
            //return _shaft.RPM;
            float rpm = 0;
            foreach (AxleInfo axle in _axleInfos) {
                rpm += axle.GetRpm();
            }
            return rpm;
        }
        private void InitializeTypeTransmission()
        {
            if (_vehicleData.typeTransmission == Data.TypeTransmission.RWD)
            {
                SetRWD();
            }
            else if (_vehicleData.typeTransmission == Data.TypeTransmission.FWD)
            {
                SetFWD();
            }
            else if (_vehicleData.typeTransmission == Data.TypeTransmission._4WD)
            {
                SetFullWD();
            }
            else if (_vehicleData.typeTransmission == Data.TypeTransmission.AWD)
            {
                _isAWD = true;
                SetRWD();
            }
        }
        private void ChangeHandBrake()
        {
            if (_handBrakeValue > 0)
                SetHandBrake(0);
            else
                SetHandBrake(1);
            if (_containerData == null)
                return;
            _containerData.SetHandBrake(_handBrakeValue > 0);
        }
        private void SetHandBrake(float value = 0)
        {
            _handBrakeValue = value;
            if (_containerData != null)
                _containerData.SetHandBrake(_handBrakeValue > 0);
        }
        private void ChangeTypeTransmission()
        {
            if (_isAWD)
            {
                if (_is4WD)
                {
                    SetRWD();
                }
                else
                {
                    SetFullWD();
                }
            }
        }
        private void SetFWD()
        {
            float i = 0;
            int axles = _frontAxles;
            foreach (AxleInfo axle in _axleInfos)
            {
                if (i < _frontAxles)
                {
                    axle.koefTransmissionAxle = 1f / axles;
                }
                else
                {
                    axle.koefTransmissionAxle = 0;
                }
                i++;
            }
            _is4WD = false;
        }
        private void SetRWD()
        {
            float i = 0;
            int axles = _axleInfos.Length - _frontAxles;
            foreach (AxleInfo axle in _axleInfos)
            {
                if (i < _frontAxles)
                {
                    axle.koefTransmissionAxle = 0;
                }
                else
                {
                    axle.koefTransmissionAxle = 1f / axles;
                }
                i++;
            }
            _is4WD = false;
        }
        private void SetFullWD()
        {
            int axles = _axleInfos.Length;
            foreach (AxleInfo axle in _axleInfos)
            {
                axle.koefTransmissionAxle = 1f / axles;
            }
            _is4WD = true;
        }
        private void AxleUpdate()
        {
            foreach (AxleInfo axle in _axleInfos)
            {
                axle.Update();
            }
        }

    }
}