using Live_Document___Rich_Text_Editor.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Live_Document___Rich_Text_Editor.Controllers
{
    public class HomeController : Controller
    {
        public static UserModel User { get; set; }
        public static DocumentModel document { get; set; }
        public static DocumentEntityModel Specificdocument { get; set; }
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult login(UserModel user)
        {

            if (user.verifyUser())
            {
                try
                {
                    User = user;
                    string token = CreateToken();
                    //Response.Headers.Add("Authorization", "Bearer "+ token);
                    Response.Cookies.Append("auth_token", token);
                    return RedirectToAction("dashboard");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Login Post Controller "+ex.Message);
                }
            }
            return View();
        }

        [HttpPost]
        public string CreateToken()
        {
            try
            {
                List<Claim> claim = new List<Claim>() {
            new Claim(ClaimTypes.Name, User.username),
            new Claim(ClaimTypes.Sid,User.id),
            new Claim(ClaimTypes.MobilePhone,User.phone)
            };
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value
                    ));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var token = new JwtSecurityToken(
                    claims: claim,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: cred
                    );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return jwt;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Create token Controller"+ex.Message);
            }
            return "";
            
        }


        [HttpGet]
        public IActionResult register()
        {

            return View();
        }

        [HttpPost]
        public IActionResult register(UserModel user)
        {

            if(user.registerUser(Request.Form["username"], Request.Form["Password"], Request.Form["phone"], Request.Form["email"]))
            {
                return login(user);
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult Submit()
        {
            try
            {
                DocumentModel model = new DocumentModel();
                var user = User;

                Console.WriteLine("Submit Controller" + Request.Form["DocumentTitle"]);
                model.SaveDocument(Convert.ToInt32(user.id), Request.Form["DocumentTitle"], Request.Form["Content"]);
               
            }
            catch(Exception ex)
            {
                Console.WriteLine("Submit Controller Exception "+ex.Message);
            }
            return RedirectToAction("dashboard");
        }
        private UserModel getCurrentUser()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                {
                    string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                    // Do something with the token
                    Console.WriteLine(token);
                }
                if (identity != null)
                {
                    var userClaims = identity.Claims;
                    string userid = (userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Sid)?.Value);
                    string username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value;
                    Console.WriteLine("Get curr user "+userid+" name "+username);
                    return new UserModel
                    {
                        username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                        id = (userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Sid)?.Value),
                        phone = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.MobilePhone)?.Value
                    };
                }
            }
            catch(Exception ex )
            {
                Console.WriteLine("Get current user controller "+ex.Message);
            }
            return null;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult dashboard()
        {
            try
            {
                UserModel user = getCurrentUser();
                ViewBag.user = user;
                document = new DocumentModel();
                document.getDocumentList(Convert.ToInt32(user.id));
                ViewBag.documents = document.DocumentList;

            }
            catch(Exception ex)
            {
                Console.WriteLine("Dashboard Controller "+ex.Message);
            }
            return View();
        }

        [HttpGet]
        public IActionResult ViewDocs(int docId)
        {
            try
            {
                Console.WriteLine("Document Id " + docId + " User " + User.id);
                var specificDocument = document.getSpecificDocument(docId, Convert.ToInt32(User.id));
                var documentList = new List<DocumentEntityModel> { specificDocument };
                return View(documentList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("View Docs Controller " + ex.Message);
                return RedirectToAction("dashboard", "Home");
            }
        }

        [HttpGet]
        public IActionResult EditDocs(int docId)
        {
            try
            {
                Console.WriteLine("Document Id " + docId + " User " + User.id);
                DocumentModel model = new DocumentModel();
                var document = model.getSpecificDocument(docId, Convert.ToInt32(User.id));
                return View(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Edit Docs Get Catch "+ex.Message);
            }
            return RedirectToAction("dashboard", "Home");
        }

        [HttpPost]
        public IActionResult EditDocs()
        {
            int docId = Convert.ToInt32(Request.Form["docId"]);
            string docTitle = Request.Form["DocumentTitle"];
            string Content = Request.Form["Content"];   
            try
            {
                Console.WriteLine("Edit Document Id " + Request.Form["docId"] + " User " + User.id);
                if(document.UpdateDocument(docId,docTitle,Content, Convert.ToInt32(User.id)))
                {
                    Console.WriteLine("Update Success");
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Edit Docs Controller " + ex.Message);
                
            }
            return RedirectToAction("dashboard", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}