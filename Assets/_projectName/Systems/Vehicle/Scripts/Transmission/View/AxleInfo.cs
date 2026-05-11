using UnityEngine;
namespace Systems.Vehicle.Transmission.View
{
    [System.Serializable]
    public class AxleInfo
    {
        private readonly Data.WheelData _wheelData;
        private readonly Data.AxleData _axleData;//config
        private readonly ContainerData.AxleDataVehicle _data;
        private readonly ContainerData.VehicleContainerData _vehicleContainerData;
        private readonly float _z;
        //public float leftWheelGrip;
        public float Z => _z;
        private float _widthBase;
        private readonly float _axleHalfWidth;
        private Input.VehicleInput _input;
        private readonly Rigidbody _body;
        private Wheel _left;
        private Wheel _right;
        public float leftRPM;
        public float rightRPM;
        public float leftTorque1 = 0;
        public float rightTorque1 = 0;
        public Wheel Left => _left;
        public Wheel Right => _right;
        private readonly bool _steering = false;
        private readonly bool _handBrake = true;
        public bool Steering =>_steering;
        public float koefTransmissionAxle = .5f;
        private bool _lockDifferential = false;
        private readonly bool _hasLockedDifferential = true;
        private readonly bool _hasLockDifferential = false;
        private int _sleepLockDifferentialRPM;
        private int _highTorqueLock;

        private float _koefDifferentialLeft = .5f;
        private float _koefDifferentialRight = .5f;
        public float KoefDifferentialLeft => _koefDifferentialLeft;
        public float KoefDifferentialRight => _koefDifferentialRight;
        private int _brakeTorque = 15000;
        private readonly float _antiroll = 10000;
        private GameObject _prefabAxleCollider;
        private GameObject _prefabAxleController;
        public bool IsActive => koefTransmissionAxle > 0;
        private float AntrollForce = 0;


        public float ackermanSteering = 1f;
        public void SetAxlePrefab(Data.WheelSetupData data, int id)
        {
            if(_hasLockedDifferential)
                LockedDifferential();

            if (id < data.FrontAxleCount)
            {
                _brakeTorque = data.BrakeTorqueFront / data.FrontAxleCount;
            }
            else
            {
                _brakeTorque = data.BrakeTorqueRear / data.RearAxleCount;
            }
            if (data.typeWheel == Data.TypeWheel.Custom3D)
            {
                if (CreateAxleViewController()) return;
            }
            CreateAxleViewCollider();
        }
        public void LockedDifferential()
        {
            UnlockedDifferential();
            _lockDifferential = true;
        }
        public void UnlockedDifferential()
        {
            _lockDifferential = false;
        }
        public void SetWidthBase(float widthBase)
        {
            _widthBase = widthBase;
        }
        public void ApplyTraction()
        {
            _left.ApplyTraction();
            _right.ApplyTraction();
            //leftWheelGrip = Left.Traction;
            _left.UpdateFriction();
            _right.UpdateFriction();
        }
        public AxleInfo(Rigidbody body, ContainerData.AxleDataVehicle data, Data.AxleData axleData, Data.WheelData wheelData, GameObject axlePrefabCollider, GameObject axlePrefabController, ContainerData.VehicleContainerData vehicleContainerData)
        {
            _prefabAxleCollider = axlePrefabCollider;
            _prefabAxleController = axlePrefabController;
            _body = body;
            _data = data;
            _wheelData = wheelData;
            _axleData = axleData;
            _handBrake = _axleData.HandBrake;
            _steering = _axleData.Steering;
            _antiroll = _axleData.Antiroll;

            _axleHalfWidth = _axleData.X;
            _z = _axleData.Z;
            _vehicleContainerData = vehicleContainerData;
        }
        private void CreateAxleViewCollider()
        {
            FactoryWheels.CreateWheelCollider(out _left, out _right, _prefabAxleCollider, _body, _axleData, _wheelData);

            _sleepLockDifferentialRPM = _axleData.SleepLockDifferentialRPM;
            _highTorqueLock = _axleData.HighTorqueLock;
        }
        private bool CreateAxleViewController()
        {
            return FactoryWheels.CreateWheelController3D(out _left, out _right, _prefabAxleController, _body, _axleData, _wheelData);
        }
        private void EnableEvent()
        {
            if (_input == null) return;
        }
        private void DisableEvent()
        {
            if (_input == null) return;
        }
        public void SetInput(Input.VehicleInput input)
        {
            DisableEvent();
            _input = input;
            EnableEvent();
        }
        public void SteerAngle(float radius)
        {
            if (radius == 0) {
                _left.SteerAngle(0);
                _right.SteerAngle(0);
                return;
            }
            _left.SteerAngle(Mathf.Atan(_widthBase / (radius + _axleHalfWidth)) * Mathf.Rad2Deg);
            _right.SteerAngle(Mathf.Atan(_widthBase / (radius - _axleHalfWidth)) * Mathf.Rad2Deg);
        }
        public float GetRpm()
        {
            leftRPM = _left.Rpm;
            rightRPM = _right.Rpm;
            return (_left.Rpm + _right.Rpm) / 2 * koefTransmissionAxle;
        }
        public void SetTorque(float torque = 0, float brake = 0, float handBrake = 0)
        {
            SetBrake(_brakeTorque * Mathf.Clamp01(brake + (_handBrake ? handBrake : 0)));

            torque *= koefTransmissionAxle;
            float avgRPM = (_left.Rpm + _right.Rpm) / 2;
            float lockStrength = (Mathf.Abs(avgRPM) < _sleepLockDifferentialRPM) ? 0 : _highTorqueLock;
            float leftTorque = torque;
            float rightTorque = torque;
            if (_lockDifferential)
            {
                leftTorque -= lockStrength * (_left.Rpm - avgRPM);
                rightTorque -= lockStrength * (_right.Rpm - avgRPM);

                float totalTorque = Mathf.Abs(leftTorque) + Mathf.Abs(rightTorque);
                if (totalTorque >= 0)
                {
                    _koefDifferentialLeft = Mathf.Abs(leftTorque) / totalTorque;
                    _koefDifferentialRight = 1.0f - _koefDifferentialLeft;
                }
            }
            _left.SetTorque(leftTorque / 2);
            _right.SetTorque(rightTorque / 2);
            leftTorque1 = _left.Torque;
            rightTorque1 = _right.Torque;
            if (_data == null) return;
            _data.TorqueLeft = _koefDifferentialLeft;
            _data.TorqueRight = _koefDifferentialRight;
        }
        public void SetBrake(float torque = 0)
        {
            torque /= 2;
            _left.BrakeTorque(torque);
            _right.BrakeTorque(torque);
        }
        public void Update()
        {
            _left.Update();
            _right.Update();
            CalculateAndApplyAntiRollForce();
        }
        private void CalculateAndApplyAntiRollForce()
        {
            AntrollForce = (_left.Travel() - _right.Travel()) * _antiroll;

            if (_left.Grounded)
                _body.AddForceAtPosition(_left.Transform.up * -AntrollForce, _left.Transform.position);
            if (_right.Grounded)
                _body.AddForceAtPosition(_right.Transform.up * AntrollForce, _right.Transform.position);
        }
    }
}
