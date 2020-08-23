using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSendingService
{
	[Serializable]
	public class FileLocationFetchModel
	{
		public int File_id { get; set; }
		public string FileName { get; set; }
		public string FileLocation { get; set; }
		public string FileExtension { get; set; }
		public string FirstDelieryMethod { get; set; }
		public string SeconddeliveryMethod { get; set; }
		public string Status { get; set; }

	}
	public enum DeliveryMethods
	{
		SFTP=1,
		NewFolder=2,
		PushUrl=3,
		SendMail=4

	}
}
