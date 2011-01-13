using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MooGet.Options;

namespace MooGet.Commands {

	public class PushCommand {

		[Command(Name = "push", Description = "Push a package to source")]
		public static object Run(string[] args) {
			//if (args.Length == 1 && args[0].ToLower().EndsWith(".nupkg"))
			return Push(args[0]);
			//else
			//	return "Usage: moo push foo.nupkg";
		}

		static byte[] GetAllBytes(string nupkg) {
			// byte[] bytes;
			// using (var stream = new FileStream(nupkg, FileMode.Open, FileAccess.Read)) {
			// 	bytes = new byte[stream.Length];
			// 	stream.Read(bytes, 0, bytes.Length);
			// }
			// return bytes;

			FileStream fs = new FileStream(nupkg, FileMode.Open, FileAccess.Read);
			byte[] data = new byte[fs.Length];
			fs.Read(data, 0, data.Length);
			fs.Close();
			return data;
		}

		static string Push(string nupkg) {
			// var url = "http://localhost:3000/packages?auth_token=dx1vnLEJ1d2IDCOkVKoB";
			var url = "http://localhost:9393/";

			string filePath = Path.GetFullPath(nupkg);
			string responseText;
			Upload.PostFile(new Uri(url), null, filePath, null, null, null, null);
			return "hi\n";

			/*
			System.Net.ServicePointManager.Expect100Continue = false;


			var parameters = new Dictionary<string, object>();
			//parameters.Add("nuspec", "hello world");
			//parameters.Add("nuspec", Util.ReadNuspecInNupkg(nupkg));
			//parameters.Add("file", new FormUpload.FileParameter(File.ReadAllBytes(nupkg), Path.GetFileName(nupkg), "application/octet-stream"));
			//parameters.Add("file", new FormUpload.FileParameter(GetAllBytes(nupkg), Path.GetFileName(nupkg), "text/plain"));
			parameters.Add("file", new FormUpload.FileParameter(GetAllBytes(nupkg), Path.GetFileName(nupkg), "text/plain"));
			//parameters.Add("file", new FormUpload.FileParameter(Encoding.UTF8.GetBytes(Util.ReadFile(nupkg)), Path.GetFileName(nupkg), "plain/text"));
			//parameters.Add("file", new FormUpload.FileParameter(GetAllBytes(nupkg), Path.GetFileName(nupkg), "plain/text"));
			FormUpload.MultipartFormDataPost(url, null, null, parameters);
			//var response = FormUpload.MultipartFormDataPost(url, null, null, parameters);
			return "hi";
			//return string.Format("Response: {0}", response.StatusCode);
			//*/
		}
	}
}
