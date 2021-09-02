using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Media;
using System.Dynamic;

namespace Shooter
{
    public partial class Form1 : Form
    {
        Player p;
        static int _WIDTH;
        static int _HEIGHT;
        static Random r;
        public const int spawnInterval = 50;
        public int interval = spawnInterval;
        static bool pressedA = false;
        static bool pressedD = false;
        static bool pressedW = false;
        static bool pressedS = false;
        static bool pressedR = false;
        static bool pressedShift = false;
        static bool pressedSpace = false;
        public static List<Rectangle> bullets;
        static List<Enemy> enemies;
        
        class Guns
        {
            public static readonly Gun normal = new Gun(1,4,10,1,10);
            public static readonly Gun doubleGun = new Gun(2, 3, 10, 1,10);
            public static readonly Gun tripleGun = new Gun(3, 3, 10, 1,10);
            public static readonly Gun speedGun = new Gun(1, 15, 10, 1,10);
            public static readonly Gun cannon = new Gun(1, 3, 30, 3,40);
        }
        class Gun
        {
            public int atkRate;
            public int bullets;
            public int bulletSpd;
            public int bulletSize;
            public int damage;
            public Gun(int bs, int bSpd, int bSize, int d, int atr)
            {
                bullets = bs;
                bulletSize = bSize;
                bulletSpd = bSpd;
                damage = d;
                atkRate = atr;
            }
        }
        class Enemy
        {
            static Random r = new Random();
            public Point pos;
            public Size size;
            public Color color;
            public int hp;
            public int curHp;
            
            public int speed;
            public Enemy()
            {
                hp = r.Next(1,16);
                size = new Size((int)Math.Round(map(hp, 1,10,20,100)),(int)Math.Round(map(hp, 1, 10, 20, 100)));
                color = Color.FromArgb((int)Math.Round(map(hp, 1, 10, 255, 150)), 0, (int)Math.Round(map(hp, 1, 10, 255, 150)));
                pos = new Point { X = r.Next(0, _WIDTH-size.Width), Y = r.Next(-_HEIGHT,0) };
                
                curHp = hp;
                speed = (int)Math.Round(map(hp,1,15,6,1));
            }
            public void Move()
            {
                pos.Y += speed;
               
            }
        }
        public Form1()
        {
            InitializeComponent();
            r = new Random();
            _HEIGHT = ClientSize.Height;
            _WIDTH = ClientSize.Width;
            p = new Player(new Point(_WIDTH / 2, _HEIGHT - 50 * 2), new Size(50, 50), Color.Black, Guns.normal);
            bullets = new List<Rectangle>();

            enemies = new List<Enemy>();
            BulletTimer.Interval = 10;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }
        class Player
        {
            public int counterAt;
            public Point pos;
            public Size size;
            public Color color;
            public int speed;
            public const int normalSpeed = 10;
            public int score = 0;
            public Gun gun;
            public int ultCount;
            public Player(Point p, Size s, Color c, Gun g)
            {
                pos = p;
                speed = normalSpeed;
                size = s;
                color = c;
                gun = g;
                counterAt = gun.atkRate;
                ultCount = 3;
            }
            public void Shoot()
            {
                bullets.Add(new Rectangle(pos.X + size.Width / 2 - gun.bulletSize / 2, pos.Y, gun.bulletSize, gun.bulletSize));
                for (int i = 1; i < gun.bullets; i++)
                {
                    bullets.Add(new Rectangle((size.Width/(gun.bullets+1)*i)+pos.X, pos.Y, gun.bulletSize, gun.bulletSize));
                }
            }
            public void Move()
            {
                if (pressedA)
                {
                    if (pos.X <= 0)  pos.X = 0;
                    else pos.X -= speed;
                    
                }
                if (pressedD)
                {
                    if (pos.X + size.Width >= _WIDTH) pos.X = _WIDTH - size.Width;
                    else pos.X += speed;
                   
                }
                if (pressedW)
                {
                    if (pos.Y <= 0) pos.Y = 0;
                    else pos.Y -= speed;
                }
                if (pressedS)
                {
                    if (pos.Y + size.Height >= _HEIGHT) pos.Y = _HEIGHT - size.Height;
                    else pos.Y += speed;

                }
                if (pressedR)
                {
                    if (ultCount != 0)
                    {
                        enemies.Clear();
                        ultCount--;
                        pressedR = false;
                    }
                }
                if (pressedSpace)
                {
                    if (counterAt == gun.atkRate)
                    {
                        Shoot();
                        counterAt = 0;
                    }
                    counterAt++;
                }
                if (pressedShift)
                {
                    speed = 5;
                }
                else
                {
                    speed = normalSpeed;
                }
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
           //
           //
        }
        public static float map(float value, float low1, float high1, float low2, float high2)
        {
            return low2 + (high2 - low2) * (value - low1) / (high1 - low1);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawRectangle(new Pen(p.color), new Rectangle(p.pos, p.size));
            for (int i = 0; i < enemies.Count; i++)
            {
                g.FillRectangle(new SolidBrush(enemies[i].color),new Rectangle(enemies[i].pos,enemies[i].size));
                if (enemies[i].curHp != enemies[i].hp)
                {
                    g.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(new Point(enemies[i].pos.X, enemies[i].pos.Y + enemies[i].size.Height), new Size(enemies[i].size.Width / enemies[i].hp * enemies[i].curHp, 5)));

                }
            }
            for (int i = 0; i < p.ultCount; i++)
            {
                g.FillEllipse(Brushes.Gold,new Rectangle(25*i+5,100,15,15));
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                g.FillEllipse(new SolidBrush(Color.Black), bullets[i]);
            }
            TextRenderer.DrawText(g,p.score.ToString(),new Font(FontFamily.GenericSansSerif,25,FontStyle.Bold),new Rectangle(25,25,100,50),Color.Black,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter);
            base.OnPaint(e);
        }
        public void GameOver()
        {
            DialogResult res = MessageBox.Show("New game?", "Game over", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                Application.Restart();
                Environment.Exit(0);
            }
            else
            {
                Application.Exit();
            }
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
            Rectangle Y;
            if(interval == 0)
            {
                enemies.Add(new Enemy());
                interval = spawnInterval;
            }
            interval--;           
            
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                for (int k = 0; k < bullets.Count; k++)
                {
                    
                    if (enemies.Any(ce => ce.pos == enemies[i].pos))
                    {
                        if (bullets[k].IntersectsWith(new Rectangle(enemies[i].pos, enemies[i].size)))
                        {
                            enemies[i].curHp -= p.gun.damage;
                            bullets.RemoveAt(k);
                            if (enemies[i].curHp <= 0)
                            {
                                p.score += enemies[i].hp;
                                enemies.RemoveAt(i);
                            }

                        }
                    }
                }
                if (enemies[i] != null && enemies.Count > 0 && enemies != null)
                {
                    if ((new Rectangle(p.pos, p.size).IntersectsWith(new Rectangle(enemies[i].pos, enemies[i].size))))
                    {
                        BulletTimer.Stop();
                        GameOver();
                    }
                    if (enemies.Any(ce => ce.pos == enemies[i].pos))
                    {
                        enemies[i].Move();
                        if (enemies[i].pos.Y > _HEIGHT) enemies.RemoveAt(i);
                        
                    }
                    
                }
                
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                Y = bullets[i];
                Y.Y -= p.gun.bulletSpd;
                bullets[i] = Y;
                if (bullets[i].Y<0) bullets.RemoveAt(i);
                
            }
            p.Move();
            
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) pressedA = true;
            if (e.KeyCode == Keys.W) pressedW = true;
            if (e.KeyCode == Keys.D) pressedD = true;
            if (e.KeyCode == Keys.S) pressedS = true;
            if (e.KeyCode == Keys.Space) pressedSpace = true;
            if (e.KeyCode == Keys.ShiftKey) pressedShift = true;

        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) pressedA = false;
            if (e.KeyCode == Keys.W) pressedW = false;
            if (e.KeyCode == Keys.D) pressedD = false;
            if (e.KeyCode == Keys.S) pressedS = false;
            if (e.KeyCode == Keys.R) pressedR = true;
            if (e.KeyCode == Keys.Space) pressedSpace = false;
            if (e.KeyCode == Keys.ShiftKey) pressedShift = false;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == ' ') pressedSpace = true;
        }
    }
}
