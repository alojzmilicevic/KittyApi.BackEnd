namespace KittyAPI.Dto;

public class SignalRMessageDto
{
    public SignalRMessageDto(string type)
    {
        Type = type;
    }
    public Object Payload { get; set; }
    public string Type { get; set; }

}
