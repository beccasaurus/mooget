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

		public object ListQuery(string query) {
			return "QUERYING NOT SUPPORTED YET";
		}

		public object ListAll() {
			var packages = new List<IPackage>();
			// if source ... else ...
			if (QueryLocal == true) packages.AddRange(Moo.Dir.Packages);
			// if ...
			return ListPackages(packages);
		}

		public object ListPackages(List<IPackage> packages) {
			if (packages.Count == 0)
				return "No packages";

			Output.Line("Found {0} packages", packages.Count);
			var grouped = packages.GroupBy(pkg => pkg.Id);
			foreach (var group in grouped)
				foreach (var item in group)
					Output.Line("group Key:{0} Value:{1}", group.Key, item);
			return Output;
		}
	}
}
