﻿using System;
using System.Collections.Generic;

namespace EbayProject.Api.models;

public partial class RoleGroup
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int GroupId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? Deleted { get; set; }
}
