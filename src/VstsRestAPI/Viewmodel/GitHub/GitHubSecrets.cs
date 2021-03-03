using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestAPI.Viewmodel.GitHub
{
    public class GitHubSecrets
    {
        public class Secrets
        {
            public string secretName { get; set; }
            public string secretValue { get; set; }
        }

        public class GitHubSecret
        {
            public Secrets secrets { get; set; }
        }
    }
}
