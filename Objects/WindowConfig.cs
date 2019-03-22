using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OpenCGE.Objects
{
    public class WindowConfig
    {
        private string mGameName;
        private int mWidth;
        private int mHeight;

        private string[] mSceneNames;

        public WindowConfig()
        {
            string path = ("Config.xml");
            LoadConfigFile(path);
        }

        private void LoadConfigFile(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    CreateConfigFile(filename);
                }
                else
                {
                    XDocument file = XDocument.Load(filename);
                    var settings = file.Element("CONFIGURATION").Elements();
                    foreach (var setting in settings)
                    {
                        switch (setting.Name.ToString())
                        {
                            case "NAME":
                                mGameName = setting.Value;
                                break;
                            case "RESOLUTION":
                                string[] size = setting.Value.Split('x');
                                mWidth = Int32.Parse(size[0]);
                                mHeight = Int32.Parse(size[1]);
                                break;
                            case "SCENES":
                                var allScenes = setting.Elements();
                                List<string> values = new List<string>();
                                foreach (var newScene in allScenes)
                                {
                                    values.Add(newScene.Value);
                                }
                                mSceneNames = values.ToArray();
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void CreateConfigFile(string filename)
        {
            mGameName = "Default Game";
            mWidth = 1600;
            mHeight = 900;

            new XDocument(
                new XElement("CONFIGURATION",
                    new XElement("NAME", "Default Game"),
                    new XElement("RESOLUTION", "1600x900"),
                    new XElement("SCENES",
                        new XElement("SCENE", "ExampleScene1")
                        ))
                ).Save(filename);

        }

        public string Name
        {
            get { return mGameName; }
        }
        public string[] Scenes
        {
            get { return mSceneNames; }
        }
        public int Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }
        public int Height
        {
            get { return mHeight; }
            set { mHeight = value; }
        }
    } 
}
