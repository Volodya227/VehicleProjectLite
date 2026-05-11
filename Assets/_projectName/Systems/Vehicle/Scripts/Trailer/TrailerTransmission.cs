using UnityEngine;
namespace Systems.Vehicle.Trailer.Transmission
{
    [System.Serializable]
    public class TrailerTransmission
    {
        [SerializeField] private Vehicle.Transmission.View.AxleInfo[] _axleInfos;
        private Input.TrailerInput _input;
        [SerializeField] private float _brake;
        public TrailerTransmission(Rigidbody body, Data.TrailerData data, GameObject prefabAxleCollider, GameObject prefabAxleController)
        {
            _axleInfos = new Vehicle.Transmission.View.AxleInfo[data.AxleCount];
            for (int i = 0; i < data.AxleCount; i++)
            {
                _axleInfos[i] = new Vehicle.Transmission.View.AxleInfo(body, null, data.GetAxleRear(i), data.WheelData, prefabAxleCollider, prefabAxleController, null);
                _axleInfos[i].SetAxlePrefab(data.GetWheelSetupData(), i);
                _axleInfos[i].SetTorque(.001f, 0);
                _axleInfos[i].UnlockedDifferential();
            }
        }
        public void SetInput(Input.TrailerInput input)
        {
            _input = input;
        }
        public void Update()
        {
            _brake = _input.BrakingTrailer;
            for (int i = 0; i < _axleInfos.Length; i++)
            {
                _axleInfos[i].SetBrake(_input.BrakingTrailer);
                _axleInfos[i].Update();
            }
        }
    }
}