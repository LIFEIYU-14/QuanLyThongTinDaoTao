﻿using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;
using System.IO;

namespace QuanLyThongTinDaoTao.Services
{
    public class EmailService
    {
        public static void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                string fromEmail = ConfigurationManager.AppSettings["EmailUsername"];
                string fromPassword = ConfigurationManager.AppSettings["EmailPassword"];

                var fromAddress = new MailAddress(fromEmail, "Hệ Thống Đào Tạo");
                var toAddress = new MailAddress(toEmail);

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi gửi email: " + ex.Message);
            }
        }

    }
}
