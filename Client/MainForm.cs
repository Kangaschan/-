using GameClasses;
using System.Diagnostics.Metrics;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Taskbar;
namespace Client
{
    public partial class MainForm : Form
    {
        Bitmap BackgroundBMP;
        Graphics g, BufferG, BackG;
        GameFiled gameFiled;
        private int Port;
        private string Username;
        private string IP;
        UdpClient client;
        Thread ServerListener;
        Thread ServerSender;
        Thread DrawThread;
        private Bitmap buffer;
        int screenWidth, screenHeight;
        int screenWidthOffset, screenHeightOffset;
        const int FieldSize = 16;
        int BlockPixelAmount;
        ThreadSafeList<Tank> Enemies;
        ThreadSafeList<bullet> Bullets;
        bool playerFireFlag;
        TankPlayer Player;
        
        public MainForm()
        {
            Screen screen = Screen.PrimaryScreen;
            screenWidth = screen.Bounds.Width;
            screenHeight = screen.Bounds.Height;

            InitForm initForm = new InitForm();
            try
            {

                initForm.ShowDialog();
                Port = initForm.port;
                IP = initForm.ip;
                Username = initForm.Username;
                if (initForm.quit)
                    Close();
            }
            catch
            {
                MessageBox.Show("что-то не так");
            }
            InitializeComponent();
            this.Width = (int)screenWidth + 50;
            this.Height = (int)screenHeight + 50;

            pictureBox.Width = screenWidth;
            pictureBox.Height = screenHeight;

            BlockPixelAmount = (int)screenHeight / (FieldSize + 5);
            screenWidthOffset = (int)((screenWidth - BlockPixelAmount * FieldSize)/2);
            screenHeightOffset = (int)((screenHeight - BlockPixelAmount * FieldSize)/2);
            gameFiled = new GameFiled(BlockPixelAmount, FieldSize, FieldSize);
            g = pictureBox.CreateGraphics();
            BackgroundBMP = new Bitmap(screenWidth,screenHeight);
            BackG = Graphics.FromImage(BackgroundBMP);
            buffer = new Bitmap(this.Width, this.Height);
            BufferG = Graphics.FromImage(buffer);

            Enemies = new ThreadSafeList<Tank>();
            Bullets = new ThreadSafeList<bullet> ();
            Player = new TankPlayer(BlockPixelAmount, initForm.Username);
            Player.x = 1; Player.y = 1;


            // Создаем и запускаем таймер
            int dueTime = 0;  // Время задержки перед первым запуском
            int period = 10;  // Период между запусками
            //ServerTimer = new System.Threading.Timer(TimerMoveCallback, this, dueTime, period);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            
            try
            {
                client = new UdpClient(IP, Port);
                string message = "Hello, Server!" + Username + "**";
                message = await client.SendAndReceiveAsync(message);

                string gamestart = await client.ReceiveAsync();
                Thread.Sleep(1000);
                gamestart = GetSubStringInd(gamestart, "GAMESTART", "ENOOFMESSAGE");
                string[] paraments = gamestart.Split('|');
                if (paraments[0] == Username) 
                {
                    Player.x = Convert.ToInt32(paraments[1]);
                    Player.y = Convert.ToInt32(paraments[2]);
                    Enemies.Add(new Tank(BlockPixelAmount, paraments[3]));
                    Enemies[0].x = Convert.ToInt32(paraments[4]);
                    Enemies[0].y = Convert.ToInt32(paraments[5]);
                }
                else
                {
                    Player.x = Convert.ToInt32(paraments[4]);
                    Player.y = Convert.ToInt32(paraments[5]);
                    Enemies.Add(new Tank(BlockPixelAmount, paraments[0]));
                    Enemies[0].x = Convert.ToInt32(paraments[1]);
                    Enemies[0].y = Convert.ToInt32(paraments[2]);
                }
                ServerListener = new Thread(new ThreadStart(ReceiveMessageTread));
                ServerListener.Start();

                ServerSender = new Thread(new ThreadStart(SendToServerThread));
                ServerSender.Start();

                DrawThread = new Thread(new ThreadStart(DrawThreadTask));
                DrawThread.Start();

                UpdateBackGroung();
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void UpdateBackGroung()
        {
            BackG.FillRectangle(Brushes.Black, 0, 0, screenWidth, screenHeight);
            for (int i = 0; i < FieldSize; i++)
            {
                for (int j = 0; j < FieldSize; j++)
                {
                    BackG.DrawImage(gameFiled.arr[i, j].Texture, screenWidthOffset +  i * (BlockPixelAmount), screenHeightOffset + j * (BlockPixelAmount));
                }
            }
        }
        private void BufferedDrawField()
        {
            //BufferG.Clear(Color.Black);
            BufferG.DrawImage(BackgroundBMP, 0, 0);
            BufferdDrawTankPlayer();
            BufferdDrawTank(Enemies[0]);
            
            for (int i = 0;i< Bullets.Count; i++)
            {
                    BufferG.DrawImage(Bullets[i].Texture[Bullets[i].pos], (int)(screenWidthOffset + Bullets[i].x * (BlockPixelAmount)), (int)(screenHeightOffset + 
                        Bullets[i].y * (BlockPixelAmount)));
            }
            g.DrawImage(buffer, 0, 0);
        }
       
        private async void DrawThreadTask()
        {
            while (true)
            {
                BufferedDrawField();
                Thread.Sleep(20);
            }
        }
        private async void ReceiveMessageTread()
        {
            while (true)
            {
                string message = await client.ReceiveAsync();
                if (message != "")
                    if(message.IndexOf("GAMEEND") < 0)
                    {
                        string FieldChange = GetSubStringInd(message, "FieldChanges:", "Tanks:");
                        string[] fldchanges = FieldChange.Split('$');
                        for (int i = 0; i < fldchanges.Length - 1; i++)
                        {
                            int x = Convert.ToInt32(GetSubStringInd(fldchanges[i], "BACK", "|"));
                            int y = Convert.ToInt32(GetSubStringInd(fldchanges[i], "|", "#"));
                            gameFiled.arr[x, y] = new BackGround(BlockPixelAmount);
                        }
                        if(fldchanges.Length - 1 > 0)
                        {
                            UpdateBackGroung();
                        }
                        string TanksPos = GetSubStringInd(message, "Tanks:", "Bullets:");
                        string[] tanks = TanksPos.Split('$');
                        for (int i = 0; i < tanks.Length - 1; i++)
                        {
                            string username = GetSubStringInd(tanks[i], "Tank", "|");
                            double x = Convert.ToDouble(GetSubStringInd(tanks[i], "|", "#"));
                            double y = Convert.ToDouble(GetSubStringInd(tanks[i], "#", "%"));
                            int pos = Convert.ToInt32(GetSubStringInd(tanks[i], "%", "@"));
                            if (username == Player.name)
                            {
                                Player.x = x; Player.y = y;
                                //Player.pos = pos;
                            }
                            else
                            {
                                foreach (var tank in Enemies)
                                {
                                    if (tank.name == username)
                                    {
                                        tank.x = x; tank.y = y;
                                        tank.pos = pos;
                                    }
                                }
                            }
                        }

                        string BulletsPos = GetSubStringInd(message, "Bullets:", "**");
                        if (BulletsPos != "")
                        {
                            int t = 0;
                        }
                        string[] BulletsStr = BulletsPos.Split('$');
                        //Bullets.Clear();
                        ThreadSafeList<bullet> bulletstmp = new ThreadSafeList<bullet>();
                        if (BulletsStr[0] != "")
                            for (int i = 0; i < BulletsStr.Length-1; i++)
                            {
                                double x = Convert.ToDouble(GetSubStringInd(BulletsStr[i], "Bullet", "|"));
                                double y = Convert.ToDouble(GetSubStringInd(BulletsStr[i], "|", "@"));
                                int pos = Convert.ToInt32(GetSubStringInd(BulletsStr[i], "@", "#"));
                                bullet tmp = new bullet(BlockPixelAmount, 1, 1, pos);
                                tmp.x = x; tmp.y = y;

                                bulletstmp.Add(tmp);
                            }
                        Bullets = bulletstmp;
                    }
                    else
                    {
                        string[] info = GetSubStringInd(message, "GAMEEND", "ENOOFMESSAGE").Split('|');
                        if (info[0] == Username)
                        {
                            if (Convert.ToInt32(info[1]) == 3)
                                MessageBox.Show("Победа!");
                            else
                                MessageBox.Show("Поражение!");
                        }
                        else
                        {
                            if (Convert.ToInt32(info[1]) == 3)
                                MessageBox.Show("Поражение!");
                            else
                                MessageBox.Show("Победа!");

                        }
                        this.Close();
                    }

            }
        }
        private async void SendToServerThread()
        {
            while (true)
            {
               
                string message = "UserName" + Player.name + "<" + Player.WPress.ToString() + "@" + Player.APress.ToString() + "#" + Player.SPress.ToString() + "$" + Player.DPress.ToString() + "%" + Player.pos.ToString() + "^" + playerFireFlag.ToString() + "&(";
                if (playerFireFlag == true)
                {
                    playerFireFlag = false;
                };
                await client.SendAsync(message);
                Thread.Sleep(50);
            }

        }
        private void BufferdDrawTankPlayer()
        {
            BufferG.DrawImage(Player.Texture[Player.pos], (int)(screenWidthOffset + Player.x * BlockPixelAmount), (int)(screenHeightOffset +  Player.y * BlockPixelAmount));
        }
        private void BufferdDrawTank(Tank tank)
        {
            BufferG.DrawImage(tank.Texture[tank.pos], (int)(screenWidthOffset + tank.x * BlockPixelAmount), (int)(screenHeightOffset +  tank.y * BlockPixelAmount));
        }
        private void pictureBox_Click(object sender, EventArgs e)
        {
            // BufferedDrawField();
        }

        
        private void nullPressedButtons()
        {
            Player.WPress = false;
            Player.APress = false;
            Player.SPress = false;
            Player.DPress = false;
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.W:
                    nullPressedButtons();
                    Player.WPress = true;
                    Player.pos = 0;
                    break;
                case Keys.A:
                    nullPressedButtons();
                    Player.APress = true;
                    Player.pos = 3;
                    break;
                case Keys.S:
                    nullPressedButtons();
                    Player.SPress = true;
                    Player.pos = 2;
                    break;
                case Keys.D:
                    nullPressedButtons();
                    Player.DPress = true;
                    Player.pos = 1;
                    break;
                case Keys.Space:
                    playerFireFlag = true;
                    break;
            }
        }
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.W:
                    Player.WPress = false;
                    break;
                case Keys.A:
                    Player.APress = false;
                    break;
                case Keys.S:
                    Player.SPress = false;
                    break;
                case Keys.D:
                    Player.DPress = false;  
                    break;
            }
        }
        static string GetSubStringInd(string input, string key1, string key2)
        {
            return (GetSubstring(input, input.IndexOf(key1) + key1.Length, input.IndexOf(key2) - input.IndexOf(key1) - key1.Length));
        }
        static string GetSubstring(string input, int startIndex, int length)
        {
            if (startIndex < 0 || startIndex >= input.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range.");
            }

            if (length < 0 || startIndex + length > input.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length is out of range.");
            }

            return input.Substring(startIndex, length);
        }


    }
}
