﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estimatorx.Data.Mongo.Providers
{
    public interface ISampleGenerator
    {
        Project GenerateProject(string organizationId);

        Template GenerateTemplate(string organizationId);
    }
}
