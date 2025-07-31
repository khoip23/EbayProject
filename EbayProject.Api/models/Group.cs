﻿using System;
using System.Collections.Generic;

namespace EbayProject.Api.models;

public partial class Group
{
    public int Id { get; set; }

    public string GroupName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? Deleted { get; set; }
}
