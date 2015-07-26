using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace League_of_Legends_Counterpicks.DataModel
{
    public class SyncHandler : IMobileServiceSyncHandler
    {
        public async Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            try { return await operation.ExecuteAsync(); 
                
            
            }
            catch (MobileServicePreconditionFailedException e) {
                return null;
            }
        }

        public Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            throw new NotImplementedException();
        }
    }
}
