using UnityEngine;
using System.Collections;

public class CarCameraFollow1 : MonoBehaviour
{
    public GameObject carObject;
    private ArcadeVehicle m_car;
    public Transform cameraSets;

    public Transform targetLookat;
    public Transform targetPosition;
    public Transform raycastEnd;

    public float moveSmoothness = 2f;
    public float rotateSmoothness = 2f;

    public LayerMask layerMasks;

    void Awake()
    {
         m_car = carObject.GetComponent<ArcadeVehicle>();
    }

    void Start()
    {

    }

    //Make camera look at car from low angle. if camera hits layer move up and rotate down towards car.
    void LateUpdate()
    {
        cameraSets.position = carObject.transform.position;

        float rotSmooth = 1f + (m_car.getSpeed() / 10) * rotateSmoothness;


        cameraSets.rotation = Quaternion.Lerp(cameraSets.rotation, carObject.transform.rotation, rotSmooth * Time.deltaTime);

        float yPosition = Mathf.Lerp(transform.position.y, targetPosition.position.y, moveSmoothness * Time.deltaTime);
        if (yPosition < m_car.transform.position.y)
        {
            yPosition = m_car.transform.position.y;
        }
        float xdiff;
        float zdiff;

        Vector3 forward = targetPosition.TransformDirection(Vector3.forward);
        //print(forward);
        if (!Main.S.practicing && !Main.S.raceStarted)
        {
            xdiff = Mathf.Lerp(transform.position.x, targetPosition.position.x, moveSmoothness * Time.deltaTime);
            zdiff = Mathf.Lerp(transform.position.z, targetPosition.position.z, moveSmoothness * Time.deltaTime);
        }
        else
        {
            xdiff = targetPosition.position.x;
            zdiff = targetPosition.position.z;
        }
        //Vector3 newPosition = new Vector3(targetPosition.position.x, yPosition, targetPosition.position.z);
        Vector3 newPosition = new Vector3(xdiff, yPosition, zdiff);

        RaycastHit wallHit = new RaycastHit();

        if (Physics.Linecast(targetLookat.position, raycastEnd.position, out wallHit, layerMasks))
        {
            Vector3 hitPoint = wallHit.point + 0.5f * wallHit.normal.normalized;

            newPosition.x = hitPoint.x; newPosition.z = hitPoint.z;
        }

        transform.position = newPosition;

        var quaterion = Quaternion.LookRotation(targetLookat.position - transform.position);
        transform.rotation = quaterion;
    }


}
