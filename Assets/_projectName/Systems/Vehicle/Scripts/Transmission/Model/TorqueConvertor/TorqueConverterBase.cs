using UnityEngine;

namespace Systems.Vehicle.Transmission.Model.TorqueConverter
{
    public abstract class TorqueConverterBase
    {
        protected abstract ITorqueConvertorConnection GetConnection { get; }
        protected readonly bool _isClutch;
        protected float _clutch;
        private float _forceTransmission;
        protected float _engineTorque;
        protected float _gearKoef;
        protected float _engineShaftRPM = 0;
        protected float _gearBoxShaftRPM = 0;
        public float GetRPMGearBoxShaft => _gearBoxShaftRPM;
        public float GetClutch => _clutch;
        public bool IsSynchronized => GetConnection.IsSynchronized;
        public bool IsEngaged => _clutch >= 1;
        public abstract bool CanChangeGear { get; }
        public TorqueConverterBase(bool clutch = false)
        {
            _isClutch = clutch;
        }
        public virtual void SetUnlock() { }
        public void SetGearBoxShaftRPM(float value) { _gearBoxShaftRPM = value; }
        public void SetTorqueEngine(float value) { _engineTorque = value; }
        public void SetGearKoef(float value) { _gearKoef = value; }
        public abstract void UpdateInput(float clutch = 1);
        public float SetTorqueToEngine(float RPM)
        {
            _forceTransmission = 0;
            _engineShaftRPM = RPM;
            if (!IsIncludeTransmission())
                return 0;
            ITorqueConvertorConnection connection = GetConnection;
            ComputeConnectionTorque();
            _forceTransmission = connection.TorqueToTransmission;
            return connection.TorqueToEngine;
        }
        public abstract void ComputeConnectionTorque();
        protected bool IsIncludeTransmission()
        {
            if (_gearKoef == 0) return false;
            return true;
        }
        public float GetTorqueFromEngine()
        {
            if (GetConnection.IsSynchronized) return _engineTorque;
            return _forceTransmission;
        }
    }
}