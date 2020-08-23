using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FileSendingService
{
	public class FileOperations
	{
		private FileLocationFetchModel fetchModl { get; set; }
		private string OtherFolderLocation { get; set; }
		private string mailUserName { get; set; }
		private string mailPassword { get; set; }
	    public FileOperations(FileLocationFetchModel md)
		{
			this.fetchModl = md;
		}
		public int SendMail()
		{
			try
			{
				MailMessage mail = new MailMessage();
				SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
				mail.From = new MailAddress("006ananth@gmail.com");
				this.mailUserName = ConfigurationManager.AppSettings["mailUserName"].ToString();
				this.mailPassword = ConfigurationManager.AppSettings["mailPassword"].ToString();
				mail.To.Add("006ananth@gmail.com");
				mail.Subject = "Test Service Mail - 1";
				mail.Body = "mail with attachment";

				System.Net.Mail.Attachment attachment;
				attachment = new System.Net.Mail.Attachment(fetchModl.FileLocation + fetchModl.FileName + fetchModl.FileExtension);
				mail.Attachments.Add(attachment);

				SmtpServer.Port = 587;
				SmtpServer.UseDefaultCredentials = false;
				SmtpServer.Credentials = new System.Net.NetworkCredential(this.mailUserName,this.mailPassword);
				SmtpServer.EnableSsl = true;


				SmtpServer.Send(mail);
				return 1;
			}
			catch (Exception ex)
			{
				DataLayer dt = new DataLayer();
				dt.InsertException("File Service App", System.Reflection.MethodBase.GetCurrentMethod().Name, DateTime.Now, ex.Message);
				return 0;
			}
		}
		public int SaveToAnotherFolder()
		{
			try
			{
				OtherFolderLocation = "D:\\NewTestFolder\\";
				string[] filePaths = Directory.GetFiles(fetchModl.FileLocation);
				string target = "";
				foreach (string file in filePaths)
				{
					if (file == fetchModl.FileLocation + fetchModl.FileName + fetchModl.FileExtension)
					{
						target = OtherFolderLocation + fetchModl.FileName + fetchModl.FileExtension;
					}
				}
				if (!Directory.Exists(OtherFolderLocation))
				{
					Directory.CreateDirectory(OtherFolderLocation);
					File.Move(fetchModl.FileLocation + fetchModl.FileName + fetchModl.FileExtension, target);
					return 1;
				}
				else
				{
					File.Move(fetchModl.FileLocation + fetchModl.FileName + fetchModl.FileExtension, target);
					return 1;
				}
			}
			catch (Exception ex)
			{
				DataLayer dt = new DataLayer();
				dt.InsertException("File Service App", System.Reflection.MethodBase.GetCurrentMethod().Name, DateTime.Now, ex.Message);
				return 0;
			}



		}
		public void AttachToUrl()
		{

		}
		public void SFTP()
		{
			//to implement
		}
	}
}
