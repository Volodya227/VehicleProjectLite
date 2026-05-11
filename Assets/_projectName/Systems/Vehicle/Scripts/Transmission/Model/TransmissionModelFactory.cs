namespace Systems.Vehicle.Transmission.Model
{
    public struct TransmissionModelContext
    {
        public Data.VehicleData data;
        public FuelSystem.FuelSystem fuelSystem;
        public ContainerData.VehicleContainerData vehicleContainerData;
    }
    public struct TransmissionModelGeneratedState
    {
        public TorqueConverter.TorqueConverterBase TorqueConverter;
        public Engine Engine;
        public TCU.TransmissionControlUnitBase TCU;
        public GearBoxBase GearBox;
        public GearBoxControl.GearBoxControlBase GearControl;
    }
    public static class TransmissionModelFactory
    {
        public static TransmissionModelGeneratedState CreateDefault(TransmissionModelContext context) => CreateManual(context);
        public static TransmissionModelGeneratedState CreateManual(TransmissionModelContext context)
        {
            TorqueConverter.TorqueConverterBase torqueConverter = new TorqueConverter.Clutch(context.data.EngineData);//Clutch
            Engine engine = new(context.data.EngineData, torqueConverter, context.vehicleContainerData, context.fuelSystem);
            GearBoxManual gearBoxManual = new(context.data, context.vehicleContainerData, torqueConverter);
            GearBoxControl.GearBoxControlBase gearBoxControlBase = new GearBoxControl.GearBoxManualControl(context.data.GearBoxManualControlData, context.vehicleContainerData);
            TCU.TransmissionControlUnitBase transmissionControlUnitBase = new TCU.TransmissionControlUnitManual(gearBoxManual, engine, torqueConverter, gearBoxControlBase);
            return new TransmissionModelGeneratedState()
            {
                TorqueConverter = torqueConverter,
                Engine = engine,
                TCU = transmissionControlUnitBase,
                GearBox = gearBoxManual,
                GearControl = gearBoxControlBase
            };
        }
        public static TransmissionModelGeneratedState CreateAutomatedManual(TransmissionModelContext context)
        {
            TorqueConverter.TorqueConverterBase torqueConverter = new TorqueConverter.Clutch(context.data.EngineData, true);
            Engine engine = new(context.data.EngineData, torqueConverter, context.vehicleContainerData, context.fuelSystem);
            GearBoxManual gearBoxManual = new(context.data, context.vehicleContainerData, torqueConverter);
            GearBoxControl.GearBoxAutomaticControl gearBoxControlBase = new(context.data.GearBoxAutomaticControlData, context.vehicleContainerData);
            TCU.TransmissionControlUnitBase transmissionControlUnitBase = new TCU.TransmissionControlUnitAutomatedManual(gearBoxManual, engine, torqueConverter, gearBoxControlBase);
            return new TransmissionModelGeneratedState()
            {
                TorqueConverter = torqueConverter,
                Engine = engine,
                TCU = transmissionControlUnitBase,
                GearBox = gearBoxManual,
                GearControl = gearBoxControlBase
            };
        }
        public static TransmissionModelGeneratedState CreateAutomated(TransmissionModelContext context)
        {
            TorqueConverter.TorqueConverterBase torqueConverter = new TorqueConverter.HydroTorqueConverter(context.data.EngineData);
            Engine engine = new(context.data.EngineData, torqueConverter, context.vehicleContainerData, context.fuelSystem);
            GearBoxAuto gearBoxAuto = new(context.data, context.vehicleContainerData, torqueConverter);
            GearBoxControl.GearBoxAutomaticControl gearBoxControlBase = new(context.data.GearBoxAutomaticControlData, context.vehicleContainerData);
            TCU.TransmissionControlUnitBase transmissionControlUnitBase = new TCU.TransmissionControlUnitAutomatic(gearBoxAuto, engine, torqueConverter, gearBoxControlBase);
            return new TransmissionModelGeneratedState()
            {
                TorqueConverter = torqueConverter,
                Engine = engine,
                TCU = transmissionControlUnitBase,
                GearBox = gearBoxAuto,
                GearControl = gearBoxControlBase
            };
        }
        public static TransmissionModelGeneratedState CreatePlanetary(TransmissionModelContext context)
        {
            TorqueConverter.TorqueConverterBase torqueConverter = new TorqueConverter.HydroTorqueConverter(context.data.EngineData);
            Engine engine = new(context.data.EngineData, torqueConverter, context.vehicleContainerData, context.fuelSystem);
            GearBoxAuto gearBoxAuto = new(context.data, context.vehicleContainerData, torqueConverter);
            GearBoxControl.GearBoxControlBase gearBoxControlBase = new GearBoxControl.GearBoxPlanetaryControl(context.data.GearBoxManualControlData, context.vehicleContainerData);
            TCU.TransmissionControlUnitBase transmissionControlUnitBase = new TCU.TransmissionControlUnitPlanetary(gearBoxAuto, engine, torqueConverter, gearBoxControlBase);
            return new TransmissionModelGeneratedState()
            {
                TorqueConverter = torqueConverter,
                Engine = engine,
                TCU = transmissionControlUnitBase,
                GearBox = gearBoxAuto,
                GearControl = gearBoxControlBase
            };
        }
    }
}