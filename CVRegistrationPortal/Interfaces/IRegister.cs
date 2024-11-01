global using CVRegistrationPortal.DTOs;

namespace CVRegistrationPortal.Interfaces
{
    public interface IRegister
    {
        Task<User> Register(UserCreateDTO model);
    }
}
