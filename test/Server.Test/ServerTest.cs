using System;
using NUnit.Framework;
using Funq;
using System.Linq;

namespace Sioux.TechRadar
{
	[TestFixture()]
	public class ServerTest
	{
		[Test()]
		public void TestServerConstruction ()
		{
			using (var testServer = new Server()) {
				Assert.IsNotNull(testServer.Container);
				Assert.AreEqual(8888,testServer.Port);
			}
		}
		
		[Test()]
		public void TestFunqEasy()
		{
			using(var container = new Container())
			{
				using (var fakeThings = new FakeThingsRepository()){
					var mike = fakeThings.SetupMike();
					container.Register<IThingsRepository>(fakeThings);
					
					var things = container.Resolve<IThingsRepository>().GetByName(new string[]{"Mike"});
					Assert.That(things.First(),Is.EqualTo(mike));
				}
			}
		}
		
		[Test()]
		public void TestFunqChildContainer()
		{
			using (var fakeThings = new FakeThingsRepository()){
				var mike = fakeThings.SetupMike();
				
				using(var container = new Container())
				{
					container.Register<IThingsRepository>(fakeThings);
					
					using (var container2 = container.CreateChildContainer())
					{
						var things = container2.Resolve<IThingsRepository>().GetByName(new string[]{"Mike"});
						Assert.That(things.First(),Is.EqualTo(mike));
					}
				}
			}
		}
		
		[Test()]
		public void TestReusingContainerFromServer()
		{
			using (var fakeThings = new FakeThingsRepository()) {
				
				var mike = fakeThings.SetupMike();
				
				using (var server = new Server()){
					var container = server.Container;
					container.Register<IThingsRepository> (fakeThings);
					
					var things = container.Resolve<IThingsRepository>().GetByName(new string[]{"Mike"});
					Assert.That(things.First(),Is.EqualTo(mike));
					
					server.Start();
				}
			}
		}	
	}
}

