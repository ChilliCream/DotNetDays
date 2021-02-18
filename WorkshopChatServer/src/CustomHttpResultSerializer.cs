using System.Linq;
using System.Net;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;

namespace WorkshopChatServer
{
    public class CustomHttpResultSerializer: DefaultHttpResultSerializer
    {
        public override HttpStatusCode GetStatusCode(IExecutionResult result)
        {
            if (result.Errors is not null && result.Errors.Any(error => error.Code == "AUTH_NOT_AUTHENTICATED"))
            {
                return HttpStatusCode.Forbidden;
            }
            return base.GetStatusCode(result);
        }
    }
}