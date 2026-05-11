using UnityEngine;
namespace Systems.Vehicle.Transmission.View
{
    public class FactoryWheels
    {
        public static void CreateWheelCollider(out Wheel left, out Wheel right, GameObject prefabAxle, Rigidbody body, Data.AxleData axleData, Data.WheelData wheelData)
        {
            GameObject axle = Object.Instantiate(original: prefabAxle, parent: body.transform);
            axle.transform.SetLocalPositionAndRotation(new Vector3(0, axleData.Y, axleData.Z), Quaternion.identity);
            WheelCollider[] wl = axle.GetComponentsInChildren<WheelCollider>();
            wl[0].transform.localPosition = new Vector3(-axleData.X, 0, 0);
            wl[0].mass = axleData.WheelMassCollider + wheelData.WheelMass;
            wl[0].suspensionDistance = axleData.SuspensionDistance;
            wl[0].suspensionSpring = wl[1].suspensionSpring = new()
            {
                spring = axleData.SpringCollider,
                damper = axleData.DamperCollider,
                targetPosition = axleData.TargetPosition
            };

            wl[0].forceAppPointDistance = axleData.ForceAppPointDistance;

            wl[1].transform.localPosition = new Vector3(axleData.X, 0, 0);
            wl[1].mass = axleData.WheelMassCollider + wheelData.WheelMass;
            wl[1].suspensionDistance = axleData.SuspensionDistance;

            wl[1].forceAppPointDistance = axleData.ForceAppPointDistance;

            left = new WheelWithCollider(wl[0], wheelData, false);
            right = new WheelWithCollider(wl[1], wheelData, true);
        }
        public static bool CreateWheelController3D(out Wheel left, out Wheel right, GameObject prefabAxle, Rigidbody body, Data.AxleData axleData, Data.WheelData wheelData) {
            left = null;
            right = null;
            return false;
            //WheelWithController3d.Factory(out left, out right, prefabAxle, body, axleData, wheelData);
            //return true;
        }
    }
}