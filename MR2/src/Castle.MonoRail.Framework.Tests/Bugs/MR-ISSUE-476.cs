// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Bugs
{
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class MR_ISSUE_476
	{
		public void StubMonoRailServices_Should_Return_Null_If_Service_Cannot_Be_Found()
		{
			var services = new StubMonoRailServices();

			Assert.IsNull(services.GetService<MR_ISSUE_476>());
		}
	}
}