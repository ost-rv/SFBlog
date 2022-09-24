using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFBlog.BLL.Response
{
    public class EntityBaseResponse<T> : BaseResponse
        where T : class 
    {
        public T? Entity { get; protected set; }

        public EntityBaseResponse(bool success, string message) : base(success, message)
        {
        }

        public EntityBaseResponse(bool success, string message, T entity): this(success, message)
        {
            Entity = entity;
        }

        public EntityBaseResponse(T entity) : this(true, string.Empty, entity)
        { 
        }

        public EntityBaseResponse(string message) : this(false, message)
        {
            Entity = default;
        }

        public EntityBaseResponse(string message, List<string> modelErrors) : this(message)
        {
            ModelErrors = modelErrors;
        }
    }
}
