using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System.Net.Mail;
using System.Net;

//2024-12-24 iwai-tamura upd-str ---
namespace EmployeeAdjustmentConnectionSystem.BL.Common {
    /// <summary>
    /// メール送信に関わる共通クラス
    /// </summary>
    public class SendMailCommonBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        /// <summary>
        /// メールデータクラス - メールの送信に必要な情報を格納するクラス
        /// </summary>
        public class EmailData{
            /// <summary>
            /// 送信元メールアドレス
            /// </summary>
            public string From { get; set; }

            /// <summary>
            /// 送信先メールアドレス（複数の場合はカンマ区切り）
            /// </summary>
            public string To { get; set; }

            /// <summary>
            /// CCメールアドレス（複数の場合はカンマ区切り）
            /// </summary>
            public string CC { get; set; }

            /// <summary>
            /// BCCメールアドレス（複数の場合はカンマ区切り）
            /// </summary>
            public string BCC { get; set; }

            /// <summary>
            /// メールの件名
            /// </summary>
            public string Subject { get; set; }

            /// <summary>
            /// メールの本文
            /// </summary>
            public string Body { get; set; }

            /// <summary>
            /// 添付ファイルのパスのリスト
            /// </summary>
            public List<string> Attachments { get; set; } = new List<string>();
        }

        /// <summary>
        /// 指定されたメールデータを使ってメールを送信するメソッド
        /// </summary>
        /// <param name="emailData">送信するメールのデータ</param>
        /// <param name="priority">メールの優先度（デフォルトは3）</param>
        /// <returns>メール送信に成功した場合はtrue、失敗した場合はfalse</returns>
        public bool SendEmail(EmailData emailData, int priority = 3)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(emailData.From),
                    Subject = emailData.Subject,
                    Body = emailData.Body,
                    IsBodyHtml = true // HTML形式で送信  falseでテキスト
                };

                foreach (var toAddress in emailData.To.Split(','))
                {
                    mail.To.Add(toAddress.Trim());
                }

                if (!string.IsNullOrEmpty(emailData.CC))
                {
                    foreach (var ccAddress in emailData.CC.Split(','))
                    {
                        mail.CC.Add(ccAddress.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(emailData.BCC))
                {
                    foreach (var bccAddress in emailData.BCC.Split(','))
                    {
                        mail.Bcc.Add(bccAddress.Trim());
                    }
                }

                foreach (var attachment in emailData.Attachments)
                {
                    mail.Attachments.Add(new Attachment(attachment));
                }

                switch (priority)
                {
                    case 1:
                        mail.Priority = MailPriority.High;
                        break;
                    case 2:
                        mail.Priority = MailPriority.High;
                        break;
                    case 3:
                        mail.Priority = MailPriority.Normal;
                        break;
                    case 4:
                        mail.Priority = MailPriority.Low;
                        break;
                    default:
                        mail.Priority = MailPriority.Normal;
                        break;
                }

                //SmtpClient smtpClient = new SmtpClient("smtp.iwaisc.co.jp")
                SmtpClient smtpClient = new SmtpClient("172.16.0.6")
                {
                    Port = 25,
                    Timeout = 60000,    // タイムアウト時間(ms)
                    UseDefaultCredentials = true,
                    //Credentials = new NetworkCredential("username", "password"),
                    EnableSsl = false // セキュリティ保護を無効化
                };

                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
}
//2024-12-24 iwai-tamura upd-end ---
