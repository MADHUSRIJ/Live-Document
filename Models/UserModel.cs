using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Data;

namespace Live_Document___Rich_Text_Editor

{
    public class UserModel
    {
        

        public string id { get; set; }
        public string phone { get; set; }
        public string Password { get; set; }
        public string username { get; set; }
        public string email { get; set; }   

        public bool verifyUser() {
            string sqlConnectionString = "Data Source=localhost;Initial Catalog=TextEditor;Integrated Security=True;Encrypt=False";


            try {
                using (SqlConnection connection = new SqlConnection(sqlConnectionString)) {

                    SqlCommand cmd = new SqlCommand($"select * from Users where username='{this.username}' and password= '{this.Password}'"
                        , connection);
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        this.id = Convert.ToString(reader["UserId"]);
                        this.username =(string) reader["UserName"];
                        this.phone = (string)reader["Mobile"];
                        this.email = (string)reader["Email"];

                        // Erasing the password.
                        this.Password = "";
                        return true;
                    }
                }
            }

            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

    }


}
