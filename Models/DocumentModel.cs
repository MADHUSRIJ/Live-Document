﻿using Microsoft.Data.SqlClient;

namespace Live_Document___Rich_Text_Editor.Models
{
    public class DocumentModel
    {
        public List<DocumentEntityModel> DocumentList = new List<DocumentEntityModel>();
        public void getDocumentList(int UserId)
        {
            try
            {
             
                    SqlConnection conn = new SqlConnection("Data Source=5CG9441HWP;Initial Catalog=TextEditor;Integrated Security=True;Encrypt=False;");
                conn.Open();
                SqlCommand command = conn.CreateCommand();
                command.CommandText = $"SELECT * FROM DOCUMENTS WHERE UserId = {UserId}";

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DocumentEntityModel model = new DocumentEntityModel();

                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);

            }
        }
    }
}