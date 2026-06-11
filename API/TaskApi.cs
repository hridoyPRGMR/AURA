using Core.IServices;

namespace API
{
    public static class TaskApi
    {
        public static void MapTaskApi(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/user", async (IUserService userService) =>
            {
                var user = userService.GetUser();
                return user;
            });
        }
    }
}