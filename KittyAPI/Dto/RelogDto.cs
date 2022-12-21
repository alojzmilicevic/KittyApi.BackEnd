using KittyAPI.Dto.Auth;

namespace KittyAPI.Dto;

public class RelogDto
{
    public UserDetailDto User { get; set; }
    public AuthenticationResult AuthResult { get; set; }
}
