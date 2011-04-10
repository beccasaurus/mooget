using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Clide {

	// TODO - I don't think I like the static methods ... they're sorta helpful, but not really needed.
	//        Might wanna delete them and re-create them later once we realize exactly which methods are useful?

	/// <summary>Class for handling "PP" (Project Properties or Pre-Processed) templates using project configuration variables</summary>
	public class PP : Tokenizer {
		public PP() : base() {
			FileExtensionToProcess = "pp";
		}
		public PP(string text) : this(){
			Text = text;
		}
		public PP(Project project) : this() {
			Project = project;
		}

		/// <summary>The project associated with this PP processor.</summary>
		public virtual Project Project { get; set; }

		/// <summary>Returns a Dictionary from the given project's properties that can be used as tokens for string replacement</summary>
		public virtual Dictionary<string,string> ProjectToDictionary(Project project, string config = null, bool includeGlobal = true) {
			var properties = new Dictionary<string,string>();
			if (project == null) return properties;

			if (config == null) config = project.DefaultConfigurationName;

			if (includeGlobal && project.Global != null)
				foreach (var property in project.Global.Properties)
					properties[property.Name] = property.Text;

			if (! string.IsNullOrEmpty(config) && project.Config[config] != null)
				foreach (var property in project.Config[config].Properties)
					properties[property.Name] = property.Text;

			// the 'configuration' token is set in the global properties, but we should override it if a config is passed in
			if (! string.IsNullOrEmpty(config)) {
				var configToken = properties.Select(prop => prop.Key).FirstOrDefault(key => key.ToLower() == "configuration");
				if (configToken == null) configToken = "configuration";
				properties[configToken] = config;
			}

			return properties;
		}

		/// <summary>Returns the result of replacing the project properties from the given project in Text</summary>
		public virtual string Render(Project project, string config = null, bool includeGlobal = true) {
			return Render(Text, project, config, includeGlobal);
		}

		/// <summary>Returns the result of replacing the project properties from the given project in the given text</summary>
		public virtual string Render(string text, Project project, string config = null, bool includeGlobal = true) {
			return Render(text, ProjectToDictionary(project, config, includeGlobal));
		}

		/// <summary>Processes the provided directory and its pp files.</summary>
		public virtual string ProcessDirectory(string path, string outputDir = null, Project project = null, object tokens = null) {
			if (project == null) project = Project;
			// TODO DRY (ProcessFile uses the same)
			var allTokens = (project == null) ? new Dictionary<string,string>() : ProjectToDictionary(project);
			if (tokens != null)
				foreach (var item in ToDictionary(tokens))
					allTokens[item.Key] = (item.Value == null) ? null : item.Value.ToString();
			return base.ProcessDirectory(path: path, outputDir: outputDir, tokens: allTokens);
		}

		/// <summary>Processes the provided pp file.</summary>
		public virtual string ProcessFile(string path, string outputPath = null, Project project = null, object tokens = null) {
			if (project == null) project = Project;
			var allTokens = (project == null) ? null : ProjectToDictionary(project);
			if (tokens != null)
				foreach (var item in ToDictionary(tokens))
					allTokens[item.Key] = (item.Value == null) ? null : item.Value.ToString();
			return base.ProcessFile(path: path, outputPath: outputPath, tokens: allTokens);
		}

		/// <summary>Helper method for replacing the project properties from the given project in the given string</summary>
		public static string Replace(string text, Project project, string config = null, bool includeGlobal = true) {
			return new PP().Render(text, project, config, includeGlobal);
		}
	}

	/// <summary>Class for replacing tokens in text</summary>
	public class Tokenizer {

        /// <summary>SImple delegate for matching on paths.  Given a path, you return a bool for whether that path matches.</summary>
        public delegate bool MatchPath(string path);

		public static string DefaultLeftDelimiter           = "$";
		public static string DefaultRightDelimiter          = "$";
        public static string DefaultRegexSafeLeftDelimiter  = "\\$";
		public static string DefaultRegexSafeRightDelimiter = "\\$";
		public static bool   DefaultCaseInsensitive         = true;
        public static bool   DefaultSkipIfMissingTokens     = true;

		public Tokenizer() {
			LeftDelimiter           = Tokenizer.DefaultLeftDelimiter;
			RightDelimiter          = Tokenizer.DefaultRightDelimiter;
			CaseInsensitive         = Tokenizer.DefaultCaseInsensitive;
            SkipIfMissingTokens     = Tokenizer.DefaultSkipIfMissingTokens;
            RegexSafeLeftDelimiter  = Tokenizer.DefaultRegexSafeLeftDelimiter;
            RegexSafeRightDelimiter = Tokenizer.DefaultRegexSafeRightDelimiter;
            Excludes                = new List<MatchPath>();
		}

		public Tokenizer(string text) : this() {
			Text = text;
		}

		string _workingDirectory;
        Regex _tokenFindingRegex;

        /// <summary>A list of lambdas that, if they return false, will exlude the path that we're trying to render</summary>
        public virtual List<MatchPath> Excludes { get; set; }

        /// <summary>If this returns true, we don't render this file/directory when we ProcessFile/ProcessDirectory.  Uses Excludes.</summary>
        public virtual bool PathExcluded(string path) {
            return Excludes.Any(exclude => exclude(path));
        }

		/// <summary>The string to look for at the left of a token</summary>
		public virtual string LeftDelimiter { get; set; }

		/// <summary>The string to look for at the right of a token</summary>
		public virtual string RightDelimiter { get; set; }

        /// <summary>A version of the string to look for at the left of a token that has been excaped and can be used in Regex</summary>
		public virtual string RegexSafeLeftDelimiter { get; set; }

		/// <summary>A version of the string to look for at the right of a token that has been excaped and can be used in Regex</summary>
		public virtual string RegexSafeRightDelimiter { get; set; }

        /// <summary>The regular expression we use in Tokens() to find tokens.  Default uses RegexSafeLeftDelimiter and RegexSafeRightDelimiter.</summary>
        public virtual Regex TokenFindingRegex {
            get { return _tokenFindingRegex ?? (_tokenFindingRegex = new Regex(RegexSafeLeftDelimiter + "([^\\s" + RegexSafeRightDelimiter + "]+)" + RegexSafeRightDelimiter)); }
            set { _tokenFindingRegex = value; }
        }

		/// <summary>The text that we want to replace tokens in</summary>
		public virtual string Text { get; set; }

		/// <summary>The current working directory (defaults to Directory.GetCurrentDirectory())</summary>
		public virtual string WorkingDirectory {
			get { return _workingDirectory ?? (_workingDirectory = Directory.GetCurrentDirectory()); }
			set { _workingDirectory = value; }
		}

		/// <summary>Whether or not we should replace tokens case insensitively</summary>
		public virtual bool CaseInsensitive { get; set; }

        /// <summary>If set to true, ProcessDirectory() will skip files/directories with tokens in the name that aren't found</summary>
        /// <remarks>Default: true</remarks>
        public virtual bool SkipIfMissingTokens { get; set; }

		/// <summary>The file extension that we should process.  If this is set, we won't process files without this extension.</summary>
		public virtual string FileExtensionToProcess { get; set; }

		/// <summary>Returns the result of replacing the given tokens in Text</summary>
		public virtual string Render(Dictionary<string,object> tokens) {
			return Render(Text, tokens);
		}

		/// <summary>Returns the result of replacing the given tokens in the given string</summary>
		public virtual string Render(string text, Dictionary<string,object> tokens) {
			var stringTokens = new Dictionary<string,string>();
			foreach (var token in tokens)
				stringTokens[token.Key] = (token.Value == null) ? string.Empty : token.Value.ToString();
			return Render(text, stringTokens);
		}

		/// <summary>Returns the result of replacing the given tokens in the given string</summary>
		public virtual string Render(string text, Dictionary<string,string> tokens) {
			var builder = new StringBuilder(text);
            if (tokens == null) return builder.ToString();
			foreach (var token in tokens)
				ReplaceToken(builder, key: token.Key, value: token.Value.ToString());
			return builder.ToString();
		}

		/// <summary>Given a StringBuilder, replaces the given key (wrapped with LeftDelimiter and RightDelimiter) with the value</summary>
		public virtual void ReplaceToken(StringBuilder builder, string key, string value, bool? caseInsensitive = null) {
			ReplaceString(builder, string.Format("{0}{1}{2}", LeftDelimiter, key, RightDelimiter), value, caseInsensitive);
		}

		/// <summary>Given a StringBuilder, replaces the given key with the value</summary>
		public virtual void ReplaceString(StringBuilder builder, string key, string value, bool? caseInsensitive = null) {
			if (caseInsensitive == null) caseInsensitive = CaseInsensitive;
			var comparison = ((bool) caseInsensitive) ? StringComparison.OrdinalIgnoreCase : StringComparison.InvariantCulture;
			ReplaceString(builder, key, value, comparison);
		}

		/// <summary>Given a StringBuilder, replaces the given key with the value</summary>
		/// <remarks>
		/// This method does the real work of finding and replacing strings.
		///
		/// Note: this currently loops and doesn't stop until it can't find the key anymore.  It doesn't move through the text.
		/// Note: this also calls StringBuilder.ToString() more often than I'd like so it can execute IndexOf() and Substring()
		/// </remarks>
		public virtual void ReplaceString(StringBuilder builder, string key, string value, StringComparison comparison) {
			if (builder == null || builder.Length == 0 || string.IsNullOrEmpty(key)) return;

			var original = builder.ToString();
			var index    = original.IndexOf(key, comparison);

			while (index > -1) {
				// We have an index!  Let's replace all instances of the found key with the value ...
				var foundKey = original.Substring(index, key.Length);

				builder.Replace(foundKey, value); // let the builder replace all instances of the key we found

				// Get the new string and look for another index
				original = builder.ToString();
				index    = original.IndexOf(key, comparison);
			}
		}

		/// <summary>Processes the given directory (with an optional output path and file extension to check for)</summary>
		/// <remarks>
		/// If the output path is null, we output to WorkingDirectory (using the same file name).
		/// If the fileExtension is null, we use FileExtensionToProcess or we process the file anyway.
		/// </remarks>
		public virtual string ProcessDirectory(string path, Dictionary<string,string> tokens, string outputDir = null, string fileExtension = null) {
            if (PathExcluded(path))       return null; // Skip this path and return null to indicate that we didn't render it
			if (! Directory.Exists(path)) throw new DirectoryNotFoundException("Could not find directory to process: " + path);
			if (outputDir == null)        outputDir = WorkingDirectory;

			// Create the outputDir and all necessary subdirectories (replacing tokens in directory names)
			Directory.CreateDirectory(outputDir);
			outputDir = Path.GetFullPath(outputDir);
			path      = Path.GetFullPath(path);

            // Directories
			foreach (var dir in Directory.GetDirectories(path, "*", SearchOption.AllDirectories)) {
                if (PathExcluded(dir)) continue; // Skip this path
				var relative = dir.Substring(path.Length).TrimStart(@"\/".ToCharArray());
				relative     = Replace(relative, tokens);
                if (SkipIfMissingTokens && Tokens(relative).Any()) continue; // there are still tokens in the filename ... next!
				Directory.CreateDirectory(Path.Combine(outputDir, relative));
			}

            // Files
            foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories)) {
                if (PathExcluded(file)) continue; // Skip this path
				var relative = file.Substring(path.Length).TrimStart(@"\/".ToCharArray());
				relative     = Replace(relative, tokens);
                var output   = Path.Combine(outputDir, relative);
                if (SkipIfMissingTokens && Tokens(relative).Any())     continue; // there are still tokens in the filename ... next!
                if (! Directory.Exists(Path.GetDirectoryName(output))) continue; // if the directory was skipped, we skip this file too
                ProcessFile(path: file, outputPath: output, tokens: tokens, fileExtension: fileExtension);
			}

			return outputDir;
		}

		/// <summary>Processes the given file (with an optional output path and file extension to check for)</summary>
		/// <remarks>
		/// If the output path is null, we output to WorkingDirectory (using the same file name).
		/// If the fileExtension is null, we use FileExtensionToProcess or we process the file anyway.
		/// </remarks>
		public virtual string ProcessFile(string path, Dictionary<string,string> tokens, string outputPath = null, string fileExtension = null) {
            if (PathExcluded(path))           return null; // Skip this path and return null to indicate that we didn't render it
			if (! File.Exists(path))          throw new FileNotFoundException("Could not find file to process", path);
			if (outputPath == null)           outputPath    = Path.Combine(WorkingDirectory, Path.GetFileName(path));
			if (Directory.Exists(outputPath)) outputPath    = Path.Combine(outputPath, Path.GetFileName(path));
			if (fileExtension == null)        fileExtension = FileExtensionToProcess;

			if (fileExtension != null && ! path.ToLower().EndsWith(fileExtension.ToLower())) {
				// We specified a file extension, but this file doesn't end with it.  Simply copy the file (without processing it).
				File.Copy(path, outputPath, true);
			} else {
				// Remove dot and extension if output path has extension
				if (fileExtension != null && outputPath.ToLower().EndsWith(fileExtension.ToLower()))
					outputPath = outputPath.Substring(0, outputPath.Length - fileExtension.Length - 1);

				outputPath = Replace(outputPath, tokens);

				var text = Replace(File.ReadAllText(path), tokens);
				using (var writer = new StreamWriter(outputPath))
					writer.Write(text);
			}

			return outputPath;
		}

        /// <summary>Returns a list of what we believe to be tokens from the provided text.</summary>
        /// <remarks>
        /// NOTE: This does NOT use LeftDelimiter/RightDelimiter.
        ///       It uses RegexSafeLeftDelimiter/RegexSafeRightDelimiter instead!
        ///       
        /// NOTE: This does NOT support any spaces in tokens.  Ideally, tokens should never have spaces in them.
        /// </remarks>
        public virtual List<string> Tokens(string text) {
            var tokens = new List<string>();
            foreach (Match match in TokenFindingRegex.Matches(text))
                tokens.Add(match.Groups[1].ToString());
            return tokens;
        }

		/// <summary>Helper method for replacing the given tokens in the given string</summary>
		public static string Replace(string text, Dictionary<string,object> tokens) {
			return new Tokenizer().Render(text, tokens);
		}

		/// <summary>Helper method for replacing the given tokens in the given string</summary>
		public static string Replace(string text, Dictionary<string,string> tokens) {
			return new Tokenizer().Render(text, tokens);
		}

		/// <summary>Helper method for replacing the given tokens (as an anonymous object) in the given string</summary>
		public static string Replace(string text, object tokens) {
			return new Tokenizer().Render(text, ToDictionary(tokens));
		}

		/// <summary>Given an anonymous object, this returns a Dictionary of strings to objects</summary>	
		public static Dictionary<string, object> ToDictionary(object anonymousType) {
			if (anonymousType == null)                       return null;
			if (anonymousType is Dictionary<string, object>) return anonymousType as Dictionary<string, object>;
			var dict = new Dictionary<string, object>();
			if (anonymousType is Dictionary<string, string>) {
				foreach (var item in anonymousType as Dictionary<string, string>)
					dict[item.Key] = item.Value;
				return dict;
			}
			var attr = BindingFlags.Public | BindingFlags.Instance;
			foreach (var property in anonymousType.GetType().GetProperties(attr))
				if (property.CanRead)
					dict.Add(property.Name, property.GetValue(anonymousType, null));
			return dict;
		} 
	}
}
