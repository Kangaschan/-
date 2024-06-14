using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading;
using System.Windows.Forms;

namespace GameClasses
{

    public class GameFiled
    {
        
        public Block [,] arr;
        int size;
        int width, height;
        public GameFiled(int Size, int Width, int Height)
        {
            size = Size;
            width = Width;
            height = Height;
            arr = new Block[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j< height; j++)
                {
                    arr[i,j] = new BackGround(size);
                }
            }
            if (width == height && width == 16)
            {
                for (int i = 0; i < height; i++)
                {
                    arr[0, i] = new UnbBrick(size);
                }
                for (int i = 0; i < height; i++)
                {
                    arr[15, i] = new UnbBrick(size);
                }
                for (int i = 0; i < height; i++)
                {
                    arr[i, 0] = new UnbBrick(size);
                }
                for (int i = 0; i < height; i++)
                {
                    arr[15, i] = new UnbBrick(size);
                }
                for (int i = 1; i < height - 1; i++)
                {
                    arr[5, i] = new Brick(size);
                }
                for (int i = 1; i < height - 1; i++)
                {
                    arr[10, i] = new Brick(size);
                }
            }
        }
    }
    public class Block
    {
        public int x, y;
        public bool passable;
        public Bitmap Texture;
        public Block() 
        { }
    }
    public class BackGround : Block
    {
        public BackGround(int size) 
        {   
            Bitmap tmp = (Bitmap)Bitmap.FromFile("..//..//..//..//textures//asph.bmp");
            this.Texture = new Bitmap(tmp,size,size);
            this.passable = true;
        }

    }   
    public class  UnbBrick : Block
    {
        public UnbBrick(int size)
        {
            Bitmap tmp = (Bitmap)Bitmap.FromFile("..//..//..//..//textures//UnrBreak.bmp");
            this.Texture = new Bitmap(tmp, size, size);
            this.passable = false;
        }
    }
    public class Brick : Block
    {
        public int hp; 
        public Brick(int size)
        {
            Bitmap tmp = new Bitmap("..//..//..//..//textures//Breack.bmp");
            this.Texture = new Bitmap(tmp, size, size);
            this.passable = false;
            this.hp = 2;
        }
    }
    public class Tank
    {
        public string name;
        public int pos;
        public double x, y;
        public int hp;
        public int ammo;
        public Bitmap[] Texture;
        private int check;
        private const int firerate = 240;
        private const int maxAmmo = 5;
        public int score;
        public double speed;

        public bool WPress, APress, SPress, DPress;
        public Tank(int size, string name)
        {
           // Bitmap tmp = new Bitmap("C://MYSTUFF//Kursach2//textures//Tank.png");
            Bitmap tmp = new Bitmap("..//..//..//..//textures//tank.png");
            this.Texture = new Bitmap[4];
            this.Texture[0] = new Bitmap(tmp, size, size);
            this.Texture[1] = new Bitmap(tmp, size, size);
            this.Texture[2] = new Bitmap(tmp, size, size);
            this.Texture[3] = new Bitmap(tmp, size, size);
            this.Texture[1].RotateFlip(RotateFlipType.Rotate90FlipNone);
            this.Texture[2].RotateFlip(RotateFlipType.Rotate180FlipNone);
            this.Texture[3].RotateFlip(RotateFlipType.Rotate270FlipNone);
            this.hp = 3;
            this.ammo = maxAmmo;
            this.check = 0;
            this.name = name;
            this.score = 0;
            this.speed = 0.05;
            this.WPress = false;
            this.APress = false;
            this.SPress = false;
            this.DPress = false;
        }
        public bool fire()
        {
            if (ammo > 0)
            {
                ammo--;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void timeivent()
        {
            check++;
            if(check >= firerate)
            {
                check = 0;
                if(ammo < maxAmmo)
                {
                    ammo++;
                }
            }
        }
    }
    public class TankPlayer

    {
        public string name;
        public int pos;
        public double x, y;
        public int hp;
        public int ammo;
        public Bitmap[] Texture;
        private int check;
        private const int firerate = 240;
        private const int maxAmmo = 5;
        public int score;
        public double speed;

        public bool WPress, APress, SPress, DPress;
        public TankPlayer(int size, string name)
        {

            // Bitmap tmp = new Bitmap("C://MYSTUFF//Kursach2//textures//Tank.png");
            Bitmap tmp = new Bitmap("..//..//..//..//textures//tankPlayer.png");
            this.Texture = new Bitmap[4];
            this.Texture[0] = new Bitmap(tmp, size, size);
            this.Texture[1] = new Bitmap(tmp, size, size);
            this.Texture[2] = new Bitmap(tmp, size, size);
            this.Texture[3] = new Bitmap(tmp, size, size);
            this.Texture[1].RotateFlip(RotateFlipType.Rotate90FlipNone);
            this.Texture[2].RotateFlip(RotateFlipType.Rotate180FlipNone);
            this.Texture[3].RotateFlip(RotateFlipType.Rotate270FlipNone);
            this.hp = 3;
            this.ammo = maxAmmo;
            this.check = 0;
            this.name = name;
            this.score = 0;
            this.speed = 0.05;
            this.WPress = false;
            this.APress = false;
            this.SPress = false;
            this.DPress = false;
        }
        public bool fire()
        {
            if (ammo > 0)
            {
                ammo--;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void timeivent()
        {
            check++;
            if (check >= firerate)
            {
                check = 0;
                if (ammo < maxAmmo)
                {
                    ammo++;
                }
            }
        }
    }
    public class bullet
    {

        public Bitmap[] Texture;
        public int pos;
        public double speed;
        public double x,y;
        public bullet(int size, double Px, double Py, int Ppos)
        {
            this.Texture = new Bitmap[4];
            Bitmap tmp = new Bitmap("..//..//..//..//textures//bullet.png");
            this.Texture[0] = new Bitmap(tmp, (int)(size), (int)(size));
            this.Texture[1] = new Bitmap(tmp, (int)(size), (int)(size));
            this.Texture[2] = new Bitmap(tmp, (int)(size    ), (int)(size));
            this.Texture[3] = new Bitmap(tmp, (int)(size ), (int)(size));

            this.Texture[1].RotateFlip(RotateFlipType.Rotate90FlipNone);
            this.Texture[2].RotateFlip(RotateFlipType.Rotate180FlipNone);
            this.Texture[3].RotateFlip(RotateFlipType.Rotate270FlipNone);
            this.speed = 0.1;

            this.x = Px;
            this.y = Py;
            this.pos = Ppos;
        }
    }

    public class ThreadSafeList<T> : IEnumerable<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public void Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                _list.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        public bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _list.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _list.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        public T this[int index]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _list[index];
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _list[index] = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _list.Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            List<T> snapshot;
            _lock.EnterReadLock();
            try
            {
                snapshot = new List<T>(_list); // Create a snapshot of the list
            }
            finally
            {
                _lock.ExitReadLock();
            }

            foreach (var item in snapshot)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
}
