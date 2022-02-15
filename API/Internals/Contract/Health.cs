namespace Diorama.Internals.Contract;

public class HealthContract
{
    public static readonly DateTime StartTime = DateTime.Now;
    public DateTime Date { get; set; }
    public String UpTime { get; set; } = "0:0:0.0";

    public string ServiceName { get; set; } = "unnamed";

}