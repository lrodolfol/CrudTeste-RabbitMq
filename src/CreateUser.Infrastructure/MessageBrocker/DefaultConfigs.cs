namespace CreateUser.Infrastructure.MessageBrocker;
internal struct DefaultConfigs
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Queue { get; set; }
    public string Exchange { get; set; }
    public string RoutingKey { get; set; }

    public bool Durable { get; set; }
    public bool Transient { get; set; }
    public bool AutoDeleteQueue { get; set; }
    public bool ExclusiveQueue { get; set; }

    public int Port { get; set; }
}
