using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace RedisApp.Controllers
{

    public class AccountController : Controller
    {
        
        private static Dictionary<String, String> UsernameAndPasswords = new Dictionary<String, String>();
        private static Dictionary<String, String> UsernameAndSessionID = new Dictionary<String, String>();

        private bool CookieCheck(String name,HttpContext Context,HttpRequest Request,HttpResponse Response){
            if (Request.Cookies["MySessionCookie"] == null) 
            {
                return false;
            }else{
                if(HttpContext.Session==null) 
                {   
                    Response.Cookies.Delete("MySessionCookie");
                    return false;
                }
                else{
                    String username = HttpContext.Session.GetString("username");

                    if(username==null)
                    {
                        return false;
                    }
                    else {
                        try
                            {
                                if ((UsernameAndSessionID[username]).Equals(HttpContext.Session.Id))
                                {
                                    return true;
                                }
                                else
                                {
                                    HttpContext.Session.Clear();
                                    Response.Cookies.Delete("MySessionCookie");
                                    return false;
                                }
                            }
                            catch (KeyNotFoundException)
                            {
                                return false;
                            }
                    }
                }
            }
        }

        private void LogToFile(String text)
        {
            string path = "/Users/geelaputru.k/Projects/RedisApp2/RedisApp2/Files/sample.txt";
        
            using (StreamWriter sw = System.IO.File.AppendText(path))
            {
                sw.WriteLine(text);
                
            }
        }

        [HttpGet("/register")]
        public IActionResult GetRegister()
        {
            if(CookieCheck("GetRegister",HttpContext,Request,Response)==true) return LocalRedirect("/welcome");

            ViewBag.Title = "Register";
            return View("Views/Account/register.cshtml");

        }

        [HttpPost("/register")]
        public LocalRedirectResult PostRegister()
        {
            if(CookieCheck("PostRegister",HttpContext,Request,Response)==true) return LocalRedirect("/welcome");

            String username = HttpContext.Request.Form["username"];
            String password = HttpContext.Request.Form["password"];

            if(UsernameAndPasswords.ContainsKey(username)) return LocalRedirect("/register");

            UsernameAndPasswords.Add(username, password);

            HttpContext.Session.SetString("username", username);
            UsernameAndSessionID.Add(username, HttpContext.Session.Id);

            return LocalRedirect("/login");

        }


        [HttpGet("/login")]
        public IActionResult GetLogIn()
        {
            if(CookieCheck("GetLogin",HttpContext,Request,Response)==true) return LocalRedirect("/welcome");
            
            ViewBag.Title = "Login";
            return View("Views/Account/login.cshtml");

        }

        [HttpPost("/login")]
        public LocalRedirectResult PostLogIn()
        {
            if(CookieCheck("PostLogin",HttpContext,Request,Response)==true) return LocalRedirect("/welcome");

            String username = HttpContext.Request.Form["username"];
            String password = HttpContext.Request.Form["password"];

            try
            {
                if (UsernameAndPasswords[username]==password)
                {
                    HttpContext.Session.SetString("username", username);
                    UsernameAndSessionID.Add(username,HttpContext.Session.Id);
                    
                    return LocalRedirect("/welcome");
                }
                else
                {
                    return LocalRedirect("/login");
                }

            }catch(KeyNotFoundException)
            {
                return LocalRedirect("/login");

            }
        }

        [HttpGet("/welcome")]
        public IActionResult Welcome()
        {
            if(CookieCheck("Welciome",HttpContext,Request,Response)==false) return LocalRedirect("/login");
            
            String username = HttpContext.Session.GetString("username");

            ViewBag.username = username;
            return View("Views/Account/welcome.cshtml");
            
        }

        [HttpPost("/logout")]
        public LocalRedirectResult Logout()
        {
            if(CookieCheck("Logout",HttpContext,Request,Response)==false) return LocalRedirect("/login");
            
            UsernameAndSessionID.Remove(HttpContext.Session.GetString("username"));
           
            Response.Cookies.Delete("MySessionCookie") ;
            HttpContext.Session.Clear();

            return LocalRedirect("/login");

        }

    }
}
