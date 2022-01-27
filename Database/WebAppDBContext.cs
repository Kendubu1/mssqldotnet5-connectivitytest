using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace WebApplication.Database
{
    public class WebAppDBContext : DbContext
    {
        public WebAppDBContext(DbContextOptions<WebAppDBContext> options) : base(options)
        {
        }
    }
    
}
