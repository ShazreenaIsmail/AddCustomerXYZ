using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AddCustomerXYZ
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Manage().Window.Maximize();

            try
            {
                // login as manager
                driver.Navigate().GoToUrl("https://www.globalsqa.com/angularJs-protractor/BankingProject/#/login");
                driver.FindElement(By.XPath("//button[text()='Bank Manager Login']")).Click();

                //add customer
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(text(),'Add Customer')]"))).Click();

                //add customers
                List<Customer> customers = new List<Customer>
                {
                    new Customer("Christopher", "Connely", "L789C349"),
                    new Customer("Frank", "Christopher", "A897N450"),
                    new Customer("Christopher", "Minka", "M098Q585"),
                    new Customer("Connely", "Jackson", "L789C349"),
                    new Customer("Jackson", "Frank", "L789C349"),
                    new Customer("Minka", "Jackson", "A897N450"),
                    new Customer("Jackson", "Connely", "L789C349")
                };

                foreach (var cust in customers)
                {
                    AddCustomer(driver, wait, cust);
                }

                Console.WriteLine("✔ All customers added.");

                // open customers table
                driver.FindElement(By.XPath("//button[contains(text(),'Customers')]")).Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//table")));

                // verify customers are inserted
                var listedCustomers = GetCustomerNamesFromTable(driver);
                var expectedNames = customers.Select(c => c.FullName).ToHashSet();

                var missing = expectedNames.Except(listedCustomers);
                if (!missing.Any())
                    Console.WriteLine("All customers are inserted");
                else
                {
                    Console.WriteLine("Customer not exists");
                    foreach (var name in missing)
                        Console.WriteLine($" - {name}");
                }

                // delete customer
                var namesToDelete = new List<string> { "Jackson Frank", "Christopher Connely" };

                foreach (var name in namesToDelete)
                {
                    if (DeleteCustomerByName(driver, name))
                        Console.WriteLine($"Deleted: {name}");
                    else
                        Console.WriteLine($"User not find: {name}");
                }

                // verify customer deleted
                listedCustomers = GetCustomerNamesFromTable(driver);
                foreach (var name in namesToDelete)
                {
                    if (!listedCustomers.Contains(name))
                        Console.WriteLine($"Confirmed deleted: {name}");
                    else
                        Console.WriteLine($"Still present: {name}");
                }

                Console.WriteLine("\n Test completed.");
                Console.WriteLine("Press any key to close...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }

        // references

        static void AddCustomer(IWebDriver driver, WebDriverWait wait, Customer customer)
        {
            driver.FindElement(By.XPath("//input[@placeholder='First Name']")).Clear();
            driver.FindElement(By.XPath("//input[@placeholder='First Name']")).SendKeys(customer.FirstName);
            driver.FindElement(By.XPath("//input[@placeholder='Last Name']")).Clear();
            driver.FindElement(By.XPath("//input[@placeholder='Last Name']")).SendKeys(customer.LastName);
            driver.FindElement(By.XPath("//input[@placeholder='Post Code']")).Clear();
            driver.FindElement(By.XPath("//input[@placeholder='Post Code']")).SendKeys(customer.PostCode);
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            wait.Until(ExpectedConditions.AlertIsPresent());
            driver.SwitchTo().Alert().Accept();
        }

        static HashSet<string> GetCustomerNamesFromTable(IWebDriver driver)
        {
            var names = new HashSet<string>();
            var rows = driver.FindElements(By.XPath("//table/tbody/tr"));
            foreach (var row in rows)
            {
                var firstName = row.FindElement(By.XPath("./td[1]")).Text.Trim();
                var lastName = row.FindElement(By.XPath("./td[2]")).Text.Trim();
                names.Add($"{firstName} {lastName}");
            }
            return names;
        }

        static bool DeleteCustomerByName(IWebDriver driver, string fullName)
        {
            var rows = driver.FindElements(By.XPath("//table/tbody/tr"));
            foreach (var row in rows)
            {
                var firstName = row.FindElement(By.XPath("./td[1]")).Text.Trim();
                var lastName = row.FindElement(By.XPath("./td[2]")).Text.Trim();
                if ($"{firstName} {lastName}" == fullName)
                {
                    row.FindElement(By.XPath("./td[5]/button")).Click();
                    return true;
                }
            }
            return false;
        }
    }

    // customer references
    class Customer
    {
        public Customer(string first, string last, string post)
        {
            FirstName = first;
            LastName = last;
            PostCode = post;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string PostCode { get; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
