using MessagePack;
using MessagePack.Resolvers;

namespace VoidNetworking
{
    public static class VoidUtils
    {
        public static void RegisterGeneratedResolver(IFormatterResolver generated)
        {
            StaticCompositeResolver.Instance.Register(
                generated,
                StandardResolver.Instance
                );
            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
            
            MessagePackSerializer.DefaultOptions = option;
        }
    }
}
