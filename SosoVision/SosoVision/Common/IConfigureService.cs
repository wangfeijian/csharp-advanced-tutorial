using SosoVision.Extensions;

namespace SosoVision.Common
{
    public interface IConfigureService
    {
        SerializationData SerializationData { get; set; }

        void Configure(bool isSave = false);
    }
}
