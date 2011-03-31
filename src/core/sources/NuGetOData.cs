using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using EasyOData;
using EasyOData.Filters.Extensions;

namespace MooGet {

	/// <summary>An ISource representing an OData service with NuGet packages (using the "official" NuGet WCF service)</summary>
	public class NuGetOData : Source, ISource, IDirectory {

		public NuGetOData() : base() {}
		public NuGetOData(string path) : this() {
			Path = path;
		}

		Service _service;
		Collection _packagesCollection;

		/// <summary>The OData Service (for the provided Path)</summary>
		public virtual Service Service {
			get { return _service ?? (_service = new Service(Path)); }
			set { _service = value; }
		}

		/// <summary>The OData collection of Packages</summary>
		public virtual Collection OData {
			get {
				if (_packagesCollection == null) {
					try {
						if (! Service.CollectionNames.Contains("Packages"))
							throw new Exception(string.Format("OData service {0} has no Packages collection.  Wrong path?", Path));
						_packagesCollection = Service["Packages"];
					} catch (Exception ex) {
						Console.WriteLine("Could not get Packages collection from OData service: {0}", Path);
						throw ex;
					}
				}
				return _packagesCollection;
			}
			set { _packagesCollection = value; }
		}

		/// <summary>Returns true if the given path looks like a valid NuGetOData, else false</summary>
		/// <remarks>
		/// Right now, this is true for ANY path that starts with http://
		/// </remarks>
		public static bool IsValidPath(string path) {
			return path.StartsWith("http");
		}

		/// <summary>Returns all of the Nupkg provided by this service</summary>
		public override List<IPackage> Packages {
			get { return EntitiesToPackages(OData.All); }
		}

		public override List<IPackage> GetPackagesWithIdStartingWith(string query) {
			return EntitiesToPackages(OData.Where("Id"._StartsWith(query)));
		}

		public override List<IPackage> GetPackagesWithId(string id) {
			return EntitiesToPackages(OData.Where("Id"._Equals(id)));
		}

		// TODO test that this works with a dependency that specified a version like "greater than 1.0 but less than 2.0"
		public override IPackage Get(PackageDependency dependency) {
			if (dependency.Versions.Count == 1 && dependency.Versions.First().Operator == PackageDependency.Operators.EqualTo)
				return EntityToPackage(OData.Get(new {
					Id      = dependency.PackageId,
					Version = dependency.Versions.First().VersionText
				}));
			else
				return GetPackagesWithId(dependency.PackageId).Where(pkg => dependency.Matches(pkg)).ToList().Latest();
		}

		// TODO add a test that calls source.FindDependencies, which calls this ... it needs to be optimized ... it currently uses Packages
		// public virtual List<IPackage> GetPackagesMatchingDependencies(params PackageDependency[] dependencies) {
		// 	return Packages.Where(pkg => PackageDependency.MatchesAll(pkg, dependencies)).ToList();
		// }

		// public override List<IPackage> LatestPackages {
		// 	get { return Packages.Where(pkg => pkg.Version == this.HighestVersionAvailableOf(pkg.Id)).ToList(); }
		// }

		/// <summary>Given an enumerable of Entity (from the OData service), this returns a list of IPackage</summary>
		public virtual List<IPackage> EntitiesToPackages(IEnumerable<Entity> entities) {
			var packages = new List<IPackage>();
			foreach (var entity in entities)
				packages.Add(EntityToPackage(entity) as IPackage);
			return packages;
		}

		/// <summary>Given an Entity (from the OData service), this returns a NuGetODataPackage</summary>
		public virtual NuGetODataPackage EntityToPackage(Entity entity) {
			return (entity == null) ? null : new NuGetODataPackage(entity, this);
		}

		public override IPackageFile Fetch(PackageDependency dependency, string directory) {
			return null;
		}

		public override IPackage Push(IPackageFile file) {
			return null;
		}

		public override bool Yank(PackageDependency dependency) {
			return false;
		}
	}
}
