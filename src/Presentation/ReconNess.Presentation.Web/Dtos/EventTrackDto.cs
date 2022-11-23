﻿using System;

namespace ReconNess.Web.Dtos;

public class EventTrackDto
{
    public string Description { get; set; }

    public string Username { get; set; }

    public string Status { get; set; }

    public bool Read { get; set; }

    public DateTime CreatedAt { get; set; }
}
