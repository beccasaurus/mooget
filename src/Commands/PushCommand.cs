using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using MooGet.Options;

namespace MooGet.Commands {

	public class PushCommand {

		[Command(Name = "push", Description = "Push a package to source")]
		public static object Run(string[] args) {
			if (args.Length == 1 && args[0].ToLower().EndsWith(".nupkg"))
				return Push(args[0]);
			else
				return "Usage: moo push foo.nupkg";
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
			string responseText = null;
			var response = UploadNupkg(nupkg, "http://meerkat:3000/packages?auth_token=rxDoWk3RxcDTIhxW2eac", out responseText);
			return string.Format("Upload Status {0}\n\n{1}", response.StatusCode, responseText);	
		}

		static HttpWebResponse UploadNupkg(string nupkg, string url, out string responseText) {
			NameValueCollection postData = new NameValueCollection();
			postData.Add("nuspec", Util.ReadNuspecInNupkg(nupkg));
			var response = Upload.PostFile(new Uri(url), postData, Path.GetFullPath(nupkg), null, null, null, null) as HttpWebResponse;
			using (var reader = new StreamReader(response.GetResponseStream()))
				responseText = reader.ReadToEnd();
			return response;
		}
	}
}
