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

namespace Castle.MonoRail.Framework.Tests.Routing
{
	using System.Collections;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Routing;
	using NUnit.Framework;

	[TestFixture]
	public class PatternRouteCreateUrlTestCase : BaseRuleTestFixture
	{
		private IDictionary standardRouteParamsWithArea, standardRouteParamsWithoutArea;

		[SetUp]
		public void Init()
		{
			// Those simulate the parameters created by DefaultUrlBuilder
			standardRouteParamsWithArea = DictHelper.Create("area=area", "controller=home", "action=index");
			standardRouteParamsWithoutArea = DictHelper.Create("area=", "controller=home", "action=index");
		}

		[Test, Ignore("Need to re-think this one")]
		public void ShouldNotMatchStaticRule()
		{
			var route = new PatternRoute("/some/path");
			Assert.IsNull(route.CreateUrl(DictHelper.Create("")));
		}

		[Test]
		public void ShouldNotMatchIfParameterIsNotPresent()
		{
			var route = new PatternRoute("/some/<controller>");
			Assert.IsNull(route.CreateUrl(DictHelper.Create("")));
		}

		[Test]
		public void ShouldMatchNamedRequiredParameter()
		{
			var route = new PatternRoute("/some/<controller>");
			Assert.AreEqual("/some/home", route.CreateUrl(DictHelper.Create("controller=home")));
			Assert.AreEqual("/some/home", route.CreateUrl(standardRouteParamsWithoutArea));
			Assert.IsNull(route.CreateUrl(standardRouteParamsWithArea));

			route = new PatternRoute("/some/<controller>/<action>");
			Assert.AreEqual("/some/home/index", route.CreateUrl(DictHelper.Create("controller=home", "action=index")));
			Assert.AreEqual("/some/home/index", route.CreateUrl(standardRouteParamsWithoutArea));
			Assert.IsNull(route.CreateUrl(standardRouteParamsWithArea));
		}

		[Test]
		public void ShouldSkipIfDifferentParameterWasPassed()
		{
			var route = new PatternRoute("/some/<controller>");
			Assert.IsNull(route.CreateUrl(DictHelper.Create("project=MR")));
			Assert.IsNull(route.CreateUrl(DictHelper.Create("controller=home", "project=MR")));
		}

		[Test]
		public void ShouldMatchNamedRequiredParametersWithExtension()
		{
			var route = new PatternRoute("/some/<controller>.castle/<action>");

			Assert.AreEqual("/some/home.castle/index", 
				route.CreateUrl(DictHelper.Create("controller=home", "action=index")));
		}

		[Test]
		public void ShouldIgnoreOptionalParameterButMatchOthers()
		{
			var route = new PatternRoute("/<controller>/[action]");

			Assert.AreEqual("/home", 
				route.CreateUrl(DictHelper.Create("controller=home")));
		}

		[Test]
		public void ShouldApplyRestrictionsToParameters()
		{
			var route = new PatternRoute("/projects/<project>/<controller>/[action]/[id]").
					DefaultFor("action").Is("index").
					Restrict("controller").AnyOf("stories", "bugs", "tasks").
					Restrict("id").ValidInteger;

			Assert.IsNull(
				route.CreateUrl(DictHelper.Create("project=MonoRail", "controller=home")));

			Assert.AreEqual("/projects/MonoRail/Stories", 
				route.CreateUrl(DictHelper.Create("project=MonoRail", "controller=Stories")));
			
			Assert.AreEqual("/projects/MonoRail/bugs", 
				route.CreateUrl(DictHelper.Create("project=MonoRail", "controller=bugs", "action=index")));
		}

		[Test]
		public void ShouldOmitOptionalParameterIfMatchesWithDefault()
		{
			var route = new PatternRoute("/projects/<project>/<controller>/[action]/[id]").
				DefaultFor("action").Is("index").
				Restrict("controller").AnyOf("stories", "bugs", "tasks").
				Restrict("id").ValidInteger;

			Assert.AreEqual("/projects/MonoRail/bugs", 
				route.CreateUrl(DictHelper.Create("project=MonoRail", "controller=bugs", "action=index")));
		}

		[Test]
		public void ShouldNotCreateRouteUrlIfDefaultsDoNotMatchAndDefaultDoesNotHaveARestriction()
		{
			var route = new PatternRoute("/people/<id>/edit").
				DefaultForAction().Is("edit").
				DefaultForController().Is("companies").
				Restrict("id").ValidInteger;

			Assert.IsNull(route.CreateUrl(DictHelper.Create("controller=people", "action=edit", "id=1")));
		}

		[Test]
		public void ShouldCreateRouteUrlIfDefaultsDoNotMatchAndDefaultsHaveRestrictions()
		{
			var route = new PatternRoute("/people/<id>/edit.[format]").
				DefaultForAction().Is("edit").
				DefaultForController().Is("people").
				Restrict("id").ValidInteger.
				Restrict("format").AnyOf(new[]{"html", "json", "xml"}).
				DefaultFor("format").Is("html");

			Assert.AreEqual("/people/1/edit.json", route.CreateUrl(DictHelper.Create("id=1", "format=json")));
		}

		[Test]
		public void ShouldCreateRouteUrlIfDefaultsAreNotSupplied()
		{
			var route = new PatternRoute("/people/<id>/edit").
				DefaultForAction().Is("edit").
				DefaultForController().Is("people").
				Restrict("id").ValidInteger;

			Assert.AreEqual("/people/1/edit", route.CreateUrl(DictHelper.Create("id=1")));
		}

		[Test]
		public void ShouldNotLeaveATrailingDot()
		{
			var route = new PatternRoute("/people/<id>/edit.[format]").
				DefaultForAction().Is("edit").
				DefaultForController().Is("people").
				Restrict("id").ValidInteger.
				Restrict("format").AnyOf(new[] { "html", "json", "xml" }).
				DefaultFor("format").Is("html");

			Assert.AreEqual("/people/1/edit", route.CreateUrl(DictHelper.Create("id=1")));
		}

		[Test]
		public void ShouldNotLeaveATrailingSlash()
		{
			var route = new PatternRoute("/people/<id>/edit.[format]/").
				DefaultForAction().Is("edit").
				DefaultForController().Is("people").
				Restrict("id").ValidInteger.
				Restrict("format").AnyOf(new[] { "html", "json", "xml" }).
				DefaultFor("format").Is("html");

			Assert.AreEqual("/people/1/edit", route.CreateUrl(DictHelper.Create("id=1")));
		}
	}
}
