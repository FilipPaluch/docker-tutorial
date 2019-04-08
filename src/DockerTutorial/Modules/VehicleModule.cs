using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using DockerTutorial.Infrastructure.Config;
using DockerTutorial.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Microsoft.AspNetCore.Mvc;

namespace DockerTutorial.Modules
{
    [ApiController]
    [Route("vehicles")]
    public class VehicleModule : ControllerBase
    {
        private readonly string _connectionString;

        public VehicleModule(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("Database").Get<DatabaseConfig>().ConnectionString;
        }

        [HttpGet("get-all")]
        public async Task<IEnumerable<Vehicle>> GetVehicles()
        {
            using (var dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                return await dbConnection.QueryAsync<Vehicle>($"SELECT * FROM vehicle");
            }
        }

        [HttpPost("add-vehicle")]
        public async Task AddVehicle([FromBody]Vehicle vehicle)
        {
            using (var dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                await dbConnection.QueryAsync($"INSERT INTO vehicle (Brand, Model) VALUES ( @Brand, @Model )", vehicle);
            }
        }
    }
}
