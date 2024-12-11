global using System.ComponentModel.DataAnnotations;


namespace CVRegistrationPortal.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public required string FirstName { get; set; } 
        public required string LastName { get; set; } 
        public required string Email { get; set; } 
        public required string UserName { get; set; } 
        public string Phone { get; set; } 
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string VerificationToken { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public string TokenExpiration { get; set; } = string.Empty;
        public string VerifiedAt { get; set; } = string.Empty;
        public List<Document> Documents { get; set; } = [];
    }
}
