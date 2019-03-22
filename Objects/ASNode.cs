using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenCGE.Objects
{
    public class ASNode
    {
        private ASNode mParent;
        private Vector3 mPosition;

        private float mF, mG, mH;

        /// <summary>
        /// Creates a new node for use in an A* Algorithm
        /// </summary>
        /// <param name="pParent">Parent node of this new node</param>
        /// <param name="pPosition">Position of this new node</param>
        public ASNode(ASNode pParent, Vector3 pPosition)
        {
            mParent = pParent;
            mPosition = pPosition;
        }
        /// <summary>
        /// Creates a new node for use in an A* Algorithm where no parent is present (e.g. start/end node)
        /// </summary>
        /// <param name="pPosition">Position of the new node</param>
        public ASNode(Vector3 pPosition)
        {
            mPosition = pPosition;
        }
        /// <summary>
        /// Get/Set the parent of selected node
        /// </summary>
        public ASNode Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }
        /// <summary>
        /// Returns the vector3 position of selected node
        /// </summary>
        public Vector3 Position
        {
            get { return mPosition; }
        }
        /// <summary>
        /// Get/Set for F value in A-star algorithm
        /// </summary>
        public float F
        {
            get { return mF; }
            set { mF = value; }
        }
        /// <summary>
        /// Get/Set for G value in A-star algorithm
        /// </summary>
        public float G
        {
            get { return mG; }
            set { mG = value; }
        }
        /// <summary>
        /// Get/Set for H value in A-star algorithm
        /// </summary>
        public float H
        {
            get { return mH; }
            set { mH = value; }
        }
    }
}
