using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SimpleGame
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	/// 

	public partial class MainWindow : Window
	{

		private Game game = new Game();

		public MainWindow()
		{
			InitializeComponent();



			this.DataContext = game;

			var timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(1000 / 30);
			timer.Tick += Timer_Tick;
			timer.Start();


			
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			Canvas Map = game.Go();

			GameCanvas.Children.Clear();
			GameCanvas.Children.Add(Map);
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			string command = string.Empty;
			switch (e.Key)
			{
				case Key.Left:
					command = "Left";
					break;
				case Key.Right:
					command = "Right";
					break;
				case Key.Up:
					command = "Up";
					break;
				case Key.RightCtrl:
					command = "Atack";
					break;
			}

			game.Control(command);
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			
		}
	}
}
