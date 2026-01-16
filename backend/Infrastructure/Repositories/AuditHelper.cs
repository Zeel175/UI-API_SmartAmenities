using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public static class AuditHelper
    {
        public static List<AuditLog> GenerateAuditLogs<TEntity>(TEntity oldEntity, TEntity newEntity, string pageName, string userId, string actionType)
        where TEntity : class
        {
            var auditLogs = new List<AuditLog>();
            var properties = typeof(TEntity).GetProperties();

            foreach (var property in properties)
            {
                var oldValue = oldEntity != null ? property.GetValue(oldEntity)?.ToString() : null;
                var newValue = newEntity != null ? property.GetValue(newEntity)?.ToString() : null;

                if (oldValue != newValue)
                {
                    auditLogs.Add(new AuditLog
                    {
                        PageName = pageName,
                        UserId = userId,
                        ActionDateTime = DateTime.UtcNow,
                        ActionType = actionType,
                        FieldName = property.Name,
                        OldValue = oldValue,
                        NewValue = newValue,
                        EntityName = typeof(TEntity).Name,
                        EntityId = oldEntity != null ? GetEntityId(oldEntity) : GetEntityId(newEntity)
                    });
                }
            }

            return auditLogs;
        }

        private static string GetEntityId<TEntity>(TEntity entity) where TEntity : class
        {
            var keyName = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.Name.ToLower().Contains("id"))?.Name;

            return keyName != null ? entity.GetType().GetProperty(keyName)?.GetValue(entity)?.ToString() : null;
        }
    }
}
