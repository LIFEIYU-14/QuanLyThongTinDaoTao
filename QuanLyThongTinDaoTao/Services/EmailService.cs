using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
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
        public async Task SendTeacherAccountWithQrEmail(string toEmail, string taiKhoan, string matKhau, string qrCode)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(smtpUser);
                    mail.To.Add(toEmail);
                    mail.Subject = "Thông tin tài khoản và QR Code giảng viên";

                    // Tạo nội dung email HTML với thông tin tài khoản và QR code
                    mail.Body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #f8f9fa; padding: 15px; text-align: center; }}
                        .content {{ padding: 20px; }}
                        .info-card {{ background-color: #f1f8ff; border-left: 4px solid #007bff; padding: 15px; margin-bottom: 20px; }}
                        .qr-section {{ text-align: center; margin: 20px 0; }}
                        .footer {{ margin-top: 20px; font-size: 0.9em; color: #6c757d; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>Thông Tin Tài Khoản Giảng Viên</h2>
                        </div>
                        
                        <div class='content'>
                            <p>Chào bạn,</p>
                            <p>Hệ thống đã tạo tài khoản giảng viên cho bạn với thông tin sau:</p>
                            
                            <div class='info-card'>
                                <p><strong>Tài khoản:</strong> {taiKhoan}</p>
                                <p><strong>Mật khẩu:</strong> {matKhau}</p>
                            </div>
                            
                            <p>Vui lòng đăng nhập và đổi mật khẩu sau lần đăng nhập đầu tiên.</p>
                            
                            <div class='qr-section'>
                                <h3>Mã QR Code Điểm Danh</h3>
                                <p>Sử dụng mã QR này để điểm danh khi giảng dạy:</p>
                                <img src='cid:qrCodeImage' alt='QR Code' style='max-width: 200px;' />
                                <p><small>Quét mã này bằng ứng dụng điểm danh của hệ thống</small></p>
                            </div>
                        </div>
                        
                        <div class='footer'>
                            <p>Đây là email tự động, vui lòng không trả lời.</p>
                        </div>
                    </div>
                </body>
                </html>";

                    mail.IsBodyHtml = true;

                    // Đính kèm QR code như một embedded image
                    if (!string.IsNullOrEmpty(qrCode))
                    {
                        byte[] qrCodeBytes = Convert.FromBase64String(qrCode);
                        using (var ms = new MemoryStream(qrCodeBytes))
                        {
                            var qrImage = new Attachment(ms, "QRCode_GV.png", "image/png");
                            qrImage.ContentId = "qrCodeImage"; // ID để tham chiếu trong HTML
                            qrImage.ContentDisposition.Inline = true;
                            mail.Attachments.Add(qrImage);
                        }
                    }

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
                Console.WriteLine($"Lỗi gửi email thông tin tài khoản và QR code: {ex.Message}");
                throw;
            }
        }

    }
}
