using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CastExtension
{


    static public bool CylinderCast(Vector3 position, Vector3 up, Vector3 forward, float width, float radius, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, out RaycastHit hit, int segmentsPerQuater = 4, bool showDebugLines = false)
    {
        IEnumerable<RaycastHit> hits = CylinderCastAll(position, up, forward, width, radius, direction, maxDistance, layerMask, queryTriggerInteraction, segmentsPerQuater, showDebugLines);

        bool didHit = hits.Count() > 0;
        float closestDist = didHit ? Enumerable.Min(hits.Select(hit => hit.distance)) : 0;
        hit = hits.Where(hit => hit.distance == closestDist).FirstOrDefault();
        return didHit;
    }
    static public RaycastHit[] CylinderCastAll(Vector3 position, Vector3 up, Vector3 forward, float width, float radius, Vector3 direction, float maxDistance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, int segmentsPerQuater = 4, bool showDebugLines = false)
    {
        float anglePerSegment = 90f / segmentsPerQuater;
        Vector2 angleVecotor = new Vector2(Mathf.Cos(anglePerSegment * Mathf.Deg2Rad), Mathf.Sin(anglePerSegment * Mathf.Deg2Rad));

        float halfBoxWidth = width / 2;
        float halfBoxLength = (-Vector2.right + angleVecotor).magnitude * radius / 2;
        float halfBoxHeight = (Vector2.right + angleVecotor).magnitude * radius / 2;
        Vector3 boxHalfExtents = new Vector3(halfBoxWidth, halfBoxLength, halfBoxHeight);

        List<RaycastHit> hits = new();
        for (int i = 0; i < segmentsPerQuater * 2; i++)
        {
            hits.AddRange(Physics.BoxCastAll(position, boxHalfExtents, direction,
                Quaternion.AngleAxis((i + 0.5f) * anglePerSegment, up) * Quaternion.LookRotation(forward, Vector3.Cross(forward, up)),
                maxDistance, layerMask, queryTriggerInteraction));
        }

        // Debug
        if (showDebugLines)
        {
            for (int i = 0; i < segmentsPerQuater * 4; i++)
            {
                Vector3 rotation0 = Quaternion.AngleAxis(i * anglePerSegment, up) * forward;
                Vector3 rotation1 = Quaternion.AngleAxis((i + 1) * anglePerSegment, up) * forward;
                Vector3 pos1 = position + rotation1 * radius;
                Vector3 pos0 = position + rotation0 * radius;
                Vector3 rightOffset = up * halfBoxWidth;

                Debug.DrawLine(pos0 + rightOffset, pos1 + rightOffset);
                Debug.DrawLine(pos0 - rightOffset, pos1 - rightOffset);
                Debug.DrawLine(pos0 - rightOffset, pos0 + rightOffset);
                Debug.DrawLine(pos1 - rightOffset, pos1 + rightOffset);

                Vector3 directionOffset = direction * maxDistance;
                Debug.DrawLine(pos0 + rightOffset + directionOffset, pos1 + rightOffset + directionOffset, Color.cyan);
                Debug.DrawLine(pos0 - rightOffset + directionOffset, pos1 - rightOffset + directionOffset, Color.cyan);
                Debug.DrawLine(pos0 - rightOffset + directionOffset, pos0 + rightOffset + directionOffset, Color.cyan);
                Debug.DrawLine(pos1 - rightOffset + directionOffset, pos1 + rightOffset + directionOffset, Color.cyan);
            }

            foreach (var hit in hits) if (hit.point != Vector3.zero) Debug.DrawLine(hit.point - direction * hit.distance, hit.point, Color.green);
        }
        return hits.ToArray();
    }

}
