using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using Stripe;
using Stripe.Checkout;
using System.Drawing;

namespace GrowGreenWeb.Pages.Donation
{
    public class mainModel : PageModel
    {
        public void OnGet()
        {

        }
        public async Task<IActionResult> ManualDonation(string id)
        {
            string PriceID = "none";
            var domain = "http://localhost:7198";
            List<string> optionList = new List<string>
            {
                "https://drive.google.com/file/d/1cM-KXhuTgzccp5cggmff93bSvZ3wObYu/view?usp=sharing",
            };
            if (id == "10")
            {
                PriceID = "price_1MRzOZGtntpklPoVA2mdyUJE";
            }
            if (id == "20")
            {
                PriceID = "price_1MRzO6GtntpklPoVRtGZIbss";
            }
            if (id == "50")
            {
                PriceID = "price_1MRzNbGtntpklPoVnjm3sdoB";
            }
            if (id == "100")
            {
                PriceID = "price_1MRyZBGtntpklPoVG13ZdGTZ";
            }

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {

                    Price = PriceID,
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = domain + "/Donation/success",
                CancelUrl = domain + "/Donation/cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
    }
        public IActionResult OnPost(string DonationAmount)
        {
            var domain = "http://localhost:7198";
            List<string> optionList = new List<string>
            {
                "https://drive.google.com/file/d/1cM-KXhuTgzccp5cggmff93bSvZ3wObYu/view?usp=sharing",
            };
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                   
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = Convert.ToInt32(DonationAmount) * 100,
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Donation",
                            Description = "Donation to GrowGreen's effort to save the environment",
                            Images = optionList

                        },
                    },
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = domain + "/Donation/success",
                CancelUrl = domain + "/Donation/cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}