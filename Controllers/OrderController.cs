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
            "ϳ�� ���������",
            "ϳ�� �������",
            "ϳ�� ���������",
            "ϳ�� ������ ����"
        };

 

        [HttpGet("register")]
        public ContentResult RegisterForm()
        {
            return Content(CssLink + "<form method='post' action='/Order/register'>" +
                           "<h2>���������</h2>" +
                           "<label>��'�: <input type='text' name='Name' required /></label>" +
                           "<label>���� ����������: <input type='date' name='BirthDate' required /></label>" +
                           "<button type='submit'>��������������</button>" +
                           "</form>", "text/html; charset=utf-8");
        }

        [HttpPost("register")]
        public IActionResult Register([FromForm] User user)
        {
            if (!ModelState.IsValid || user.Age < 16)
            {
                return Content("�������, ��� ��� �� ���� ����� 16 ���� � �� ���� ����� ���� ��������.");
            }

            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetInt32("UserAge", user.Age);

            return Redirect("/Order/orderform");
        }

        [HttpGet("orderform")]
        public ContentResult OrderForm()
        {
            return Content(CssLink + "<form method='post' action='/Order/order'>" +
                           "<h2>������ ʳ������ ������</h2>" +
                           "<label>������ ������� ������ �� ������ ��������? <input type='number' name='Quantity' min='1' required /></label>" +
                           "<button type='submit'>ϳ���������</button>" +
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
                formHtml += $"<label>����� {i}: " +
                            $"<select name='ProductName{i}' required>";

                foreach (var product in AvailableProducts)
                {
                    formHtml += $"<option value='{product}'>{product}</option>";
                }

                formHtml += "</select></label><br>" +
                            $"<label>ʳ������: <input type='number' name='ProductQuantity{i}' min='1' required /></label><br><br>";
            }
            formHtml += "<button type='submit'>ϳ��������� ����������</button></form>";
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

            string summaryHtml = CssLink + "<h2>���� ����������</h2><div class='summary'>";
            foreach (var product in orderedProducts)
            {
                summaryHtml += $"<p>{product.Name} - {product.Quantity} ��.</p>";
            }

            if (orderedProducts.Count == 0)
            {
                summaryHtml += "<p>���� ���������� ������.</p>";
            }

            summaryHtml += "</div>";
            return Content(summaryHtml, "text/html; charset=utf-8");
        }
    }
}
