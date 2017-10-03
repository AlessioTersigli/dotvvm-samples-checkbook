using System;
using System.Collections;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Runtime.Filters;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.ViewModel;
using System.Net.Mail;
using System.Configuration;

namespace CheckBook.App.ViewModels
{
    [Authorize]
    public class PaymentViewModel : AppViewModelBase
    {
        public override string ActivePage => "home";

        [FromRoute("GroupId")]
        public int GroupId { get; set; }

        [FromRoute("Id")]
        public int PaymentId { get; set; }

        public PaymentData Data { get; set; }

        public List<TransactionData> Payers { get; set; } = new List<TransactionData>();

        public List<TransactionData> Debtors { get; set; } = new List<TransactionData>();


        [Bind(Direction.ServerToClient)]
        public decimal AmountDifference { get; set; }


        [Bind(Direction.ServerToClient)]
        public string ErrorMessage { get; set; }
        
        [Protect(ProtectMode.SignData)]
        public bool IsEditable { get; set; }

        [Protect(ProtectMode.SignData)]
        public bool IsDeletable { get; set; }
        

        [Bind(Direction.ServerToClientFirstRequest)]
        public string GroupName { get; set; }

        [Bind(Direction.ServerToClientFirstRequest)]
        public List<UserBasicInfoData> AllUsers { get; set; }


        public override Task Load()
        {
            // load all users
            AllUsers = UserService.GetUserBasicInfoList(GroupId);

            // load data
            if (!Context.IsPostBack)
            {
                CreateOrLoadData();
            }

            return base.Load();
        }

        /// <summary>
        /// Loads the data in the form
        /// </summary>
        private void CreateOrLoadData()
        {
            // get group
            var userId = GetUserId();
            var group = GroupService.GetGroup(GroupId, userId);

            // get or create the payment
            if (PaymentId > 0)
            {
                // load
                Data = PaymentService.GetPayment(PaymentId);
                IsEditable = IsDeletable = PaymentService.IsPaymentEditable(userId, PaymentId);
            }
            else
            {
                // create new
                Data = new PaymentData()
                {
                    GroupId = GroupId,
                    CreatedDate = DateTime.Today,
                    Currency = group.Currency
                };
                IsEditable = true;
                IsDeletable = false;
            }

            GroupName = group.Name;

            // load payers and debtors
            Payers = PaymentService.GetPayers(GroupId, PaymentId);
            EnsureInsertRowPresent(Payers);
            UpdateNameAndImageUrl(Payers);

            Debtors = PaymentService.GetDebtors(GroupId, PaymentId);
            EnsureInsertRowPresent(Debtors);
            UpdateNameAndImageUrl(Debtors);

            Recalculate();
        }

        private void EnsureInsertRowPresent(List<TransactionData> list)
        {
            var emptyRows = list.Where(r => r.UserId == null && r.Amount == null).ToList();

            if (emptyRows.Count == 0)
            {
                list.Add(new TransactionData());
            }
            else if (emptyRows.Count > 1)
            {
                foreach (var emptyRow in emptyRows.Skip(1))
                {
                    list.Remove(emptyRow);
                }
            }
        }

        private void UpdateNameAndImageUrl(List<TransactionData> list)
        {
            foreach (var row in list)
            {
                var user = AllUsers.FirstOrDefault(u => u.Id == row.UserId);
                row.Name = user?.Name;
                row.ImageUrl = user?.ImageUrl;
            }
        }

        public void PayersChanged()
        {
            UpdateNameAndImageUrl(Payers);
            EnsureInsertRowPresent(Payers);
            Recalculate();
        }

        public void DeletePayer(TransactionData payer)
        {
            Payers.Remove(payer);
            EnsureInsertRowPresent(Payers);
            Recalculate();
        }

        public void DebtorsChanged()
        {
            UpdateNameAndImageUrl(Debtors);
            EnsureInsertRowPresent(Debtors);
            Recalculate();
        }

        public void DeleteDebtor(TransactionData debtor)
        {
            Debtors.Remove(debtor);
            EnsureInsertRowPresent(Debtors);
            Recalculate();
        }

        /// <summary>
        /// Recalculates the remaining amount.
        /// </summary>
        public void Recalculate()
        {
            AmountDifference = (Payers.Where(p => p.Amount != null).Sum(p => p.Amount) ?? 0) - (Debtors.Where(p => p.Amount != null).Sum(p => p.Amount) ?? 0);
        }

        /// <summary>
        /// Saves the payment.
        /// </summary>
        public void Save()
        {
            try
            {
                var userId = GetUserId();
                PaymentService.SavePayment(userId, Data, Payers, Debtors);
                Copy();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            GoBack();
        }
        
        public void Copy()
        {
            var userId = GetUserId();
            var groupId = Convert.ToInt32(Context.Parameters["GroupId"]);
            var paymentId = Context.Parameters["Id"];
            string uID = userId.ToString();
            string gID = Convert.ToString(Context.Parameters["GroupId"]);
            string date = DateTime.Today.ToString();
            string text = "Payment details:\r\n" + "UserID:" + uID + "\r\n" + "GroupID:" + gID + "\r\n" + "Date:" +date+ "\r\n";
            text += "\r\nDescription\r\n";
            var a = Data.Description;
            string aus = a.ToString();
            text += aus + "\r\n";
            text += "\r\nCurrency\r\n";
            a = Data.Currency;
            aus = a.ToString();
            text += aus + "\r\n";
            text += "\r\nTotal Amount\r\n";
            var ta = Payers.Sum(p => p.Amount);
            aus = ta.ToString();
            text += aus + "\r\n";
            text += "\r\nPayers\r\n";
            ///Data.Currency
            foreach (var payer in Payers)
            {
                text += payer.Name + " " + payer.Amount + "\r\n";
            }
            text += "\r\nDebtors\r\n";
            foreach (var debtor in Debtors)
            {
                text += debtor.Name+ " " + debtor.Amount + "\r\n";
            }
            System.IO.File.WriteAllText(@"C:\Users\AlessioTersigli\Desktop\copies.txt", text);
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ConfigurationManager.AppSettings.Get("Email:EmailAddress"), ConfigurationManager.AppSettings.Get("Email:UserName"));
            mail.To.Add(new MailAddress("alexfasulli2@gmail.com", "alex fasulli2"));
            mail.Subject = "Payment details";
            mail.Body = text;
            mail.IsBodyHtml = false;
            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings.Get("Email:Server"));
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings.Get("Email:EmailAddress"), ConfigurationManager.AppSettings.Get("Email:Password"));
            client.Send(mail);
        }

        /*static public void SendMail(string host, string userName, string password, string from, string to, string subject, string body)
        {

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from, from);
            mail.To.Add(new MailAddress(to, to));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpClient client = new SmtpClient(host);
            client.Credentials = new System.Net.NetworkCredential(userName, password);
            client.Send(mail);
        }*/

        /// <summary>
        /// Deletes the current payment.
        /// </summary>
        public void Delete()
        {
            try
            {
                var userId = GetUserId();
                PaymentService.DeletePayment(userId, Data);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            GoBack();
        }

        /// <summary>
        /// Redirects back to the group page.
        /// </summary>
        public void GoBack()
        {
            Context.RedirectToRoute("group", new { Id = Data.GroupId });
        }
        
        
    }
}
