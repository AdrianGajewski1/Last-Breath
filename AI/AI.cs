﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LB.AI 
{

    public abstract class AI : MonoBehaviour
    {

        //NOTE: Call this function inside a OnDrawGizmos function
        //This can be used to visualize field of view
        public void DrawGizmos(Transform center, float radius, float angle)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center.position, radius);

            Vector3 firstLine = Quaternion.AngleAxis(angle, center.up) * center.forward * radius;
            Vector3 secondLine = Quaternion.AngleAxis(-angle, center.up) * center.forward * radius;

            Gizmos.color = Color.blue;

            Gizmos.DrawRay(center.position, firstLine);
            Gizmos.DrawRay(center.position, secondLine);
        }

        //NOTE: Call this inside Update or FixedUpdate function
        public Transform ScanForTarget<T>(Transform center, LayerMask l_mask, float maxRadius, float maxAngle) where T: MonoBehaviour
        {
            Collider[] potentialTargets = Physics.OverlapSphere(center.position, maxRadius);

            foreach (Collider col in potentialTargets)
            {
                var requiredComponent = col.transform.GetComponent<T>();

                if (requiredComponent != null && requiredComponent.enabled == true)
                {
                    var target = col.transform;

                    if (target != null && IsInLineOfSight(target, center, maxAngle, maxRadius, l_mask))
                        return target;
                    else
                        return null;
                }
            }

            return null;
        }

        public Transform[] ScanForTargets<T>(Transform center, LayerMask l_mask, float maxRadius, float maxAngle) where T : MonoBehaviour
        {
            Queue<Transform> targets = new Queue<Transform>();

            Collider[] potentialTargets = Physics.OverlapSphere(center.position, maxRadius);

            //if we have any targets
            if(potentialTargets.Length > 0)
            {
                foreach (var target in potentialTargets)
                {
                    var requiredComponent = target.transform.GetComponent<T>();


                    if (requiredComponent != null && requiredComponent.enabled == true)
                    {
                        if (IsInLineOfSight(target.transform, center, maxAngle, maxRadius, l_mask)) 
                            targets.Enqueue(target.transform);
                    }
                    else
                        continue;
                }
            }

            return targets.ToArray();
        }

        public bool IsInLineOfSight(Transform target, Transform checkingObject, float maxAngle, float range, LayerMask mask)
        {
            Vector3 direction = (target.position - checkingObject.position).normalized;

            float distance = Vector3.Distance(checkingObject.position, target.position);
            if (Vector3.Angle(checkingObject.forward, direction) <= maxAngle)
            {
                if (Physics.Raycast(checkingObject.position, direction.normalized, distance, mask))
                    return false;

                return true;
            }

            return false;
        }

        public Quaternion RotateToTarget(Transform obj, Transform target, float speed)
        {
            var targetRotation = Quaternion.LookRotation(target.transform.position - obj.position);

            obj.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);

            return obj.rotation;
        }

        public void SetDestination(ref NavMeshAgent agent, IList<Transform> waypoints)
        {
            var index = UnityEngine.Random.Range(0, waypoints.Count);
            var waypoint = waypoints[index].position;

            agent.SetDestination(waypoint);
        }

        public IEnumerator WaitForAction(float s)
        {
            yield return new WaitForSeconds(s);
        }
    }
}
