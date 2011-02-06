using System;
using System.Linq;
using System.Collections.Generic;

namespace MooGet {

	/// <summary>ISource implementation useful as a baseclass for other sources</summary>
	/// <remarks>
	/// Used for:
	///  - useful base class for ISource implementations
	///  - managing local moo sources
	/// </remarks>
	public abstract class Source : ISource {

		#region Abstract Members
		public abstract Nupkg          Fetch(PackageDependency dependency, string directory);
		public abstract IPackage       Push(Nupkg nupkg);
		public abstract bool           Yank(PackageDependency dependency);
		public abstract IPackage       Install(PackageDependency dependency, params ISource[] sourcesForDependencies);
		public abstract bool           Uninstall(PackageDependency dependency, bool uninstallDependencies);
		public abstract List<IPackage> Packages { get; }
		#endregion

		public virtual string Name     { get; set; }
		public virtual string Path     { get; set; }
		public virtual object AuthData { get; set; }

		public virtual IPackage Get(PackageDependency dependency) {
			return Packages.FirstOrDefault(pkg => dependency.Matches(pkg));
		}

		public virtual List<IPackage> LatestPackages {
			get { return Packages.Where(pkg => pkg.Version == this.HighestVersionAvailableOf(pkg.Id)).ToList(); }
		}

		public virtual List<IPackage> GetPackagesWithId(string id) {
			id = id.ToLower();
			return Packages.Where(pkg => pkg.Id.ToLower() == id).ToList();
		}

		public virtual List<IPackage> GetPackagesWithIdStartingWith(string query) {
			query = query.ToLower();
			return Packages.Where(pkg => pkg.Id.ToLower().StartsWith(query)).ToList();
		}

		public virtual List<IPackage> GetPackagesMatchingDependencies(params PackageDependency[] dependencies) {
			return Packages.Where(pkg => PackageDependency.MatchesAll(pkg, dependencies)).ToList();
		}

		public static List<IPackage> FindDependencies(IPackage package, params ISource[] sources) {
			return FindDependencies(new IPackage[] { package }, sources);
		}

		public static List<IPackage> FindDependencies(IPackage[] packages, params ISource[] sources) {
			return FindDependencies(null, packages, sources);
		}

		/// <summary>The main dependency searching logic</summary>
		public static List<IPackage> FindDependencies(
				IDictionary<string, List<PackageDependency>> discoveredDependencies, 
				IPackage[] packages, 
				params ISource[] sources) {
		
			var disc = (discoveredDependencies == null) ? "null" : discoveredDependencies.Count.ToString();
			Console.WriteLine("Source.FindDependencies({0}, {1}, {2})", disc, new List<IPackage>(packages).Select(p => p.IdAndVersion()).ToList().Join(", "), sources);

			var found           = new List<IPackage>();
			var packageIds      = packages.Select(p => p.Id);
			bool throwIfMissing = (discoveredDependencies == null); // if this is null, then we're not recursing

			// TODO this should be pulled out into its own method that JUST returns a list of PackageDependency for us to find
			//
			// get ALL of the dependencies for these packages, grouped by package Id
			// eg. { "log4net" => ["log4net > 2.0", "log4net < 2.5"] }
			var allDependencies = new Dictionary<string, List<PackageDependency>>();
			foreach (var package in packages) {
				foreach (var packageDependency in package.Details.Dependencies) {
					if (packageIds.Contains(packageDependency.PackageId)) continue;
					if (! allDependencies.ContainsKey(packageDependency.PackageId)) 
						allDependencies[packageDependency.PackageId] = new List<PackageDependency>();
					if (! allDependencies[packageDependency.PackageId].Contains(packageDependency))
						allDependencies[packageDependency.PackageId].Add(packageDependency);
				}
			}

			// add these packages' dependencies into discoveredDependencies.
			// we track these to know whether or not we're missing any dependencies for any of the packages found.
			if (discoveredDependencies == null)
				discoveredDependencies = new Dictionary<string, List<PackageDependency>>();
			foreach (var packageDependency in allDependencies) {
				var dependencyId = packageDependency.Key;
				var dependencies = packageDependency.Value.ToArray();
				if (! discoveredDependencies.ContainsKey(dependencyId))
					discoveredDependencies[dependencyId] = new List<PackageDependency>();
				foreach (var dependency in dependencies)
					if (! discoveredDependencies[dependencyId].Contains(dependency))
						discoveredDependencies[dependencyId].Add(dependency);
			}

			// actually go and look for these dependencies
			foreach (var packageDependency in allDependencies) {
				var dependencyId = packageDependency.Key;
				var dependencies = packageDependency.Value.ToArray();

				// go through all sources and get the *latest* version of this dependency (that matches)
				IPackage dependencyPackage = null;
				foreach (var source in sources) {
					// find highest version of package from this source that matches ALL of the dependencies that we've found for this package
					var match = source.GetPackagesMatchingDependencies(dependencies).OrderBy(pkg => pkg.Version).Reverse().FirstOrDefault();
					if (match != null)
						if (dependencyPackage == null || dependencyPackage.Version < match.Version)
							dependencyPackage = match;
				}

				if (dependencyPackage != null) {
					found.Add(dependencyPackage);
					if (dependencyPackage.Details.Dependencies.Any())
						found.AddRange(Source.FindDependencies(discoveredDependencies, new IPackage[]{ dependencyPackage }, sources)); // <--- recurse!
				}
				else
					Console.WriteLine("Could not find dependency: {0}", dependencyId);
			}

			// throw a MissingDependencyException if any of the discovered dependencies were not found
			if (throwIfMissing) {
				var foundIds = found.Select(pkg => pkg.Id);
				var missing  = new List<PackageDependency>();

				foreach (var dependencyPackage in discoveredDependencies)
					if (! foundIds.Contains(dependencyPackage.Key))
						missing.AddRange(dependencyPackage.Value);

				if (missing.Count > 0)
					throw new MissingDependencyException(missing);
			}

			// TODO instead of just doing a Distinct(), we need to actually inspect the dependencies ...

			// do not include any of the packages that were passed in as dependencies
			return found.Where(pkg => ! packageIds.Contains(pkg.Id)).Distinct().ToList();
		}
	}
}
