namespace Systems.Vehicle.Transmission.Model.TCU
{
    public class TransmissionControlUnitManual : TransmissionControlUnitBase
    {
        public TransmissionControlUnitManual(GearBoxBase gearBox, Engine engine, TorqueConverter.TorqueConverterBase clutch, GearBoxControl.GearBoxControlBase gearBoxControl) : base(gearBox, engine, clutch, gearBoxControl)
        {
            _gearBoxControl.EventTrySetGear += UpdateGearBoxControl;
        }
        private void UpdateGearBoxControl(int gear)
        {
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