using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPReadApp
{
    class FtpValues
    {
        public List<string> IpList;
        public List<string> FileList;
        public List<string> DifferencesList;
        public string login { get; set; }
        public string password { get; set; }
        public string[] tableFolder;
        public string[] splitFile;
        public string Folder = "";
  

    }
}
