using Microsoft.AspNetCore.Identity;

 namespace auth_server.Data.AuthServer
{
    public class IdentityServerUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
