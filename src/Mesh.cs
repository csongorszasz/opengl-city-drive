// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Numerics;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;

namespace projekt
{
    public class Mesh : IDisposable
    {
        public Mesh(GL gl, float[] vertices, uint[] indices, List<Texture> textures)
        {
            GL = gl;
            Vertices = vertices;
            Indices = indices;
            Textures = textures;
            SetupMesh();
        }

        public float[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }
        public IReadOnlyList<Texture> Textures { get; private set; }
        public VertexArrayObject<float, uint> VAO { get; set; }
        public BufferObject<float> VBO { get; set; }
        public BufferObject<uint> EBO { get; set; }
        public GL GL { get; }

        public unsafe void SetupMesh()
        {
            EBO = new BufferObject<uint>(GL, Indices, BufferTargetARB.ElementArrayBuffer);
            VBO = new BufferObject<float>(GL, Vertices, BufferTargetARB.ArrayBuffer);
            VAO = new VertexArrayObject<float, uint>(GL, VBO, EBO);
            VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
            VAO.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 8, 3);
            VAO.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 8, 5);
            GL.BindVertexArray(0);
        }

        public void Bind()
        {
            VAO.Bind();
        }
        
        public (Vector3 min, Vector3 max) CalculateBoundingBox()
        {
            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);
            for (var i = 0; i < Vertices.Length; i += 8)
            {
                var vertex = new Vector3(Vertices[i], Vertices[i + 1], Vertices[i + 2]);
                min = Vector3.Min(min, vertex);
                max = Vector3.Max(max, vertex);
            }

            return (min, max);
        }

        public unsafe void Draw(ref Shader shader)
        {
            uint diffuseNr = 1;
            uint specularNr = 1;
            for (var i = 0; i < Textures.Count; i++)
            {
                // GL.ActiveTexture(TextureUnit.Texture0 + i);  // this is done in the Texture.Bind() method
                var name = Textures[i].Type switch
                {
                    TextureType.Diffuse => "texture_diffuse",
                    TextureType.Specular => "texture_specular",
                    _ => throw new Exception("Unknown texture type.")
                };

                var number = name switch
                {
                    "texture_diffuse" => diffuseNr++,
                    "texture_specular" => specularNr++,
                    _ => throw new Exception("Unknown texture name.")
                };

                // TODO: add "material." to the resulting uniform name (source: https://learnopengl.com/Model-Loading/Mesh#:~:text=We%20also%20added%20%22material.%22%20to%20the%20resulting%20uniform%20name%20because%20we%20usually%20store%20the%20textures%20in%20a%20material%20struct%20(this%20may%20differ%20per%20implementation).)
                // set the sampler to the correct texture unit
                shader.Use();                
                shader.SetUniform($"{name}{number}", i);
                
                // bind the texture
                Textures[i].Bind(TextureUnit.Texture0 + i);
            }
            
            // draw mesh
            Bind();
            GL.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
            GL.BindVertexArray(0);
            
            // always good practice to set everything back to defaults once configured
            GL.ActiveTexture(TextureUnit.Texture0);
        }
        
        public void Dispose()
        {
            Textures = null;
            VAO.Dispose();
            VBO.Dispose();
            EBO.Dispose();
        }
    }
}
