using UnityEngine;
namespace Systems.Vehicle.Data
{
    [System.Serializable]
    public class TorqueConvertorController
    {
        [SerializeField] private float _minClutchValue = .15f;
        [SerializeField] private float _clutchRPMFactor = .35f;
        [SerializeField] private float _maxTorqueScale = 3;
        [SerializeField] private float _minTorqueScale = .6f;
        public float MinClutchValue => _minClutchValue;
        public float ClutchRPMFactor => _clutchRPMFactor;
        public float MaxTorqueScale => _maxTorqueScale;
        public float MinTorqueScale => _minTorqueScale;
    }
    [System.Serializable]
    public class HydroTorqueConvertorController
    {
        [SerializeField] private TorqueConvertorController _controller = new();
        [SerializeField] private float _minKoef = 0.03f;
        [SerializeField] private float _maxKoef = 2.9f;
        public TorqueConvertorController Controller => _controller;
        public float MinKoef => _minKoef;
        public float MaxKoef => _maxKoef;
    }
    [System.Serializable]
    public class EngineDataSound
    {
        [SerializeField] private AudioClip _audioClipStart;
        [SerializeField] private AudioClip _audioClipLoop;
        [SerializeField] private AudioClip _audioClipStop;

        public AudioClip AudioClipStart => _audioClipStart;
        public AudioClip AudioClipLoop => _audioClipLoop;
        public AudioClip AudioClipStop => _audioClipStop;
        private int _limitRPM;
        private int _idleRPM;
        public int LimitRPM => _limitRPM;
        public int IdleRPM => _idleRPM;
        public void SetData(int idleRPM, int limitRPM)
        {
            _idleRPM = idleRPM;
            _limitRPM = limitRPM;
        }
    }
    [CreateAssetMenu(fileName = "data Vehicle item", menuName = "Vehicle/DATA/Engine")]
    public class EngineData : ScriptableObject
    {
        [SerializeField] private EngineDataSound _sound;
        public EngineDataSound GetSound()
        {
            _sound.SetData(_idleRPM, _limitRPM);
            return _sound;
        }
        public enum TypeEngine { Diesel, B95 }
        [Header("Engine Type")]
        [SerializeField] private TypeEngine _engineType = TypeEngine.B95;
        public TypeEngine EngineType => _engineType;
        [Header("RPM Range")]
        [SerializeField] private int _stallRPM = 750;
        [SerializeField] private int _limitRPM = 6000;
        [SerializeField] private int _idleRPM = 1000;
        [SerializeField] private int _postIdleRPM = 1500;
        public int StallRPM => _stallRPM;
        public int LimitRPM => _limitRPM;
        public int IdleRPM => _idleRPM;
        public int PostIdleRPM => _postIdleRPM;
        [Header("Torque Curve")]
        [SerializeField] private float _idleTorque = 80f;
        [SerializeField] private int _idleTorqueRPM = 1000;
        [SerializeField] private float _peakTorque = 180f;
        [SerializeField] private int _peakTorqueRPMStart = 3200;
        [SerializeField] private int _peakTorqueRPMEnd = 4000;
        [SerializeField] private float _torqueAtLimitRPM = 120f;
        public float IdleTorque => _idleTorque;
        public int IdleTorqueRPM => _idleTorqueRPM;
        public float PeakTorque => _peakTorque;
        public int PeakTorqueRPMStart => _peakTorqueRPMStart;
        public int PeakTorqueRPMEnd => _peakTorqueRPMEnd;
        public float TorqueEnd => _torqueAtLimitRPM;
        public int TorqueEndRPM => _limitRPM;
        public float TorqueAtLimitRPM => _torqueAtLimitRPM;
        [Header("Friction")]
        [SerializeField] private float _idleFriction = 40f;
        [SerializeField] private float _idleFrictionMax = 40;
        [SerializeField] private float _maxFriction = 100f;
        [SerializeField] private float _extraFriction = 40f;
        [SerializeField] private int _extraFrictionMaxRPM = 525;
        public float IdleFriction => _idleFriction;
        public float IdleFrictionMax => _idleFrictionMax;
        public float MaxFriction => _maxFriction;
        public float ExtraFriction => _extraFriction;
        public int ExtraFrictionMaxRPM => _extraFrictionMaxRPM;
        [Header("starter")]
        [SerializeField] private int _starterTorqueActive = 190;
        public int StarterTorqueActive => _starterTorqueActive;
        [Header("Engine Inertia")]
        [SerializeField] private float _inertia = 0.2f;
        public float Inertia => _inertia;
        [Header("IdleSupport")]
        [SerializeField] private int _idleHRPM = -120;
        [SerializeField] private float _deltaSupport = .02f;
        public float DeltaSupport => _deltaSupport;
        public int IdleHRPM => _idleHRPM;
        [Header("Clutch Settings")]
        [SerializeField] private float _clutchSoftStiffness = 0.6f;
        [SerializeField] private float _clutchHardStiffness = 1.0f;
        [SerializeField] private float _hydroTorqueConvertorHardStiffness = 1.0f;
        [SerializeField, Range(0, 1)] private float _clutchSlipCoefficient = 0.45f;
        [SerializeField] private int _clutchMaxFrictionLimit = 550;
        public float ClutchSoftStiffness => _clutchSoftStiffness;
        public float ClutchHardStiffness => _clutchHardStiffness;
        public float HydroTorqueConvertorHardStiffness => _hydroTorqueConvertorHardStiffness;
        public float ClutchSlipCoefficient => _clutchSlipCoefficient;
        public int ClutchMaxFrictionLimit => _clutchMaxFrictionLimit;
        [Header("Fuel system")]
        [SerializeField] private float _maxFuelLose = 120;
        public float MaxFuelLose => _maxFuelLose / 3600;
        private float _syncWindowRPM;
        public float SyncWindowRPM => _syncWindowRPM;
        [SerializeField] private int _upShiftRPMMin = 1100;
        [SerializeField] private int _upShiftRPMMax = 1600;
        [SerializeField] private int _downShiftRPMMin = 700;
        [SerializeField] private int _downShiftRPMMax = 1100;
        public int UpShiftRPMMin => _upShiftRPMMin;
        public int UpShiftRPMMax => _upShiftRPMMax;
        public int DownShiftRPMMin => _downShiftRPMMin;
        public int DownShiftRPMMax => _downShiftRPMMax;
        [Header("AutoClutchController")]
        [SerializeField] private TorqueConvertorController _autoClutchController = new();
        [SerializeField] private HydroTorqueConvertorController _hydroTorqueConvertorController = new();
        public TorqueConvertorController AutoClutchController => _autoClutchController;
        public HydroTorqueConvertorController HydroTorqueConvertorController => _hydroTorqueConvertorController;
        public void SetSyncWindowRPM(float syncWindowRPM)
        {
            _syncWindowRPM = syncWindowRPM;
        }
    }
}
