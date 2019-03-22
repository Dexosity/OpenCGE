using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenCGE.Components;
using OpenCGE.Objects;
using OpenCGE.Scenes;
using OpenCGE.Managers;
using System.Threading;

namespace OpenCGE.Systems
{
    public class SystemColliderXY : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_COLLIDER);
        ComponentPosition mPosition;
        Vector3 LastPosition;

        List<string> EntityNameList;
        string LastEntityToCheck;
        string LastEntityTriggered;

        public SystemColliderXY(ref EntityManager entityManager, ComponentPosition pPosition)
        {
            // Used to gain player position
            mPosition = pPosition;
            LastPosition = mPosition.Position;

            // Makes a list of all collidable entities and works out the last entity that will be checked
            // This is used at the end of the collision test to update the LastPosition vector
            EntityNameList = new List<string>();
            for (int i = 0; i < entityManager.EntityNameList().Length; i++)
            {
                Entity value = entityManager.FindEntity(entityManager.EntityNameList()[i]);
                if ((value.Mask & MASK) == MASK)
                {
                    EntityNameList.Add(value.Name);
                }
            }
            LastEntityToCheck = EntityNameList[EntityNameList.Count - 1];
        }

        public string Name
        {
            get { return "SystemColliderXY"; }
        }

        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent geometryComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_GEOMETRY;
                });
                float[] normals = ((ComponentGeometry)geometryComponent).Geometry().Vertices;

                IComponent positionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });
                Vector3 position = ((ComponentPosition)positionComponent).Position;

                IComponent transformComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });
                Matrix4 transform = ((ComponentTransform)transformComponent).Transform;

                IComponent colliderComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_COLLIDER;
                });
                bool isCollidable = ((ComponentCollider)colliderComponent).isCollidable;

                OnCollision(position, normals, transform, entity.Name, isCollidable);

            }
        }

        public void OnCollision(Vector3 position, float[] entityNormals, Matrix4 transform, string name, bool collidable)
        {
            List<Vector3> NormVectors = new List<Vector3>();
            // Sorts through the full list of vertices, tex coords and normals
            // Removes uneccasary values (including duplicate normals)
            for (int i = 0; i < entityNormals.Length; i += 8)
            {
                Vector3 newVec = new Vector3(entityNormals[i + 5], entityNormals[i + 6], entityNormals[i + 7]);
                if (NormVectors.Contains(newVec)) { continue; }
                // Ignores top and bottom faces
                if (newVec.Z == 1.0 || newVec.Z == -1.0) { continue; }

                NormVectors.Add(newVec);
            }


            // COLLISION CHECKS //
            // Checks for collisions on the y axis
            CollisionCheckY(position, NormVectors.ToArray(), transform, name, collidable);

            // Checks for collsisions on the x axis
            CollisionCheckX(position, NormVectors.ToArray(), transform, name, collidable);

            // If its the last collidable entity then update last position
            if (name == LastEntityToCheck)
            {
                LastPosition = mPosition.Position;
            }

        }
        /// <summary>
        /// Calculates collsision checks for object faces on the Z axis
        /// </summary>
        /// <param name="position">Entity position</param>
        /// <param name="normals">Normal vector of each face</param>
        /// <param name="transform">Transform matrix performed on entity</param>
        private void CollisionCheckY(Vector3 position, Vector3[] normals, Matrix4 transform, string entity, bool collider)
        {
            for (int i = 0; i < normals.Length; i++)
            {
                if (normals[i].X == 1.0f || normals[i].X == -1.0f) { continue; }

                // Use offset to account for perspective of the camereas view to prevent camerea clipping
                float offset = (transform.ExtractScale().Y);
                Vector3 PosOffset = new Vector3(position.X, offset * normals[i].Y, position.Z);
                Vector3 p1 = LastPosition - (position + PosOffset);
                Vector3 p2 = mPosition.Position - (position + PosOffset);

                // Dot product used to work out the relation of the two vectors to the normal of the entity
                float dotOld = Vector3.Dot(p1, normals[i]);
                float dotNew = Vector3.Dot(p2, normals[i]);
                // 1.0 would be same dir, -1.0 would be oposite
                float value = dotOld * dotNew;

                // If true then passed test for checks on the Z axis
                if (value < 0)
                {
                    // Continues to now checking if the player position is within the X axis range of the entity
                    PosOffset = new Vector3(((transform.ExtractScale().X / 2)) * normals[i].X, position.Y, position.Z);
                    offset = (transform.ExtractScale().X);
                    p1 = new Vector3(position.X + (offset), position.Y + (transform.ExtractScale().Y / 2), position.Z);
                    p2 = new Vector3(position.X - (offset), position.Y + (transform.ExtractScale().Y / 2), position.Z);

                    // Works out players normal if non-axis aligned
                    //Vector3 vertPos = new Vector3(LastPosition.X, LastPosition.Y + 1.0f, LastPosition.Z);
                    //Vector3 playerNormal = Vector3.Cross(LastPosition - vertPos, mCamera.Position - vertPos);
                    Vector3 playerNormal = new Vector3(1, 0, 0);

                    playerNormal.Normalize();
                    dotOld = Vector3.Dot(p1 - (LastPosition), playerNormal);
                    dotNew = Vector3.Dot(p2 - (LastPosition), playerNormal);

                    value = dotOld * dotNew;

                    // If two corner points are on either side of players normal
                    // Then player is within the X axis range of the entity 

                    // At this point it has collided so resets the possive to before the collsion happened
                    if (value < 0)
                    {
                        HasCollided(entity, collider);
                    }
                }
            }
        }
        /// <summary>
        /// Calculates collsision checks for object faces on the X axis
        /// </summary>
        /// <param name="position">Entity position</param>
        /// <param name="normals">Normal vector of each face</param>
        /// <param name="transform">Transform matrix performed on entity</param>
        private void CollisionCheckX(Vector3 position, Vector3[] normals, Matrix4 transform, string entity, bool collider)
        {
            for (int i = 0; i < normals.Length; i++)
            {
                if (normals[i].Y == 1.0f || normals[i].Y == -1.0f) { continue; }

                // Use offset to account for perspective of the camereas view to prevent camerea clipping
                float offset = (transform.ExtractScale().X);
                Vector3 PosOffset = new Vector3(offset * normals[i].X, position.Y, position.Z);
                Vector3 p1 = LastPosition - (position + PosOffset);
                Vector3 p2 = mPosition.Position - (position + PosOffset);

                // Dot product used to work out the relation of the two vectors to the normal of the entity
                float dotOld = Vector3.Dot(p1, normals[i]);
                float dotNew = Vector3.Dot(p2, normals[i]);
                // 1.0 would be same dir, -1.0 would be oposite
                float value = dotOld * dotNew;

                // If true then passed test for checks on the X axis
                if (value < 0)
                {
                    // Continues to now checking if the player position is within the X axis range of the entity
                    PosOffset = new Vector3(position.X, ((transform.ExtractScale().Y / 2)) * normals[i].Y, position.Z);
                    offset = transform.ExtractScale().Y;

                    p1 = new Vector3(position.X + (transform.ExtractScale().X / 2), position.Y + (offset), position.Z);
                    p2 = new Vector3(position.X + (transform.ExtractScale().X / 2), position.Y - (offset), position.Z);

                    // Works out players normal if non-axis aligned
                    //Vector3 vertPos = new Vector3(LastPosition.X, LastPosition.Y + 1.0f, LastPosition.Z);
                    //Vector3 playerNormal = Vector3.Cross(LastPosition - vertPos, mCamera.Position - vertPos);
                    Vector3 playerNormal = new Vector3(0, 1, 0);

                    playerNormal.Normalize();
                    dotOld = Vector3.Dot(p1 - (LastPosition), playerNormal);
                    dotNew = Vector3.Dot(p2 - (LastPosition), playerNormal);

                    value = dotOld * dotNew;

                    // If two corner points are on either side of players normal
                    // Then player is within the Z axis range of the entity 

                    // At this point it has collided so resets the possive to before the collsion happened
                    if (value < 0)
                    {
                        HasCollided(entity, collider);
                    }
                }
            }
        }

        /// <summary>
        /// If a collision has occured then this method handles the action taken
        /// </summary>
        /// <param name="collidable">Has collided with entity</param>
        private void HasCollided(string entity, bool collidable)
        {
            //Console.WriteLine("Collided with: " + entity);
            switch (collidable)
            {
                // Is Collidable
                case true:
                    mPosition.Position = LastPosition;
                    break;
                // Is Trigger
                case false:
                    LastEntityTriggered = entity;
                    break;
            }
        }
        /// <summary>
        /// When an entity is removed this method must be called so the algorithm knows which entity to stop searching at
        /// </summary>
        private void UpdateLastEntity()
        {

            for (int i = 0; i < EntityNameList.Count; i++)
            {
                if (EntityNameList[i] == GetEntityCollisionTrigger())
                {
                    EntityNameList.RemoveAt(i);
                }
            }

            LastEntityToCheck = EntityNameList[EntityNameList.Count - 1];
        }
        /// <summary>
        /// This method checks if the provided array of entities exist in the known collidables list and removes if true
        /// </summary>
        /// <param name="pNames"></param>
        public void RemoveDestructables(string[] pNames)
        {
            for (int i = 0; i < pNames.Length; i++)
            {
                if(EntityNameList.Exists(delegate (string n){ return n == pNames[i]; }))
                {
                    EntityNameList.Remove(pNames[i]);
                } 
            }
        }

        /// <summary>
        /// Returns the name of the last entity that collided with the player
        /// </summary>
        /// <returns></returns>
        public string GetEntityCollisionTrigger()
        {
            return LastEntityTriggered;
        }
        public void ClearLastTrigger()
        {
            LastEntityTriggered = "";
        }
        public void ClearLastDestructable()
        {
            UpdateLastEntity();
            LastEntityTriggered = "";
        }
        public void UpdateLastPosition(Vector3 pPos)
        {
            LastPosition = pPos;
        }
        public void UpdatePosition(Vector3 pPos)
        {
            mPosition.Position = pPos;
        }
    }
}
