﻿// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Framework.Tests.Actions
{
	using Castle.MonoRail.Framework.Descriptors;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Test;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultActionSelectorTestCase
	{
		private DefaultActionSelector selector;
		private StubEngineContext engine;

		[SetUp]
		public void Init()
		{
			selector = new DefaultActionSelector();

			var request = new StubRequest();
			var response = new StubResponse();
			var services = new StubMonoRailServices();
			engine = new StubEngineContext(request, response, services, new UrlInfo("area", "controller", "action1"));
		}

		[Test]
		public void CanSelectMethodAndCreatesAnActionMethodExecutorForIt()
		{
			var controllerMeta = new ControllerMetaDescriptor();
			var controller = new BaseClassController();
			var context = new ControllerContext("baseclass", "", "action1", controllerMeta);

			controllerMeta.Actions["action1"] = typeof(BaseClassController).GetMethod("Action1");

			var action = selector.Select(engine, controller, context,ActionType.Sync);
			Assert.IsNotNull(action);
			Assert.IsInstanceOf(typeof(ActionMethodExecutor), action);
		}

		[Test]
		public void CanSelectDynActionAndCreatesADynamicActionExecutor()
		{
			var controllerMeta = new ControllerMetaDescriptor();
			var controller = new BaseClassController();
			var context = new ControllerContext("baseclass", "", "action2", controllerMeta);

			context.DynamicActions.Add("action2", new DummyDynamicAction());

			var action = selector.Select(engine, controller, context,ActionType.Sync);
			Assert.IsNotNull(action);
			Assert.IsInstanceOf(typeof(DynamicActionExecutor), action);
		}

		public class BaseClassController : Controller
		{
			public void Action1()
			{
			}
		}

		public class DummyDynamicAction : IDynamicAction
		{
			public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
			{
				return null;
			}
		}
	}
}