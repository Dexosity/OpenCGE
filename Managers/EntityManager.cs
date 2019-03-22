using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OpenCGE.Objects;
using OpenCGE.Components;
using OpenTK.Graphics.OpenGL;

namespace OpenCGE.Managers
{
    public class EntityManager
    {
        List<Entity> entityList;

        public EntityManager()
        {
            entityList = new List<Entity>();
        }
        /// <summary>
        /// Adds a new entity to the main game entity list
        /// </summary>
        /// <param name="entity">Entity to be added</param>
        public void AddEntity(Entity entity)
        {
            Entity result = FindEntity(entity.Name);
            Debug.Assert(result == null, "Entity '" + entity.Name + "' already exists");
            entityList.Add(entity);
        }
        /// <summary>
        /// Removes the provided entity from the main game entity list
        /// </summary>
        /// <param name="entity">Entity to be removed</param>
        public void RemoveEntity(Entity entity)
        {
            DeleteEntity(entity);
            entityList.Remove(entity);
        }
        /// <summary>
        /// Removes the provided entity from the main game entity list by name
        /// </summary>
        /// <param name="name">Name of entity to be removed (First occurance)</param>
        public void RemoveEntity(string name)
        {
            DeleteEntity(FindEntity(name));
            entityList.Remove(FindEntity(name));
        }
        /// <summary>
        /// Returns entity that matches provided name
        /// </summary>
        /// <param name="name">Name of entity to find</param>
        /// <returns>Entity found</returns>
        public Entity FindEntity(string name)
        {
            return entityList.Find(delegate(Entity e)
            {
                return e.Name == name;
            }
            );
        }
        /// <summary>
        /// Finds entity that matches provided mask
        /// </summary>
        /// <param name="type">Mask to search entity list for</param>
        /// <returns></returns>
        public Entity[] FindEntityWithMask(ComponentTypes type)
        {
            ComponentTypes MASK = type;
            List<Entity> list = new List<Entity>();
            for (int i = 0; i < entityList.Count; i++)
            {
                if((entityList[i].Mask & MASK) == MASK)
                {
                    list.Add(entityList[i]);
                }
            }
            return list.ToArray();
        }
        /// <summary>
        /// Loads in all entities from the provided XML file
        /// </summary>
        /// <param name="filename">XML file to load from (structure in documentation)</param>
        public void LoadSceneEntities(string filename)
        {
            Entity[] entities = ResourceManager.LoadSceneEntities(filename);
            for (int i = 0; i < entities.Length; i++)
            {
                AddEntity(entities[i]);
            }
        }
        /// <summary>
        /// Returns the list of entities
        /// </summary>
        /// <returns></returns>
        public List<Entity> Entities()
        {
            return entityList;
        }

        /// <summary>
        /// Returns a list of names of all entities loaded into current scene
        /// </summary>
        /// <returns>String array of entity names</returns>
        public string[] EntityNameList()
        {
            List<string> names = new List<string>();
            foreach(Entity entity in entityList)
            {
                names.Add(entity.Name);
            }
            return names.ToArray();
        }
        /// <summary>
        /// Called on closing of scene to clean up entity data (e.g. audio component)
        /// </summary>
        public void DeleteEntities()
        {
            foreach (Entity entity in entityList)
            {
                ComponentTypes MASK = ComponentTypes.COMPONENT_AUDIO;
                if ((entity.Mask & MASK) == MASK)
                {

                    IComponent audioComponent = entity.Components.Find(delegate (IComponent component)
                    {
                        return component.ComponentType == ComponentTypes.COMPONENT_AUDIO;
                    });
                    Audio audio = ((ComponentAudio)audioComponent).AudioObject;
                    audio.Close();
                }

            }
        }
        void DeleteEntity(Entity entity)
        {
            ComponentTypes MASK = ComponentTypes.COMPONENT_AUDIO;
            if ((entity.Mask & MASK) == MASK)
            {

                IComponent audioComponent = entity.Components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_AUDIO;
                });
                Audio audio = ((ComponentAudio)audioComponent).AudioObject;
                audio.Close();
            }
        }
        /// <summary>
        /// Clears the entire entity list
        /// </summary>
        public void ClearEntities()
        {
            entityList = new List<Entity>();
        }
    }
}
