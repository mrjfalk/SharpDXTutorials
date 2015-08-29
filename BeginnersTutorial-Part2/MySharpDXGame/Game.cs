using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySharpDXGame
{
	public class Game : IDisposable
	{
		private RenderForm renderForm;

		private const int Width = 1280;
		private const int Height = 720;

		public Game()
		{
			renderForm = new RenderForm("My first SharpDX game");
			renderForm.ClientSize = new Size(Width, Height);
			renderForm.AllowUserResizing = false;
		}

		public void Run()
		{
			RenderLoop.Run(renderForm, RenderCallback);
		}

		private void RenderCallback()
		{

		}

		public void Dispose()
		{
			renderForm.Dispose();
		}
	}
}
