namespace Systems.Vehicle.Transmission.Model.TCU
{
    public class TransmissionControlUnitAutomatedManual : TransmissionControlUnitAutomaticBase
    {
        private readonly GearBoxControl.GearBoxAutomaticControl _gearBoxAutomaticControl;
        private readonly float _delayShift;
        private float _delayTimeShift;
        public TransmissionControlUnitAutomatedManual(GearBoxManual gearBox, Engine engine, TorqueConverter.TorqueConverterBase clutch, GearBoxControl.GearBoxAutomaticControl gearBoxControl) : base(gearBox, engine, clutch, gearBoxControl)
        {
            _gearBoxAutomaticControl = gearBoxControl;
            _gearBoxAutomaticControl.EventChangeState += SetNewMode;
            _gearBoxAutomaticControl.EventChangeGearDelta += ChangeBySequentialInManualMode;
        }
        public override void Dispose()
        {
            _gearBoxAutomaticControl.EventChangeState -= SetNewMode;
            _gearBoxAutomaticControl.EventChangeGearDelta -= ChangeBySequentialInManualMode;
        }
    }
}