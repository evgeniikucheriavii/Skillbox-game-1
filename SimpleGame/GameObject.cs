using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGame
{
	class GameObject : IDisposable
	{
		private const int gForce = 10;

		private int x;
		private int y;
		private int kineticX = 0;
		private int kineticY = 0;

		private int lastX = 0;

		private int maxKineticX = 25;
		private int speedX = 0;
		private int speedY = 0;

		private bool isOnGround = false;

		private string type;

		private int width;
		private int height;
		private string color;

		private int maxHealth = 100;
		private int currHealth = 100;
		private int expirience = 0;

		private GameObject killer;


		public GameObject(string type)
		{
			this.type = type;
		}

		public void Move(List<GameObject> objects) // Функция движения
		{

			if (this.type != "Decoration") //Если объект не является декорацией
			{

				// Ось X

				this.speedX = this.kineticX;

				this.x += this.speedX;
				
				if (this.x <= 0)
				{
					this.x = 0;
				}

				bool canMove = true;

				if (this.kineticX < 0) // Движение влево
				{
					foreach (GameObject obj in objects) // Проверяем столкновения с другими объектами
					{
						if (!this.Equals(obj)) // Проверяем все объекты кроме текущего
						{
							if ((this.x + this.kineticX) <= obj.X + obj.Width
								&& this.x + this.width + this.kineticX >= obj.X) 
							{ // Если совпадают координаты по X
								if (this.y - this.height >= obj.Y - obj.Height
									&& this.y <= obj.Y)
								{ // Если совпадают координаты по Y
									this.x += this.kineticX;
									canMove = false;
									break;
								}
							}
						}
					}

					if (canMove)
					{
						this.kineticX++;
					}
					else
					{
						this.kineticX = 1;
					}

				}
				else if (this.kineticX > 0) // Движение вправо
				{
					foreach (GameObject obj in objects) // Проверяем каждый объект
					{
						if (!this.Equals(obj))
						{
							if ((this.x + this.width + 1 + this.kineticX >= obj.X
								|| this.x + this.width + 1 >= obj.X)
								&& this.x <= obj.X + obj.Width)
							{
								if (this.y - this.height >= obj.Y - obj.Height
									&& this.y <= obj.Y)
								{
									this.x -= this.kineticX;
									canMove = false;
									break;
								}
							}
						}
					}

					if (canMove)
					{
						this.kineticX--;
					}
					else
					{
						this.kineticX = -1;
					}
				}

				// Ось Y

				//Находим низ

				if (this.isOnGround) // Если объект на земле
				{
					this.kineticY = 0; // Обнуляем кинетическую энергию по вертикали
					this.speedY = 0; // Обнуляем скорость
				}
				else
				{
					this.speedY = this.kineticY; // Формула скорости, её можно переделать, чтобы учитывать массу объекта

					this.y += this.speedY; // Меняем положение по оси Y

					// Отнимаем от кинетической энергии ускорение свободного падения
					// Если игрок летит вверх, он будет замедляться, пока кинетическая энергия не иссякнет
					// Потом он начнёт падать вниз, ускоряясь
					this.kineticY -= gForce;
				}

				//Падение

				bool wasOnGround = this.isOnGround;

				
				this.isOnGround = false;

				int bottomY = this.y - this.height;

				foreach (GameObject obj in objects)
				{
					if (!this.Equals(obj))
					{
						if ((this.x >= obj.X && this.x <= obj.X + obj.Width)
							|| (this.x + this.width >= obj.X && this.x + this.width <= obj.X + obj.Width))
						{
							if (bottomY - 1 <= obj.Y && obj.Y - obj.Height <= bottomY)
							{
								this.y = obj.Y + this.height;
								if (Math.Abs(this.kineticY) >= 70)
								{
									this.currHealth -= 5;
								}

								this.isOnGround = true;
								break;
							}
						}
					}
				}
			}
		}

		public void BulletHit(Bullet bullet) // Попадание пули
		{
			this.currHealth -= 5; // Отнятие здоровья

			if (this.currHealth <= 0) // Если жизни не осталось
			{
				this.killer = bullet.BulletSender; // Указываем убийцу
			}
		}

		public bool Control(string command)
		{

			bool isBullet = false;

			switch (command)
			{
				case "Left":
					//Влево
					this.lastX = -1;
					if (Math.Abs(this.kineticX) <= this.maxKineticX)
					{
						this.kineticX--;
					}
					break;
				case "Right":
					//Вправо
					this.lastX = 1;
					if (Math.Abs(this.kineticX) <= this.maxKineticX)
					{
						this.kineticX++;
					}
					break;
				case "Up":
					//Прыжок
					if(this.isOnGround) //Если игрок на земле, добавление кинетической энергии
					{
						this.kineticY += 50;
						this.isOnGround = false;
					}
					break;
				case "Atack":
					//Атака
					isBullet = true;
					break;
			}

			return isBullet;
		}

		public void Dispose()
		{
			//Проигрывание звука при уничтожении объекта
			SoundPlayer pl = new SoundPlayer("die.wav");

			pl.Play();
		}

		public int X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}
		public int Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		public string Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
			}
		}

		public int LastX
		{
			get
			{
				return this.lastX;
			}

		}

		public string FullHeath
		{
			get
			{
				return this.currHealth.ToString() + " / " + this.maxHealth;
			}
		}

		public int CurrHealt
		{
			get
			{
				return this.currHealth;
			}
		}

		public int MaxHealth
		{
			get
			{
				return this.maxHealth;
			}
		}

		public int Expirience
		{
			get
			{
				return this.expirience;
			}
			set
			{
				this.expirience = value;
			}
		}

		public GameObject Killer
		{
			get
			{
				return this.killer;
			}
		}
	}
}
