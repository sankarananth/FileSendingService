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
		}

		protected override void OnStart(string[] args)
		{
			 timer = new Timer();
			timer.Interval = 900000; //15 minutes
			timer.Start();
			timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
			
		}

		protected override void OnStop()
		{
			timer.Stop();
		}
		public void OnTimer(object sender, ElapsedEventArgs args)
		{
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
							data.UpdateFileOperationStatus(fetchModel.File_id,fetchModel.Status);
						}
						else
						{
							data.InsertException("File Service", MethodBase.GetCurrentMethod().Name, DateTime.Now, "Mail Sending Failed");
						}
						break;
					case DeliveryMethods.NewFolder:
						
						int successs = file.SendMail();
						if (successs == 1)
						{
							data.UpdateFileOperationStatus(fetchModel.File_id,fetchModel.Status);
						}
						else
						{
							data.InsertException("File Service", MethodBase.GetCurrentMethod().Name, DateTime.Now, "File Transfer Failed");
						}
						break;
				}
			}
		}
	}
}
