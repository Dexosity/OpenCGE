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

namespace OpenCGE.Systems
{
    public class SystemAI : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_VELOCITY | ComponentTypes.COMPONENT_AI);

        Camera mCamera;
        Entity[] Collidables;
        bool[,] Traversable;
        int mWidth, mHeight;
        bool targetIsPlayer = false;

        float deltaTime = 0.0f;
       
        /// <summary>
        /// Initilises a new system AI
        /// </summary>
        /// <param name="MapWidth">Width of the map (x axis)</param>
        /// <param name="MapHeight">Height of the map (z axis)</param>
        public SystemAI(int MapWidth, int MapHeight, Entity[] Collidables)
        {
            mWidth = MapHeight;
            mHeight = MapHeight;
            this.Collidables = CollidableEntities(Collidables);
            Traversable = new bool[mWidth, mHeight];
            CalculateTraversableTiles();
        }
        /// <summary>
        /// Initilises a new system AI
        /// </summary>
        /// <param name="MapWidth">Width of the map (x axis)</param>
        /// <param name="MapHeight">Height of the map (z axis)</param>
        /// <param name="pCamera">Use if player is to be AI target</param>
        public SystemAI(int MapWidth, int MapHeight, Entity[] Collidables, ref Camera pCamera)
        {
            mWidth = MapHeight;
            mHeight = MapHeight;
            mCamera = pCamera;
            Traversable = new bool[mWidth, mHeight];
            this.Collidables = CollidableEntities(Collidables);
            targetIsPlayer = true;
            CalculateTraversableTiles();
        }
        /// <summary>
        /// Returns a list of entities that are rigid colliders that the AI has to avoid
        /// </summary>
        /// <param name="pEntities">Lst of scene entites from the scene</param>
        /// <returns></returns>
        Entity[] CollidableEntities(Entity[] pEntities)
        {
            List<Entity> entities = new List<Entity>();

            for (int i = 0; i < pEntities.Length; i++)
            {
                if((pEntities[i].Mask & ComponentTypes.COMPONENT_COLLIDER) == ComponentTypes.COMPONENT_COLLIDER)
                {
                    // This gets access to the collision component to check if it is rigid or a trigger
                    // This is so trigger items such as coins arent included in the path finding for avoidable entities
                    IComponent collisionComponent = pEntities[i].Components.Find(delegate (IComponent component)
                    {
                        return component.ComponentType == ComponentTypes.COMPONENT_COLLIDER;
                    });
                    ComponentCollider triggerCheck = (ComponentCollider)collisionComponent;
                    if (!triggerCheck.isCollidable) { continue; }
                    
                    
                    // If it reaches this point then the entity is a rigid collider that is to be avoided by the AI
                    entities.Add(pEntities[i]);
                    
                }
            }

            return entities.ToArray();
        }

        public string Name
        {
            get { return "SystemAI"; }
        }

        public void OnAction(Entity entity)
        { 
            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent positionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });
                IComponent velocityComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_VELOCITY;
                });
                IComponent AiComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_AI;
                }
                );
                ComponentPosition position = (ComponentPosition)positionComponent;
                
                Motion((ComponentPosition)positionComponent, (ComponentVelocity)velocityComponent, (ComponentAI)AiComponent, entity.Name);
            }
        }

        public void Motion(ComponentPosition pPos, ComponentVelocity pVel, ComponentAI pAI, string name)
        {
            // Updates the target with the most recent player position
            if (targetIsPlayer) { pAI.Target = mCamera.Position; }

            // Checks if the AI has found its target and skips search code for efficency
            if (RoundVectorPosition(pPos.Position).Xz == pAI.Target.Xz) { return; }

            // Checks if the AI's target is within map boundaries
            if (!(pAI.Target.X < (mWidth / 2) && !(pAI.Target.X > (mWidth / 2)) && pAI.Target.Z < (mHeight / 2) && !(pAI.Target.Z > (mHeight / 2))))
            {
                return;
            }

            // Makes sure target is in a traversable area
            if (!Traversable[(int)pAI.Target.Z + (mHeight / 2), (int)pAI.Target.X + (mWidth / 2)]) { return; }
         
            if (pAI.Path.Length > 0 && pAI.ogTarget == pAI.Target)
            {
                // Path following
                Vector3 direction = pAI.Path[0] - pPos.Position;
                pPos.Position += direction * pVel.Veclotiy * deltaTime;

            }
            else
            {
                // Updates path finding
                pAI.ogTarget = pAI.Target;
                pAI.Path = ASPath(RoundVectorPosition(pPos.Position), RoundVectorPosition(pAI.Target));
                // Path following
                if(pAI.Path.Length <= 0)
                {
                    // Path finding failed
                    return;
                }
                Vector3 direction =  pAI.Path[0] - pPos.Position;
                pPos.Position += direction * pVel.Veclotiy * deltaTime;
            }
            // Check to see if AI has hit closest point in path and removes it if true so it can follow the next point
            if (RoundVectorPosition(pPos.Position) == pAI.Path[0])
            {
                pAI.Path = pAI.Path.Skip(1).ToArray();
            }
        }

        /// <summary>
        /// Works out which 1x1 unit tiles of the map can be walked on by the AI
        /// </summary>
        void CalculateTraversableTiles()
        {
            // This method works in a similar idea to battleships to set units in the
            // 2D grid to say if something is there or not, in this case it will be true if its clear, false if its blocked
            //Entity[] test = Collidables;
            for (int x = 0; x < Traversable.GetLength(0); x++)
            {
                for (int z = 0; z < Traversable.GetLength(1); z++)
                {
                    Traversable[x,z] = true;
                }
            }

            foreach(Entity entity in Collidables)
            {
                ComponentTransform t = (ComponentTransform)entity.Components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });
                ComponentPosition p = (ComponentPosition)entity.Components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });

                // Works out position of each entity and the starting point of the shape (e.g. position is centra of entity)
                Vector3 scale = new Vector3(t.Transform.ExtractScale().X, 0.0f, t.Transform.ExtractScale().Z);
                Vector3 start = new Vector3((p.Position.X - (scale.X)), 0.0f, (p.Position.Z - (scale.Z)));
                // Offset changes from world coords to grid coords (e.g. a 50x50 world would centre at 0,0 but grid centre would be 25,25)
                int xOffset = (int)start.X + (mWidth / 2);
                int zOffset = (int)start.Z + (mHeight / 2);

                if (!(xOffset >= Traversable.GetLength(0)) && !(zOffset > Traversable.GetLength(1)) && !(xOffset < 0) && !(zOffset < 0))
                {
                    // This sets units where a collidable entity is to false to say the AI cannot traverse this unit
                    for (int x = xOffset; x < (xOffset + t.Transform.ExtractScale().X * 2); x++)
                    {
                        for (int z = zOffset; z < (zOffset + t.Transform.ExtractScale().Z * 2); z++)
                        {
                            float xSize = (Traversable.GetLength(0)) - 2;
                            float zSize = (Traversable.GetLength(1)) - 2;

                            xSize++;
                            zSize++;
                            if (!(x <= 0) && !(x >= xSize) && !(z <= 0) && !(z >= zSize))
                            {
                                // This creates the cube for AI to avoid apposed to avoid just a single vertex
                                Traversable[z, x + 1] = false;
                                Traversable[z + 1, x] = false;
                                Traversable[z + 1, x + 1] = false;
                            }
                            Traversable[z, x] = false;
                        }
                    }
                }
            }          
        }
        /// <summary>
        /// Returns the shortest traversable route for AI from supplied start point to target
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        Vector3[] ASPath(Vector3 start, Vector3 target)
        {
            // Creates both the start node and end node for the algorithm
            ASNode node_start = new ASNode(start);
            ASNode node_target = new ASNode(target);
            // Creates two empty list of nodes one for avalible nodes to cover and one for nodes already checked
            List<ASNode> OpenNodes = new List<ASNode>();
            List<ASNode> ClosedNodes = new List<ASNode>();

            // Sets the first nodes F values (its distance score) to zero as its the starting point
            node_start.F = 0.0f;
            OpenNodes.Add(node_start);
            int count = 0;
            // Cycles through all nodes until target is found or no more nodes to check
            while(OpenNodes.Count > 0)
            {
                ASNode node_current = OpenNodes[0];
                int index = 0;

                // Finds the node and sets the current node to the node with the lowest F value
                for (int i = 0; i < OpenNodes.Count; i++)
                {
                    if(OpenNodes[i].F < node_current.F)
                    {
                        // Updates current node and current index
                        node_current = OpenNodes[i];
                        index = i;
                    }
                }
                //Console.WriteLine(node_current.Position);
                // Remove from availble nodes and add to closed nodes list
                OpenNodes.RemoveAt(index);
                ClosedNodes.Add(node_current);
                
                // Check if current node is the target
                if(node_current.Position.Xz == node_target.Position.Xz)
                {
                    // Backtrack through parent of each node and add to list
                    List<Vector3> path = new List<Vector3>();
                    while(node_current.Parent != null)
                    {
                        // Add node position to list then set to parent node
                        path.Add(node_current.Position);
                        node_current = node_current.Parent;
                    }
                    // Reverse the list and return as path is found
                    path.Reverse();
                    return path.ToArray();
                }

                // Check surround nodes around current node
                List<ASNode> children = new List<ASNode>();
                // Creates array of all the directions to check
                Vector3[] direction = new Vector3[]
                {
                    new Vector3(node_current.Position.X, 0.0f, node_current.Position.Z + 1.0f), // Forward 
                    new Vector3(node_current.Position.X, 0.0f, node_current.Position.Z - 1.0f), // Back     
                    new Vector3(node_current.Position.X + 1.0f, 0.0f, node_current.Position.Z), // Right
                    new Vector3(node_current.Position.X - 1.0f, 0.0f, node_current.Position.Z),  // Left


                    new Vector3(node_current.Position.X - 1.0f, 0.0f, node_current.Position.Z + 1.0f),  // TopLeft  
                    new Vector3(node_current.Position.X + 1.0f, 0.0f, node_current.Position.Z + 1.0f),  // TopRight 
                    new Vector3(node_current.Position.X - 1.0f, 0.0f, node_current.Position.Z - 1.0f),  // BottomLeft  
                    new Vector3(node_current.Position.X + 1.0f, 0.0f, node_current.Position.Z - 1.0f)  // BottomLeft  
                };

                // Checks all directions and if its traversable/within boundaries then creates new child
                for (int i = 0; i < direction.Length; i++)
                {
                    // Offset to go from world coords to 2d array coords
                    int xOffset = mWidth / 2;
                    int zOffset = mHeight / 2;

                    // Checks if the direction would be out of map boundaries
                    if (direction[i].X + xOffset >= Traversable.GetLength(1) - 1 || direction[i].X + xOffset < 0 || direction[i].Z + zOffset >= Traversable.GetLength(0) - 1 || direction[i].Z + zOffset < 0) { continue; }

                    // Checks if that direction would hit a rigid collider (created using CalculateTraversableTiles())
                    if (!Traversable[(int)direction[i].Z + xOffset, (int)direction[i].X + zOffset]) { continue; }

                    // At this point this child should have a traversable position so add to list
                    ASNode node_newChild = new ASNode(node_current, direction[i]);
                    children.Add(node_newChild);
                }
                // Compare all the available children to find child with best F score
                for(int child = 0; child < children.Count; child++)
                {
                    // Checks if this child has been checked before
                    for (int i = 0; i < ClosedNodes.Count; i++)
                    {
                        if (ClosedNodes[i].Position == children[child].Position)
                        { goto NewChild; }
                    }
                    // Calculate childs F score
                    children[child].G = node_current.G + 1.0f;
                    // Calculate H using pythagoris
                    float x = (float)Math.Pow(children[child].Position.X - node_target.Position.X, 2);
                    float z = (float)Math.Pow(children[child].Position.Z - node_target.Position.Z, 2);
                    children[child].H = x + z;
                    // Childs score
                    children[child].F = children[child].G + children[child].H;

                    // Checks if child is already in OpenNodes before adding
                    foreach(ASNode openChild in OpenNodes)
                    {
                        if(children[child].Position == openChild.Position && children[child].G > openChild.G)
                        { goto NewChild; }
                    }

                    // Child is valid so add to OpenNodes
                    OpenNodes.Add(children[child]);

                    NewChild:;
                }

                // Failsafe to stop endless loop incase of bugs
                // Checks to see if the loop count has exceeded the total number of tiles
                count++;
                if(count > Traversable.Length * 4) { break; }
            }
            // Should never hit this but if it does then AI's path with be nothing
            return null;
        }

        Vector3 DirectPath(Vector3 pos, Vector3 target)
        {
            Vector3 direction;
            if (pos != target)
            {
                direction = target - pos;
                direction.Normalize();

            }
            else
            {
                direction = new Vector3();
            }
            return direction;
        }

        Vector3 PotentialFunction(Vector3 pos, Vector3 target)
        {
            // ---------  //
            //    f2      //
            // f3 AI f4   //
            //    f1      //
            // ---------  //
            Vector3 f1 = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 f2 = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 f3 = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 f4 = new Vector3(1.0f, 0.0f, 0.0f);

            float d1 = (target.X - (pos.X + f1.X)) * (target.X - (pos.X + f1.X)) + (target.Z - (pos.Z + f1.Z)) * (target.Z - (pos.Z + f1.Z));
            float d2 = (target.X - (pos.X + f2.X)) * (target.X - (pos.X + f2.X)) + (target.Z - (pos.Z + f2.Z)) * (target.Z - (pos.Z + f2.Z));
            float d3 = (target.X - (pos.X + f3.X)) * (target.X - (pos.X + f3.X)) + (target.Z - (pos.Z + f3.Z)) * (target.Z - (pos.Z + f3.Z));
            float d4 = (target.X - (pos.X + f4.X)) * (target.X - (pos.X + f4.X)) + (target.Z - (pos.Z + f4.Z)) * (target.Z - (pos.Z + f4.Z));



            Vector3[] vectors = new Vector3[] { f1, f2, f3, f4 };
            float[] scores = new float[] { d1, d2, d3, d4 };

            int xOffset = mWidth / 2;
            int zOffset = mHeight / 2;

            List<Vector3> holder = new List<Vector3>();
            List<float> fholder = new List<float>();

            for (int i = 0; i < vectors.Length; i++)
            {
                if (!Traversable[(int)(vectors[i].Z + Math.Round(pos.Z)) + xOffset, (int)(vectors[i].X + Math.Round(pos.X)) + zOffset])
                {
                    continue;
                }
                holder.Add(vectors[i]);
                fholder.Add(scores[i]);

            }
            vectors = holder.ToArray();
            scores = fholder.ToArray();
            
            Vector3 shortest = vectors[0];
            float lowest = scores[0];

            
            for (int i = 0; i < vectors.Length; i++)
            {    
                // Checks if next vector is shortest distance than last and if that route is traversable
                if (Math.Round(scores[i]) < Math.Round(lowest))
                {   
                    shortest = vectors[i];
                    lowest = scores[i];
                }
            }

            return shortest;
        }

        Vector3 RoundVectorPosition(Vector3 pos)
        {
            return new Vector3((float)Math.Round(pos.X), pos.Y, (float)Math.Round(pos.Z));
        }

        public void UpdateDeltaTime(float time)
        {
            deltaTime = time;
        }
    }
}
