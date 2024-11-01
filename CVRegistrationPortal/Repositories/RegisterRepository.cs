global using Microsoft.Extensions.Options;
using CVRegistrationPortal.DTOs;

namespace CVRegistrationPortal.Repositories
{
    public class RegisterRepository : IRegister
    {
        private readonly CVContext context;
        private readonly AppSettings appSettings;

        public RegisterRepository(CVContext context, IOptions<AppSettings> appSettings)
        {
            this.context = context;
            this.appSettings = appSettings.Value;
        }

        public async Task<User> Register(UserCreateDTO model)
        {
            var maxFileSize = appSettings.MaxFileSize * 1024 *1024;
            if (model.IdentificationDocument.Length > maxFileSize)
            {
                throw new ArgumentOutOfRangeException($"File size exceeds the max allowed size of {maxFileSize}");
            }
            var fileExtention = Path.GetExtension(model.IdentificationDocument.FileName);
            //if (!fileExtention.ToLower().Equals(".pdf"))
            //{
            //    throw new FileLoadException("Only Pdf Documents are allowed");
            //}
            //    var allowedMimeTypes = new[]
            //    {
            //    "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .docx
            //    "application/msword" // .doc
            //    };

            //    if (!allowedMimeTypes.Contains(userDto.DocumentFile.ContentType))
            //    {
            //        return BadRequest("Invalid file type. Only Word documents are allowed.");
            //    }
            if (model.IdentificationDocument.ContentType != "application/pdf")
            {
                throw new FileLoadException("Only Pdf Documents are allowed");
            }
            try
            {
                var usernameExists = await context.Users.AnyAsync(x => x.UserName == model.UserName);
                if (usernameExists)
                {
                    throw new ArgumentException($"{model.UserName} already exists");
                }
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var basePath = Path.Combine( baseDirectory, "..", "..", "..", "..");
                var parentPath = Path.GetFullPath(basePath);
                var folderPath = Path.Combine(parentPath, appSettings.StoragePath, $"{model.UserName}");
                Console.WriteLine($"{parentPath}\n\n\n");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, model.IdentificationDocument.FileName.ToLower());
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.IdentificationDocument.CopyToAsync(stream);
                }

                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    UserName = model.UserName,
                    Documents = []
                };

               
                await context.AddAsync(user);
                await context.SaveChangesAsync();
                var document = new Document()
                {
                    Name = model.IdentificationDocument.FileName,
                    Path = filePath,
                    UserID = user.Id
                };
                user.Documents.Add(document);
                await context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}