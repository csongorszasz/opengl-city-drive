using Silk.NET.OpenGL;
using System.Numerics;

namespace projekt
{
    public class Drawer
    {
        private static Shader shader;
        private static bool initialized = false;

        public static void Initialize(GL gl, Shader sh)
        {
            shader = sh;
            initialized = true;
        }

        public static unsafe void DrawBoundingBox(GL gl, Matrix4x4 model, Matrix4x4 view, Matrix4x4 projection, Vector3 min, Vector3 max)
        {
            if (!initialized)
            {
                throw new InvalidOperationException("Drawer has not been initialized. Call Drawer.Initialize(gl, vertexShaderPath, fragmentShaderPath) first.");
            }

            Vector3[] vertices = new Vector3[8];
            vertices[0] = new Vector3(min.X, min.Y, min.Z);
            vertices[1] = new Vector3(max.X, min.Y, min.Z);
            vertices[2] = new Vector3(max.X, max.Y, min.Z);
            vertices[3] = new Vector3(min.X, max.Y, min.Z);
            vertices[4] = new Vector3(min.X, min.Y, max.Z);
            vertices[5] = new Vector3(max.X, min.Y, max.Z);
            vertices[6] = new Vector3(max.X, max.Y, max.Z);
            vertices[7] = new Vector3(min.X, max.Y, max.Z);

            uint[] indices = new uint[]
            {
                0, 1, 1, 2, 2, 3, 3, 0,
                4, 5, 5, 6, 6, 7, 7, 4,
                0, 4, 1, 5, 2, 6, 3, 7
            };

            uint vbo = gl.GenBuffer();
            uint ebo = gl.GenBuffer();
            uint vao = gl.GenVertexArray();

            gl.BindVertexArray(vao);

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (Vector3* vertexPtr = vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(Vector3)), vertexPtr, BufferUsageARB.StaticDraw);
            }

            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            fixed (uint* indexPtr = indices)
            {
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), indexPtr, BufferUsageARB.StaticDraw);
            }

            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vector3), (void*)0);
            gl.EnableVertexAttribArray(0);

            shader.Use();

            shader.SetUniform("uModel", model);
            shader.SetUniform("uView", view);
            shader.SetUniform("uProjection", projection);

            gl.BindVertexArray(vao);
            gl.DrawElements(PrimitiveType.Lines, (uint)indices.Length, DrawElementsType.UnsignedInt, null);
            gl.BindVertexArray(0);

            gl.DeleteBuffer(vbo);
            gl.DeleteBuffer(ebo);
            gl.DeleteVertexArray(vao);
        }
    }
}
