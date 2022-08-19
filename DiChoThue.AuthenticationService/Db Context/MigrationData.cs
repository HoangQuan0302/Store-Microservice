using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using System;

namespace DiChoThue.AuthenticationService.Db_Context
{
    public static class MigrationData
    {
        public static IHost MigrateDatabase<TContext>(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postresql database.");

                    var retry = Policy.Handle<NpgsqlException>()
                            .WaitAndRetry(
                                retryCount: 5,
                                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2,4,8,16,32 sc
                                onRetry: (exception, retryCount, context) =>
                                {
                                    logger.LogError($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                                });

                    //if the postgresql server container is not created on run docker compose this
                    //migration can't fail for network related exception. The retry options for database operations
                    //apply to transient exceptions                    
                    retry.Execute(() => ExecuteMigrations(configuration));

                    logger.LogInformation("Migrated postresql database.");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the postresql database");
                }
            }

            return host;
        }

        private static void ExecuteMigrations(IConfiguration configuration)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            connection.Open();

            using var command = new NpgsqlCommand
            {
                Connection = connection
            };


            command.CommandText = @"
                                                                CREATE TABLE  IF NOT EXISTS UserAccount ( 
                                                                UserId int PRIMARY KEY GENERATED ALWAYS AS IDENTITY, 
                                                                EmailAddress TEXT,
                                                                Password TEXT,
                                                                Source TEXT,
                                                                FirstName TEXT,
                                                                MiddleName TEXT,
                                                                LastName TEXT,
                                                                RoleId int,
                                                                HireDate Date
                                                                );       
 
                                                                CREATE TABLE  IF NOT EXISTS Role ( 
                                                                RoleId int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                                                                RoleDesc TEXT
                                                                );

                                                                CREATE TABLE  IF NOT EXISTS RefreshToken(
                                                                TokenId int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                                                                UserId int,
                                                                Token TEXT,
                                                                ExpiryDate Date
                                                                )
                                                                ";
            command.ExecuteNonQuery();


            command.CommandText = @"INSERT INTO Role (RoleDesc) 
                                                                    VALUES
                                                                    ('Admin'),
                                                                    ('Store'),
                                                                    ('Customer'),
                                                                    ('Shipper')
                                                                   ";
            command.ExecuteNonQuery();
        }
    }
}


                                                                