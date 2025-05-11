using Dapper;
using System.Data;

namespace MessagingProcessor.Persistence
{
    public class GuidTypeHandler : SqlMapper.ITypeHandler
    {
        private readonly ILogger<GuidTypeHandler> _logger;

        public GuidTypeHandler(ILogger<GuidTypeHandler> logger)
        {
            _logger = logger;
        }

        public void SetValue(IDbDataParameter parameter, object value)
        {
            if (value is Guid guidValue)
            {
                parameter.Value = guidValue.ToString();
            }
            else
            {
                _logger.LogWarning("Unexpected value passed to SetValue: {Value}", value);
                parameter.Value = DBNull.Value;
            }
        }

        public object Parse(Type destinationType, object value)
        {
            try
            {
                return Guid.Parse(value.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse GUID from database value: {Value}", value);
                throw;
            }
        }
    }
}
