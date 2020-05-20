namespace Initium.Portal.Web.Controllers.Api
{
    public class BasicApiResponse
    {
        public BasicApiResponse(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; }
    }
}