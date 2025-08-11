using System.Collections.Concurrent;

namespace Project.Server.Service
{
    public class InMemoryVerificationStore : IVerificationStore
    {
        private static readonly ConcurrentDictionary<string, VerificationRecord> _verificationStore = new();

        public Task SaveVerificationCode(string userId, string email, string code, TimeSpan expiration)
        {
            var key = GetKey(userId,email);
            var expirationTime = DateTime.UtcNow.Add(expiration);

            _verificationStore[key] = new VerificationRecord
            {
                Code = code,
                ExpirationTime = expirationTime,
            };

            return Task.CompletedTask;
        }

        public Task<bool> VerifiyCode(string userId, string email, string code)
        {
            var key = GetKey(userId,email);
            if (_verificationStore.TryGetValue(key, out var record)) {
                if (record.Code == code && record.ExpirationTime >= DateTime.UtcNow) { 
                    _verificationStore.TryRemove(key, out _);
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        private string GetKey(string userId, string email) => $"{userId}:{email}";

        private class VerificationRecord
        {
            public string Code { get; set; }
            public DateTime ExpirationTime { get; set; }
        }
    }
}
