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
                User = user;
                string token = CreateToken();
                //Response.Headers.Add("Authorization", "Bearer "+ token);
                Response.Cookies.Append("auth_token", token);
                return RedirectToAction("dashboard");
            }
            return View();
        }

        [HttpPost]
        public string CreateToken()
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


        [HttpGet]
        public IActionResult register()
        {

            return View();
        }

        private UserModel getCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Console.WriteLine(identity);

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
                return new UserModel
                {
                    username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                    id = (userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Sid)?.Value),
                    phone = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.MobilePhone)?.Value
                };
            }
            return null;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult dashboard()
        {
            UserModel user = getCurrentUser();
            ViewBag.user = user;
            DocumentModel document = new DocumentModel();
            document.getDocumentList(Convert.ToInt32(user.id));
            ViewBag.documents = document.DocumentList;

            return View();
        }

       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}