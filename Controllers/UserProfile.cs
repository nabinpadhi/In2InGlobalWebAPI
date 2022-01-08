using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using Microsoft.Extensions.Configuration;
using Npgsql;
namespace In2InGlobalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfile : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private IConfiguration Configuration;
        private readonly string _connectionString;
        public UserProfile(IWebHostEnvironment env, IConfiguration _configuration)
        {
            _env = env;
            Configuration = _configuration;
            _connectionString = this.Configuration.GetConnectionString("In2InDBConnection");
        }
        [HttpGet(Name = "GetUserID")]
        public String ShowUserDeatils(string email)
        {
            DataSet dsMyProfile = new DataSet();
            NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter();
            string query = @"SELECT * FROM dbo.getprofiledetails(@email)";
            using (var connection = GetDBConnection())
            {
                try
                {
                    connection.Open();
                    NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, connection);
                    npgsqlCommand.Parameters.AddWithValue("@email", email);
                    npgsqlCommand.CommandType = CommandType.Text;
                    npgsqlDataAdapter.SelectCommand = npgsqlCommand;
                    npgsqlDataAdapter.Fill(dsMyProfile);

                    return JsonConvert.SerializeObject(dsMyProfile.Tables[0]).ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Dispose();
                    npgsqlDataAdapter.Dispose();
                }

            }
        }
        private NpgsqlConnection GetDBConnection()
        {
            var databaseConnection = new NpgsqlConnection(_connectionString);
            return databaseConnection;

        }
    }
}
