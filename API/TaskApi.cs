using Core.IServices;
using Shared.Dtos;

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

            routes.MapPost("/create-task", async(TaskItemCreateDto input, ITaskService taskService) =>
            {
                await taskService.CreateTask(input);
            });
        }
    }
}