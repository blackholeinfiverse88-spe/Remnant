
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserReflectionController : MonoBehaviour
{
    public float maxDistance = 30f;
    public int maxReflections = 5;
    public LayerMask laserLayers = ~0;

    private LineRenderer line;
    private readonly List<Vector3> points = new List<Vector3>();

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
    }

    private void Update()
    {
        TraceLaser();
    }

    private void TraceLaser()
    {
        points.Clear();

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        points.Add(origin);

        for (int i = 0; i <= maxReflections; i++)
        {
            if (!Physics.Raycast(origin, direction, out RaycastHit hit,
                maxDistance, laserLayers, QueryTriggerInteraction.Ignore))
            {
                points.Add(origin + direction * maxDistance);
                break;
            }

            Debug.DrawLine(origin, hit.point, Color.red);
            points.Add(hit.point);

            Debug.Log("Laser hit: " + hit.collider.name);

            LaserMirror mirror =
                hit.collider.GetComponentInParent<LaserMirror>();

            if (mirror == null)
            {
                Debug.Log("Not a mirror. Stopping beam.");
                break;
            }

            Debug.Log("Reflecting from: " + mirror.name);

            direction = Vector3.Reflect(direction, hit.normal).normalized;
            origin = hit.point + direction * 0.02f;
        }

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }
}   