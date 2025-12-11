using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartMentor.Persistence.Identity;


namespace SmartMentor.Persistence.Data
{
    public  class DataSeeder
    {
        private readonly UserManager<ApplicationUser> usermanger;
        private readonly RoleManager<ApplicationRole> rolemanager;
        private readonly ILogger<DataSeeder> logger;

        public DataSeeder(UserManager<ApplicationUser> usermanger,
            RoleManager<ApplicationRole> rolemanager, 
            ILogger<DataSeeder> logger)
        {
            this.usermanger = usermanger;
            this.rolemanager = rolemanager;
            this.logger = logger;
        }
        public  async Task SeedRolesAndUsersAsync(IServiceProvider service)
        {
            logger.LogInformation("Starting identity data seeding: ensuring essential roles and sample users exist.");
            try
            {
                var roles = new List<ApplicationRole>
                {
                    new ApplicationRole(){Name="Student",Description="Student Role "},
                    new ApplicationRole(){Name="Mentor",Description="Mentor the Students"},
                    new ApplicationRole(){Name="Admin",Description="Moderate the App"}
                };
                foreach (var role in roles) {
                    // CHeck if there is any roles added in the database 
                    var rolename = role.Name??string.Empty;
                    logger.LogDebug("Checking existence of role '{RoleName}'", rolename);
                    if (!await rolemanager.RoleExistsAsync(rolename))
                    {
                        logger.LogInformation("Role '{RoleName}' not found. Attempting to create role with description: '{Description}'", rolename, role.Description);
                        var IsRoleCreated= await rolemanager.CreateAsync(new ApplicationRole() { Name=role.Name,Description=role.Description});
                        if(IsRoleCreated.Succeeded)
                        {
                            logger.LogInformation("Successfully created role '{RoleName}'", rolename);
                        }
                        else
                        {
                            var errors = string.Join(", ", IsRoleCreated.Errors.Select(e => e.Description));
                            logger.LogError("Failed to create role '{RoleName}'. Errors: {Errors}", rolename, errors);
                        }
                    }
                    else
                    {
                        logger.LogDebug("Role '{RoleName}' already exists in store", rolename);
                    }
                }
                // after Creating the Roles now we need to seed the MockUpData
                var Studentfirstname = "Ahmed";
                var Studentlastname = "Abdelnaser";
                var Studentemail = "Ahmed@gmail.com";
                var Studentphonenumber = "0123456789";
                var StudentPassword = "Ahmed@gmail.com";
                if(await usermanger.FindByEmailAsync(Studentemail) == null)
                {
                    logger.LogDebug("Sample Student user not found. Preparing to create sample Student: FirstName={FirstName}, LastName={LastName}, Email={Email}, Phone={Phone}", Studentfirstname, Studentlastname, Studentemail, Studentphonenumber);
                    var Studentuser = new ApplicationUser
                    {
                        FirstName = Studentfirstname,
                        LastName = Studentlastname,
                        Email = Studentemail,
                        PhoneNumber = Studentphonenumber,
                        UserName = "AhmedAbdelnaser",
                        NormalizedUserName="AHMEDABDELNASER"
                    };
                    var IsUserCreated=await usermanger.CreateAsync(Studentuser, StudentPassword);
                    if (IsUserCreated.Succeeded)
                    {
                        await usermanger.AddToRoleAsync(Studentuser, "Student");
                        logger.LogInformation("Created sample Student user '{UserName}' (Email: {Email}) and assigned to role 'Student'.", Studentuser.UserName, Studentuser.Email);
                    }
                    else
                    {
                        var errors = string.Join(",", IsUserCreated.Errors.Select(e => e.Description));
                        logger.LogError("Failed to create sample Student user (Email: {Email}). Errors: {Errors}", Studentemail, errors);
                        throw new Exception("Failed to create sample Student user: " + errors);
                    }

                }
                else
                {
                    logger.LogDebug("Sample Student user already exists with Email: {Email}", Studentemail);
                }
                // now lets seed the Mentor mouckUp data
                
                var mentoremail = "Mentor@gmail.com";
                var mentorpassword = "Mentor@123";

                if (await usermanger.FindByEmailAsync(mentoremail) == null)
                {
                    logger.LogDebug("Sample Mentor user not found. Preparing to create sample Mentor with Email: {Email}", mentoremail);
                    var Mentoruser = new ApplicationUser
                    {
                        FirstName = "Mentor",
                        LastName = "Test",
                        Email = mentoremail,
                        PhoneNumber = "01234567785",
                        UserName = "MentotTestAhmed",
                        NormalizedUserName = "MENTORTESTAHMED"
                    };
                    var IsMentorCreated = await usermanger.CreateAsync(Mentoruser, mentorpassword);
                    if (IsMentorCreated.Succeeded)
                    {
                        await usermanger.AddToRoleAsync(Mentoruser, "Mentor");
                        logger.LogInformation("Created sample Mentor user '{UserName}' (Email: {Email}) and assigned to role 'Mentor'.", Mentoruser.UserName, Mentoruser.Email);
                    }
                    else
                    {
                        var errors = string.Join(",", IsMentorCreated.Errors.Select(e => e.Description));
                        logger.LogError("Failed to create sample Mentor user (Email: {Email}). Errors: {Errors}", mentoremail, errors);
                        throw new Exception("Failed to create sample Mentor user: " + errors);
                    }

                }
                else
                {
                    logger.LogDebug("Sample Mentor user already exists with Email: {Email}", mentoremail);
                }
                // Finally the Admin MockupData
                var adminemail = "Admin@gmail.com";
                var adminpassword = "Admin@123";

                if (await usermanger.FindByEmailAsync(adminemail) == null)
                {
                    logger.LogDebug("Sample Admin user not found. Preparing to create sample Admin with Email: {Email}", adminemail);
                    var Adminuser = new ApplicationUser
                    {
                        FirstName = "Admin",
                        LastName = "Ahmed",
                        Email = adminemail,
                        PhoneNumber = "0123456455",
                        UserName = "AdminAhmed",
                        NormalizedUserName = "ADMINAHMED"
                    };
                    var IsAdminCreated = await usermanger.CreateAsync(Adminuser, adminpassword);
                    if (IsAdminCreated.Succeeded)
                    {
                        await usermanger.AddToRoleAsync(Adminuser, "Admin");
                        logger.LogInformation("Created sample Admin user '{UserName}' (Email: {Email}) and assigned to role 'Admin'.", Adminuser.UserName, Adminuser.Email);
                    }
                    else
                    {
                        var errors = string.Join(",", IsAdminCreated.Errors.Select(e => e.Description));
                        logger.LogError("Failed to create sample Admin user (Email: {Email}). Errors: {Errors}", adminemail, errors);
                        throw new Exception("Failed to create sample Admin user: " + errors);
                    }

                }
                else
                {
                    logger.LogDebug("Sample Admin user already exists with Email: {Email}", adminemail);
                }


            }
            catch (Exception ex) {
                logger.LogError(ex, "An exception occurred during identity seeding. See exception for details.");
                throw;

            }
            finally
            {
                logger.LogInformation("Identity data seeding process finished.");
            }

        }



    }
}
