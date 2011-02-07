using System;
using System.IO;
using NUnit.Framework;
using MooGet;

namespace MooGet.Specs.Core {

	[TestFixture]
	public class IFileSpec : Spec {

		public class FileClass : IFile {
			public FileClass(string path) { Path = path; }
			public string Path { get; set; }
		}

		string dir_1 = PathToTemp("my/dir");
		string dir_2 = PathToTemp("my/other/dir");

		string[] AllFiles { get { return Directory.GetFiles(PathToTemp(), "*", SearchOption.AllDirectories); } }

		IFile Dir1File(string name) { return new FileClass(Path.Combine(dir_1, name)); }
		IFile Dir2File(string name) { return new FileClass(Path.Combine(dir_2, name)); }

		[SetUp]
		public void Before() {
			base.BeforeEach();
			Directory.CreateDirectory(dir_1);
			Directory.CreateDirectory(dir_2);
		}

		[Test]
		public void FileName() {
			Dir1File("foo").FileName().ShouldEqual("foo");
			Dir1File("foo.txt").FileName().ShouldEqual("foo.txt");
		}

		[Test]
		public void DirectoryName() {
			Dir1File("foo").DirectoryName().ShouldEqual(dir_1);
			Dir2File("foo").DirectoryName().ShouldEqual(dir_2);

			// shortcut
			Dir1File("foo").DirName().ShouldEqual(dir_1);
			Dir2File("foo").DirName().ShouldEqual(dir_2);
		}

		[Test]
		public void Touch() {
			AllFiles.Should(Be.Empty);

			Dir1File("foo").Touch();

			AllFiles.ShouldEqual(new string[]{ Path.Combine(dir_1, "foo") });

			Dir2File("README.txt").Touch();

			AllFiles.ShouldEqual(new string[]{ Path.Combine(dir_1, "foo"), Path.Combine(dir_2, "README.txt") });
		}

		[Test]
		public void Exists() {
			var file = Dir1File("foo");
			file.Exists().Should(Be.False);
			file.Touch();
			file.Exists().Should(Be.True);
		}

		[Test]
		public void Copy() {
			var file = Dir1File("foo").Touch();
			AllFiles.ShouldEqual(new string[]{ Path.Combine(dir_1, "foo") });

			// copy to new name
			file.Copy(dir_1, "bar.txt");
			AllFiles.ShouldEqual(new string[]{ Path.Combine(dir_1, "bar.txt"), Path.Combine(dir_1, "foo") });

			// copy to directory (no new name)
			file.Copy(dir_2);
			AllFiles.ShouldEqual(new string[]{ Path.Combine(dir_1, "bar.txt"), Path.Combine(dir_1, "foo"), Path.Combine(dir_2, "foo") });

			// TODO test overwrite (File.Copy(x, y, true))
		}

		[Test]
		public void Move() {
			var file = Dir1File("foo").Touch();
			AllFiles.ShouldEqual(new string[]{ Path.Combine(dir_1, "foo") });

			// move to new name
			file.Move(dir_1, "bar.txt");
			AllFiles.ShouldEqual(new string[]{ Path.Combine(dir_1, "bar.txt") });

			// move to directory (no new name)
			file.Move(dir_2);
			AllFiles.ShouldEqual(new string[]{ Path.Combine(dir_2, "bar.txt") });
		}

		[Test]
		public void Delete() {
			var file1 = Dir1File("foo").Touch();
			var file2 = Dir2File("bar").Touch();
			AllFiles.ShouldEqual(new string[]{ Path.Combine(dir_1, "foo"), Path.Combine(dir_2, "bar") });

			file1.Delete();
			AllFiles.ShouldEqual(new string[]{ Path.Combine(dir_2, "bar") });

			file2.Delete();
			AllFiles.ShouldEqual(new string[]{ });
		}
	}
}
