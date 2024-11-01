namespace CVRegistrationPortal.DTOs
{
    public class UserDTO
    {
    }
    public class UserGetDTO
    {
        public required string FirstName { get; set; }


        public required string LastName { get; set; }


        public required string UserName { get; set; }

        public required string Phone { get; set; }


        public required string Email { get; set; }
    }
    public class UserCreateDTO
    {
        public required string FirstName { get; set; } 

       
        public required string LastName { get; set; } 

   
        public required string UserName { get; set; } 

        public required string Phone { get; set; } 

       
        public required string Email { get; set; } 

        public required IFormFile IdentificationDocument { get; set; }
    }
}
