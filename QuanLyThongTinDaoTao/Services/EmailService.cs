using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace QuanLyThongTinDaoTao.Services
{
    public class EmailService
    {
        private readonly string smtpHost = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
        private readonly int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
        private readonly string smtpUser = ConfigurationManager.AppSettings["EmailUsername"] ?? "tn216003@gmail.com";
        private readonly string smtpPass = ConfigurationManager.AppSettings["EmailPassword"] ?? "yltr uivx kovk owwc";

        public async Task SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(smtpUser, "Hệ Thống Quản Lý Đào Tạo");
                    mail.To.Add(toEmail);
                    mail.Subject = "Mã xác nhận đăng ký lớp học";
                    mail.Body = $"<p>Mã xác nhận của bạn là: <strong>{otp}</strong></p>";
                    mail.IsBodyHtml = true;

                    using (var smtp = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"Lỗi SMTP: {smtpEx.StatusCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                throw;
            }
        }

        public async Task SendQrCodeEmail(string toEmail, string qrCode)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(smtpUser, "Hệ Thống Quản Lý Đào Tạo");
                    mail.To.Add(toEmail);
                    mail.Subject = "Mã QR Code điểm danh";
                    mail.Body = "<p>Đây là mã QR Code điểm danh của bạn:</p>";

                    // Lưu ảnh QR vào một tệp tạm thời
                    byte[] qrCodeBytes = Convert.FromBase64String(qrCode);
                    using (var ms = new MemoryStream(qrCodeBytes))
                    {
                        // Đính kèm mã QR như một file ảnh
                        var attachment = new Attachment(ms, "QRCode.png", "image/png");
                        mail.Attachments.Add(attachment);

                        using (var smtp = new SmtpClient(smtpHost, smtpPort))
                        {
                            smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                            smtp.EnableSsl = true;
                            await smtp.SendMailAsync(mail);
                        }
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"Lỗi SMTP: {smtpEx.StatusCode}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                throw;
            }
        }
        public Task SendTeacherAccountWithQrEmail(string toEmail, string taiKhoan, string matKhau, string qrCodeBase64)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(smtpUser, "Hệ Thống Quản Lý Đào Tạo");
                    mail.To.Add(toEmail);
                    mail.Subject = "Thông tin tài khoản và mã QR điểm danh";

                    var htmlBody = $@"
                <p>Chào bạn,</p>
                <p>Tài khoản giảng viên của bạn đã được tạo:</p>
                <ul>
                    <li><strong>Tài khoản:</strong> {taiKhoan}</li>
                    <li><strong>Mật khẩu:</strong> {matKhau}</li>
                </ul>
                <p>Đây là mã QR dùng để điểm danh:</p>
                <p><img src='cid:qrCodeImage' alt='QR Code' /></p>";

                    var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

                    // Xử lý nếu chuỗi có "data:image/png;base64,"
                    if (qrCodeBase64.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                    {
                        qrCodeBase64 = qrCodeBase64.Substring(qrCodeBase64.IndexOf(",") + 1);
                    }

                    byte[] qrBytes = Convert.FromBase64String(qrCodeBase64);

                    // Quan trọng: tạo stream rồi giữ tham chiếu
                    MemoryStream stream = new MemoryStream(qrBytes);

                    var linkedResource = new LinkedResource(stream, MediaTypeNames.Image.Jpeg) // hoặc "image/png"
                    {
                        ContentId = "qrCodeImage",
                        TransferEncoding = TransferEncoding.Base64
                    };

                    alternateView.LinkedResources.Add(linkedResource);
                    mail.AlternateViews.Add(alternateView);
                    mail.IsBodyHtml = true;

                    using (var smtp = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                        smtp.EnableSsl = true;

                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email error: " + ex.ToString());
                throw;
            }

            return Task.CompletedTask;
        }
        public async Task SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var fromAddress = new MailAddress(smtpUser, "Hệ Thống Quản Lý Đào Tạo");
                var toAddress = new MailAddress(toEmail);

                using (var smtp = new SmtpClient(smtpHost, smtpPort))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(fromAddress.Address, smtpPass);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Timeout = 20000;

                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    })
                    {
                        await smtp.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý tùy ý
                System.Diagnostics.Debug.WriteLine("Email error: " + ex);
                throw;
            }
        }
     

    }
}
