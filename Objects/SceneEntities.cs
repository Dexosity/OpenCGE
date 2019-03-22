using System;
using System.Collections.Generic;
using System.Xml.Linq;
using OpenTK;
using OpenCGE.Components;
using OpenTK.Audio;

namespace OpenCGE.Objects
{
    static class SceneEntities
    {
        private static AudioContext audioContext = new AudioContext();
        public static Entity[] LoadObject(string filename)
        {
            // Creates an empty entity list to be filled with all entities loaded for supplied xml file
            List<Entity> entityList = new List<Entity>();
            try
            {
                // Loads in xml file with supplied name
                XDocument file = XDocument.Load(filename);
                // Will loop through each entity in file untill all have been added
                var entities = file.Descendants("entity");
                foreach (var entity in entities)
                {
                    // Initlises new entity with name found from file
                    Entity newEntity = new Entity(entity.Element("name").Value.ToString());
                    var components = entity.Element("components").Elements();
                    // Searches through all components supplied and when matched adds that componenet to this new entity
                    foreach (var component in components)
                    {
                        switch (component.Name.ToString())
                        {
                            // POSITION COMPONENT
                            case "position":
                                // Splits by , to make a new vector from supplied floats
                                string[] Pos = component.Value.Split(',');
                                float[] posValues = Array.ConvertAll<string, float>(Pos, float.Parse);
                                newEntity.AddComponent(new ComponentPosition(new Vector3(posValues[0], posValues[1], posValues[2])));
                                break;
                            // TRANSFORM COMPONENT
                            case "transform":
                                // Splits by , to make a new vector from supplied floats
                                string[] trans = component.Value.Split(',');
                                float[] transValues = Array.ConvertAll<string, float>(trans, float.Parse);
                                Vector3 scale = new Vector3(transValues[0], transValues[1], transValues[2]);
                                Vector3 rotation = new Vector3(transValues[3], transValues[4], transValues[5]);
                                newEntity.AddComponent(new ComponentTransform(scale, rotation));
                                break;
                            // TEXTURE COMPONENT
                            case "texture":
                                // Splits by , to see if texture has supplied scaler value
                                string[] texs = component.Value.Trim().Split(',');
                                if(texs.Length == 1) { newEntity.AddComponent(new ComponentTexture(component.Value.Trim())); }
                                else if(texs.Length == 2) { newEntity.AddComponent(new ComponentTexture(texs[0], float.Parse(texs[1]))); }
                                break;
                            // GEOMETRY COMPONENT
                            case "geometry":
                                newEntity.AddComponent(new ComponentGeometry(component.Value.Trim()));
                                break;
                            // VELOCITY COMPONENT
                            case "velocity":
                                // Splits by , to make a new vector from supplied floats
                                string[] Vel = component.Value.Split(',');
                                float[] velValues = Array.ConvertAll<string, float>(Vel, float.Parse);
                                newEntity.AddComponent(new ComponentVelocity(new Vector3(velValues[0], velValues[1], velValues[2])));
                                break;
                            // AUDIO COMPONENT
                            case "audio":
                                // Splits to check if audio file and volume is supplied with auto play and auto loop
                                string[] aud = component.Value.Split(',');
                                // Checks for number of parameters supplied
                                if (aud.Length == 2)
                                { newEntity.AddComponent(new ComponentAudio(aud[0], float.Parse(aud[1]))); }
                                else if(aud.Length == 3)
                                { newEntity.AddComponent(new ComponentAudio(aud[0], float.Parse(aud[1]), bool.Parse(aud[1]))); }
                                else if(aud.Length == 4)
                                { newEntity.AddComponent(new ComponentAudio(aud[0], float.Parse(aud[1]), bool.Parse(aud[2]), bool.Parse(aud[3]))); }
                                // Reloads the audio object so settings are applied
                                ComponentAudio audio = (ComponentAudio)newEntity.FindComponent(ComponentTypes.COMPONENT_AUDIO);
                                audio.CloseAudio();
                                audio.AudioObject.ReloadAudio();
                                break;
                            // SKYBOX COMPONENT
                            case "skybox":
                                // Splits by , to get each texture for skybox
                                string[] skybox = component.Value.Trim().Split(',');
                                newEntity.AddComponent(new ComponentSkyBox(skybox));
                                break;
                            // COLLISION COMPONENT
                            case "collision":
                                newEntity.AddComponent(new ComponentCollider(bool.Parse(component.Value.Trim())));
                                break;
                            // AI COMPONENT
                            case "ai":
                                newEntity.AddComponent(new ComponentAI(component.Value.Trim()));
                                break;
                            // ANIMATION COMPONENT
                            case "animator":
                                // Splits by , to get 3 direction to rotate in and final value is speed to rotate at
                                string[] AniVals = component.Value.Split(',');
                                float[] Ani = Array.ConvertAll<string, float>(AniVals, float.Parse);
                                newEntity.AddComponent(new ComponentAnimation(
                                    new Vector3(Ani[0], Ani[1], Ani[2]), Ani[3]));
                                break;
                        }
                    }
                    // Finally adds this new entity to the list
                    entityList.Add(newEntity);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return entityList.ToArray();
        }
    }
}
