using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FFXIV.Framework.Common;
using FFXIV.Framework.Extensions;
using FFXIV.Framework.Globalization;
using Newtonsoft.Json.Linq;
using RazorEngine.Compilation;
using RazorEngine.Compilation.ReferenceResolver;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    public class TimelineRazorModel
    {
        public DateTimeOffset LT => DateTimeOffset.Now;
#if false
        public EorzeaTime ET => this.LT.ToEorzeaTime();
#endif
        public string Zone { get; set; } = string.Empty;

        public string Locale { get; set; } = Locales.JA.ToString();

        public TimelineRazorPlayer Player { get; set; }

        public TimelineRazorPlayer[] Party { get; set; }

        public bool InZone(
            string zone)
            => this.Zone.ContainsIgnoreCase(zone);

        public dynamic ParseJsonString(
            string json)
            => JObject.Parse(json);

        public dynamic ParseJsonFile(
            string file)
        {
            if (!File.Exists(file))
            {
                file = Path.Combine(
                    TimelineManager.Instance.TimelineDirectory,
                    file);

                if (!File.Exists(file))
                {
                    return null;
                }
            }

            return this.ParseJsonString(
                File.ReadAllText(file, new UTF8Encoding(false)));
        }
    }

    public class TimelineRazorPlayer
    {
        public int Number { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public string Job { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool InJob(
            params string[] jobs)
        {
            if (jobs == null)
            {
                return false;
            }

            return jobs.Any(x => this.Job.ContainsIgnoreCase(x));
        }

        public bool InRole(
            params string[] roles)
        {
            if (roles == null)
            {
                return false;
            }

            return roles.Any(x => this.Role.ContainsIgnoreCase(x));
        }
    }

    public class RazorReferenceResolver :
        IReferenceResolver
    {
        public IEnumerable<CompilerReference> GetReferences(
            TypeContext context,
            IEnumerable<CompilerReference> includeAssemblies = null)
        {
            var loadedAssemblies = (new UseCurrentAssembliesReferenceResolver())
                .GetReferences(context, includeAssemblies)
                .Select(r => r.GetFile())
                .ToArray();

            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "mscorlib.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "System.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "System.Core.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "Microsoft.CSharp.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "Microsoft.VisualBasic.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "RazorEngine.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "Prism.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "Prism.Wpf.dll"));
            yield return CompilerReference.From(typeof(AppLog).Assembly);
            yield return CompilerReference.From(typeof(RazorReferenceResolver).Assembly);
        }

        public string FindLoaded(
            IEnumerable<string> refs,
            string find)
            => refs.First(r => r.EndsWith(Path.DirectorySeparatorChar + find));
    }
}
