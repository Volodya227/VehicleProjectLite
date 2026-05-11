using UnityEngine;
namespace Systems.Vehicle.Passenger
{
    public interface IVehiclePassenger
    {
        public void EnterVehicle(Transform thirdView, Transform enterPoint, ContainerData.IVehicleContainerData vehicleContainerData);
        public void ExitVehicle(Transform exitPoint);
        public void SitInVehicleChair(Transform firstView, bool driver);
        public void StandUpVehicleChair(Transform exitPoint);
        public void LoseVehicle();
    }
}