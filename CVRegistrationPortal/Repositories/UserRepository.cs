
using Azure;

namespace CVRegistrationPortal.Repositories
{
    public class UserRepository : IUserService
    {
        private readonly CVContext context;
        public UserRepository(CVContext context)
        {
            this.context = context;
        }
        public async Task<ResponseDetail<User>> GetUser(int Id)
        {
            var response = new ResponseDetail<User>();
            try
            {
                var user = await context.Users.FindAsync(Id);
                if(user is null)
                {
                    response = response.FailedResultData(default,"User doesn't exist", 404);
                }
                response = response.SuccessResultData(user);
            }
            catch(Exception ex)
            {
                response = response.FailedResultData(ex.Message, ex.HResult);
            }
            return response;

        }
    }
}
