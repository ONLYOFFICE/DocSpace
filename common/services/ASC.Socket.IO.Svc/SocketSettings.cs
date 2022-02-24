namespace ASC.Socket.IO.Svc
{
    public class SocketSettings
    {
        public string Path { get; set; }
        public string Port { get; set; }
        public string RedisHost { get; set; }
        public string RedisPort { get; set; }
        public int? PingInterval { get; set; }
        public int? ReconnectAttempts { get; set; }

    }
}
