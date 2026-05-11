namespace Systems.Vehicle.Transmission.Model.TCU
{
    public abstract class TransmissionControlUnitBase
    {
        protected readonly GearBoxBase _gearBox;
        protected readonly Engine _engine;
        protected readonly TorqueConverter.TorqueConverterBase _clutch;
        protected readonly GearBoxControl.GearBoxControlBase _gearBoxControl;
        public TransmissionControlUnitBase(GearBoxBase gearBox, Engine engine, TorqueConverter.TorqueConverterBase clutch, GearBoxControl.GearBoxControlBase gearBoxControl)
        {
            _gearBox = gearBox;
            _engine = engine;
            _clutch = clutch;
            _gearBoxControl = gearBoxControl;
        }
        public abstract void Dispose();
        public abstract void Update();
    }
    public abstract class TransmissionControlUnitAutomaticBase : TransmissionControlUnitBase
    {
        protected GearBoxControl.GearBoxState _mode;
        private readonly float _delayShift;
        private float _delayTimeShift;

        public TransmissionControlUnitAutomaticBase(GearBoxBase gearBox, Engine engine, TorqueConverter.TorqueConverterBase clutch, GearBoxControl.GearBoxAutomaticControl gearBoxControl) : base(gearBox, engine, clutch, gearBoxControl)
        {
            _delayShift = .15f;
        }
        protected void UpdateGearBoxControl(int gear)
        {
            _clutch.SetUnlock();
            _gearBox.TrySetGear(gear);
        }
        protected void SetNewMode(GearBoxControl.GearBoxState mode)
        {
            _mode = mode;
            if (_mode == GearBoxControl.GearBoxState.R)
            {
                UpdateGearBoxControl(-1);
            }
            if (_mode == GearBoxControl.GearBoxState.N)
            {
                UpdateGearBoxControl(0);
            }
            if (_mode == GearBoxControl.GearBoxState.D)
            {
                if (_gearBox.GetGear <= 0)
                    UpdateGearBoxControl(1);
            }
            if (_mode == GearBoxControl.GearBoxState.M)
            {
                if (_gearBox.GetGear <= 0)
                    UpdateGearBoxControl(1);
            }
        }
        public override void Update()
        {
            if (_mode == GearBoxControl.GearBoxState.D)
            {
                UpdateDriveLogic();
            }
        }
        protected void ChangeBySequentialInManualMode(int delta)
        {
            UpdateGearBoxControl(UnityEngine.Mathf.Max(_gearBox.GetGear + delta, 1));
        }
        protected void UpdateDriveLogic()
        {
            if (_clutch.IsEngaged)
            {
                if (_delayTimeShift <= 0)
                {
                    int gear = _gearBox.GetGear;
                    if (gear > 1)
                    {
                        if (_engine.GetRPM < _engine.DownShiftRPM)
                        {
                            UpdateGearBoxControl(gear - 1);
                            _delayTimeShift = _delayShift;
                            return;
                        }
                    }
                    if (gear < _gearBox.GetMaxGear)
                    {
                        if (_engine.GetRPM > _engine.UpShiftRPM)
                        {
                            UpdateGearBoxControl(gear + 1);
                            _delayTimeShift = _delayShift;
                            return;
                        }
                    }
                }
                else
                {
                    _delayTimeShift -= UnityEngine.Time.fixedDeltaTime;
                }
            }
            else
            {
                if (_gearBox.GetGear > 1)
                    UpdateGearBoxControl(1);
            }
        }
    }
}