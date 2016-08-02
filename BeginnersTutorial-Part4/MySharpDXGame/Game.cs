using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3D11 = SharpDX.Direct3D11;

namespace MySharpDXGame
{
	public class Game : IDisposable
	{
		private RenderForm renderForm;

		private const int Width = 1280;
		private const int Height = 720;

		private D3D11.Device d3dDevice;
		private D3D11.DeviceContext d3dDeviceContext;
		private SwapChain swapChain;
		private D3D11.RenderTargetView renderTargetView;
		private Viewport viewport;

		// Shaders
		private D3D11.VertexShader vertexShader;
		private D3D11.PixelShader pixelShader;
		private ShaderSignature inputSignature;
		private D3D11.InputLayout inputLayout;

		private D3D11.InputElement[] inputElements = new D3D11.InputElement[] 
		{
			new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0)
		};

		// Triangle vertices
		private Vector3[] vertices = new Vector3[] { new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(0.5f, 0.5f, 0.0f), new Vector3(0.0f, -0.5f, 0.0f) };
		private D3D11.Buffer triangleVertexBuffer;

		/// <summary>
		/// Create and initialize a new game.
		/// </summary>
		public Game()
		{
			// Set window properties
			renderForm = new RenderForm("My first SharpDX game");
			renderForm.ClientSize = new Size(Width, Height);
			renderForm.AllowUserResizing = false;

			InitializeDeviceResources();
			InitializeShaders();
			InitializeTriangle();
		}

		/// <summary>
		/// Start the game.
		/// </summary>
		public void Run()
		{
			// Start the render loop
			RenderLoop.Run(renderForm, RenderCallback);
		}

		private void RenderCallback()
		{
			Draw();
		}

		private void InitializeDeviceResources()
		{
			ModeDescription backBufferDesc = new ModeDescription(Width, Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);
			
			// Descriptor for the swap chain
			SwapChainDescription swapChainDesc = new SwapChainDescription()
			{
				ModeDescription = backBufferDesc,
				SampleDescription = new SampleDescription(1, 0),
				Usage = Usage.RenderTargetOutput,
				BufferCount = 1,
				OutputHandle = renderForm.Handle,
				IsWindowed = true
			};

			// Create device and swap chain
			D3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3D11.DeviceCreationFlags.None, swapChainDesc, out d3dDevice, out swapChain);
			d3dDeviceContext = d3dDevice.ImmediateContext;

			viewport = new Viewport(0, 0, Width, Height);
			d3dDeviceContext.Rasterizer.SetViewport(viewport);

			// Create render target view for back buffer
			using(D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0))
			{
				renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
			}
		}

		private void InitializeShaders()
		{
			// Compile the vertex shader code
			using(var vertexShaderByteCode = ShaderBytecode.CompileFromFile("vertexShader.hlsl", "main", "vs_4_0", ShaderFlags.Debug))
			{
				// Read input signature from shader code
				inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);

				vertexShader = new D3D11.VertexShader(d3dDevice, vertexShaderByteCode);
			}

			// Compile the pixel shader code
			using(var pixelShaderByteCode = ShaderBytecode.CompileFromFile("pixelShader.hlsl", "main", "ps_4_0", ShaderFlags.Debug))
			{
				pixelShader = new D3D11.PixelShader(d3dDevice, pixelShaderByteCode);
			}

			// Set as current vertex and pixel shaders
			d3dDeviceContext.VertexShader.Set(vertexShader);
			d3dDeviceContext.PixelShader.Set(pixelShader);

			d3dDeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

			// Create the input layout from the input signature and the input elements
			inputLayout = new D3D11.InputLayout(d3dDevice, inputSignature, inputElements);

			// Set input layout to use
			d3dDeviceContext.InputAssembler.InputLayout = inputLayout;
		}

		private void InitializeTriangle()
		{
			// Create a vertex buffer, and use our array with vertices as data
			triangleVertexBuffer = D3D11.Buffer.Create<Vector3>(d3dDevice, D3D11.BindFlags.VertexBuffer, vertices);
		}
		
		/// <summary>
		/// Draw the game.
		/// </summary>
		private void Draw()
		{
			// Set back buffer as current render target view
			d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);

			// Clear the screen
			d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(32, 103, 178));
			
			// Set vertex buffer
			d3dDeviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(triangleVertexBuffer, Utilities.SizeOf<Vector3>(), 0));

			// Draw the triangle
			d3dDeviceContext.Draw(vertices.Count(), 0);

			// Swap front and back buffer
			swapChain.Present(1, PresentFlags.None);
		}

		public void Dispose()
		{
			inputLayout.Dispose();
			inputSignature.Dispose();
			triangleVertexBuffer.Dispose();
			vertexShader.Dispose();
			pixelShader.Dispose();
			renderTargetView.Dispose();
			swapChain.Dispose();
			d3dDevice.Dispose();
			d3dDeviceContext.Dispose();
			renderForm.Dispose();
		}
	}
}
