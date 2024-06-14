using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameClasses;

public class UdpServer
{

    public bool stop;
    private readonly IPEndPoint _endPoint;
    private readonly Socket _listener;
    //private readonly List<EndPoint> _clients;
    private Timer _timer;

    System.Threading.Timer TickTimer;
    const int FieldSize = 16;
    const int PlayersAmount = 2;

    List<Thread> playersListeners;

    ThreadSafeList<playerInfo> _players;
    ThreadSafeList<bullet> _bullets;
    ThreadSafeList<Block> GameFieldGhanges;
    public bool ChangeSaved;

    GameFiled gameFiled;
    public bool PlayerFire;
    static public bool MoveUpCheck(double x, double y, GameFiled gameFiled)
    {
        int x1, y1, x2, y2;
        x1 = (int)(x + 0.2);
        x2 = (int)(x - 0.2 + 1);
        y1 = (int)(y);
        if (x1 < gameFiled.arr.Length && x2 < gameFiled.arr.Length)
        {
            if (gameFiled.arr[x1, y1].passable && gameFiled.arr[x2, y1].passable)
                return true;
        }
        return false;
    }
    static public bool MoveDownCheck(double x, double y, GameFiled gameFiled)
    {
        int x1, y1, x2, y2;
        x1 = (int)(x + 0.2);
        x2 = (int)(x - 0.2 + 1);
        y1 = (int)(y) + 1;
        int tmplen = (int)Math.Sqrt(gameFiled.arr.Length + 0.01);
        if (y1 >= tmplen)
            return false;
        if (x1 < gameFiled.arr.Length && x2 < gameFiled.arr.Length)
        {
            if (gameFiled.arr[x1, y1].passable && gameFiled.arr[x2, y1].passable)
                return true;
        }
        return false;
    }
    static public bool MoveRightCheck(double x, double y, GameFiled gameFiled)
    {
        int x1, y1, x2, y2;
        y1 = (int)(y + 0.2);
        y2 = (int)(y - 0.2 + 1);
        x1 = (int)x + 1;
        int tmplen = (int)Math.Sqrt(gameFiled.arr.Length + 0.01);
        if (x1 >= tmplen)
            return false;
        if (y2 < gameFiled.arr.Length && y1 < gameFiled.arr.Length)
        {
            if (gameFiled.arr[x1, y1].passable && gameFiled.arr[x1, y2].passable)
                return true;
        }
        return false;
    }
    static public bool MoveLeftCheck(double x, double y, GameFiled gameFiled)
    {

        int x1, y1, x2, y2;
        y1 = (int)(y + 0.2);
        y2 = (int)(y - 0.2 + 1);
        x1 = (int)x;
        if (y2 < gameFiled.arr.Length && y1 < gameFiled.arr.Length)
        {

            if (gameFiled.arr[x1, y1].passable && gameFiled.arr[x1, y2].passable)
                return true;
        }
        return false;
    }
    static void TimerTickCallback(Object state)
    {

        UdpServer server = (UdpServer)state;
        //двигаем игроков
        foreach (var player in server._players)
        {
            if (player.WPress)
            {
                player.tank.y -= player.tank.speed;
                if (player.tank.y <= 0)
                    player.tank.y = 0;
                else
                if (!MoveUpCheck(player.tank.x, player.tank.y, server.gameFiled))
                    player.tank.y += player.tank.speed;
            }
            if (player.APress)
            {
                player.tank.x -= player.tank.speed;
                if (player.tank.x <= 0) player.tank.x = 0;
                else
                if (!MoveLeftCheck(player.tank.x, player.tank.y, server.gameFiled))
                    player.tank.x += player.tank.speed;

            }
            if (player.SPress)
            {
                player.tank.y += player.tank.speed;
                if (player.tank.y >= FieldSize)
                    player.tank.y = FieldSize;
                else
                if (!MoveDownCheck(player.tank.x, player.tank.y, server.gameFiled))
                    player.tank.y -= player.tank.speed;
            }
            if (player.DPress)
            {
                player.tank.x += player.tank.speed;
                if (player.tank.x >= FieldSize)
                    player.tank.x = FieldSize;
                else
                    if (!MoveRightCheck(player.tank.x, player.tank.y, server.gameFiled))
                    player.tank.x -= player.tank.speed;
            }
        }
        foreach (var bullet in server._bullets)
        {
            bool flag = true;
            //двигаем пули
            switch (bullet.pos)
            {
                case 0:
                    bullet.y -= bullet.speed;
                    if (bullet.y < 0)
                    {
                        server._bullets.Remove(bullet);
                        flag = false;
                    }
                    break;
                case 1:
                    bullet.x += bullet.speed;
                    if (bullet.x > FieldSize)
                    {
                        server._bullets.Remove(bullet);
                        flag = false;
                    }
                    break;
                case 2:
                    bullet.y += bullet.speed;
                    if (bullet.y > FieldSize)
                    {
                        server._bullets.Remove(bullet);
                        flag = false;
                    }
                    break;
                case 3:
                    bullet.x -= bullet.speed;
                    if (bullet.x < 0)
                    {
                        server._bullets.Remove(bullet);
                        flag = false;
                    }
                    break;
            }

            // проверяем попадания
            if (flag)
            {
                bool flag2 = false;
                foreach (var player in server._players)
                {
                    switch (bullet.pos)
                    {
                        case 0:
                            if (((int)(bullet.y - 1) == (int)(player.tank.y)) && (((int)bullet.x) == (int)player.tank.x))
                            {
                                player.tank.hp--;
                                if (player.tank.hp == 0)
                                {
                                    player.deaths++;
                                    player.tank.x = player.defaultX;
                                    player.tank.y = player.defaultY;
                                    player.tank.hp = 3;
                                    if(player.deaths == 3)
                                    {
                                        server.EndGame();
                                    }
                                }
                                flag2 = true;
                            }
                            break;
                        case 1:
                            if (((int)bullet.y == (int)(player.tank.y)) && (((int)bullet.x + 1) == (int)player.tank.x))
                            {
                                player.tank.hp--;
                                if (player.tank.hp == 0)
                                {
                                    player.deaths++;
                                    player.tank.x = player.defaultX;
                                    player.tank.y = player.defaultY;
                                    player.tank.hp = 3;
                                    if (player.deaths == 3)
                                    {
                                        server.EndGame();
                                    }
                                }
                                flag2 = true;
                            }
                            break;
                        case 2:
                            if (((int)(bullet.y + 1) == (int)(player.tank.y)) && (((int)bullet.x) == (int)player.tank.x))
                            {
                                player.tank.hp--;
                                if (player.tank.hp == 0)
                                {
                                    player.deaths++;
                                    player.tank.x = player.defaultX;
                                    player.tank.y = player.defaultY;
                                    player.tank.hp = 3;
                                    if (player.deaths == 3)
                                    {
                                        server.EndGame();
                                    }
                                }
                                flag2 = true;
                            }
                            break;
                        case 3:
                            if (((int)bullet.y == (int)(player.tank.y)) && (((int)bullet.x - 1) == (int)player.tank.x))
                            {
                                player.tank.hp--;
                                if (player.tank.hp == 0)
                                {
                                    player.deaths++;
                                    player.tank.x = player.defaultX;
                                    player.tank.y = player.defaultY;
                                    player.tank.hp = 3;
                                    if (player.deaths == 3)
                                    {
                                        server.EndGame();
                                    }
                                }
                                flag2 = true;
                            }
                            break;
                    }
                }
                int x, y;
                int FieldLen = server.gameFiled.arr.Length;
                FieldLen = (int)Math.Sqrt(FieldLen + 0.001);
                switch (bullet.pos)
                {
                    case 0:
                        x = (int)(bullet.x + 0.5);
                        y = (int)(bullet.y - 1);
                        if (x < FieldLen && y > 0)
                        {
                            if (server.gameFiled.arr[x, y] is Brick)
                            {
                                 Brick brick = server.gameFiled.arr[x, y] as Brick;
                                brick.hp--;
                                if (brick.hp == 0)
                                {
                                    server.gameFiled.arr[x, y] = new BackGround(1);
                                    BackGround tmp = new BackGround(1);
                                    tmp.x = (int)x;
                                    tmp.y = (int)y;
                                    server.GameFieldGhanges.Add(tmp);
                                    server.ChangeSaved = false;
                                }
                                flag2 = true;
                            }
                            if (server.gameFiled.arr[x,y] is UnbBrick)
                            {
                                flag2 = true;
                            }
                        }
                        break;
                    case 1:
                        x = (int)(bullet.x + 1);
                        y = (int)(bullet.y + 0.5);
                        if (x < FieldLen && y < FieldLen)
                        {
                            if (server.gameFiled.arr[x, y] is Brick)
                            {
                                Brick brick = server.gameFiled.arr[x, y] as Brick;
                                brick.hp--;
                                if (brick.hp == 0)
                                {
                                    server.gameFiled.arr[x, y] = new BackGround(1);
                                    BackGround tmp = new BackGround(1);
                                    tmp.x = (int)x;
                                    tmp.y = (int)y;
                                    server.GameFieldGhanges.Add(tmp);
                                    server.ChangeSaved = false;
                                }
                                flag2 = true;
                            }
                            if (server.gameFiled.arr[x, y] is UnbBrick)
                            {
                                flag2 = true;
                            }
                        }
                        break;
                    case 2:
                        x = (int)(bullet.x + 0.5);
                        y = (int)(bullet.y + 1);
                        if (x < FieldLen && y < FieldLen)
                        {
                            if (server.gameFiled.arr[x, y] is Brick)
                            {
                                Brick brick = server.gameFiled.arr[x, y] as Brick;
                                brick.hp--;
                                if (brick.hp == 0)
                                {
                                    server.gameFiled.arr[x, y] = new BackGround(1);
                                    BackGround tmp = new BackGround(1);
                                    tmp.x = (int)x;
                                    tmp.y = (int)y;
                                    server.GameFieldGhanges.Add(tmp);
                                    server.ChangeSaved = false;
                                }
                                flag2 = true;
                            }
                            if (server.gameFiled.arr[x, y] is UnbBrick)
                            {
                                flag2 = true;
                            }
                        }
                        break;
                    case 3:
                        x = (int)(bullet.x - 1);
                        y = (int)(bullet.y + 0.5);
                        if (x > 0 && y < FieldLen)
                        {
                            if (server.gameFiled.arr[x, y] is Brick)
                            {
                                Brick brick = server.gameFiled.arr[x, y] as Brick;
                                brick.hp--;
                                if (brick.hp == 0)
                                {
                                    server.gameFiled.arr[x, y] = new BackGround(1);
                                    BackGround tmp = new BackGround(1);
                                    tmp.x = (int)x;
                                    tmp.y = (int)y;
                                    server.GameFieldGhanges.Add(tmp);
                                    server.ChangeSaved = false;
                                }
                                flag2 = true;
                            }
                            if (server.gameFiled.arr[x, y] is UnbBrick)
                            {
                                flag2 = true;
                            }
                        }
                        break;
                }

                if (flag2)
                    server._bullets.Remove(bullet);
            }
            else
            {
                server._bullets.Remove(bullet);
            }
        }


    }
    private void StartGame()
    {
        string message = "GAMESTART" +_players[0].userName + "|" + _players[0].tank.x.ToString() 
            + "|" + _players[0].tank.y.ToString() + "|" + _players[1].userName + "|" 
            + _players[1].tank.x.ToString() + "|" + _players[1].tank.y.ToString() + "ENOOFMESSAGE";
        byte[] data = Encoding.UTF8.GetBytes(message);
        foreach (var client in _players)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _listener.SendToAsync(new ArraySegment<byte>(data), SocketFlags.None, client.clientEndPoint);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred while sending periodic message to {client}: {ex.Message}");
                    _players.Remove(client); // Remove client if sending fails
                }
            });
        } 

        //Thread.Sleep(100);

        _timer = new Timer(SendPeriodicMessages, null, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10)); ;

        // Создаем и запускаем таймер
        int dueTime = 0;  // Время задержки перед первым запуском
        int period = 15;  // Период между запусками
        TickTimer = new System.Threading.Timer(TimerTickCallback, this, dueTime, period);
        playersListeners = new List<Thread>();
    }

    public void EndGame() 
    {
        string message = "GAMEEND" + _players[0].userName + "|" + _players[0].deaths + "|" + _players[1].userName + "|"
          + _players[1].deaths + "ENOOFMESSAGE";
        byte[] data = Encoding.UTF8.GetBytes(message);
        foreach (var client in _players)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _listener.SendToAsync(new ArraySegment<byte>(data), SocketFlags.None, client.clientEndPoint);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred while sending periodic message to {client}: {ex.Message}");
                    _players.Remove(client); // Remove client if sending fails
                }
            });
        }
        Thread.Sleep(1000);
        stop = true;

    }

    public UdpServer(string ipAddress, int port)
    {
        stop = false;
        _endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        _listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        
       

        _players = new ThreadSafeList<playerInfo>();
        _bullets = new ThreadSafeList<bullet>();
        GameFieldGhanges = new ThreadSafeList<Block>();
        ChangeSaved = true;
        gameFiled = new GameFiled(1, FieldSize, FieldSize);



    }

    public async Task StartAsync()
    {
        _listener.Bind(_endPoint);
        Console.WriteLine("Server started...");

        while (true)
        {
            try
            {
                await HandleIncomingMessagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }
    }

    private async Task HandleIncomingMessagesAsync()
    {
        byte[] buffer = new byte[1024];
        EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

        var result = await _listener.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, clientEndPoint);
        clientEndPoint = result.RemoteEndPoint;

        string message = Encoding.UTF8.GetString(buffer, 6, result.ReceivedBytes);
        // Console.WriteLine($"Received from {clientEndPoint}: {message}");

        // Add player to the list if it's not already present
        var flag = true;
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].clientEndPoint.Equals(clientEndPoint))
            {
                flag = false;
            }
        }
        if (flag)//новый игрок
        {
            string Username = GetSubStringInd(message, "Hello, Server!", "**");
            if (_players.Count == 0)
            {
                _players.Add(new playerInfo(Username, new Tank(1, Username), clientEndPoint));
                _players[0].tank.x = 1;
                _players[0].tank.y = 1;
                _players[0].defaultX = 1;
                _players[0].defaultY = 1;
            }
            else
            if (_players.Count == 1)
            {
                _players.Add(new playerInfo(Username, new Tank(1, Username), clientEndPoint));
                _players[1].tank.x = 14;
                _players[1].tank.y = 14;
                _players[1].defaultX = 14;
                _players[1].defaultY = 14;

                StartGame();
            }
        }
        else// старый игрок
        {
            string username = GetSubStringInd(message, "UserName", "<");
            bool WPESS = Convert.ToBoolean(GetSubStringInd(message, "<", "@"));
            bool APESS = Convert.ToBoolean(GetSubStringInd(message, "@", "#"));
            bool SPESS = Convert.ToBoolean(GetSubStringInd(message, "#", "$"));
            bool DPESS = Convert.ToBoolean(GetSubStringInd(message, "$", "%"));
            int POS = Convert.ToInt32(GetSubStringInd(message, "%", "^"));
            bool FIRE = Convert.ToBoolean(GetSubStringInd(message, "^", "&"));

            foreach (var user in _players)
            {
                if (user.userName == username)
                {
                    user.WPress = WPESS;
                    user.APress = APESS;
                    user.SPress = SPESS;
                    user.DPress = DPESS;
                    user.tank.pos = POS;
                    if (FIRE)
                    {
                        Console.WriteLine("FIRE");
                        switch (POS)
                        {
                            case 0:
                                _bullets.Add(new bullet(10, user.tank.x, user.tank.y - 1.1, user.tank.pos));
                                break;
                            case 1:
                                _bullets.Add(new bullet(10, user.tank.x + 1.1, user.tank.y, user.tank.pos));
                                break;
                            case 2:
                                _bullets.Add(new bullet(10, user.tank.x, user.tank.y + 1.1, user.tank.pos));
                                break;
                            case 3:
                                _bullets.Add(new bullet(10, user.tank.x - 1.1, user.tank.y, user.tank.pos));
                                break;

                        }
                    }
                }
            }
        }

        // Echo the message back to the client in a separate task
        Task.Run(async () =>
        {
            try
            {
                await _listener.SendToAsync(new ArraySegment<byte>(buffer, 0, result.ReceivedBytes), SocketFlags.None, clientEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while sending message to {clientEndPoint}: {ex.Message}");
                //      _clients.Remove(clientEndPoint); // Remove client if sending fails
            }
        });
    }

    private void SendPeriodicMessages(object state)
    {

        string message = "FieldChanges:";
        foreach (BackGround backGround in GameFieldGhanges)
        {
            message += "BACK" + backGround.x.ToString() + "|" + backGround.y.ToString() + "#$";
            GameFieldGhanges.Remove(backGround);
        }
        message += "Tanks:";
        ThreadSafeList<playerInfo> players = new ThreadSafeList<playerInfo>();
        players = _players;
        foreach (var player in players)
        {
            message += "Tank" + player.userName + "|" + player.tank.x.ToString() + "#" + player.tank.y.ToString() + "%" + player.tank.pos.ToString() + "@$";
        }
        message += "Bullets:";
        ThreadSafeList<bullet> bullets= new ThreadSafeList<bullet>();
        bullets = _bullets;
        foreach (var bullet in bullets)
        {
            message += "Bullet" + bullet.x.ToString() + "|" + bullet.y.ToString() + "@" + bullet.pos.ToString() + "#$";
        }

        message += "**";
        //Console.WriteLine(message);
        byte[] data = Encoding.UTF8.GetBytes(message);

        foreach (var client in _players)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _listener.SendToAsync(new ArraySegment<byte>(data), SocketFlags.None, client.clientEndPoint);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred while sending periodic message to {client}: {ex.Message}");
                    _players.Remove(client); // Remove client if sending fails
                }
            });
        }
    }

    public class playerInfo
    {
        public double defaultX, defaultY;
        public int deaths;
        public string userName;
        public Tank tank;
        public EndPoint clientEndPoint;

        public bool WPress, APress, SPress, DPress;
        public playerInfo(string username, Tank tank, EndPoint clientEndPoint)
        {
            this.userName = username;
            this.tank = tank;
            this.clientEndPoint = clientEndPoint;

            WPress = false;
            APress = false;
            SPress = false;
            DPress = false;
            deaths = 0;
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
