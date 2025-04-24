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
                    mail.From = new MailAddress(smtpUser);
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
                    mail.From = new MailAddress(smtpUser);
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


        public async Task SendTeacherAccountWithQrEmail(string toEmail, string taiKhoan, string matKhau, string qrCodeBase64)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(smtpUser);
                    mail.To.Add(toEmail);
                    mail.Subject = "Thông tin tài khoản và mã QR điểm danh";

                    // Tạo AlternateView để hỗ trợ hình ảnh inline
                    var htmlBody = $@"
                <p>Chào bạn,</p>
                <p>Tài khoản giảng viên của bạn đã được tạo:</p>
                <ul>
                    <li><strong>Tài khoản:</strong> {taiKhoan}</li>
                    <li><strong>Mật khẩu:</strong> {matKhau}</li>
                </ul>
                <p>Đây là mã QR dùng để điểm danh:</p>
                <p><img src='cid:qrCodeImage' alt='QR Code' /></p>";

                    var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");

                    // Chuyển base64 thành stream ảnh
                    byte[] qrBytes = Convert.FromBase64String(qrCodeBase64);
                    var stream = new MemoryStream(qrBytes);

                    var linkedResource = new LinkedResource(stream, "image/png")
                    {
                        ContentId = "qrCodeImage",
                        TransferEncoding = System.Net.Mime.TransferEncoding.Base64,
                        ContentType = new ContentType("image/png"),
                        ContentLink = new Uri("cid:qrCodeImage")
                    };

                    alternateView.LinkedResources.Add(linkedResource);

                    mail.AlternateViews.Add(alternateView);

                    mail.IsBodyHtml = true;

                    using (var smtp = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email tài khoản GV: {ex.Message}");
                throw;
            }
        }

    }
}
