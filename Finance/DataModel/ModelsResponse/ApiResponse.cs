using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ModelsResponse
{
    public class ApiResponse<T> : BaseResponse
    {
        public T Data { get; set; }
    }
}
