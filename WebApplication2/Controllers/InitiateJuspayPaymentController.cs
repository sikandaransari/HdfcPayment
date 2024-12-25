using Juspay;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class InitiateJuspayPaymentController : ApiController
    {
      
        public async Task<ActionResult> Post([FromBody]string value)
        {

            string orderId = $"order_{new Random().Next()}";
            string customerId = "testing-customer-one";
            int amount = new Random().Next(0, 100);
            RequestOptions requestOptions = new RequestOptions { CustomerId = customerId };
            CreateOrderSessionInput createOrderSessionInput = new CreateOrderSessionInput(new Dictionary<string, object> { { "amount", amount }, { "order_id", orderId }, { "customer_id", customerId }, { "payment_page_client_id", Init.Config.PaymentPageClientId }, { "action", "paymentPage" }, { "return_url", "http://localhost:5000/handleJuspayResponse" } });
            try
            {
                JuspayResponse sessionRes = await new OrderSession().CreateAsync(createOrderSessionInput, requestOptions);
                Dictionary<string, object> res = new Dictionary<string, object> {
                    { "orderId", sessionRes.Response.order_id },
                    { "id", sessionRes.Response.id },
                    { "status", sessionRes.Response.status },
                    { "paymentLinks", sessionRes.Response.payment_links },
                    { "sdkPayload", sessionRes.Response.sdk_payload },
                };
                if (sessionRes.Response.status == "NEW")
                {
                    return new ContentResult
                    {
                        Content = JsonConvert.SerializeObject(res),
                        ContentType = "application/json"
                    };
                }
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"invalid session status: {sessionRes.Response.status}");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}
