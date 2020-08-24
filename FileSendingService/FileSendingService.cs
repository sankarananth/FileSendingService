using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FileSendingService
{
	public partial class FileSendingService : ServiceBase
	{
		Timer timer;
		public FileSendingService()
		{
			InitializeComponent();
			eventLog1 = new System.Diagnostics.EventLog();
			if (!System.Diagnostics.EventLog.SourceExists("MySource"))
			{
				System.Diagnostics.EventLog.CreateEventSource(
					"MySource", "MyNewLog");
			}
			eventLog1.Source = "FileSource";
			eventLog1.Log = "FileServiceLog";
		}

		protected override void OnStart(string[] args)
		{
			eventLog1.WriteEntry("InOnstart");
			 timer = new Timer();
			timer.Interval = 300000; //15 minutes
			
			timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
			timer.Start();

		}

		protected override void OnStop()
		{
			eventLog1.WriteEntry("service stopped");
			timer.Stop();
		}
		public void OnTimer(object sender, ElapsedEventArgs args)
		{
			eventLog1.WriteEntry("Entered Event");
			DataLayer data = new DataLayer();
			FileLocationFetchModel fetchModel=data.GetfileInformation();
			if(fetchModel!=null)

			{
				FileOperations file = new FileOperations(fetchModel);
				DeliveryMethods deliveryMethod;
				if(fetchModel.Status=="")
				{
					Enum.TryParse(fetchModel.FirstDelieryMethod, out deliveryMethod);
				}
				else
				{
					Enum.TryParse(fetchModel.SeconddeliveryMethod, out deliveryMethod);
				}
				
				switch(deliveryMethod)
				{
					case DeliveryMethods.SendMail:
						
						int ret = file.SendMail();
						if(ret==1)
						{
							eventLog1.WriteEntry("Mail Event Successfull");
							data.UpdateFileOperationStatus(fetchModel.File_id,fetchModel.Status);
						}
						else
						{
							eventLog1.WriteEntry("Mail Event Unsuccessfull");
							data.InsertException("File Service", MethodBase.GetCurrentMethod().Name, DateTime.Now, "Mail Sending Failed");
						}
						break;
					case DeliveryMethods.NewFolder:
						
						int successs = file.SaveToAnotherFolder();
						if (successs == 1)
						{
							eventLog1.WriteEntry("Folder Event Successfull");
							data.UpdateFileOperationStatus(fetchModel.File_id,fetchModel.Status);
						}
						else
						{
							eventLog1.WriteEntry("Folder Event UnSuccessfull");
							data.InsertException("File Service", MethodBase.GetCurrentMethod().Name, DateTime.Now, "File Transfer Failed");
						}
						break;
				}
			}
		}
	}
}
