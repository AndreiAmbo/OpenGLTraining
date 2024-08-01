using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace CubeOpenGl
{
    public class Game : GameWindow
    {
        private int vertexBufferHandle;
        private int shaderHandle;
        private int vertexArrayHandle;
        private int indexBufferHandle;

        public Game(int width = 1280, int height = 768, string title = "Game")
            : base(GameWindowSettings.Default,
                  new NativeWindowSettings
                  {
                      Title = title,
                      Size = new Vector2i(width, height),
                      StartVisible = false,
                      StartFocused = true,
                      API = ContextAPI.OpenGL,
                      Profile = ContextProfile.Core,
                      APIVersion = new Version(3, 3)
                  })
        {
            CenterWindow(new Vector2i(1280, 768));
        }

        protected override void OnLoad()
        {
            IsVisible = true;
            GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1f));

            float[] vertices =
            {
               -0.5f, 0.5f,  0f, 1f, 0f, 0f, 1f,    // vertex0   // positions = 3 floats, color = 4 floats
               0.5f, 0.5f, 0f,  0f, 1f, 0f, 1f,    // vertex1
               0.5f, -0.5f, 0f, 0f, 0f, 1f, 1f,   // vertex2
               -0.5f, -0.5f, 0f, 1f, 1f, 0f, 1f  // vertex3
            };

            int[] indices =
            {
                0, 1, 2,
                0, 2, 3,
            };

            GenerateAndBindVertexBuffer(vertices);
            GenerateAndBindIndexBuffer(indices);
            GenerateAndBindVertexArray();
            SetupVertexAttributes();
            HandleShaders();

            base.OnLoad();
        }

        private static void SetupVertexAttributes()
        {
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);
        }

        private void GenerateAndBindVertexArray()
        {
            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
        }

        private void HandleShaders()
        {
            string vertexShaderCode = File.ReadAllText("Shaders/shader.vertex");
            string fragmentShaderCode = File.ReadAllText("Shaders/shader.frag");

            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);

            // Combine shaders and create a Shader Program
            shaderHandle = GL.CreateProgram();

            // Atach shaders
            GL.AttachShader(shaderHandle, vertexShaderHandle);
            GL.AttachShader(shaderHandle, fragmentShaderHandle);

            // Link Program
            GL.LinkProgram(shaderHandle);

            //Detach Shaders (dispose resources)
            GL.DetachShader(shaderHandle, vertexShaderHandle);
            GL.DetachShader(shaderHandle, fragmentShaderHandle);

            // Delete Shaders
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);
        }

        private void GenerateAndBindIndexBuffer(int[] indices)
        {
            indexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private void GenerateAndBindVertexBuffer(float[] vertices)
        {
            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected override void OnUnload()
        {
            // relsease resources

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(indexBufferHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(shaderHandle);


            GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1f));
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.UseProgram(shaderHandle);

            GL.BindVertexArray(vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}