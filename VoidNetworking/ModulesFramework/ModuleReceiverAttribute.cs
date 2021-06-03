using System;

namespace VoidNetworking.ModulesFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleReceiverAttribute: Attribute
    {
        internal long RouteId;

        public ModuleReceiverAttribute(long routeId)
        {
            RouteId = routeId;
        }
    }
}