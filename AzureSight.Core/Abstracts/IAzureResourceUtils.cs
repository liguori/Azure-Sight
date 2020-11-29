using AzureSight.Core.Models;

namespace AzureSight.Core.Abstracts
{
    public interface IAzureResourceUtils
    {
        ResourceInfo GetInfoFromResourceID(string resourceID);
    }
}