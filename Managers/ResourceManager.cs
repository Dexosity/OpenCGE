using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenCGE.Objects;
using OpenTK.Audio;
using System.IO;
using OpenTK.Audio.OpenAL;

namespace OpenCGE.Managers
{
    public static class ResourceManager
    {
        static Dictionary<string, Geometry> geometryDictionary = new Dictionary<string, Geometry>();
        static Dictionary<string, int> textureDictionary = new Dictionary<string, int>();
        static Dictionary<string, Entity[]> entityDictionary = new Dictionary<string, Entity[]>();
        static Dictionary<string, int> audioDictionary = new Dictionary<string, int>();
        static Dictionary<string[], int> cubeMapTextureDictionary = new Dictionary<string[], int>();
        static Dictionary<string, Image> imageDictionary = new Dictionary<string, Image>();

        /// <summary>
        /// Loads the provided audio file into game storage. If it already exists will return object.
        /// </summary>
        /// <param name="filename">File to load from</param>
        /// <returns>Audio provide or Audio found if already exists</returns>
        public static int LoadAudio(string filename)
        {
            int buffer;
            audioDictionary.TryGetValue(filename, out buffer);
            if (buffer == 0)
            {
                try
                {
                    int channels, bits_per_sample, sample_rate;
                    // reserve a Handle for the audio file
                    buffer = AL.GenBuffer();

                    // Load a .wav file from disk.
                    //int channels, bits_per_sample, sample_rate;
                    byte[] sound_data = LoadWave(
                        File.Open(filename, FileMode.Open),
                        out channels,
                        out bits_per_sample,
                        out sample_rate);
                    ALFormat sound_format =
                    channels == 1 && bits_per_sample == 8 ? ALFormat.Mono8 :
                    channels == 1 && bits_per_sample == 16 ? ALFormat.Mono16 :
                    channels == 2 && bits_per_sample == 8 ? ALFormat.Stereo8 :
                    channels == 2 && bits_per_sample == 16 ? ALFormat.Stereo16 :
                    (ALFormat)0; // unknown
                    AL.BufferData(buffer, sound_format, sound_data, sound_data.Length, sample_rate);
                    string error = AL.GetError().ToString();
                    if (error != "NoError")
                    {
                        Console.WriteLine(error);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                
                
            }
            return buffer;
        }
        /// <summary>
        /// Loads all entities from the provided file. If it already exists will return object.
        /// </summary>
        /// <param name="filename">File to load from</param>
        /// <returns></returns>
        public static Entity[] LoadSceneEntities(string filename)
        {
            Entity[] entities;
            entityDictionary.TryGetValue(filename, out entities);
            if(entities == null)
            {
                entities = SceneEntities.LoadObject(filename);
            }
            return entities;
        }
        /// <summary>
        /// Loads in the provided geometry file. If it already exists will return object.
        /// </summary>
        /// <param name="filename">File to be loaded in</param>
        /// <returns></returns>
        public static Geometry LoadGeometry(string filename)
        {
            Geometry geometry;
            geometryDictionary.TryGetValue(filename, out geometry);
            if (geometry == null)
            {
                geometry = new Geometry();
                geometry.LoadObject(filename);
                geometryDictionary.Add(filename, geometry);
            }

            return geometry;
        }
        /// <summary>
        /// Loads in a texture from provided file. If it already exists will return object.
        /// </summary>
        /// <param name="filename">File to load in</param>
        /// <returns></returns>
        public static int LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int texture;
            textureDictionary.TryGetValue(filename, out texture);
            if (texture == 0)
            {
                texture = GL.GenTexture();
                textureDictionary.Add(filename, texture);
                GL.BindTexture(TextureTarget.Texture2D, texture);

                // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
                // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
                // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                // Removes the edges on skybox to appear smoother
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);

                Bitmap bmp = new Bitmap(filename);
                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                bmp.UnlockBits(bmp_data);
            }
 
            return texture;
        }
        /// <summary>
        /// Loads in textures for a cubemap. If it already exists will return object.
        /// </summary>
        /// <param name="filename">Files to be loaded</param>
        /// <returns></returns>
        public static int LoadCubeMapTexture(string[] filename)
        {
            int texture;
            cubeMapTextureDictionary.TryGetValue(filename, out texture);
            if (texture == 0)
            {
                texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.TextureCubeMap, texture);
                for (int i = 0; i < filename.Length; i++)
                {
                    if (String.IsNullOrEmpty(filename[i]))
                        throw new ArgumentException(filename[i]);

                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    // Removes the edges on skybox to appear smoother
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge);

                    Bitmap bmp = new Bitmap(filename[i].Trim());
                    BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                    bmp.UnlockBits(bmp_data);
                }
                cubeMapTextureDictionary.Add(filename, texture);
            }
            return texture;
        }
        /// <summary>
        /// Loads an image from provided file for UI. If it already exists will return object.
        /// </summary>
        /// <param name="filename">File to be loaded</param>
        /// <returns></returns>
        public static Image LoadImage(string filename)
        {
            Image image;
            imageDictionary.TryGetValue(filename, out image);
            if (image == null)
            {
                image = Image.FromFile(filename);
            }
            return image;
        }
        private static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }
    }
}
