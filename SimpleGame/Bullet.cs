using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Media;

namespace SimpleGame
{
	class Bullet : IDisposable
	{
		private int x;
		private int y;
		private int kineticX;

		private int maxLifetime = 100;
		private int lifetime = 0;

		private GameObject bulletSender;

		public Bullet(GameObject bulletSender, int x, int y, int kineticX)
		{
			this.x = x;
			this.y = y;
			this.kineticX = kineticX;

			this.bulletSender = bulletSender;
		}

		public void Move(List<GameObject> objects)
		{
			this.x += this.kineticX;
			this.lifetime++;
			if (this.lifetime >= this.maxLifetime)
			{
				this.Dispose();
			}
		}

		public void Dispose()
		{
			//Звук попадания
			SoundPlayer pl = new SoundPlayer("bullethit.wav");

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

		public GameObject BulletSender
		{
			get
			{
				return this.bulletSender;
			}
		}

		public int Lifetime
		{
			get
			{
				return this.lifetime;
			}
		}

		public int MaxLifetime
		{
			get
			{
				return this.maxLifetime;
			}
		}
	}
}
