using System;
using System.Collections;
using DotVVM.Framework.Controls;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Threading.Tasks;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.ViewModel;
using System.Net.Mail;
using System.Configuration;

public static class EmailService
{
    public static void SendEmail(List<TransactionData> users, string text) {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(ConfigurationManager.AppSettings.Get("Email:EmailAddress"), ConfigurationManager.AppSettings.Get("Email:UserName"));
        foreach (var user in users)
        {
            if (user.UserId != null)
            {
                UserInfoData userData = UserService.GetUserInfo(user.UserId.Value);
                mail.To.Add(new MailAddress(userData.Email, userData.Name));
            }
        }
        mail.Subject = "Payment details";
        mail.Body = text;
        mail.IsBodyHtml = false;
        SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings.Get("Email:Server"));
        client.UseDefaultCredentials = false;
        client.EnableSsl = true;
        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings.Get("Email:EmailAddress"), ConfigurationManager.AppSettings.Get("Email:Password"));
        client.Send(mail);
    }

    public static string Text(List<TransactionData> Payers, List<TransactionData> Debtors, string uID, string gID, string date, string Description, string Currency)
    {
        string text = "Payment details:\r\n" + "UserID:" + uID + "\r\n" + "GroupID:" + gID + "\r\n" + "Date:" + date + "\r\n";
        text += "\r\nDescription\r\n";
        text += Description + "\r\n";
        text += "\r\nCurrency\r\n";
        text += Currency + "\r\n";
        text += "\r\nTotal Amount\r\n";
        var ta = Payers.Sum(p => p.Amount);
        string aus = ta.ToString();
        text += aus + "\r\n";
        text += "\r\nPayers\r\n";
        foreach (var payer in Payers)
        {
            text += payer.Name + " " + payer.Amount + "\r\n";
        }
        text += "\r\nDebtors\r\n";
        foreach (var debtor in Debtors)
        {
            text += debtor.Name + " " + debtor.Amount + "\r\n";
        }
        return text;
    }
}

