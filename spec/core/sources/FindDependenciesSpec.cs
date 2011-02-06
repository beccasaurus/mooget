using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;
using MooGet.Test;

namespace MooGet.Specs.Core {

	// TODO once we fully port Old/DependenciesSpec, delete it

	/// <summary>Tests Source.FindDependencies for a IPackage, given an array of ISource to search from</summary>
	[TestFixture]
	public class FindDependenciesSpec : Spec {

		[SetUp]
		public void Before() {
			base.BeforeEach();
			FakePackage.DefaultVersionText = "1.0";
		}

		[TearDown]
		public void After() {
			base.AfterEach();
			FakePackage.DefaultVersionText = "1.0.0.0";
		}

		[Test]
		public void can_return_dependencies_for_package_that_has_dependencies_which_have_subdependencies_which_have_subdependencies() {
			/*
			 * Package overview.
			 *
			 * Package1
			 *		P1Sub
			 *			P1SubSub1
			 *			P1SubSub2
			 *				P1SubSubSub
			 *
			 *	Package2
			 *		P2Sub1
			 *		P2Sub2
			 *		P2Sub3 = 1.0
			 *			P2Sub3Sub
			 *				P2Sub3SubSub ~> 1.5
			 *					P2Sub3SubSubSub
			 *
			 * P2Sub3SubSub 1.5.0, 1.5.9, 1.6.0, 2.0.0
			 *
			 * P2Sub3 0.9 (no deps)
			 * P2Sub3 1.0 (has the dependencies above)
			 */
			var source = new FakeSource();
			source.Add("Package1", "P1Sub");
			source.Add("P1Sub", "P1SubSub1", "P1SubSub2");
			source.Add("P1SubSub1");
			source.Add("P1SubSub2", "P1SubSubSub");
			source.Add("P1SubSubSub");
			source.Add("Package2", "P2Sub1", "P2Sub2", "P2Sub3 1.0");
			source.Add("P2Sub1");
			source.Add("P2Sub2");
			source.Add("P2Sub3 0.9");
			source.Add("P2Sub3 1.0", "P2Sub3Sub");
			source.Add("P2Sub3Sub", "P2Sub3SubSub ~> 1.5");
			source.Add("P2Sub3SubSub 1.5.0", "P2Sub3SubSubSub");
			source.Add("P2Sub3SubSub 1.5.9", "P2Sub3SubSubSub");
			source.Add("P2Sub3SubSub 1.6.0");
			source.Add("P2Sub3SubSub 2.0.0");
			source.Add("P2Sub3SubSubSub");

			var package1 = source.GetPackagesWithId("Package1").First();
			var package2 = source.GetPackagesWithId("Package2").First();

			// check that a few things are setup correctly ...
			source.Packages.First(p => p.Id == "P2Sub3Sub").Details.Dependencies.First().ToString().ShouldEqual("P2Sub3SubSub ~> 1.5");

			/* get Package1's dependencies
			 *
			 * Package1
			 *		P1Sub
			 *			P1SubSub1
			 *			P1SubSub2
			 *				P1SubSubSub
			 */
			var found = package1.FindDependencies(source);
			found.Count.ShouldEqual(4);
			found.Select(p => p.Id).ShouldContain("P1Sub");
			found.Select(p => p.Id).ShouldContain("P1SubSub1");
			found.Select(p => p.Id).ShouldContain("P1SubSub2");
			found.Select(p => p.Id).ShouldContain("P1SubSubSub");

			// get Package2's dependencies
			/*
			 *	Package2
			 *		P2Sub1
			 *		P2Sub2
			 *		P2Sub3 = 1.0
			 *			P2Sub3Sub
			 *				P2Sub3SubSub ~> 1.5
			 *					P2Sub3SubSubSub
			 */
			found = package2.FindDependencies(source);
			found.Count.ShouldEqual(6);
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub1-1.0");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub2-1.0");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub3-1.0");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub3Sub-1.0");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub3SubSub-1.5.9");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub3SubSubSub-1.0");

			// get Package1 and Package2's dependencies
			found = Source.FindDependencies(new IPackage[] { package1, package2 }, source);
			found.Count.ShouldEqual(10);
			found.Select(p => p.Id).ToArray().ShouldContain("P1Sub");
			found.Select(p => p.Id).ToArray().ShouldContain("P1SubSub1");
			found.Select(p => p.Id).ToArray().ShouldContain("P1SubSub2");
			found.Select(p => p.Id).ToArray().ShouldContain("P1SubSubSub");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub1-1.0");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub2-1.0");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub3-1.0");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub3Sub-1.0");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub3SubSub-1.5.9");
			found.Select(p => p.IdAndVersion()).ToArray().ShouldContain("P2Sub3SubSubSub-1.0");

			// while we have all of this loaded up, let's test missing dependencies ...
			// let's remove P2Sub3SubSub 1.5.* so P2Sub3SubSub ~> 1.5 fails
			source.Packages.Where(p => p.IdAndVersion().StartsWith("P2Sub3SubSub-1.5.")).ToList().ForEach(p => source.Packages.Remove(p));

			try {
				 Source.FindDependencies(new IPackage[] { package1, package2 }, source);
				 Assert.Fail("Expected MissingDependencyException to be thrown");
			} catch (MooGet.MissingDependencyException ex) {
				ex.Dependencies.Count.ShouldEqual(1);
				ex.Dependencies.First().ToString().ShouldEqual("P2Sub3SubSub ~> 1.5");
			}
		}
	}
}
