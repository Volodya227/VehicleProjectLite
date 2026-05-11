using System.Collections.Generic;
using UnityEngine;
namespace Systems.Vehicle.Passenger
{
    public class VehiclePassengerSystem
    {
        public event System.Action ResetInput;
        private List<VehiclePassengerZone> _zones;
        private readonly Visual.ModelItem _visual;
        private readonly ContainerData.VehicleContainerData _vehicleContainerData;
        public VehiclePassengerSystem(Visual.ModelItem visual, ContainerData.VehicleContainerData vehicleContainerData)
        {
            _visual = visual;
            InitZones();
            _vehicleContainerData = vehicleContainerData;
        }
        private void InitZones()
        {
            _zones = new List<VehiclePassengerZone>();
            VehiclePassengerZone zone = null;
            Visual.ZonePassenger config = null;
            List<Visual.VehicleVisualDoor> doors = null;
            List<VehicleChair> chairs = null;
            Visual.VehicleVisualChair chairConfig = null;
            for (int i = 0; i < _visual.ZonePassengerCount; i++)
            {
                config = _visual.GetZonePassenger(i);
                chairs = new List<VehicleChair>();
                doors = new List<Visual.VehicleVisualDoor>();
                for (int j = 0; j < config.DoorsCount; j++)
                {
                    doors.Add(config.getDoor(j));
                }
                for (int j = 0; j < config.ChairsCount; j++)
                {
                    chairConfig = config.getChair(j);
                    chairs.Add(new VehicleChair(chairConfig.Driver, true, chairConfig.Collider, chairConfig.OutPoint, chairConfig.FirstPersonView));
                }
                //TODO
                zone = new VehiclePassengerZone(i, config.Moveable, 1, doors, chairs);
                _zones.Add(zone);
            }
        }
        public void InteractPassengerWithVehicle(IVehiclePassenger passenger, Collider collider)
        {
            int zoneID = PassengerInZone(passenger);
            if (zoneID < 0)
            {
                zoneID = DoorInZone(collider);
                if (_zones[zoneID].IsFreeForPassenger())
                {
                    if (_zones[zoneID].Moveable)
                    {
                        // only entry
                        _zones[zoneID].SetNewPassenger(passenger);//save passenger
                        passenger.EnterVehicle(_visual.ThirdView, _zones[zoneID].Door(_zones[zoneID].GetDoorIndex(collider)).EnterPoint, _vehicleContainerData);
                    }
                    else
                    {
                        _zones[zoneID].SetNewPassenger(passenger);//save passenger
                        passenger.EnterVehicle(_visual.ThirdView, null, _vehicleContainerData);// copy paste from moveable

                        // coult do when Moveable == false;
                        VehicleChair chair = _zones[zoneID].GetChairFree();
                        chair.Assign(passenger);
                        passenger.SitInVehicleChair(chair.FirstPersonView, chair.Driver);
                    }
                }
                else { passenger.LoseVehicle(); }
            }
            else
            {
                if (_zones[zoneID].Moveable) {
                    //find door, or chair not now
                }
                else
                {
                    if (_zones[zoneID].Chair(0).Single)
                    {
                        //passenger standUp
                        VehicleChair chair = _zones[zoneID].Chair(0);
                        chair.Leave();
                        if (chair.Driver)
                        {
                            
                            ResetInput.Invoke();
                        }
                        passenger.StandUpVehicleChair(null);
                        //passenger exit
                        _zones[zoneID].ForgetPassenger(passenger);
                        passenger.ExitVehicle(_zones[zoneID].Door(0).ExitPoint);
                    }
                    else
                    {
                        int chairIndex = _zones[zoneID].ChairIndex(collider);// this zone
                        if (chairIndex < 0)// don't find chair
                        {
                            int indexDoor = _zones[zoneID].GetDoorIndex(collider);//only this zone
                            if (indexDoor < 0) { indexDoor = 0; }

                            VehicleChair chair = _zones[zoneID].getChairByPassenger(passenger);
                            chair.Leave();
                            if (chair.Driver)
                            {
                                ResetInput.Invoke();
                            }
                            passenger.StandUpVehicleChair(null);
                            //passenger exit
                            _zones[zoneID].ForgetPassenger(passenger);
                            passenger.ExitVehicle(_zones[zoneID].Door(indexDoor).ExitPoint);//exit by default door
                        }
                        else
                        {
                            VehicleChair newChair = _zones[zoneID].Chair(chairIndex);
                            if (newChair.IsFree) {
                                //standUp
                                VehicleChair chair = _zones[zoneID].getChairByPassenger(passenger);
                                chair.Leave();
                                if (chair.Driver)
                                {
                                    ResetInput.Invoke();
                                }
                                //sit
                                newChair.Assign(passenger);
                                passenger.SitInVehicleChair(newChair.FirstPersonView, newChair.Driver);
                            }
                        }
                    }
                }
            }
        }
        private int PassengerInZone(IVehiclePassenger passenger)
        {
            int result = -1;
            for (int i = 0; i < _zones.Count; i++)
            {
                if (_zones[i].IsPassengerPresent(passenger))
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
        private int DoorInZone(Collider door)
        {
            int result = -1;
            for (int i = 0; i < _zones.Count; i++)
            {
                if (_zones[i].GetDoorIndex(door)>=0)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
    }
}