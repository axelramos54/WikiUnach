using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace WUNACH
{
    /// <summary>
    /// Servicio de envío de correos vía SMTP.
    /// Lee las credenciales del App.config (SMTP_Host, SMTP_Port, SMTP_User, SMTP_Pass).
    /// </summary>
    public static class EmailService
    {
        private static readonly Random _rng = new Random();

        /// <summary>Genera un código numérico de 6 dígitos.</summary>
        public static string GenerarCodigo()
        {
            return _rng.Next(100000, 1000000).ToString();
        }

        /// <summary>
        /// Envía un correo HTML al destinatario con el código de verificación.
        /// Lanza excepción si el envío falla (credenciales inválidas, sin red, etc).
        /// </summary>
        public static void EnviarCodigoVerificacion(string destinatario, string codigo)
        {
            string host = ConfigurationManager.AppSettings["SMTP_Host"];
            string portStr = ConfigurationManager.AppSettings["SMTP_Port"];
            string user = ConfigurationManager.AppSettings["SMTP_User"];
            string pass = ConfigurationManager.AppSettings["SMTP_Pass"];
            string fromName = ConfigurationManager.AppSettings["SMTP_FromName"] ?? "WUNACH";

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                throw new InvalidOperationException(
                    "Falta configurar SMTP_Host / SMTP_User / SMTP_Pass en App.config");

            int port = int.TryParse(portStr, out int p) ? p : 587;

            using (var smtp = new SmtpClient(host, port))
            {
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(user, pass);
                smtp.Timeout = 15000;   // 15s

                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(user, fromName);
                    msg.To.Add(destinatario);
                    msg.Subject = "Tu código de verificación WUNACH";
                    msg.IsBodyHtml = true;
                    msg.Body = $@"
                        <div style='font-family:Segoe UI,Arial,sans-serif;max-width:480px;margin:auto;
                                    background:#f7f9fc;padding:24px;border-radius:8px;'>
                          <h2 style='color:#1e3a8a;margin-top:0'>WUNACH — WikiEstudiante UNACH</h2>
                          <p>Hola,</p>
                          <p>Recibimos una solicitud para crear una cuenta con este correo.
                             Tu código de verificación es:</p>
                          <div style='font-family:Consolas,monospace;font-size:32px;font-weight:bold;
                                      letter-spacing:8px;color:#1e3a8a;text-align:center;
                                      background:#fff;padding:18px;border-radius:6px;
                                      border:1px solid #e0e6f0;margin:18px 0;'>
                            {codigo}
                          </div>
                          <p style='color:#666;font-size:13px'>
                             Este código expira en <b>10 minutos</b>.<br>
                             Si tú no solicitaste esto, ignora este mensaje.
                          </p>
                          <hr style='border:none;border-top:1px solid #e0e6f0'>
                          <p style='color:#888;font-size:11px;text-align:center'>
                             Mensaje automático — no respondas a este correo.
                          </p>
                        </div>";
                    smtp.Send(msg);
                }
            }
        }
    }
}
