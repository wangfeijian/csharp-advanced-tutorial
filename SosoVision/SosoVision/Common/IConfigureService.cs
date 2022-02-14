using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SosoVision.Extensions;

namespace SosoVision.Common
{
    public interface IConfigureService
    {
        SerializationData SerializationData { get; set; }

        void Configure(bool isSave = false);
    }
}
