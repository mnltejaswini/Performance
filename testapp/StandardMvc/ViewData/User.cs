// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace StandardMvc.ViewData
{
    public class SiteUser
    {
        private static int _userCount = 0;
        private static Random _rand = new Random();

        public int Id { get; set; }

        [Required(ErrorMessage = "The name is required")]
        [MinLength(4)]
        public string Name { get; set; }

        [Range(27, 70, ErrorMessage = "Age must be between 27 and 70")]
        public int Age { get; set; }

        public static SiteUser CreateNewUser()
        {
            _userCount++;

            return GetUser(_userCount);
        }

        public static SiteUser GetUser(int id)
        {
            return new SiteUser
            {
                Id = id,
                Name = string.Format("User{0:D4}", id),
                Age = _rand.Next(70 - 27) + 27
            };
        }
    }
}