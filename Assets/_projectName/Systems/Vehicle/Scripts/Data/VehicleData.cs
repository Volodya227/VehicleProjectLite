using System.Collections.Generic;
using UnityEngine;
namespace Systems.Vehicle.Data
{
    public enum TypeWheel { Collider, Custom3D };
    public enum TypeTransmission { FWD, RWD, _4WD, AWD };
    public enum TypeControlTransmission { Manual, AutomatedManual, Planerary, Automatic};

    [System.Serializable]
    public class WheelSetupData
    {
        public TypeWheel typeWheel = TypeWheel.Collider;
        private int _axleCount;
        [SerializeField] private int _frontAxleCount = 1;
        [SerializeField] private int _brakeTorque = 2000;
        [SerializeField, Range(0, 1)] private float _brakeTorqueBias = .7f;
        public int BrakeTorqueFront => (int)(_brakeTorque * _brakeTorqueBias);
        public int BrakeTorqueRear => (int)(_brakeTorque * (1 - _brakeTorqueBias));
        public int FrontAxleCount => _frontAxleCount;
        public int RearAxleCount => _axleCount - _frontAxleCount;
        public void SetAxleCount(int value) { _axleCount = value; }
    }
    [System.Serializable]
    public class AxleData
    {
        [SerializeField] private float _x;
        [SerializeField] private float _y;
        [SerializeField] private float _z;
        [SerializeField] private bool _steering;
        [SerializeField] private bool _handBrake;
        [Header("Suspension")]
        [SerializeField] private float _suspensionDistance = .3f;
        [SerializeField] private float _forceAppPointDistance = 0;
        [SerializeField] private float _springCollider = 35000;
        [SerializeField] private float _damperCollider = 5000;
        [SerializeField] private float _targetPosition = .5f;
        [SerializeField] private float _antiroll = 0;

        [SerializeField] private float _springController = 35000;
        [SerializeField] private float _damperController = 5000;
        [Header("wheel mass from vehicle")]
        [SerializeField] private float _wheelMassCollider = 200;
        [SerializeField] private float _wheelMassController = 200;
        [Header("Differential Config")]
        [SerializeField] private bool _hasLockDifferential = false;
        [SerializeField] private int _sleepLockDifferentialRPM;
        [SerializeField] private int _highTorqueLock;
        public float X => _x;
        public float Y => _y;
        public float Z => _z;
        public bool Steering => _steering;
        public bool HandBrake => _handBrake;
        public float SuspensionDistance => _suspensionDistance;
        public float ForceAppPointDistance => _forceAppPointDistance;
        public float SpringCollider => _springCollider;
        public float DamperCollider => _damperCollider;
        public float TargetPosition => _targetPosition;
        public float Antiroll => _antiroll;

        public float SpringController => _springController;
        public float DampingController => _damperController;

        public float WheelMassCollider =>_wheelMassCollider;
        public float WheelMassController => _wheelMassController;
        public bool HasLockDifferential => _hasLockDifferential;
        public int SleepLockDifferentialRPM => _sleepLockDifferentialRPM;
        public int HighTorqueLock => _highTorqueLock;

    }
    [CreateAssetMenu(fileName = "data Vehicle item", menuName = "Vehicle/DATA/Vehicle")]
    public class VehicleData : ScriptableObject
    {
        [SerializeField] private WheelSetupData _wheelSetupData = new();
        public WheelSetupData GetWheelSetupData() { _wheelSetupData.SetAxleCount(AxleCount); return _wheelSetupData; }
        public TypeTransmission typeTransmission = TypeTransmission.RWD;
        [SerializeField] private bool _hasLockDifferential = false;
        [SerializeField] private int _sleepLockDifferentialRPM;
        [SerializeField] private int _highTorqueLock;
        public bool HasLockDifferential => _hasLockDifferential;
        public int SleepLockDifferentialRPM => _sleepLockDifferentialRPM;
        public int HighTorqueLock => _highTorqueLock;


        [SerializeField] private AxleData[] _axleData;
        [SerializeField] private float _maxSteering = 30;
        [SerializeField] private float _steeringWheelSensitivity = .6f;
        public int AxleCount => _axleData.Length;
        public float MaxSteering => _maxSteering;
        public float SteeringWheelSensitivity => _steeringWheelSensitivity;
        public AxleData GetAxle(int i) { return _axleData[i]; }
        [SerializeField] private int _vehicleMass = 1000;
        public int VehicleMass => _vehicleMass;
        [Header("GearBox")]//TODO
        [SerializeField] private GearBoxManualControlData _gearBoxManualControlData;
        [SerializeField] private GearBoxAutomaticControlData _gearBoxAutomaticControlData;

        public GearBoxManualControlData GearBoxManualControlData => _gearBoxManualControlData;
        public GearBoxAutomaticControlData GearBoxAutomaticControlData => _gearBoxAutomaticControlData;
        public List<float> gearKoef = new();
        [SerializeField]private float _gearKoefReverse = 20;
        public float GetGearKoefReverse => _gearKoefReverse;
        [SerializeField] private float _syncWindowRPM = 120;
        public float SyncWindowRPM => _syncWindowRPM;
        [SerializeField] private float _targetGearKoef;
        public float GetTargetGearKoef => _targetGearKoef;

        [Header("Transfer Case")]
        [SerializeField] private bool _hasLowRange = false;
        [SerializeField] private float _lowRangeRatio = 2.5f;
        [SerializeField] private bool _requiresAWDForLowRange = false;

        public bool HasLowRange => _hasLowRange;
        public float LowRangeRatio => _lowRangeRatio;
        public bool RequiresAWDForLowRange => _requiresAWDForLowRange;
        [Header("Model")]
        [SerializeField] private Visual.ModelItem _modelPrefab;//TODO
        public Visual.ModelItem ModelPrefab => _modelPrefab;
        [SerializeField] private EngineData _engineData;
        [SerializeField] private WheelData _wheelData;
        public EngineData EngineData => _engineData;
        public WheelData WheelData => _wheelData;
        [SerializeField] private int _maxFuel = 10;
        public int MaxFuel => _maxFuel;
        [SerializeField] private TypeControlTransmission _typeControlTransmission = TypeControlTransmission.Manual;
        public TypeControlTransmission TypeControlTransmission => _typeControlTransmission;
    }
}