﻿using Blazored.SessionStorage;
using System.Text;
using System.Text.Json;

namespace CourseEnrollBlaze.Client.Extentions
{
    public static class SessionStorageServiceExtention
    {
        public static async Task SaveItemEncryptedAsync<T>(this ISessionStorageService sessionStorageService,string key,T item)
        {
            var itemJson = JsonSerializer.Serialize(item);
            var itemJsonBytes = Encoding.UTF8.GetBytes(itemJson);
            var base64Json = Convert.ToBase64String(itemJsonBytes); 
            await sessionStorageService.SetItemAsync(key, base64Json);
        }
        public static async Task<T> ReadEncryptedItemAsync<T>(this ISessionStorageService sessionStorageService, string key)
        {
            var base64Json = await sessionStorageService.GetItemAsync<string>(key);
            if (string.IsNullOrEmpty(base64Json))
                return default;

            var itemJsonBytes = Convert.FromBase64String(base64Json);
            var itemJson = Encoding.UTF8.GetString(itemJsonBytes);
            return JsonSerializer.Deserialize<T>(itemJson);
        }
    }
}
