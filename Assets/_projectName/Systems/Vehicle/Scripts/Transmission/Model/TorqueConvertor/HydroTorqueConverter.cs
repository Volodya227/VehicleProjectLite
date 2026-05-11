using UnityEngine;
namespace Systems.Vehicle.Transmission.Model.TorqueConverter
{
    public class HydroTorqueConverter : TorqueConverterBase
    {
        private readonly HydroTorqueConvertorConnection _hydroTorqueConvertorConnection;
        private readonly ClutchConnection _lockConnection;
        private bool _lock;
        protected override ITorqueConvertorConnection GetConnection => _lock ? _lockConnection : _hydroTorqueConvertorConnection;
        private readonly int _stallRPM;
        private readonly int _maxRPM;
        private readonly int _idleRPM;

        private readonly float _minClutchValue = .005f;
        private readonly float _clutchRPMFactor = .4f;

        private readonly float _maxTorqueScale = 5;
        private readonly float _minTorqueScale = .6f;
        public override bool CanChangeGear => !_lock;
        public HydroTorqueConverter(Data.EngineData data) : base(false)
        {
            _lock = false;
            _hydroTorqueConvertorConnection = new HydroTorqueConvertorConnection(data);
            _lockConnection = new ClutchConnection(data);
            _stallRPM = data.StallRPM;
            _idleRPM = data.IdleRPM;
            _maxRPM = data.LimitRPM;
            _minClutchValue = data.HydroTorqueConvertorController.Controller.MinClutchValue;
            _clutchRPMFactor = data.HydroTorqueConvertorController.Controller.ClutchRPMFactor;
            _maxTorqueScale = data.HydroTorqueConvertorController.Controller.MaxTorqueScale;
            _minTorqueScale = data.HydroTorqueConvertorController.Controller.MinTorqueScale;
            UpdateInput(1);
        }
        public override void ComputeConnectionTorque()
        {
            if(_lock)
                _lockConnection.ComputeTorque(_engineShaftRPM, _gearBoxShaftRPM, 1);
            else
                _hydroTorqueConvertorConnection.ComputeTorque(_engineShaftRPM, _gearBoxShaftRPM, _engineTorque, _clutch);
        }
        public override void SetUnlock()
        {
            if (_lock)
                SetLock(false);
        }
        public override void UpdateInput(float clutch)
        {
            if (_gearKoef == 0) { _clutch = 1; return; }
            float engine = Mathf.Max(1, _engineShaftRPM);
            float min = (_gearBoxShaftRPM < -400) ? 0 : _minClutchValue + _clutchRPMFactor * (engine - _stallRPM) / (_maxRPM - _stallRPM);
            float wheel = Mathf.Max(0, _gearBoxShaftRPM);
            engine *= Mathf.Lerp(_maxTorqueScale, _minTorqueScale, (wheel - _stallRPM) / (_idleRPM - _stallRPM));
            _clutch = min == 0 ? -1 : Mathf.Max(wheel / engine, min);
            if (_lock && _clutch < 1)
            {
                SetLock(false);
            }
            if(!_lock && _clutch >= 1 && Mathf.Abs(_gearKoef) <= 50)//todo target from config?
                SetLock(true);
        }
        public void SetLock(bool value)
        {
            _lock = value;
        }
    }
}