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
            this.IsVisible = true;
            GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1f));

            float[] vertices =
            {
                // first 3 = positions, next 4 = colors
               0.0f, 0.5f,  0f, 1f, 0f, 0f, 1f, // vertex0
               0.5f, -0.5f, 0f,  0f, 1f, 0f, 1f,// vertex1
               -0.5f, -0.5f, 0f, 0f, 0f, 1f, 1f// vertex2
            };


            // Generate and bind the vertex buffer
            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);

            // Send data to graphics card
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


            // Create and bind vertex array
            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);


            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            // Indicates the positions
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);

            //Indicates the colors
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);

            // Create shaders
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

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            // relsease resources

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayHandle);

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
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3); // here 3rd parameter count is the number of vertices

            SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
