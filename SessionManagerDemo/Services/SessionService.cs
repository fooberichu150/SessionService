using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SessionManagerDemo.Extensions;

namespace SessionManagerDemo.Services
{
	public interface ISessionService
	{
		T Get<T>(string key);
		void Set<T>(string key, T value);

		void Remove(params string[] keys);
	}

	public class SessionService : ISessionService
	{
		private readonly ISession _session;

		public SessionService(IHttpContextAccessor httpContextRepository)
		{
			_session = httpContextRepository.HttpContext?.Session;

			if (_session == null)
				throw new ArgumentNullException("Session cannot be null.");
		}

		public T Get<T>(string key)
		{
			string value = _session.GetString(key);
			if (string.IsNullOrWhiteSpace(value))
				return default(T);

			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
		}

		public void Set<T>(string key, T value)
		{
			if (value == null)
				Remove(key);
			else
				_session.SetString(key, Newtonsoft.Json.JsonConvert.SerializeObject(value));
		}

		public void Remove(params string[] keys)
		{
			if (keys.IsNullOrEmpty())
				return;

			foreach (string key in keys)
				_session.Remove(key);
		}
	}
}
