using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using SqlConnectionStringBuilder = System.Data.SqlClient.SqlConnectionStringBuilder;
using SqlConnection = System.Data.SqlClient.SqlConnection;
using SqlCommand = System.Data.SqlClient.SqlCommand;
using SqlDataReader = System.Data.SqlClient.SqlDataReader;
using SqlException = System.Data.SqlClient.SqlException;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private WebAppDBContext dbContext { get; set; }
        private Microsoft.Data.SqlClient.SqlConnection sqlConnection { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDbConnection>(provider =>
            new SqlConnection(Configuration.GetConnectionString("AzureConnection")));

            services.AddDbContext<WebAppDBContext>(options =>
            {
               // SqlAuthenticationProvider.SetProvider(SqlAuthenticationMethod.ActiveDirectoryDeviceCodeFlow, new CustomAzureSQLAuthProvider());
                var connStr = Configuration.GetConnectionString("AzureConnection");
                sqlConnection = new Microsoft.Data.SqlClient.SqlConnection(connStr);
                options.UseSqlServer(sqlConnection);
    
            });
            TestDB(services);
            TestDB2(services);


            services.AddRazorPages();
        }

        private void TestDB(IServiceCollection services)
        {
            var connStr = Configuration.GetConnectionString("AzureConnection");
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            dbContext = scope.ServiceProvider.GetService<WebAppDBContext>();

            try
            {
                Console.WriteLine("\nTesting DB Context:");
                Console.WriteLine("=========================================\n");
                dbContext.Database.OpenConnection();
                Console.WriteLine(dbContext.Database.GetDbConnection());
                Console.WriteLine(dbContext.Database.CanConnect());
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }

        private void TestDB2(IServiceCollection services) {
            {
                try
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("AzureConnection"));

                   // builder.DataSource = "<your_server.database.windows.net>";
                   //builder.UserID = "<your_username>";
                   //builder.Password = "<your_password>";
                   // builder.InitialCatalog = "<your_database>";

                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        Console.WriteLine("\n Query data example:");
                        Console.WriteLine("=========================================\n");

                        connection.Open();

                        String sql = "SELECT name, collation_name FROM sys.databases";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                                }
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
               
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

            });
        }
    }
}
