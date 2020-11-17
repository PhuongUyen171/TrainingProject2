using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using UI.Models;
using Model;
using Model.Common;
using WebMatrix.WebData;
using System.Net.Mail;
using System.Net;
using System.Net.Http.Formatting;

namespace UI.Controllers
{
    public class UserController : Controller
    {
        string url;
        ServiceRepository serviceObj;
        public UserController()
        {
            serviceObj = new ServiceRepository();
            url = "https://localhost:44379/api/User_API/";
        }
        #region quy trình đăng nhập user
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = serviceObj.GetResponse(url + "GetLoginResultByUsernamePassword?user=" + model.UserName + "&pass=" + model.Password);
                response.EnsureSuccessStatusCode();
                int resultLogin = response.Content.ReadAsAsync<int>().Result;
                switch (resultLogin)
                {
                    case 1:
                        {
                            HttpResponseMessage responseUser = serviceObj.GetResponse(url + "GetCustomerByUsername?user=" + model.UserName);
                            responseUser.EnsureSuccessStatusCode();
                            CustomerModel customLogin = responseUser.Content.ReadAsAsync<CustomerModel>().Result;

                            var userSession = new UserLogin();
                            userSession.UserName = customLogin.Username;
                            userSession.Password = customLogin.Pass;

                            //Chưa xử lý quên mật khẩu
                            Session.Add(Constants.USER_SESSION, userSession);
                            //if (model.RememberMe)
                            //{
                            //    HttpCookie ckUser = new HttpCookie("username");
                            //    ckUser.Expires = DateTime.Now.AddHours(48);
                            //    ckUser.Value = model.UserName;
                            //    Response.Cookies.Add(ckUser);
                            //    HttpCookie ckPass = new HttpCookie("password");
                            //    ckPass.Expires = DateTime.Now.AddHours(48);
                            //    ckPass.Value = model.Password;
                            //    Response.Cookies.Add(ckPass);
                            //}
                            return RedirectToAction("Index", "User");
                        }
                    case 0:
                        ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không tồn tại.");
                        break;
                    default:
                        ModelState.AddModelError("", "Đăng nhập thất bại.");
                        break;
                }
            }
            return this.View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                string resetCode = Guid.NewGuid().ToString();
                var verifyUrl = "/User/ResetPassword?id=" + resetCode+"&mail="+model.Email;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                HttpResponseMessage response = serviceObj.GetResponse(url + "GetCustomerByEmail?mail=" + model.Email);
                response.EnsureSuccessStatusCode();
                CustomerModel result = response.Content.ReadAsAsync<CustomerModel>().Result;
                if (result != null)
                {
                    ResetPasswordModel resetModel = new ResetPasswordModel();
                    resetModel.ResetCode = resetCode;
                    resetModel.Id = result.CustomID;
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property
                    var subject = "Password Reset Request";
                    var body = "Hi " + result.CustomName + ", <br/> You recently requested to reset your password for your account. Click the link below to reset it. " +
                            " <br/><br/><a href='" + link + "'>" + link + "</a> <br/><br/>" +
                            "If you did not request a password reset, please ignore this email or reply to let us know.<br/><br/> Thank you";
                    SendEmail(result.Email, body, subject);
                    ModelState.AddModelError("", "Mã code đã được gửi vào email của bạn");
                }
                else
                    ModelState.AddModelError("", "Địa chỉ email không tồn tại.") ;
                return View();
            }
            return View();
        }
        private void SendEmail(string emailAddress, string body, string subject)
        {
            using (MailMessage mm = new MailMessage("mlem1701@gmail.com", emailAddress))
            {
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("mlem1701@gmail.com", "ntpu1234");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }
        public CustomerModel GetCustomerByEmail(string mail)
        {
            HttpResponseMessage response = serviceObj.GetResponse(url + "GetCustomerByEmail?mail=" + mail);
            response.EnsureSuccessStatusCode();
            CustomerModel result = response.Content.ReadAsAsync<CustomerModel>().Result;
            return result;
        }
        public ActionResult ResetPassword(string id,string mail)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrEmpty(mail))
            {
                return View("~/Views/Shared/404.cshtml");
            }

            //using (var context = new LoginRegistrationInMVCEntities())
            //{
            //var user = context.RegisterUsers.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
            //if (user != null)
            //{
            //HttpResponseMessage response = serviceObj.GetResponse(url + "GetCustomerByEmail?mail=" + mail);
            //response.EnsureSuccessStatusCode();
            CustomerModel result = GetCustomerByEmail(mail);
            ResetPasswordModel mode = new ResetPasswordModel();
            mode.Id = result.CustomID;
            mode.Mail = mail;
            mode.ResetCode = id;
            return View(mode);
                //}
                //else
                //{
                //     return View("~/Views/Shared/404.cshtml");
                //}
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            CustomerModel resultReset = GetCustomerByEmail(model.Mail);
            resultReset.Pass =  model.NewPassword;
            HttpResponseMessage responseUpdate = serviceObj.PutResponse(url + "UpdateCustomer", resultReset);
            responseUpdate.EnsureSuccessStatusCode();
            bool result= responseUpdate.Content.ReadAsAsync<bool>().Result;
            if (result)
                return RedirectToAction("Login");
            ViewBag.Warning = "Có lỗi xảy ra trong quá trình đặt lại mật khẩu.";
            return this.View();
        }
        #endregion
        public ActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signin(RegisterModel model)
        {
            return this.View();
        }
        
    }
}