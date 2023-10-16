using System.Reflection;
using System.Text.Json;
using TradingFutures.Shared.Base;
using TradingFutures.Shared.Entities;
using TradingFutures.Shared.Keys;

namespace TradingFutures.Persistence.Factories
{
    public static class SettingsFactory
    {
        #region Private Fields

        private static Dictionary<Type, Dictionary<string, PropertyInfo>> _propertiesCache = new();

        #endregion

        #region Public Methods

        public static TEntity CreateFromItems<TKey, TEntity>(IEnumerable<SettingsItemEntity> items)
            where TKey : BaseKey<TKey>, new()
            where TEntity : BaseEntity<TKey, TEntity>, new()
        {
            var entity = new TEntity();

            if (!items.Any())
                return entity;

            entity.CreateDT = items.Min(v => v.CreateDT);
            entity.UpdateDT = items.Max(v => v.UpdateDT);

            var itemsDict = items
                .ToDictionary(v => v.PropertyName);

            var propertiesDict = getPropertiesDict<TKey, TEntity>();

            foreach (var kvp in propertiesDict)
            {
                var propertyValue = getPropertyValueFromItems<TKey, TEntity>(kvp.Value, itemsDict);
                if (propertyValue != null)
                    kvp.Value.SetValue(entity, propertyValue);
            }

            return entity;
        }

        public static IEnumerable<SettingsItemEntity> MergeItems<TKey, TEntity>(TEntity entity, IEnumerable<SettingsItemEntity> items)
            where TKey : BaseKey<TKey>, new()
            where TEntity : BaseEntity<TKey, TEntity>
        {
            var itemsDict = items.ToDictionary(v => v.PropertyName);

            var propertiesDict = getPropertiesDict<TKey, TEntity>();

            var resultItems = new List<SettingsItemEntity>();

            foreach (var kvp in propertiesDict)
            {
                var propertyValue = kvp.Value.GetValue(entity);
                if (propertyValue == null)
                    continue;

                var processedItem = setPropertyValueToItem<TKey, TEntity>(kvp.Value, itemsDict, propertyValue);
                if (processedItem != null)
                    resultItems.Add(processedItem);
            }

            return resultItems;
        }

        #endregion

        #region Private Methods

        private static Dictionary<string, PropertyInfo> getPropertiesDict<TKey, TEntity>()
            where TKey : BaseKey<TKey>, new()
            where TEntity : BaseEntity<TKey, TEntity>
        {
            lock (_propertiesCache)
            {
                if (_propertiesCache.TryGetValue(typeof(TEntity), out Dictionary<string, PropertyInfo>? propertiesDict))
                    return propertiesDict;

                propertiesDict = new Dictionary<string, PropertyInfo>();
                _propertiesCache.Add(typeof(TEntity), propertiesDict);

                var properties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var property in properties)
                {
                    if (property.Name == "Key" || property.Name == "CreateDT" || property.Name == "UpdateDT")
                        continue;

                    if (!property.CanRead || !property.CanWrite)
                        continue;

                    propertiesDict.Add(property.Name, property);
                }

                return propertiesDict;
            }
        }

        private static object? getPropertyValueFromItems<TKey, TEntity>(PropertyInfo propertyInfo, Dictionary<string, SettingsItemEntity> itemsDict)
            where TKey : BaseKey<TKey>, new()
            where TEntity : BaseEntity<TKey, TEntity>
        {
            try
            {
                if (propertyInfo.PropertyType == typeof(bool))
                    return getSettingsItemOrDefault<TKey, TEntity>(itemsDict, propertyInfo.Name).BoolValue;
                else if (propertyInfo.PropertyType == typeof(int))
                    return getSettingsItemOrDefault<TKey, TEntity>(itemsDict, propertyInfo.Name).IntValue;
                else if (propertyInfo.PropertyType == typeof(decimal))
                    return getSettingsItemOrDefault<TKey, TEntity>(itemsDict, propertyInfo.Name).DecimalValue;
                else if (propertyInfo.PropertyType == typeof(DateTime))
                    return getSettingsItemOrDefault<TKey, TEntity>(itemsDict, propertyInfo.Name).DateTimeValue;
                else if (propertyInfo.PropertyType == typeof(string))
                    return getSettingsItemOrDefault<TKey, TEntity>(itemsDict, propertyInfo.Name).StringValue;
                else
                {
                    var item = getSettingsItemOrDefault<TKey, TEntity>(itemsDict, propertyInfo.Name);
                    return !string.IsNullOrEmpty(item.StringValue)
                        ? JsonSerializer.Deserialize(item.StringValue, propertyInfo.PropertyType)
                        : null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static SettingsItemEntity? setPropertyValueToItem<TKey, TEntity>(PropertyInfo propertyInfo, Dictionary<string, SettingsItemEntity> itemsDict, object? propertyValue)
            where TKey : BaseKey<TKey>, new()
            where TEntity : BaseEntity<TKey, TEntity>
        {
            try
            {
                var item = getSettingsItemOrDefault<TKey, TEntity>(itemsDict, propertyInfo.Name);

                if (propertyInfo.PropertyType == typeof(bool))
                    item.BoolValue = propertyValue as bool? ?? false;
                else if (propertyInfo.PropertyType == typeof(int))
                    item.IntValue = propertyValue as int? ?? 0;
                else if (propertyInfo.PropertyType == typeof(decimal))
                    item.DecimalValue = propertyValue as decimal? ?? 0m;
                else if (propertyInfo.PropertyType == typeof(DateTime))
                    item.DateTimeValue = propertyValue as DateTime? ?? DateTime.MinValue;
                else if (propertyInfo.PropertyType == typeof(string))
                    item.StringValue = propertyValue as string ?? string.Empty;
                else
                    item.StringValue = JsonSerializer.Serialize(propertyValue);

                return item;
            }
            catch
            {
                return null;
            }
        }

        private static SettingsItemEntity getSettingsItemOrDefault<TKey, TEntity>(Dictionary<string, SettingsItemEntity> itemsDict, string name)
            where TKey : BaseKey<TKey>, new()
            where TEntity : BaseEntity<TKey, TEntity>
        {
            if (!itemsDict.TryGetValue(name, out SettingsItemEntity? item))
            {
                var key = new SettingsItemKey(typeof(TEntity).Name, name);
                item = new SettingsItemEntity(key);
            }

            return item;
        }

        #endregion
    }
}