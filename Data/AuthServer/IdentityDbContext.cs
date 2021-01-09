using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthServer = auth_server.Data.AuthServer;

namespace auth_server.Data.AuthServer
{
    public class IdentityDbContext : IdentityDbContext<IdentityServerUser>
    {
        public IdentityDbContext(DbContextOptions<AuthServer.IdentityDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
