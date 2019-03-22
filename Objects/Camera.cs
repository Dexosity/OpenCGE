using OpenTK;
using OpenTK.Input;
using System;

namespace OpenCGE.Objects
{
    public class Camera
    {
        // Screen size values
        private int mWidth, mHeight;
        // View point matrix values
        private Matrix4 mView, mProjection;
        // Camera position and direction values
        private Vector3 mPosition, mOrientation, mTarget;
        // Last mouse x,y points for mouse controlled camera
        private float mLastX, mLastY;

        public Camera(int pWidth, int pHeight)
        {
            // View dimensions
            mWidth = pWidth;
            mHeight = pHeight;
            // Mouse values
            mLastX = 0.0f; mLastY = 0.0f;
            mPosition = new Vector3(0.0f, 0.0f, 0.0f);
            mOrientation = new Vector3((float)Math.PI, 0.0f, 0.0f);
        }
        /// <summary>
        /// Call to update the camereas view matrix when the camerea is moved
        /// </summary>
        public void UpdateCameraView()
        {
            mTarget = new Vector3();
            // Calculates the Roll (x), Pitch(y), Yaw (z) using euler angles
            mTarget.X = (float)(Math.Sin((float)mOrientation.X) * Math.Cos((float)mOrientation.Y));
            mTarget.Y = (float)Math.Sin((float)mOrientation.Y);
            mTarget.Z = (float)(Math.Cos((float)mOrientation.X) * Math.Cos((float)mOrientation.Y));
            // Returns what the camera will be looking at in relation to its position in world space
            mView = Matrix4.LookAt(mPosition, mPosition + mTarget, Vector3.UnitY);
        }
        /// <summary>
        /// Call to update the projection matrix of the camera
        /// </summary>
        public void UpdateCameraProjection()
        {
            mProjection = mView * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)mWidth / (float)mHeight, 10f, 100f);
        }
        /// <summary>
        /// Call to update the projection matrix of the camera
        /// </summary>
        public void UpdateToOrthCamera()
        {
            mProjection = Matrix4.CreateOrthographicOffCenter(0, mWidth, 0, mHeight, -1, 1);
        }
        /// <summary>
        /// Move the cameras position in a certain direction and speed
        /// </summary>
        /// <param name="pInput">Direction of traval</param>
        /// <param name="pSpeed">Spped to traval at</param>
        public void Move(Vector3 pInput, float pSpeed)
        {
            Vector3 direction = new Vector3(0.0f, pInput.Y, 0.0f);
            // Calculates the vector directions based on the orientation of the view point
            Vector3 forward = new Vector3((float)Math.Sin(mOrientation.X), 0.0f, (float)Math.Cos(mOrientation.X));
            Vector3 horizontal = new Vector3(-forward.Z, 0, forward.X);
            // Combines amount to move with new direction vector
            direction += pInput.X * horizontal;
            direction += pInput.Z * forward;
            // Updates camera position
            mPosition += direction * pSpeed;
        }
        /// <summary>
        /// Rotates the camera around its position
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <param name="pSpeed"></param>
        public void Rotate(float pX, float pY, float pZ, float pSpeed)
        {
            Vector3 movement = new Vector3(-pX, -pY, -pZ);
            Vector3 lastPos = mPosition;
            // This will rotate the camera around its position not the origin
            mPosition = -mPosition;
            // Rotate the view
            movement.Normalize();
            mOrientation.X += pX * pSpeed;
            mOrientation.Y += pY * pSpeed;
            mOrientation.Z += pZ * pSpeed;

            mPosition = lastPos;
        }
        /// <summary>
        /// Call this to link the mouse to the camera
        /// </summary>
        /// <param name="deltatime"></param>
        public void MouseLinkedCamera(float deltatime)
        {
            float offset = 100.0f;
            // Locks mouse position
            Mouse.SetPosition((double)(mWidth / 2) + offset, (double)(mHeight / 2) + offset);

            float max = 12.0f;

            float X = Mouse.GetState().X - mLastX;
            if(X > max) { X = max; }
            if (X < -max) { X = -max; }
            float Y = Mouse.GetState().Y - mLastX;
            if(Y > max) { Y = max; }
            if (Y < -max) { Y = -max; }

            mLastX = Mouse.GetState().X;
            mLastY = Mouse.GetState().Y;

            Vector2 movement = new Vector2(X, Y) * -1.0f;
            if(X != 0)
            {
                Rotate(movement.X, 0.0f, 0.0f, 0.5f * deltatime);
            }
        }
        /// <summary>
        /// Call when scene is refocused (e.g. tabbing back in)
        /// </summary>
        public void ReloadMouse()
        {
            float offset = 100.0f;
            Mouse.SetPosition((double)(mWidth / 2) + offset, (double)(mHeight / 2) + offset);
        }
        /// <summary>
        /// Call when scene is re-focused
        /// </summary>
        public void ReloadCamera()
        {
            mPosition = new Vector3(0.0f, 0.0f, 0.0f);
            mOrientation = new Vector3((float)Math.PI, 0.0f, 0.0f);
            UpdateCameraView();
            UpdateCameraProjection();
        }
        /// <summary>
        /// Get/Set for camera position
        /// </summary>
        public Vector3 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }
        /// <summary>
        /// Returns the direction the camera is pointing
        /// </summary>
        public Vector3 Direction
        {
            get { return mTarget; }
        }
        /// <summary>
        /// Returns the view matrix of the camera
        /// </summary>
        public Matrix4 View
        {
            get { return mView; }
        }
        /// <summary>
        /// Returns the projection matrix of the camera
        /// </summary>
        public Matrix4 Projection
        {
            get { return mProjection; }
        }
    }
}
