using UnityEngine;

namespace Pathfinding {
    using Pathfinding.Util;

    /** Example AI for Example scene 18.
     * This AI aligns itself to whatever node it is standing on (navmesh graphs only)
     * which makes it possible to use it for things like spherical worlds.
     */
    public class AIArbitrarySurface : AIPath {
        public LayerMask groundMask2 = 1 << 9;
        Vector3 interpolatedUp = Vector3.up;

        protected override IMovementPlane MovementPlaneFromNode (GraphNode node) {
            var forward = Vector3.Cross(Vector3.right, interpolatedUp);

            return new GraphTransform(Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(forward, interpolatedUp), Vector3.one));
        }

        protected override void Update () {
            base.Update();

            tr.position = RaycastPosition(tr.position);
        }

        /** Find the world position of the ground below the character */
        Vector3 RaycastPosition (Vector3 position) {
            RaycastHit hit;
            var normal = interpolatedUp;

            if (Physics.Raycast(position + tr.up*0.5f, -tr.up, out hit, 2f, groundMask2)) {

                normal = hit.normal;
                position = hit.point;
            }

            // Use the node surface as the movement plane
            interpolatedUp = Vector3.Slerp(interpolatedUp, normal, 4*Time.deltaTime);
            return position;
        }
    }
}