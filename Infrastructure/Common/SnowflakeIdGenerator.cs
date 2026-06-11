using SnowflakeGenerator;
using Core.IServices;

namespace Infrastructure.Common
{
    public class SnowflakeIdGenerator : IIdGenerator
    {
        public long NewId()
        {
           Snowflake snowflake = new Snowflake();
           return snowflake.NextID();
        }
    }
}