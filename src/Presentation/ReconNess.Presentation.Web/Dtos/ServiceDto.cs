﻿using System;

namespace ReconNess.Web.Dtos
{
    public class ServiceDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public int Port { get; set; }
    }
}
