using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL.Response
{
    public class BaseResponse
    {
        public bool Success { get; protected set; }
        public string Message { get; set; }
        public List<string> ModelErrors { get; protected set; }

        public BaseResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
