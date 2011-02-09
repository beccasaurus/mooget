using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class IDirectorySpec : Spec {

		public class FileClass : IFileSpec.FileClass, IFile {
			public FileClass(string path) : base(path) {}
		}

		public class DirClass : IDirectory {
			public DirClass(string path) { Path = path; }
			public string Path { get; set; }
		}

		string dir_1 = PathToTemp("base/dir");
		string dir_2 = PathToTemp("base/other/dir");

		string[] AllDirectories { get { return Directory.GetDirectories(PathToTemp("base"), "*", SearchOption.AllDirectories); } }

		IDirectory Dir1Dir(string name) { return new DirClass(Path.Combine(dir_1, name)); }
		IDirectory Dir2Dir(string name) { return new DirClass(Path.Combine(dir_2, name)); }

		[SetUp]
		public void Before() {
			base.BeforeEach();
			Directory.CreateDirectory(dir_1);
			Directory.CreateDirectory(dir_2);
		}

		[Test]
		public void Name() {
			Dir1Dir("foo").Name().ShouldEqual("foo");
			Dir1Dir("bar/whatever").Name().ShouldEqual("whatever");
		}

		[Test]
		public void Files() {
			var dir = Dir1Dir("foo").Create();
			dir.Files().Should(Be.Empty);

			dir.GetFile("README").Touch();

			dir.Files().ToStrings().ShouldEqual(new List<string>{ PathToTemp("base/dir/foo/README") });

			dir.GetDir("some/sub/dir").Create().GetFile("File.txt").Touch();

			dir.Files().ToStrings().ShouldEqual(new List<string>{ PathToTemp("base/dir/foo/README"), PathToTemp("base/dir/foo/some/sub/dir/File.txt") });
		}

		[Test]
		public void Directories() {
			new RealDirectory(PathToTemp("base")).Directories().ToStrings().ShouldEqual(new List<string>{
				PathToTemp("base/dir"),
				PathToTemp("base/other"),
				PathToTemp("base/other/dir"),
			});
			new RealDirectory(PathToTemp("base")).Dirs().ToStrings().ShouldEqual(new List<string>{
				PathToTemp("base/dir"),
				PathToTemp("base/other"),
				PathToTemp("base/other/dir"),
			});
		}

		[Test]
		public void Search() {
			var dir = Dir1Dir("foo").Create();
			dir.GetFile("index.html").Touch();
			var stuff = dir.GetDir("stuff").Create();
			stuff.GetFile("hi.html").Touch();
			stuff.GetFile("foo.txt").Touch();
			var another = stuff.GetDir("another").Create();
			another.GetFile("hi.txt").Touch();
			another.GetFile("more.html").Touch();

			dir.Files().ToStrings().ShouldEqual(new List<string>{
				PathToTemp("base/dir/foo/index.html"),
				PathToTemp("base/dir/foo/stuff/foo.txt"),
				PathToTemp("base/dir/foo/stuff/hi.html"),
				PathToTemp("base/dir/foo/stuff/another/hi.txt"),
				PathToTemp("base/dir/foo/stuff/another/more.html")
			});

			dir.Search("index.html").ToStrings().ShouldEqual(new List<string>{
				PathToTemp("base/dir/foo/index.html")
			});
			dir.Search("*.html").ToStrings().ShouldEqual(new List<string>{
				PathToTemp("base/dir/foo/index.html")
			});
			dir.Search("*/*.html").ToStrings().ShouldEqual(new List<string>{
				PathToTemp("base/dir/foo/stuff/hi.html")
			});
			dir.Search("**/*.html").ToStrings().ShouldEqual(new List<string>{
				PathToTemp("base/dir/foo/stuff/hi.html"),
				PathToTemp("base/dir/foo/stuff/another/more.html")
			});
			dir.Search("**.html").ToStrings().ShouldEqual(new List<string>{
				PathToTemp("base/dir/foo/index.html"),
				PathToTemp("base/dir/foo/stuff/hi.html"),
				PathToTemp("base/dir/foo/stuff/another/more.html")
			});
		}

		[Test]
		public void Create() {
			AllDirectories.Length.ShouldEqual(3); // the 2 base directories
			AllDirectories.ShouldContain(PathToTemp("base/dir"));
			AllDirectories.ShouldContain(PathToTemp("base/other"));
			AllDirectories.ShouldContain(PathToTemp("base/other/dir"));

			Dir1Dir("foo").Create();

			AllDirectories.Length.ShouldEqual(4);
			AllDirectories.ShouldContain(PathToTemp("base/dir"));
			AllDirectories.ShouldContain(PathToTemp("base/dir/foo"));
			AllDirectories.ShouldContain(PathToTemp("base/other"));
			AllDirectories.ShouldContain(PathToTemp("base/other/dir"));

			Dir2Dir("sub/dir").Create();

			AllDirectories.Length.ShouldEqual(6);
			AllDirectories.ShouldContain(PathToTemp("base/dir"));
			AllDirectories.ShouldContain(PathToTemp("base/dir/foo"));
			AllDirectories.ShouldContain(PathToTemp("base/other"));
			AllDirectories.ShouldContain(PathToTemp("base/other/dir"));
			AllDirectories.ShouldContain(PathToTemp("base/other/dir/sub"));
			AllDirectories.ShouldContain(PathToTemp("base/other/dir/sub/dir"));
		}

		[Test]
		public void Exists() {
			var dir = Dir1Dir("foo");
			dir.Exists().Should(Be.False);
			dir.Create();
			dir.Exists().Should(Be.True);
		}

		[Test]
		public void Copy() {
			// Make the directory to copy ...
			var dir = Dir1Dir("foo").Create();
			dir.GetFile("FILE").Touch();
			var subdir = dir.GetDir("subdir").Create();
			subdir.GetFile("MyFile").Touch();
			var subsubdir = subdir.GetDir("another").Create();
			subsubdir.GetFile("w00t").Touch();

			Directory.CreateDirectory(PathToTemp("base/other/dir/foo")); // <--- create new directory to copy into

			File.Exists(PathToTemp(      "base/dir/foo/FILE"                      )).Should(Be.True);
			File.Exists(PathToTemp(      "base/dir/foo/subdir/MyFile"             )).Should(Be.True);
			File.Exists(PathToTemp(      "base/dir/foo/subdir/another/w00t"       )).Should(Be.True);
			Directory.Exists(PathToTemp( "base/other/dir/foo"                     )).Should(Be.True);  // directory exists!
			File.Exists(PathToTemp(      "base/other/dir/foo/FILE"                )).Should(Be.False); // but content doesnt yet
			File.Exists(PathToTemp(      "base/other/dir/foo/subdir/MyFile"       )).Should(Be.False);
			File.Exists(PathToTemp(      "base/other/dir/foo/subdir/another/w00t" )).Should(Be.False);

			// Copy to an existing directory.  The directory is copied *into* the existing directory.
			dir.Copy(PathToTemp("base/other/dir"));

			File.Exists(PathToTemp(      "base/dir/foo/FILE"                      )).Should(Be.True);  // old files are STILL there
			File.Exists(PathToTemp(      "base/dir/foo/subdir/MyFile"             )).Should(Be.True);
			File.Exists(PathToTemp(      "base/dir/foo/subdir/another/w00t"       )).Should(Be.True);
			Directory.Exists(PathToTemp( "base/other/dir/foo"                     )).Should(Be.True);  // directory exists!
			File.Exists(PathToTemp(      "base/other/dir/foo/FILE"                )).Should(Be.True);  // and so does content
			File.Exists(PathToTemp(      "base/other/dir/foo/subdir/MyFile"       )).Should(Be.True);
			File.Exists(PathToTemp(      "base/other/dir/foo/subdir/another/w00t" )).Should(Be.True);
			Directory.Exists(PathToTemp( "new"                                    )).Should(Be.False); // new directory doesn't exist

			// Copy to a new directory.  The directory is copied exactly there.
			dir.Copy(PathToTemp("new"));

			Directory.Exists(PathToTemp( "new"                                    )).Should(Be.True); // new directory doesn't exist
			File.Exists(PathToTemp(      "new/FILE"                           )).Should(Be.True); // old files are STILL there
			File.Exists(PathToTemp(      "new/subdir/MyFile"                  )).Should(Be.True);
			File.Exists(PathToTemp(      "new/subdir/another/w00t"            )).Should(Be.True);
			File.Exists(PathToTemp(      "base/dir/foo/subdir/another/w00t"       )).Should(Be.True); // first directory still there
			File.Exists(PathToTemp(      "base/other/dir/foo/subdir/another/w00t" )).Should(Be.True); // second directory still there
		}

		[Test]
		public void Move() {
			Directory.Exists(PathToTemp("base/dir/foo")).Should(Be.False);
			var dir = Dir1Dir("foo").Create();
			dir.GetFile("FILE").Touch();
			Directory.Exists(PathToTemp("base/dir/foo")).Should(Be.True);
			File.Exists(PathToTemp("base/dir/foo/FILE")).Should(Be.True);

			// move to existing directory moves it INTO directory
			Directory.Exists(PathToTemp("base/other/dir")).Should(Be.True);
			Directory.Exists(PathToTemp("base/other/dir/foo")).Should(Be.False);
			File.Exists(PathToTemp("base/other/dir/foo/FILE")).Should(Be.False);
			dir.Move(PathToTemp("base/other/dir"));
			Directory.Exists(PathToTemp("base/other/dir/foo")).Should(Be.True); // it moved INTO that dir
			File.Exists(PathToTemp("base/other/dir/foo/FILE")).Should(Be.True);
			Directory.Exists(PathToTemp("base/dir/foo")).Should(Be.False); // the original dir went away
			dir.Path.ShouldEqual(PathToTemp("base/other/dir/foo")); // the Path was updated

			// move to new directory, move it there
			Directory.Exists(PathToTemp("new")).Should(Be.False);
			dir.Move(PathToTemp("new"));
			Directory.Exists(PathToTemp("new")).Should(Be.True);
			File.Exists(PathToTemp("new/FILE")).Should(Be.True);
			Directory.Exists(PathToTemp("base/other/dir/foo")).Should(Be.False); // the last directory went away
		}

		[Test]
		public void Delete() {
			var dir = Dir1Dir("foo").Create();
			dir.Exists().Should(Be.True);
			dir.Delete();
			dir.Exists().Should(Be.False);
		}
	}
}
