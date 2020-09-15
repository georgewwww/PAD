using Newtonsoft.Json;
using System.Text;
using System.Threading;

namespace Broker
{
    class Worker
    {
        private const int TIME_TO_SLEEP = 700;

        public void DoSendMessageWork()
        {
            while(true)
            {
                while(!PayloadStorage.IsEmpty())
                {
                    var payload = PayloadStorage.GetNext();

                    if (payload != null)
                    {
                        var connections = ConnectionsStorage.GetConnectionByBankName(payload.Bank);

                        foreach (var connection in connections)
                        {
                            var payloadString = JsonConvert.SerializeObject(payload);
                            byte[] data = Encoding.UTF8.GetBytes(payloadString);

                            connection.Socket.Send(data);
                        }
                    }
                }

                Thread.Sleep(TIME_TO_SLEEP);
            }
        }
    }
}
