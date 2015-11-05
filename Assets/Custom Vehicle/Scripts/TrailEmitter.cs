using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Trails;
namespace Trails
{
    // Created by Edward Kay-Coles a.k.a Hoeloe
    public class TrailEmitter : MonoBehaviour
    {
        //Stores all live trails
        private LinkedList<Trail> trails = new LinkedList<Trail>();
        public static Transform skidTrailsDetachedParent;

        //Parameters
        public float width = 0.1f;
        public float decayTime = 1f;
        public Material material;
        public int roughness = 0;
        public bool softSourceEnd = false;

        void Start()
        {
            if (skidTrailsDetachedParent == null)
            {
                skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
            }
        }

        //Checks if the most recent trail is active or not
        public bool Active
        {
            get { return (trails.Count == 0 ? false : (!trails.Last.Value.Finished)); }
        }

        // Update is called once per frame
        void Update()
        {
            //Don't update if there are no trails
            if (trails.Count == 0) return;
            //Essentially a foreach loop, allowing trails to be removed from the list if they are finished
            LinkedListNode<Trail> t = trails.First;
            LinkedListNode<Trail> n;
            do
            {
                n = t.Next;
                t.Value.UpdateTrail();
                if (t.Value.Dead)
                    trails.Remove(t);
                t = n;
            } while (n != null);
        }
        /// <summary>
        /// Creates a new trail.
        /// </summary>
        public void NewTrail(Transform transf, float wheelRadius)
        {
            if (Active) return;
            //Stops emitting the last trail and passes the parameters onto a new one
            EndTrail();

            Trail trail = new Trail(transf, material, decayTime, roughness, softSourceEnd, width, wheelRadius);
            trail.trail.transform.parent = skidTrailsDetachedParent;

            trails.AddLast(trail);
        }
        /// <summary>
        /// Deactivate the last trail if it was already active.
        /// </summary>
        public void EndTrail()
        {
            if (!Active) return;
            trails.Last.Value.Finish();
        }
    }
}