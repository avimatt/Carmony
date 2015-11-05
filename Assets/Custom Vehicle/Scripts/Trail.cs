﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Trails
{
    // Created by Edward Kay-Coles a.k.a Hoeloe
    public class Trail
    {
        //Properties of the trail
        private float width;
        private float decay;
        private Material m;
        private int rough;
        private int maxRough;
        private bool softSource;
        //Parent object
        private Transform par;
        //Pieces for the mesh generation
        public GameObject trail;
        private MeshFilter filter;
        private MeshRenderer render;
        private Mesh mesh;
        //Lists storing the mesh data
        private LinkedList<Vector3> verts = new LinkedList<Vector3>();
        private LinkedList<Vector2> uvs = new LinkedList<Vector2>();
        private LinkedList<int> tris = new LinkedList<int>();
        private LinkedList<Color> cols = new LinkedList<Color>();
        //Check if the trail is still being generated, and if it has completely faded
        private bool finished = false;
        private bool dead = false;
        private float m_wheelRadius = 0f;

        //For registering if the object has been removed from the game (so you don't have to store it any more)
        public bool Dead
        {
            get { return dead; }
            private set
            {
                dead = true;
                GameObject.Destroy(trail);
            }
        }
        //Set up the trail object, and parameters
        public Trail(Transform parent, Material material, float decayTime, int roughness, bool softSourceEdges, float wid = 0.1f, float wheelRadius=0)
        {
            softSource = softSourceEdges;
            maxRough = roughness;
            rough = 0;
            decay = decayTime;
            par = parent;
            width = wid;
            m = material;
            trail = new GameObject("Trail");
            filter = trail.AddComponent(typeof(MeshFilter)) as MeshFilter;
            render = trail.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            render.receiveShadows = false;
            mesh = new Mesh();
            render.material = m;
            filter.mesh = mesh;
            m_wheelRadius = wheelRadius;
        }
        //Call this when the trail should stop emitting
        public void Finish()
        {
            finished = true;
        }
        //Tells you if the trail is emitting or not
        public bool Finished
        {
            get { return finished; }
        }

        // Updates the state of the trail - Note: this must be called manually
        public void UpdateTrail()
        {
            if (!finished) //Only add new segments if the trail is not being emitted
            {
                //Decides how often to generate new segments. Smaller roughness values are smoother, but more expensive
                if (rough > 0)
                    rough--;
                else
                {
                    rough = maxRough;
                    //Checks in which order we should add vertices (to keep a consistent shape)
                    //bool odd = !(verts.Count % 4 == 0);
                    //Add new vertices as the current position

                    Vector3 vert;

                    vert = par.position + 1 * par.right * width;
                    vert.y -= m_wheelRadius - 0.02f;
                    verts.AddLast(vert);

                    vert = par.position + -1 * par.right * width;
                    vert.y -= m_wheelRadius - 0.02f;
                    verts.AddLast(vert);
                    
                    //Fades out the newest vertices if soft source edges is set to true
                    if (softSource)
                    {
                        if (cols.Count >= 4)
                        {
                            cols.Last.Value = Color.white;
                            cols.Last.Previous.Value = Color.white;
                        }
                        cols.AddLast(Color.clear);
                        cols.AddLast(Color.clear);
                    }
                    else //Sets the first vertices to fade out, but leaves the rest solid
                    {
                        if (cols.Count >= 2)
                        {
                            cols.AddLast(Color.white);
                            cols.AddLast(Color.white);
                        }
                        else
                        {
                            cols.AddLast(Color.clear);
                            cols.AddLast(Color.clear);
                        }
                    }
                    uvs.AddLast(new Vector2(0, 1));
                    uvs.AddLast(new Vector2(0, 0));
                    //Don't try to draw the trail unless we have at least a rectangle
                    if (verts.Count < 4) return;
                    //Add new triangles to the mesh
                    int c = verts.Count;
                    tris.AddLast(c - 4);
                    tris.AddLast(c - 3);
                    tris.AddLast(c - 2);
                    tris.AddLast(c - 3);
                    tris.AddLast(c - 1);
                    tris.AddLast(c - 2);

                    //Copy lists to arrays, ready to rebuild the mesh
                    Vector3[] v = new Vector3[c];
                    Vector2[] uv = new Vector2[c];
                    int[] t = new int[tris.Count];
                    verts.CopyTo(v, 0);
                    uvs.CopyTo(uv, 0);
                    tris.CopyTo(t, 0);
                    //Build the mesh
                    mesh.vertices = v;
                    mesh.triangles = t;
                    mesh.uv = uv;
                }
            }
            //The next section updates the colours in the mesh
            int i = cols.Count;
            //If we have no vertices, don't bother trying to update
            if (i == 0)
                return;
            //This is for checking if the trail has completely faded or not
            bool alive = false;
            //Essentially a foreach loop over the colours, but allowing editing to each node as it goes
            LinkedListNode<Color> d = cols.First;
            do
            {
                if (d.Value.a > 0)
                {
                    Color t = d.Value;
                    alive = true;
                    //Decrease the alpha value, to 0 if it would be decreased to negative
                    t.a -= Mathf.Min(Time.deltaTime / decay, t.a);
                    d.Value = t;
                }
                d = d.Next;
            } while (d != null);
            //Trail should be removed if it is not emitting and has faded out
            if (!alive && finished)
                Dead = true;
            else
            {
                //Doesn't set the colours if the number of vertices doesn't match up for whatever reason
                if (i != mesh.vertices.Length)
                    return;
                //Copy the colours to an array and build the mesh colours
                Color[] cs = new Color[i];
                cols.CopyTo(cs, 0);
                mesh.colors = cs;
            }
        }
    }
}