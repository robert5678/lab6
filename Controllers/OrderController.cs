using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PizzaOrderApp.Models;
using System;
using System.Collections.Generic;

namespace PizzaOrderApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        
        private static readonly List<string> AvailableProducts = new()
        {
            "Піца Маргарита",
            "Піца Пепероні",
            "Піца Гавайська",
            "Піца Чотири сири"
        };

 

        [HttpGet("register")]
        public ContentResult RegisterForm()
        {
            return Content(CssLink + "<form method='post' action='/Order/register'>" +
                           "<h2>Реєстрація</h2>" +
                           "<label>Ім'я: <input type='text' name='Name' required /></label>" +
                           "<label>Дата народження: <input type='date' name='BirthDate' required /></label>" +
                           "<button type='submit'>Зареєструватись</button>" +
                           "</form>", "text/html; charset=utf-8");
        }

        [HttpPost("register")]
        public IActionResult Register([FromForm] User user)
        {
            if (!ModelState.IsValid || user.Age < 16)
            {
                return Content("Вибачте, але вам має бути більше 16 років і всі поля мають бути заповнені.");
            }

            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetInt32("UserAge", user.Age);

            return Redirect("/Order/orderform");
        }

        [HttpGet("orderform")]
        public ContentResult OrderForm()
        {
            return Content(CssLink + "<form method='post' action='/Order/order'>" +
                           "<h2>Введіть Кількість Товарів</h2>" +
                           "<label>Скільки одиниць товару ви бажаєте замовити? <input type='number' name='Quantity' min='1' required /></label>" +
                           "<button type='submit'>Підтвердити</button>" +
                           "</form>", "text/html; charset=utf-8");
        }

        [HttpPost("order")]
        public IActionResult Order([FromForm] int quantity)
        {
            HttpContext.Session.SetInt32("Quantity", quantity);
            return Redirect("/Order/submitorderform");
        }

        [HttpGet("submitorderform")]
        public ContentResult SubmitOrderForm()
        {
            int quantity = HttpContext.Session.GetInt32("Quantity") ?? 0;
            string formHtml = CssLink + "<form method='post' action='/Order/submitorder'>";

            for (int i = 1; i <= quantity; i++)
            {
                formHtml += $"<label>Товар {i}: " +
                            $"<select name='ProductName{i}' required>";

                foreach (var product in AvailableProducts)
                {
                    formHtml += $"<option value='{product}'>{product}</option>";
                }

                formHtml += "</select></label><br>" +
                            $"<label>Кількість: <input type='number' name='ProductQuantity{i}' min='1' required /></label><br><br>";
            }
            formHtml += "<button type='submit'>Підтвердити замовлення</button></form>";
            return Content(formHtml, "text/html; charset=utf-8");
        }

        [HttpPost("submitorder")]
        public ContentResult SubmitOrder([FromForm] Dictionary<string, string> formValues)
        {
            int quantity = HttpContext.Session.GetInt32("Quantity") ?? 0;
            List<Product> orderedProducts = new();

            for (int i = 1; i <= quantity; i++)
            {
                var nameKey = $"ProductName{i}";
                var quantityKey = $"ProductQuantity{i}";

                if (formValues.ContainsKey(nameKey) && formValues.ContainsKey(quantityKey))
                {
                    if (int.TryParse(formValues[quantityKey], out int qty) && qty > 0)
                    {
                        orderedProducts.Add(new Product { Name = formValues[nameKey], Quantity = qty });
                    }
                }
            }

            string summaryHtml = CssLink + "<h2>Ваше замовлення</h2><div class='summary'>";
            foreach (var product in orderedProducts)
            {
                summaryHtml += $"<p>{product.Name} - {product.Quantity} шт.</p>";
            }

            if (orderedProducts.Count == 0)
            {
                summaryHtml += "<p>Немає замовлених товарів.</p>";
            }

            summaryHtml += "</div>";
            return Content(summaryHtml, "text/html; charset=utf-8");
        }
    }
}
