using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ModelsResponse
{
    public class BaseResponse
    {
        /// <summary>
        /// HTTP-статус ответа
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Сообщение, описывающее результат выполнения
        /// </summary>
        public string Message { get; set; }
    }
}
