namespace CVRegistrationPortal.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDetail<User>> GetUser(int Id);
    }
}
