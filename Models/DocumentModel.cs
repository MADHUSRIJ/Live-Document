using Azure.Core;
using Microsoft.Data.SqlClient;
using System;

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
                    model.DocumentId = (int)reader["DocumentId"];
                    model.DocumentTitle = (string)reader["DocumentTitle"];
                    model.Content = (string)reader["Content"];
                    model.CreatedOn = (DateTime)reader["CreatedAt"];
                    model.LastEdited = (DateTime)reader["LastEdited"];

                    DocumentList.Add(model);
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);

            }


        }

        public void SaveDocument(int UserId,string title,string content)
        {
            Console.WriteLine("Save Document model " + content+" User "+UserId);
            SqlConnection conn = new SqlConnection("Data Source=5CG9441HWP;Initial Catalog=TextEditor;Integrated Security=True;Encrypt=False;");
            conn.Open();
            SqlCommand command = conn.CreateCommand();
            try
            {
                Console.WriteLine("1");
                command.CommandText = "INSERT INTO DOCUMENTS(DocumentTitle, Content, CreatedAt, LastEdited, UserId) " +
                      "VALUES (@title, @content, @createdAt, @lastEdited, @userId)";
                Console.WriteLine("2");
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@content", content);
                command.Parameters.AddWithValue("@createdAt", DateTime.Now);
                command.Parameters.AddWithValue("@lastEdited", DateTime.Now);
                command.Parameters.AddWithValue("@userId", UserId);

                Console.WriteLine("3");

                int response = command.ExecuteNonQuery();
                Console.WriteLine("4");

                if (response > 0)
                {
                    Console.WriteLine("Document Saved");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Save document model try "+e.Message);
            }
        }

        public DocumentEntityModel getSpecificDocument(int docId,int UserId)
        {
            DocumentEntityModel model = new DocumentEntityModel();
            try
            {

                SqlConnection conn = new SqlConnection("Data Source=5CG9441HWP;Initial Catalog=TextEditor;Integrated Security=True;Encrypt=False;");
                conn.Open();
                SqlCommand command = conn.CreateCommand();
                command.CommandText = $"SELECT * FROM DOCUMENTS WHERE DocumentId= {docId} AND UserId = {UserId}";

                SqlDataReader reader = command.ExecuteReader();
                
                if (reader.Read())
                {
                    
                    model.DocumentId = (int)reader["DocumentId"];
                    model.DocumentTitle = (string)reader["DocumentTitle"];
                    model.Content = (string)reader["Content"];
                    model.CreatedOn = (DateTime)reader["CreatedAt"];
                    model.LastEdited = (DateTime)reader["LastEdited"];

                    return model;
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);

            }

            return model;
        }
    }
}
