namespace Systems.Vehicle.Transmission.Model.TCU
{
    public class TransmissionControlUnitPlanetary : TransmissionControlUnitBase
    {
        public TransmissionControlUnitPlanetary(GearBoxBase gearBox, Engine engine, TorqueConverter.TorqueConverterBase clutch, GearBoxControl.GearBoxControlBase gearBoxControl) : base(gearBox, engine, clutch, gearBoxControl)
        {
            _gearBoxControl.EventTrySetGear += UpdateGearBoxControl;
        }
        private void UpdateGearBoxControl(int gear)
        {
            _clutch.SetUnlock();
            if (_gearBox.TrySetGear(gear))
            {
                _gearBoxControl.UpdatePosition();
            }
        }
        public override void Dispose()
        {
            _gearBoxControl.EventTrySetGear -= UpdateGearBoxControl;
        }
        public override void Update() { }
    }
}