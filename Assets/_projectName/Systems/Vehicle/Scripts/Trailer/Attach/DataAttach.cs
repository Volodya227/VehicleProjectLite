using UnityEngine;
namespace Systems.Vehicle.Trailer.Attach.Data
{
    public class DataAttach
    {
        public Rigidbody Body { private set; get; }
        public Vector3 PositionConnectedBody { private set; get; }
        public DataAttach(Rigidbody body, Vector3 positionConnectedBody)
        {
            Body = body;
            PositionConnectedBody = positionConnectedBody;
        }
    }
}