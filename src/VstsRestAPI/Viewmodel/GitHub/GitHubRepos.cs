using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestAPI.Viewmodel.GitHub
{
    public class GitHubRepos
    {
        public class Repository
        {
            public string FullName { get; set; }
            public string EndPointName { get; set; }
            public string vcs { get; set; }
            public string vcs_url { get; set; }
        }

        public class Fork
        {
            public List<Repository> Repositories { get; set; }
        }
    }
}
