using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ModelsResponse
{
    public class FileResponse
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
