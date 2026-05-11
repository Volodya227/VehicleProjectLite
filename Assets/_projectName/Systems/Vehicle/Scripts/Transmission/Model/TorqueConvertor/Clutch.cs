using UnityEngine;
namespace Systems.Vehicle.Transmission.Model.TorqueConverter
{
    public class Clutch : TorqueConverterBase
    {
        private readonly ClutchConnection _clutchConnection;
        protected override ITorqueConvertorConnection GetConnection => _clutchConnection;
        private float _clutchInput;
        private float _clutchInputNew;
        private readonly bool _isAutoClutch;
        //autoClutch
        private readonly int _stallRPM;
        private readonly int _maxRPM;
        private readonly int _idleRPM;

        private readonly float _minClutchValue = .15f;
        private readonly float _clutchRPMFactor = .35f;

        private readonly float _maxTorqueScale = 3;
        private readonly float _minTorqueScale = .6f;
        public override bool CanChangeGear => _clutch == 0;
        public Clutch(Data.EngineData data, bool isAutoClutch = false) : base(true)
        {
            _clutchConnection = new ClutchConnection(data);
            _isAutoClutch = isAutoClutch;
            _stallRPM = data.StallRPM;
            _idleRPM = data.IdleRPM;
            _maxRPM = data.LimitRPM;
            _minClutchValue = data.AutoClutchController.MinClutchValue;
            _clutchRPMFactor = data.AutoClutchController.ClutchRPMFactor;
            _maxTorqueScale = data.AutoClutchController.MaxTorqueScale;
            _minTorqueScale = data.AutoClutchController.MinTorqueScale;
            UpdateInput(1);
        }
        public override void UpdateInput(float clutch)
        {
            if (_isAutoClutch)
                AutoClutchLogic();
            else
                _clutchInputNew = clutch;
            if (_clutchInput == _clutchInputNew)
                return;
            _clutchInput = _clutchInputNew;
            _clutch = ClutchF(_clutchInput);
            _clutchConnection.Unsynchronized();
        }
        private void AutoClutchLogic()
        {
            if (_gearKoef == 0) { _clutchInputNew = 1; return; }
            float engine = Mathf.Max(1, _engineShaftRPM);
            float min = (_gearBoxShaftRPM < -400) ? 0 : _minClutchValue + _clutchRPMFactor * (engine - _stallRPM) / (_maxRPM - _stallRPM);
            float wheel = Mathf.Max(0, _gearBoxShaftRPM);
            engine *= Mathf.Lerp(_maxTorqueScale, _minTorqueScale, (wheel - _stallRPM) / (_idleRPM - _stallRPM));
            _clutchInputNew = Mathf.Clamp(wheel / engine, min, 1);
        }
        public override void SetUnlock()
        {
            if (_isAutoClutch)
            {
                _clutch = _clutchInput = 0;
            }
        }
        private float ClutchF(float clutch)
        {
            return Mathf.Clamp01(1.75f * Mathf.Sin(.7f * (clutch - .1f)));
        }
        public override void ComputeConnectionTorque() {
            _clutchConnection.ComputeTorque(_engineShaftRPM, _gearBoxShaftRPM, _clutch);
        }
    }
}