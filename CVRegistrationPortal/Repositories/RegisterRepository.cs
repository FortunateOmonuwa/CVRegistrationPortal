global using Microsoft.Extensions.Options;
global using CVRegistrationPortal.DTOs;
global using Utilities.Responses;
global using Utilities.Configs;
global using Utilities.Services.MailService.DTO;
global using Utilities.Helpers;
global using Utilities.Services.MailService;
using CVRegistrationPortal.Models;

namespace CVRegistrationPortal.Repositories
{
    public class RegisterRepository : IRegisterService
    {
        private readonly CVContext context;
        private readonly AppSettings appSettings;
        private readonly IMailService mailService;
        private readonly ILogger<RegisterRepository> logger;

        public RegisterRepository(CVContext context, IOptions<AppSettings> appSettings, IMailService mailService, ILogger<RegisterRepository> logger)
        {
            this.context = context;
            this.appSettings = appSettings.Value;
            this.mailService = mailService;
            this.logger = logger;
        }

        public async Task<ResponseDetail<User>> Register(UserCreateDTO model)
        {
            var response = new ResponseDetail<User>();
            //var transaction = await context.Database.BeginTransactionAsync();
            var maxFileSize = appSettings.MaxFileSize * 1024 *1024;
            if (model.IdentificationDocument.Length > maxFileSize)
            {
                response.FailedResultData($"File size exceeds the max allowed size of {maxFileSize}");
            }
          
             var fileExtension = new List<string>
            {
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/pdf"
            };

            if (!fileExtension.Contains(model.IdentificationDocument.ContentType))
            {
                logger.LogError(message: $"A wrong document format was uploaded by {model.Email}");
                return response.FailedResultData("Only PDF or Word documents are allowed");
            }
            try
            {
                if (model.Password != model.ConfirmPassword)
                {
                    return response.FailedResultData("Passwords do not match");
                }

                Util.CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var usernameExists = await context.Users.AnyAsync(x => x.UserName == model.UserName);
                if (usernameExists)
                {
                    return response.FailedResultData($"{model.UserName} already exists");
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
                    Documents = [],
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    VerificationToken = Util.CreateRandomVerificationToken().Substring(0, 7),
                    TokenExpiration = DateTime.Now.AddMinutes(3).ToString(),
                    VerifiedAt = "Unverified"
                };

               
                await context.AddAsync(user);
                await context.SaveChangesAsync();
                var document = new Document()
                {
                    Name = model.IdentificationDocument.FileName,
                    Path = filePath,
                    UserID = user.Id,
                    FileExtension = Path.GetExtension(model.IdentificationDocument.FileName)
                };
                user.Documents.Add(document);
                var res = await context.SaveChangesAsync();
                if(res > 0)
                {
                    SendVerificationMail(user.Email, user.VerificationToken, user.TokenExpiration);
                }
                logger.LogInformation(message: $"Account {model.Email} was successfully created");
                return response.SuccessResultData(user);
            }
            catch (Exception ex)
            {
                logger.LogError(message:  ex.Message);
                return response.FailedResultData(default, ex.Message, ex.HResult);
            }
        }

        public async Task<ResponseDetail<string>> VerifyAccount(string token)
        {
            var response = new ResponseDetail<string>();
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.VerificationToken == token);
                if (user == null)
                {
                    response = response.FailedResultData("Invalid token", 498);

                }
                else if (DateTime.Now > DateTime.Parse(user.TokenExpiration))
                {
                    user.TokenExpiration = "";
                    user.VerificationToken = "";
                    user.VerifiedAt = "Unverified";
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
                    response = response.FailedResultData("Token expired", 498);
                }
                else
                {
                    user.IsVerified = true;
                    user.TokenExpiration = "";
                    user.VerificationToken = "";
                    user.VerifiedAt = DateTime.Now.ToString();
                    context.Users.Update(user);
                    response = response.SuccessResultData("Verification Successful");
                    logger.LogInformation(message: $"Account {user.Email} was successfully verified");

                }
            }
            catch (Exception ex)
            {
                logger.LogError(message: $"Account {ex.Message} was successfully created");
                return response.FailedResultData(default, ex.Message, ex.HResult);
            }
            return response;
        }

        public void SendVerificationMail(string email,string body, string extra)
        {
            var regMail = new MailRequest
            {
                Receiver = email,
                Subject = "Verify Your Account",
                Body = $"<p>Please verify your account with this token <b>{body}</b>...<br/>It expires in {extra ?? TimeSpan.FromMinutes(3).ToString()}, minute</p>"
            };
            mailService.SendMail(regMail);            
        }
        public async Task<ResponseDetail<string>> ResendSendVerificationToken(string email)
        {
            var response = new ResponseDetail<string>();
            try
            {
                var user = context.Users.Where(x => x.Email == email);

                if (user is null)
                {
                    response = response.FailedResultData("Invalid Email");
                }
                else
                {
                    var token = Util.CreateRandomVerificationToken().Substring(1, 8);
                    var tokenExpiration = TimeSpan.FromMinutes(3).ToString();
                  
                    var updateUserToken = await user.ExecuteUpdateAsync(p => p.SetProperty(prop => prop.VerificationToken, token)
                                                                              .SetProperty(prop => prop.TokenExpiration, tokenExpiration));

                    if (updateUserToken > 0)
                    {
                        var regMail = new MailRequest
                        {
                            Receiver = email,
                            Subject = "Verify Your Account",
                            Body = $"<p>Please verify your account with this token <b>{token}</b>...<br/>It expires in {tokenExpiration}, minute</p>"
                        };
                        await mailService.SendMail(regMail);
                        response = response.SuccessResultData($"Token:{token}","Token successfully sent");
                    }
                    else
                    {
                        response = response.FailedResultData("An error occured");
                    }
                  
                }
               
            }
            catch(Exception ex)
            {
                return response.FailedResultData(ex.Message, ex.HResult);
            }
            return response;

        }
    }
}