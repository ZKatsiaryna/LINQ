// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;
using System.Text.RegularExpressions;

// Version Mad01

namespace SampleQueries
{
    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness
    {

        private DataSource dataSource = new DataSource();

        [Category("Restriction Operators")]
        [Title("Where - Task 1")]
        [Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
        public void Linq1()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var lowNums =
                from num in numbers
                where num < 5
                select num;

            Console.WriteLine("Numbers < 5:");
            foreach (var x in lowNums)
            {
                Console.WriteLine(x);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 2")]
        [Description("This sample return return all presented in market products")]

        public void Linq2()
        {
            var products =
                from p in dataSource.Products
                where p.UnitsInStock > 0
                select p;

            foreach (var p in products)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 1.1")]
        [Description("1. Выдайте список всех клиентов, чей суммарный оборот(сумма всех заказов) превосходит некоторую величину X.")]

        public void Linq1_1()
        {
            int maxValue = 8623;

            var client =
                from c in dataSource.Customers
                where c.Orders.Sum(i => i.Total) > maxValue
                select c;

            foreach (var p in client)
            {
                ObjectDumper.Write(p);
            }
        }


        [Category("Restriction Operators")]
        [Title("Where - Task 1.2")]
        [Description("2. Для каждого клиента составьте список поставщиков, находящихся в той же стране и том же городе. ")]

        public void Linq1_2()
        {
            var customers1 = dataSource.Customers.Select(c => new
                      {                   
                          CustomerId = c.CustomerID,
                          Supliers = dataSource.Suppliers.Where(s=> s.City == c.City && s.Country == c.Country)
                      });

            var grouped = dataSource.Suppliers.GroupBy(t => new KeyValuePair<string, string>(t.Country, t.City));

            var customers2 = dataSource.Customers.Select(c => new
                        {
                            CustomerId = c.CustomerID,
                            Suppliers = grouped.FirstOrDefault(t => t.Key.Key == c.Country && t.Key.Value == c.City)
                        });
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 1.3")]
        [Description("3. Найдите всех клиентов, у которых были заказы, превосходящие по сумме величину X")]

        public void Linq1_3()
        {
            int maxValue = 8623;

            var client =
                from c in dataSource.Customers
                where c.Orders.Any(i => i.Total > maxValue)
                select c;

            foreach (var p in client)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 1.4")]
        [Description("4. Выдайте список клиентов с указанием, начиная с какого месяца какого года они стали клиентами")]

        public void Linq1_4()
        {
            var customers = dataSource.Customers.Select(t => new
            {
                t.CompanyName,
                YearGroup = t.Orders.Select(d => new
                {
                    d.OrderDate.Year,
                    d.OrderDate.Month,
                    DateFormat = d.OrderDate.ToString("yyyy-MMM")
                }).Distinct().OrderByDescending(f => f.Year).ThenByDescending(c => c.Month).Select(d => d.DateFormat).LastOrDefault()
            });

            foreach (var p in customers)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 1.5")]
        [Description("5. Сделайте предыдущее задание, но выдайте список отсортированным по году, месяцу, оборотам клиента ")]

        public void Linq1_5()
        {
            var customers = dataSource.Customers.Select(t => new
            {
                t.CompanyName,
                Total = t.Orders.Select(d => d.Total).Sum(),
                YearGroup = t.Orders.Select(d => new
                {
                    d.OrderDate.Year,
                    d.OrderDate.Month,
                    DateFormat = d.OrderDate.ToString("yyyy-MMM")
                }).OrderBy(f => f.Year).ThenByDescending(c => c.Month).Select(d => d.DateFormat).LastOrDefault()
            }).OrderByDescending(o => o.Total).ThenBy(t => t.CompanyName);

            foreach (var p in customers)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 1.6")]
        [Description("6.Укажите всех клиентов, у которых указан нецифровой код или не заполнен регион или в телефоне не указан код оператора ")]

        public void Linq1_6()
        {
            var customers = dataSource.Customers.Select(t => new
            {
                t.CompanyName,
                Phone = t.Phone
            }).Where(c => new Regex(@"^\(\d+\)").IsMatch(c.Phone) != true);

            foreach (var p in customers)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 1.7")]
        [Description("7. Сгруппируйте все продукты по категориям, внутри – по наличию на складе, внутри последней группы отсортируйте по стоимости")]

        public void Linq1_7()
        {
            var product = dataSource.Products.OrderByDescending(p => p.UnitPrice).GroupBy(p => p.Category).Select(g => g.GroupBy(t => t.UnitsInStock));

            foreach (var p in product)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 1.8")]
        [Description("8. Сгруппируйте товары по группам «дешевые», «средняя цена», «дорогие». Границы каждой группы задайте сами")]

        public void Linq1_8()
        {
            var cheapPrice = 20;
            var expensivePrice = 200;

            var one2 = dataSource.Products.GroupBy(d => new
            {
                PriceCheap = d.UnitPrice < cheapPrice,
                AveragePrice = d.UnitPrice >= cheapPrice & d.UnitPrice < expensivePrice,
                ExpensivePrice = d.UnitPrice >= expensivePrice
            }).Select(u => u);

            foreach (var p in one2)
            {
                ObjectDumper.Write(p);
            }
        }


        [Category("Restriction Operators")]
        [Title("Where - Task 1.9")]
        [Description("9. Рассчитайте среднюю прибыльность каждого города (среднюю сумму заказа по всем клиентам из данного города) и среднюю интенсивность")]

        public void Linq1_9()
        {
            var two = dataSource.Customers.GroupBy(i => i.City).Select(t => new
            {
                CityName = t.Key,
                AverageSum2 = t.Average(i => i.Orders.Sum(o => o.Total)),
                AverageIntensity = t.Average(r => r.Orders.Count())
            });

            foreach (var p in two)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 1.10")]
        [Description("10. Сделайте среднегодовую статистику активности клиентов по месяцам ")]

        public void Linq1_10()
        {
            var totalMonth = dataSource.Customers.SelectMany(t => t.Orders, (customer, order) => new
            {
                OrderDate = order.OrderDate,
                Total = order.Total
            }).GroupBy(t => t.OrderDate.Month, t => t).Select(t => new { Month = t.Key, Total = t.Sum(p => p.Total) });


            var totalYear = dataSource.Customers.SelectMany(t => t.Orders, (customer, order) => new
            {
                OrderDate = order.OrderDate,
                Total = order.Total

            }).GroupBy(t => t.OrderDate.Year, t => t).Select(t => new { Month = t.Key, Total = t.Sum(p => p.Total) });

            var totalMonthAndYear = dataSource.Customers.SelectMany(t => t.Orders, (customer, order) => new
            {
                OrderDate = order.OrderDate,
                Total = order.Total
            }).GroupBy(t => new KeyValuePair<int, int>(t.OrderDate.Year, t.OrderDate.Month))
                .Select(t => new { Year = t.Key.Key, Month = t.Key.Value, Total = t.Sum(p => p.Total) })
                .OrderBy(t => t.Year)
                .ThenBy(t => t.Month);
        }
    }
}