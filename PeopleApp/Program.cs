using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Trippin;
using System.Text.Json;
using Microsoft.OData.Client;
using System.Net.Http;

namespace PeopleApp
{
    internal class Program
    {
        // credit to: https://docs.microsoft.com/nl-nl/odata/connectedservice/getting-started
        const string serviceRoot = "https://services.odata.org/TripPinRESTierService/(S(plvya2zydqcf5zyjzagc31dg))/";
        static void Main(string[] args)
        {
            var context = new Container(new Uri(serviceRoot));

            ListPeople(context);
            FilterPeopleByName(context, "Ronald");
            GetPersonDetail(context, "russellwhyte");
            ModifyPersonLastNameByUserName(context, "russellwhyte", "RonaldAbc");
            DeletePersonByUserName(context, "scottketchum");
            AddPerson(context, Person.CreatePerson("userName1Final", "firstName1Final", PersonGender.Female, Feature.Feature4));
        }

        static void ListPeople(Container context)
        {
            IEnumerable<Person> people = context.People.Select(s => s);
            foreach (var person in people)
            {
                Console.WriteLine($"{person.FirstName} {person.LastName}");
            }
        }

        static void FilterPeopleByName(Container context, string name)
        {
            var people = context.People.Where(w => w.FirstName == name || w.LastName == name);

            foreach (var person in people)
            {
                Console.WriteLine($"{person.FirstName} {person.LastName}");
            }
        }

        static void GetPersonDetail(Container context, string userName)
        {
            var people = context.People.Where(w => w.UserName == userName);

            foreach (var person in people)
            {
                Console.WriteLine(JsonSerializer.Serialize(person, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        static bool ModifyPersonLastNameByUserName(Container context, string userName, string lastName)
        {
            //var person = context.People.Where(w => w.UserName == userName).SingleOrDefault();
            //if (person == null) throw new Exception("More than one person are using same userName");

            //person.LastName = lastName;
            //context.UpdateObject(person);
            //context.SaveChanges();
            //return true;

            // Can't make default Person Class to work
            // Has issue when posting the Gender and FavoriteFeature fields
            // Change to HttpClient instead
            var result = false;

            using (HttpClient http = new HttpClient())
            {
                var request = JsonSerializer.Serialize(new
                {
                    LastName = lastName,
                });
                StringContent httpContent = new StringContent(request, System.Text.Encoding.UTF8, "application/json");
                var response = http.PatchAsync($"{serviceRoot}People('{userName}')", httpContent).Result;
                result = response.IsSuccessStatusCode;
                Console.Write(response.Content.ReadAsStringAsync().Result);
            }

            return result;
        }

        static bool DeletePersonByUserName(Container context, string userName)
        {
            //var person = context.People.Where(w => w.UserName == userName).SingleOrDefault();
            //if (person == null) throw new Exception("Invalid User Name");
            //context.DeleteObject(person);
            //context.SaveChanges();
            //return true;

            // Can't make default Person Class to work
            // Has issue when posting the Gender and FavoriteFeature fields
            // Change to HttpClient instead
            var result = false;

            using (HttpClient http = new HttpClient())
            {
                var response = http.DeleteAsync($"{serviceRoot}People('{userName}')").Result;
                result = response.IsSuccessStatusCode;
                Console.Write(response.Content.ReadAsStringAsync().Result);
            }

            return result;
        }

        static bool AddPerson(Container context, Person person)
        {
            // Can't make default Person Class to work
            // Has issue when posting the Gender and FavoriteFeature fields
            // Change to HttpClient instead
            //context.AddObject("People", person);
            //context.SaveChanges();

            var result = false;

            using (HttpClient http = new HttpClient())
            {
                var request = JsonSerializer.Serialize(new
                {
                    person.UserName,
                    person.FirstName,
                    person.LastName,
                });
                StringContent httpContent = new StringContent(request, System.Text.Encoding.UTF8, "application/json");
                var response = http.PostAsync($"{serviceRoot}People", httpContent).Result;
                result = response.IsSuccessStatusCode;
            }

            return result;
        }
    }
}
