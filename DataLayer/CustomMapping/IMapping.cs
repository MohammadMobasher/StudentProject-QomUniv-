using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.CustomMapping
{
    public interface IMapping
    {
        void CreateMappings(Profile profile);
    }
}
