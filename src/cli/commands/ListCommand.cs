using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Options;

namespace MooGet.Commands {

	///<summary>moo list</summary>
	public class ListCommand : MooGetCommand {

		[Command("list", "Display all packages installed or from source")]
		public static object Run(string[] args) { return new ListCommand(args).Run(); }

		public ListCommand(string[] args) : base(args) {
			PrintHelpIfNoArguments = false;
			Options = new OptionSet() {
				{ "s|source=", v => Source      = v         },
				{ "l|local",   v => QueryLocal  = v != null },
				{ "r|remote",  v => QueryRemote = v != null }
			};
		}

		/// <summary>A particular source to query against</summary>
		public string Source { get; set; }

		/// <summary>Whether or not we should query the local (installed) packages</summary>
		public bool? QueryLocal { get; set; }

		/// <summary>Whether or not we should query ALL remote sources (added via 'moo source')</summary>
		public bool? QueryRemote { get; set; }

		/// <summary>moo source</summary>
		public override object RunDefault() {
			SetDefaults();

			if (Args.Count > 0)
				return ListQuery(Args.First());
			else
				return ListAll();
		}

		void SetDefaults() {
			if (QueryLocal == null && QueryRemote == null && Source == null)
				QueryLocal = true;
		}

		public object ListAll() {
			var packages = new List<IPackage>();
			SourcesToQuery.ForEach(src => packages.AddRange(src.Packages));
			return ListPackages(packages);
		}

		public object ListQuery(string query) {
			var packages = new List<IPackage>();
			SourcesToQuery.ForEach(src => packages.AddRange(src.GetPackagesWithIdStartingWith(query)));
			return ListPackages(packages);
		}

		public List<ISource> SourcesToQuery {
			get {
				var sources = new List<ISource>();

				// -s --source
				if (Source != null) {
					var src = MooGet.Source.GetSource(Source);
					if (src == null)
						throw new HandledException("Source not found: {0}", Source);
					sources.Add(src);
				}

				// -l --local
				if (QueryLocal == true)
					sources.Add(Moo.Dir);

				return sources;
			}
		}

		public object ListPackages(List<IPackage> packages) {
			if (packages.Count == 0)
				return "No packages";
			
			foreach (var item in packages.GroupById())
				Output.Line("{0} ({1})", item.Key, item.Value.Versions().ToStrings().Join(", "));

			return Output;
		}
	}
}
