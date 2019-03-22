using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenCGE.Objects
{
    public class Geometry
    {
        List<float> vertices = new List<float>();
        //float[] normals;
        int numberOfTriangles;

        // Graphics
        private int vao_Handle;
        private int vbo_verts;

        public Geometry()
        {
        }

        public void LoadObject(string filename)
        {
            string line;
            try
            {
                FileStream fin = File.OpenRead(filename);
                StreamReader sr = new StreamReader(fin);

                GL.GenVertexArrays(1, out vao_Handle);
                GL.BindVertexArray(vao_Handle);
                GL.GenBuffers(1, out vbo_verts);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] values = line.Split(',');

                    if (values[0].StartsWith("NUM_OF_TRIANGLES"))
                    {
                        numberOfTriangles = int.Parse(values[0].Remove(0, "NUM_OF_TRIANGLES".Length));
                        continue;
                    }
                    if (values[0].StartsWith("//") || values.Length < 5) continue;

                    vertices.Add(float.Parse(values[0]));
                    vertices.Add(float.Parse(values[1]));
                    vertices.Add(float.Parse(values[2]));
                    vertices.Add(float.Parse(values[3]));
                    vertices.Add(float.Parse(values[4]));
                }

                CalculateNormals();
                

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_verts);
                GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Count * 4), vertices.ToArray<float>(), BufferUsageHint.StaticDraw);

                // Positions
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8*4, 0);

                // Tex Coords
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8*4, 3*4);

                // Normals
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 8*4, 5*4);
                

                GL.BindVertexArray(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        /// <summary>
        /// Calculates the normal vectors for the model and adds it into the list of vectors for position and texture coordinates
        /// </summary>
        private void CalculateNormals()
        {
            //List<float> norms = new List<float>();
            List<float> newVertices = new List<float>();

            for (int i = 0; i < Vertices.Length; i += 15)
            {
                Vector3 t1 = new Vector3(Vertices[i], Vertices[i + 1], Vertices[i + 2]);

                Vector3 t2 = new Vector3(Vertices[i + 5], Vertices[i + 6], Vertices[i + 7]);

                Vector3 t3 = new Vector3(Vertices[i + 10], Vertices[i + 11], Vertices[i + 12]);


                Vector3 values = Vector3.Cross(t2 - t1, t3 - t1);
                values.Normalize();
                // This creates a new list which will form the verticies list so the normal vectors are with their corisponding vertexs
                for (int j = 0; j < 3; j++)
                {
                    // vertex
                    newVertices.Add(vertices[i + (j * 5)]);
                    newVertices.Add(vertices[(i + (j * 5)) + 1]);
                    newVertices.Add(vertices[(i + (j * 5)) + 2]);
                    // tex coords
                    newVertices.Add(vertices[(i + (j * 5)) + 3]);
                    newVertices.Add(vertices[(i + (j * 5)) + 4]);                   
                    // new normal vlaues
                    newVertices.Add(values.X);
                    newVertices.Add(values.Y);
                    newVertices.Add(values.Z);
                }
                
            }
            vertices = newVertices;
        }
        /// <summary>
        /// Returns the array of verticies for this model
        /// </summary>
        public float[] Vertices
        {
            get { return vertices.ToArray(); }
        }

        public void Render()
        {
            GL.BindVertexArray(vao_Handle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, numberOfTriangles * 3);
        }
    }
}
