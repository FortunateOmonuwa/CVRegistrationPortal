global using System.ComponentModel.DataAnnotations;


namespace CVRegistrationPortal.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
     
        public string FirstName { get; set; } = string.Empty;

        
        public string LastName { get; set; } = string.Empty;

 
        public string UserName { get; set; } = string.Empty;

       
        public string Phone { get; set; } = string.Empty;

   
        public string Email { get; set; } = string.Empty;
        public List<Document> Documents { get; set; } = [];
    }
}
