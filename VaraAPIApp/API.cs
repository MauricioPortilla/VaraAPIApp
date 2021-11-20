using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using static VaraAPIApp.MainWindow;

namespace VaraAPIApp {
    public class API {
        private const string API_URL = "http://ec2-3-137-222-34.us-east-2.compute.amazonaws.com/";
        private static RestClient Client = new RestClient(API_URL);
        private static string Token = "";

        public static bool login(string email, string password) {
            var request = new RestRequest("/auth/token");
            request.AddJsonBody(new {
                correoElectronico = email,
                contrasenia = password
            });
            var response = Client.Post(request);
            if (response.IsSuccessful) {
                var content = JObject.Parse(response.Content);
                dynamic json = content;
                Token = "Bearer " + json["token"];
            }
            return response.IsSuccessful;
        }

        public static dynamic loadStrandings(bool loadPage, int skip, object filters) {
            var request = new RestRequest("/yo/varamientos");
            request.AddHeader("Authorization", Token);
            if (loadPage) {
                request.AddParameter("salto", skip);
            }
            if (filters != null) {
                foreach (var filterKey in filters.GetType().GetProperties()) {
                    var value = filterKey.GetValue(filters);
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString())) {
                        continue;
                    }
                    request.AddParameter(filterKey.Name, value);
                }
            }
            var response = Client.Get(request);
            if (!response.IsSuccessful) {
                throw new Exception("Error loading strandings");
            }
            var content = JObject.Parse(response.Content);
            dynamic json = content;
            return json;
        }

        public static bool RegisterStranding(object data) {
            var request = new RestRequest("/yo/varamientos");
            request.AddJsonBody(data);
            request.AddHeader("Authorization", Token);
            var response = Client.Post(request);
            return response.IsSuccessful;
        }

        public static bool MarkStrandingAsFinished(StrandingRow stranding) {
            var request = new RestRequest("/yo/varamientos/" + stranding.Uuid + "/terminar");
            request.AddHeader("Authorization", Token);
            var response = Client.Post(request);
            return response.IsSuccessful;
        }

        public static bool DeleteStranding(StrandingRow stranding) {
            var request = new RestRequest("/yo/varamientos/" + stranding.Uuid);
            request.AddHeader("Authorization", Token);
            var response = Client.Delete(request);
            return response.IsSuccessful;
        }
    }
}
