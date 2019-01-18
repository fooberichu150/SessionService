using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SessionManagerDemo.Services;
using Unity;

namespace SessionManagerTests
{
	[TestClass]
	public class SessionServiceTests : BaseTest
	{
		IHttpContextAccessor _httpContextAccessor;
		HttpContext _httpContext;
		SessionService _target;
		ISession _session;

		[TestInitialize]
		public void Initialize()
		{
			_httpContextAccessor = Substitute.For<IHttpContextAccessor>();
			_session = Substitute.For<ISession>();

			_httpContext = new DefaultHttpContext();
			_httpContext.Session = _session;

			_httpContextAccessor.HttpContext.Returns(_httpContext);

			Container.RegisterInstance(_httpContextAccessor);

			_target = Container.Resolve<SessionService>();
		}

		[TestMethod]
		public void SessionService_SetCookie_Success()
		{
			byte[] bytes = null;
			SessionContainerFake fakeObject = new SessionContainerFake { TestPropertyInt = 25, TestPropertyString = "blah" };

			_session
				.When(x => x.TryGetValue("fakesession", out bytes))
				.Do(x => x[1] = System.Text.Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(fakeObject)));

			_target.Set("fakesession", fakeObject);

			SessionContainerFake cachedObject = _target.Get<SessionContainerFake>("fakesession");

			Assert.IsNotNull(cachedObject);
			Assert.AreEqual(fakeObject.TestPropertyInt, cachedObject.TestPropertyInt);
			Assert.AreEqual(fakeObject.TestPropertyString, cachedObject.TestPropertyString);
		}

		[TestMethod]
		public void SessionService_SetCookie_StringOnly_Success()
		{
			string value = "I'm a cookie value";
			byte[] bytes = null;
			_session
				.When(x => x.TryGetValue("fakesession", out bytes))
				.Do(x => x[1] = System.Text.Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(value)));
			_target.Set("fakesession", value);

			string result = _target.Get<string>("fakesession");

			Assert.IsFalse(string.IsNullOrWhiteSpace(result));
			Assert.AreEqual(value, result);
		}

		[TestMethod]
		public void SessionService_GetCookie_Fail()
		{
			SessionContainerFake cachedCookie = _target.Get<SessionContainerFake>("fakesession");
			Assert.IsNull(cachedCookie);
		}
	}

	public class SessionContainerFake
	{
		public int TestPropertyInt { get; set; }
		public string TestPropertyString { get; set; }
	}
}