﻿using RimuTec.Faker.Extensions;
using RimuTec.Faker.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using YamlDotNet.RepresentationModel;

namespace RimuTec.Faker
{
   /// <summary>
   /// Base class for all fake data generators.
   /// </summary>
   public class GeneratorBase<T> where T : class
   {
      internal GeneratorBase() { }

      /// <summary>
      /// Parses a template that may contain tokens like "#{address.full_address}" and
      /// either invokes generator methods or load content from yaml files as replacement
      /// for the token
      /// </summary>
      /// <param name="template"></param>
      /// <returns></returns>
      internal static string Parse(string template)
      {
         var clazz = new StackFrame(1).GetMethod().DeclaringType; // https://stackoverflow.com/a/171974/411428
         var matches = Regex.Matches(template, "#{([a-zA-Z._]{1,})}");
         for (var i = 0; i < matches.Count; i++)
         {
            string placeHolder = matches[i].Value;
            var token = matches[i].Groups[1].Value;
            if (!token.Contains("."))
            {
               // Prepend class name before fetching
               token = $"{typeof(T).Name.ToLower()}.{token}";
            }

            var className = token.Split('.')[0].ToPascalCasing();
            var method = token.Split('.')[1].ToPascalCasing();

            string replacement = null;
            var type = Array.Find(typeof(YamlLoader).Assembly.GetTypes(), t => string.Compare(t.Name, className, true) == 0);
            if (type != null)
            {
               var methodInfo = type.GetMethod(method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
               if (methodInfo != null)
               {
                  // invoke statics method, if needed with default parameter values
                  // Ref: https://stackoverflow.com/a/9916197/411428
                  var paramCount = methodInfo.GetParameters().Length;
                  object[] parameters = Enumerable.Repeat(Type.Missing, paramCount).ToArray();
                  replacement = methodInfo.Invoke(null, parameters).ToString();
               }
            }
            if (string.IsNullOrWhiteSpace(replacement))
            {
               replacement = Fetch(token);
            }

            template = ReplaceFirst(template, placeHolder, replacement);
         }
         // Parsing may have replace a place holder with a different place holder:
         if(Regex.Matches(template, "#{([a-zA-Z._]{1,})}").Count > 0) {
            template = Parse(template);
         }
         return template;
      }

      private static string ReplaceFirst(string text, string search, string replace)
      {
         // Source for this method: https://stackoverflow.com/a/8809437/411428
         int pos = text.IndexOf(search);
         if (pos < 0)
         {
            return text;
         }
         return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
      }

      internal static string Fetch(string locator)
      {
         var localeName = Config.Locale;

         // if locale hasn't been loaded yet, now is a good time
         LoadLocale(localeName);

         // at this point the locale is in the dictionary
         YamlNode fakerNode;
         var key = localeName.ToLower();
         try
         {
            fakerNode = Library._dictionary[key];
            var locatorParts = locator.Split('.');
            return Fetch(fakerNode[locatorParts[0].ToLowerInvariant()], locatorParts.Skip(1).ToArray());
         }
         //#### // example: if 'de-CH' cannot be found it should fall back to 'de' and only failing that after that to 'en'.
         catch
         {
            // fall back to locale "en"
            LoadLocale("en");
            var fileName = typeof(T).Name.FromPascalCasing();
            fileName = $"en.{fileName}";

            key = fileName.ToPascalCasing().ToLower();

            fakerNode = Library._dictionary[key];
            var locatorParts = locator.Split('.');
            return Fetch(fakerNode[locatorParts[0].ToLowerInvariant()], locatorParts.Skip(1).ToArray());
         }
      }

      internal static void LoadLocale(string localeName)
      {
         var fileName = localeName;
         if (localeName == "en")
         {
            fileName = typeof(T).Name.FromPascalCasing();
            fileName = $"{localeName}.{fileName}";
         }
         var key = fileName.ToPascalCasing().ToLower();

         if (!Library._dictionary.ContainsKey(key))
         {
            var executingAssembly = Assembly.GetExecutingAssembly();
            string embeddedResourceFileName = $"RimuTec.Faker.locales.{fileName}.yml";
            if (executingAssembly.GetManifestResourceNames().Contains(embeddedResourceFileName))
            {
               using (var reader = YamlLoader.OpenText(embeddedResourceFileName))
               {
                  AddYamlToLibrary(localeName, key, reader);
               }
            }
            else
            {
               var assemblyLocation = new FileInfo(executingAssembly.Location);
               var fileNamePath = Path.Combine(assemblyLocation.DirectoryName, $"{fileName}.yml");
               if(!File.Exists(fileNamePath))
               {
                  fileNamePath = Path.Combine(assemblyLocation.DirectoryName, "locales", $"{fileName}.yml");
               }
               if (File.Exists(fileNamePath))
               {
                  var yamlContent = File.ReadAllText(fileNamePath);
                  using (var reader = new StringReader(yamlContent))
                  {
                     AddYamlToLibrary(localeName, key, reader);
                  }
               }
            }
         }
      }

      private static void AddYamlToLibrary(string localeName, string key, TextReader reader)
      {
         var stream = new YamlStream();
         stream.Load(reader);
         YamlNode rootNode = stream.Documents[0].RootNode;
         try
         {
            var localeNode = rootNode[localeName];
            YamlNode fakerNode = localeNode["faker"];
            Library._dictionary.Add(key, fakerNode);
         }
         catch
         {
            throw new FormatException($"File for locale '{localeName}' has an invalid format. [Code 201213-1413]");
         }
      }

      private static string Fetch(YamlNode yamlNode, string[] locatorParts)
      {
         if (locatorParts.Length > 0)
         {
            return Fetch(yamlNode[locatorParts[0]], locatorParts.Skip(1).ToArray());
         }
         if (yamlNode is YamlSequenceNode sequenceNode)
         {
            IEnumerable<string> enumerable = sequenceNode.Children.Select(c => c.ToString());
            _ = enumerable.ToArray();
            return enumerable.Sample();
         }
         else if (yamlNode is YamlScalarNode scalarNode)
         {
            return scalarNode.Value;
         }
         return string.Empty;
      }

      internal static List<KeyValuePair<string,string[]>> Translate(string locator) {
         var localeName = Config.Locale;

         // if locale hasn't been loaded yet, now is a good time
         LoadLocale(localeName);

         YamlNode fakerNode;
         var key = localeName;
         try
         {
            fakerNode = Library._dictionary[key];
            var locatorParts = locator.Split('.');
            return Translate(fakerNode[locatorParts[0]], locatorParts.Skip(1).ToArray());
         }
         catch
         {
            // fall back to locale "en"
            LoadLocale("en");
            key = $"en.{locator.Split('.')[0]}";
            fakerNode = Library._dictionary[key];
            var locatorParts = locator.Split('.');
            return Translate(fakerNode[locatorParts[0]], locatorParts.Skip(1).ToArray());
         }
      }

      private static List<KeyValuePair<string, string[]>> Translate(YamlNode yamlNode, string[] locatorParts)
      {
         if (locatorParts.Length > 0)
         {
            return Translate(yamlNode[locatorParts[0]], locatorParts.Skip(1).ToArray());
         }
         if(yamlNode is YamlSequenceNode sequenceNode)
         {
            var result = new List<KeyValuePair<string, string[]>>();
            var count = 0;
            foreach(var child in sequenceNode.Children)
            {
               if(child is YamlSequenceNode childNode)
               {
                  IEnumerable<string> enumerable = childNode.Children.Select(c => c.ToString());
                  var arr = enumerable.ToArray();
                  result.Add(new KeyValuePair<string, string[]>($"{count++}", arr));
               }
            }
            return result;
         }
         else if(yamlNode is YamlMappingNode mappingNode)
         {
            var result = new List<KeyValuePair<string, string[]>>();
            foreach(var pair in mappingNode)
            {
               if(pair.Value is YamlSequenceNode sequenceNodeInner)
               {
                  IEnumerable<string> enumerable = sequenceNodeInner.Children.Select(c => c.ToString());
                  var arr = enumerable.ToArray();
                  result.Add(new KeyValuePair<string, string[]>(pair.Key.ToString(), arr));
               }
            }
            return result;
         }
         return new List<KeyValuePair<string, string[]>>();
      }

      internal static List<string> FetchList(string locator)
      {
         LoadLocale(Config.Locale);

         var key = Config.Locale;
         if (Config.Locale == "en")
         {
            key = $"{Config.Locale}.{locator.Split('.')[0]}";
         }
         key = key.ToLower();
         if (Dictionary.ContainsKey(key))
         {
            try
            {
               var yamlNode = Dictionary[key];
               var locatorParts = locator.Split('.');
               return FetchList(yamlNode[locatorParts[0].ToLowerInvariant()], locatorParts.Skip(1).ToArray());
            }
            catch
            {
               // // fall back to locale "en"
               // LoadLocale("en");
               // var fileName = typeof(T).Name.FromPascalCasing();
               // fileName = $"en.{fileName}";

               // key = fileName.ToPascalCasing().ToLower();

               // fakerNode = Library._dictionary[key];
               // var locatorParts = locator.Split('.');
               // return Fetch(fakerNode[locatorParts[0].ToLowerInvariant()], locatorParts.Skip(1).ToArray());

               // Fall back to locale "en"
               //_ = GeneratorBase<Lorem>.Fetch(locator);

               LoadLocale("en");
               var fileName = typeof(T).Name.FromPascalCasing();
               fileName = $"en.{fileName}";
               key = fileName.ToPascalCasing().ToLower();

               var fakerNode = Library._dictionary[key];
               var locatorParts = locator.Split('.');
               return FetchList(fakerNode[locatorParts[0].ToLowerInvariant()], locatorParts.Skip(1).ToArray());

               // var locatorParts = locator.Split('.');
               // string fallbackKey = $"en.{locatorParts[0].ToLowerInvariant()}";
               // var yamlNode = Dictionary[fallbackKey];
               //return Fetch22(yamlNode[locatorParts[0].ToLowerInvariant()], locatorParts.Skip(1).ToArray());
            }
         }
         throw new Exception($"Entry for locale {Config.Locale} not found.");
      }

      internal static List<string> FetchList(YamlNode yamlNode, string[] locatorParts)
      {
         if (locatorParts.Length > 0)
         {
            return FetchList(yamlNode[locatorParts[0]], locatorParts.Skip(1).ToArray());
         }
         if (yamlNode is YamlSequenceNode sequenceNode)
         {
            IEnumerable<string> enumerable = sequenceNode.Children.Select(c => c.ToString()/*.Trim(',', ' ', '.')*/);
            var arr = enumerable.ToArray();
            return enumerable.ToList();
         }
         return new List<string>();
      }

      internal static Dictionary<string, YamlNode> Dictionary => Library._dictionary;
   }
}
