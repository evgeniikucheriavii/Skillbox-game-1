using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleGame
{
	class Game
	{

		private GameObject player; //Игрок
		private List<GameObject> objects; //Все объекты на локации
		private List<Bullet> bullets; //Все пули на локации
		
		private int startX = 0; //Точка, с которой начинается отрисовка объектов
		//Она нужна, чтобы двигать камерой

		public Game() // Конструктор объекта
		{
			//Создание списков
			this.objects = new List<GameObject>(); 
			this.bullets = new List<Bullet>();

			this.startX = 0; //Точка камеры устанавливается на ноль

			this.player = new GameObject("Character") // Создание игрока
			{
				Height = 75,
				Width = 50,
				X = 15,
				Y = 85,
				Color = "#00c"
			};

			this.objects.Add(this.player); //Добавление игрока в спосок с объектами

			GameObject ground = new GameObject("Decoration") //Создание блока с землёй
			{
				Height = 50,
				Width = 10000,
				X = 0,
				Y = 40,
				Color = "#0c0"
			};

			this.objects.Add(ground); //Добавление земли в список
			
			this.GenerateObjects(); //Генерация объектов
		}

		public GameObject Player //Создание геттера для объекта с игроком
		{ 
			get
			{
				return this.player;
			}
		}

		public Canvas Go() //Метод, который приводи игру в действие
		{
			this.Move(); //Сначала все объекты приводятся в движение

			Canvas Map = this.Draw(); //Затем создаётся канвас с объектами на карте

			return Map; //Канвас будет передан в XAML
		} 

		public void Move() //Метод движения
		{
			//Списки с уничтоженными объетами
			List<GameObject> disposedObjects = new List<GameObject>();
			List<Bullet> disposedBullets = new List<Bullet>();

			foreach (GameObject obj in this.objects) //Вызов каждого игрового объекта
			{
				obj.Move(this.objects); //Вызов метода Move, которой передаётся список объектов

				if (obj.CurrHealt <= 0) //Если у объетка не осталось здоровья, он будет добавлен в список уничтоженных объектов
				{

					try
					{
						if (obj.Killer.Equals(this.player)) //Если объект умер от пули игрока, игрок получит опыт
						{
							this.player.Expirience += 5;
						}
					}
					catch (Exception exc)
					{

					}

					disposedObjects.Add(obj);
				}

				this.startX = (this.player.X - 250) * -1; //Точка камеры начинается за 250 пикселей от координат игрока

				if (this.startX >= 0)
				{
					this.startX = 0;
				}
			}

			foreach (Bullet bullet in this.bullets) //Движение снарядов
			{
				bullet.Move(this.objects);
				if (bullet.Lifetime >= bullet.MaxLifetime)
				{
					disposedBullets.Add(bullet);
					bullet.Dispose();
				}
				else
				{
					foreach (GameObject obj in this.objects)
					{
						if (obj.X <= bullet.X + 5
							&& obj.X + obj.Width >= bullet.X + 5)
						{
							if (obj.Y >= bullet.Y - 5
								&& obj.Y - obj.Height <= bullet.Y)
							{
								obj.BulletHit(bullet);
								disposedBullets.Add(bullet);
								bullet.Dispose();
							}
						}
					}
				}


			}

			foreach (GameObject obj in disposedObjects)
			{
				this.objects.Remove(obj);
			}

			foreach (Bullet bullet in disposedBullets)
			{
				this.bullets.Remove(bullet);
			}
		}

		public Canvas Draw() // Рисуем все элементы на канвас
		{
			Canvas Map = new Canvas(); // Создём канвас

			Map.Background = Brushes.Black; // Указываем фон и размеры
			Map.Width = 720;
			Map.Height = 480;

			Canvas.SetTop(Map, 0);
			Canvas.SetLeft(Map, 0);

			foreach (GameObject obj in this.objects) // Проходимся по каждому объекту
			{
				if (obj.CurrHealt >= 1)
				{
					Canvas objCanvas = new Canvas(); // Создаём кавас для объекта

					Image objImg = new Image();

					// Указываем параметры
					objCanvas.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(obj.Color));

					objCanvas.Width = obj.Width;
					objCanvas.Height = obj.Height;

					Canvas.SetTop(objCanvas, Map.Height - obj.Y);
					Canvas.SetLeft(objCanvas, this.startX + obj.X);

					// Добавляем всё к основному канвасу
					Map.Children.Add(objCanvas);
				}

			}

			foreach (Bullet bullet in this.bullets) // Добавление снарядов
			{
				Canvas bulletCanvas = new Canvas(); // Создаём канвас для объекта

				Image objImg = new Image();

				// Указываем параметры
				bulletCanvas.Background = Brushes.Yellow;

				bulletCanvas.Width = 10;
				bulletCanvas.Height = 5;

				Canvas.SetTop(bulletCanvas, Map.Height - bullet.Y);
				Canvas.SetLeft(bulletCanvas, this.startX + bullet.X);

				// Добавляем всё к основному канвасу
				Map.Children.Add(bulletCanvas);

			}

			// Рисуем интерфейс

			// Рамка для интерфейса
			StackPanel HUD = new StackPanel();

			HUD.Background = Brushes.White;

			Canvas.SetTop(HUD, 10);
			Canvas.SetLeft(HUD, 10);

			TextBlock hpTextBlock = new TextBlock();

			hpTextBlock.Text = "Здоровье: " + this.player.FullHeath;
			hpTextBlock.Foreground = Brushes.Red;

			TextBlock xpTextBlock = new TextBlock();

			xpTextBlock.Text = "Опыт: " + this.player.Expirience;
			xpTextBlock.Foreground = Brushes.Green;

			HUD.Children.Add(hpTextBlock);
			HUD.Children.Add(xpTextBlock);

			Map.Children.Add(HUD);

			return Map;
		}



		public void Control(string command) //Передача управления игроку
		{
			bool isBullet = this.player.Control(command);

			if (isBullet) //Отправка пули
			{
				int bulletKinetic = 0;
				int bulletX = 0;
				int bulletY = this.player.Y - (this.player.Height / 2);


				if (this.Player.LastX >= 0)
				{
					bulletKinetic = 15;
					bulletX = this.player.X + this.player.Width + 2;

				}
				else
				{
					bulletKinetic = -15;
					bulletX = this.player.X - 2;
				}

				this.bullets.Add(new Bullet(this.player, bulletX, bulletY, bulletKinetic));
			}

		}
		
		private void GenerateObjects()
		{
			//Генерация новых объектов
			Random rand = new Random();

			int type = 0; //Тип объекта

			for (int i = 0; i < 100; i++) //Создание 100 объектов
			{
				type = rand.Next(10); //Генерация типа

				GameObject obj;

				if (type > 5) //Если сгенерированное число больше 5 – то создаётся персонаж
				{
					obj = new GameObject("Character")
					{
						Height = 75,
						Width = 50,
						X = rand.Next(100, 7500),
						Y = rand.Next(50, 550),
						Color = "#c00"
					};
				}
				else //Иначе создаётся декорация
				{
					obj = new GameObject("Decoration")
					{
						Height = 40,
						Width = rand.Next(100, 200),
						X = i * 350,
						Y = rand.Next(140, 450),
						Color = "#0c0"
					};
				}

				

				this.objects.Add(obj); //Добавление объектов в список
			}
		}
		

	}
}
