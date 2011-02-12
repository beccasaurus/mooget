using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Linq;
using System.Collections.Generic;
using MooGet;
using NUnit.Framework;

namespace MooGet.Specs {

	[TestFixture]
	public class PackSpec : MooGetSpec {

		[Test]
		public void can_pack_a_directory_with_just_a_tool() {
			var nupkg  = PathToTemp("working", "just-a-tool-1.0.0.0.nupkg");
			var nuspec = PathToContent("package_working_directories", "just-a-tool", "just-a-tool.nuspec");

			File.Exists(nupkg).ShouldBeFalse();

			moo("pack {0}", nuspec).ShouldContain("Created just-a-tool-1.0.0.0.nupkg");

			File.Exists(nupkg).ShouldBeTrue();
			var pathsInZip = Util.PathsInZip(nupkg);
			pathsInZip.Length.ShouldEqual(2); // .exe tool and the nuspec file
			pathsInZip.ShouldContain("just-a-tool.nuspec");
			pathsInZip.ShouldContain("tools/tool.exe");
		}

		[Test]
		public void can_pack_a_directory_with_just_a_library() {
			var nupkg  = PathToTemp("working", "just-a-library-1.0.0.0.nupkg");
			var nuspec = PathToContent("package_working_directories", "just-a-library", "just-a-library.nuspec");

			File.Exists(nupkg).ShouldBeFalse();

			moo("pack {0}", nuspec).ShouldContain("Created just-a-library-1.0.0.0.nupkg");

			File.Exists(nupkg).ShouldBeTrue();
			var pathsInZip = Util.PathsInZip(nupkg);
			pathsInZip.Length.ShouldEqual(2); // .exe library and the nuspec file
			pathsInZip.ShouldContain("just-a-library.nuspec");
			pathsInZip.ShouldContain("lib/MooGet.TestExtensions.dll");
		}

		[Test]
		public void can_get_mime_type_string_for_popular_file_extensions() {
			foreach (var extension in new string[]{ "txt", "cs", "boo", "vb" })
				Util.MimeTypeFor(extension).ShouldEqual(MediaTypeNames.Text.Plain);

			foreach (var extension in new string[]{ "xml", "config", "nuspec" })
				Util.MimeTypeFor(extension).ShouldEqual(MediaTypeNames.Text.Xml);

			foreach (var extension in new string[]{ "htm", "html" })
				Util.MimeTypeFor(extension).ShouldEqual(MediaTypeNames.Text.Html);

			// we grab the extension if we have a full filename
			foreach (var extension in new string[]{ "index.htm", "default.html" })
				Util.MimeTypeFor(extension).ShouldEqual(MediaTypeNames.Text.Html);

			foreach (var extension in new string[]{ "rtf" })
				Util.MimeTypeFor(extension).ShouldEqual(MediaTypeNames.Text.RichText);

			foreach (var extension in new string[]{ "jpg", "jpeg" })
				Util.MimeTypeFor(extension).ShouldEqual(MediaTypeNames.Image.Jpeg);

			foreach (var extension in new string[]{ "gif" })
				Util.MimeTypeFor(extension).ShouldEqual(MediaTypeNames.Image.Gif);

			foreach (var extension in new string[]{ "png" })
				Util.MimeTypeFor(extension).ShouldEqual("image/png");

			foreach (var extension in new string[]{ "dll", "exe" })
				Util.MimeTypeFor(extension).ShouldEqual(MediaTypeNames.Application.Octet);

			// unknown extensions default to Octet
			foreach (var extension in new string[]{ "foo", "bar", "unknown" })
				Util.MimeTypeFor(extension).ShouldEqual(MediaTypeNames.Application.Octet);
		}
	}
}
