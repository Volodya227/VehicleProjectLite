namespace Systems.Vehicle.FuelSystem
{
    public class FuelSystem : Systems.Core.FuelSystem.FuelSystemBase
    {
        private readonly ContainerData.VehicleContainerData _containerData;
        public FuelSystem(Data.VehicleData data, ContainerData.VehicleContainerData containerData) : base(data.MaxFuel, 0)
        {
            _containerData = containerData;
            _containerData.SetMaxFuel(_maxFuel);
            AddFuel(_maxFuel);
        }
        protected override void ChangeFuel()
        {
            _containerData.SetFuel(_fuel);
        }
    }
}