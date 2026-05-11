using Systems.Vehicle.Data;

namespace Systems.Vehicle.Transmission.Model.TorqueConverter
{
    public interface ITorqueConvertorConnection
    {
        public float TorqueToEngine { get; }
        public float TorqueToTransmission { get; }
        public bool IsSynchronized { get; }
    }
    public class ClutchConnection : ITorqueConvertorConnection
    {

        private readonly float _softFriction;
        private readonly float _hardFriction;
        private readonly float _slipFriction;
        private readonly float _syncWindow;
        private readonly float _clutchTorqueLimit = 600;
        private float _stiffness;
        private float _forceStiffness;
        private float _deltaRPM;
        public float TorqueToEngine { get; private set; }
        public float TorqueToTransmission { get; private set; }
        public bool IsSynchronized { get; private set; }
        public ClutchConnection(Data.EngineData engineData)
        {
            _softFriction = engineData.ClutchSoftStiffness;
            _hardFriction = engineData.ClutchHardStiffness;
            _slipFriction = engineData.ClutchSlipCoefficient;
            _clutchTorqueLimit = engineData.ClutchMaxFrictionLimit;
            IsSynchronized = false;
            _syncWindow = engineData.SyncWindowRPM;
        }
        public void ComputeTorque(float engineRPM, float transmissionRPM, float clutch)
        {
            _stiffness = UnityEngine.Mathf.Lerp(_softFriction, _hardFriction, clutch);
            if(clutch < .95f) _stiffness *= _slipFriction;
            _deltaRPM = transmissionRPM - engineRPM;
            _forceStiffness = UnityEngine.Mathf.Clamp(_deltaRPM * _stiffness * clutch, -_clutchTorqueLimit, _clutchTorqueLimit);
            TorqueToTransmission = -_forceStiffness;
            TorqueToEngine = _forceStiffness;
            if (clutch < 1) return;
            if (!IsSynchronized)
            {
                IsSynchronized = UnityEngine.Mathf.Abs(engineRPM - transmissionRPM) < _syncWindow;
            }
        }
        public void Unsynchronized()
        {
            IsSynchronized = false;
        }
    }
    public class HydroTorqueConvertorConnection : ITorqueConvertorConnection
    {
        private readonly float _hardFriction;
        private readonly float _minKoef = 0.03f;
        private readonly float _maxKoef = 2.9f;
        private float _stiffness;
        private float _forceStiffness;
        private float _deltaRPM;
        public float TorqueToEngine { get; private set; }
        public float TorqueToTransmission { get; private set; }
        public bool IsSynchronized => false;
        public HydroTorqueConvertorConnection(Data.EngineData engineData)
        {
            _hardFriction = engineData.HydroTorqueConvertorHardStiffness;
            _minKoef = engineData.HydroTorqueConvertorController.MinKoef;
            _maxKoef = engineData.HydroTorqueConvertorController.MaxKoef - 1;
        }
        public void ComputeTorque(float engineRPM, float transmissionRPM, float engineTorque, float clutch) {
            if (clutch <= 0)
            {
                TorqueToEngine = 0;
                TorqueToTransmission = 0;
                return;
            }
            _stiffness = _hardFriction;
            _deltaRPM = transmissionRPM - engineRPM;
            _forceStiffness = _deltaRPM * _stiffness;
            TorqueToTransmission = engineTorque * Fout(clutch);
            TorqueToEngine = _forceStiffness * Fin(clutch);
        }
        private float Fin(float value)
        {
            if (value >= 1) return 1;
            return UnityEngine.Mathf.Lerp(_minKoef, 1f, value * value);
        }
        private float Fout(float value)
        {
            if (value >= 1) return 1;
            return 1 + _maxKoef * UnityEngine.Mathf.Pow(1f - value, 2);
        }
    }
    public class TorqueConvertorConnectionNullRef : ITorqueConvertorConnection
    {
        public float TorqueToEngine => 0;
        public float TorqueToTransmission => 0;
        public bool IsSynchronized => false;
    }
}