using UnityEngine;
namespace Systems.Vehicle.Trailer.Data
{
    [System.Serializable]
    public class TrailerData
    {
        [SerializeField] private Vehicle.Data.WheelSetupData _wheelSetupData = new();
        [SerializeField] private Vehicle.Data.AxleData[] _axleRearData;
        [SerializeField] private Vehicle.Data.WheelData _wheelData;
        public int AxleCount => _axleRearData.Length;
        public Vehicle.Data.WheelData WheelData => _wheelData;
        public Vehicle.Data.AxleData GetAxleRear(int index) => _axleRearData[index];
        public Vehicle.Data.WheelSetupData GetWheelSetupData() { _wheelSetupData.SetAxleCount(AxleCount); return _wheelSetupData; }
    }
}