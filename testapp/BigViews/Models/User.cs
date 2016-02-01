// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BigViews
{
    public class User
    {
        public static IList<string> AvailableJobs { get; } = new List<string>
        {
            "Construction Foreman",
            "Apprentice Carpenter",
            "Journeyman Carpenter",
            "Master Carpenter",
            "Apprentice Electrician",
            "Journeyman Electrician",
            "Master Electrician",
            "Apprentice Mason",
            "Journeyman Mason",
            "Master Mason",
            "Apprentice Plumber",
            "Journeyman Plumber",
            "Master Plumber",
        };

        // A newly-created User (i.e. in a Create view) does not have an assigned job.
        public static IEnumerable<SelectListItem> JobSelections { get; } = new SelectList(AvailableJobs);

        // A newly-created User (i.e. in a Create view) does not have any past jobs.
        public static IEnumerable<SelectListItem> PastJobSelections { get; } = new MultiSelectList(AvailableJobs);

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string HomePhone { get; set; }

        public string Mobile { get; set; }

        public string MailId { get; set; }

        public string CurrentJob { get; set; }

        public IList<string> PastJobs { get; set; } = new List<string>();
    }
}
