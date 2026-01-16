using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Threading;

namespace Infrastructure.Integrations.Hikvision
{
    public sealed class DeviceListResponse
    {
        public SearchResult? SearchResult { get; set; }
    }

    public sealed class SearchResult
    {
        public List<MatchItem>? MatchList { get; set; }
    }

    public sealed class MatchItem
    {
        public DeviceWrapper? Device { get; set; }
    }

    public sealed class DeviceWrapper
    {
        public IsapiParams? ISAPIParams { get; set; }
        public bool? ActiveStatus { get; set; }
        public string? DevIndex { get; set; }
        public string? DevMode { get; set; }
        public string? DevName { get; set; }
        public string? DevStatus { get; set; }
        public string? DevType { get; set; }
        public string? DevVersion { get; set; }
        public string? ProtocolType { get; set; }
        public int? VideoChannelNum { get; set; }
    }

    public sealed class IsapiParams
    {
        public string? Address { get; set; }
        public string? AddressingFormatType { get; set; }
        public int? PortNo { get; set; }
    }
    public sealed class DeviceCredentialCheckResult
    {
        public bool IsAuthorized { get; set; }
        public int StatusCode { get; set; }
        public string? ResponseBody { get; set; }

        // ✅ add these for debugging
        public string? RequestUrl { get; set; }
        public string? WwwAuthenticate { get; set; }
    }
    public sealed class HikvisionPersonInfo
    {
        public string EmployeeNo { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public sealed class HikvisionAddPersonRequest
    {
        // MUST be array according to your gateway/doc
        [JsonPropertyName("UserInfo")]
        public List<HikvisionUserInfo> UserInfo { get; set; } = new();
    }
    public sealed class HikvisionAddCardRequest
    {
        [JsonPropertyName("CardInfo")]
        public HikvisionCardInfo CardInfo { get; set; } = new();
    }

    public sealed class HikvisionCardInfo
    {
        [JsonPropertyName("employeeNo")]
        public string EmployeeNo { get; set; } = "";

        [JsonPropertyName("cardNo")]
        public string CardNo { get; set; } = "";

        // REQUIRED (your Postman sends this)
        [JsonPropertyName("cardType")]
        public string CardType { get; set; } = "normalCard";
    }
    public class HikvisionUserInfo
    {
        [JsonPropertyName("employeeNo")]
        public string EmployeeNo { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("userType")]
        public string UserType { get; set; }

        [JsonPropertyName("Valid")]
        public HikvisionValid Valid { get; set; }

        // ✅ ADD THESE
        [JsonPropertyName("doorRight")]
        public string DoorRight { get; set; } = "1";

        [JsonPropertyName("rightPlan")]
        public List<HikvisionRightPlan> RightPlan { get; set; } = new();

        [JsonPropertyName("password")]
        public string Password { get; set; } = "123456";

        [JsonPropertyName("gender")]
        public string Gender { get; set; } = "female";

        [JsonPropertyName("localUIRight")]
        public bool LocalUIRight { get; set; } = false;
    }

    public class HikvisionRightPlan
    {
        [JsonPropertyName("doorNo")]
        public int DoorNo { get; set; }

        [JsonPropertyName("planTemplateNo")]
        public string PlanTemplateNo { get; set; } = "1"; // if firmware complains, change to int
    }

    public sealed class HikvisionValid
    {
        [JsonPropertyName("enable")]
        public bool Enable { get; set; } = true;

        [JsonPropertyName("beginTime")]
        public string BeginTime { get; set; } = "";

        [JsonPropertyName("endTime")]
        public string EndTime { get; set; } = "";
    }
    public sealed class HikvisionAddPersonRequest_Device
    {
        [JsonPropertyName("UserInfo")]
        public HikvisionUserInfo UserInfo { get; set; } = new();
    }

    public sealed class HikvisionBiometricStatus
    {
        public bool HasFace { get; set; }
        public bool HasFingerprint { get; set; }
        public bool HasCard { get; set; }
        public string? FaceId { get; set; }
        public string? FingerprintId { get; set; }
        public string? CardNo { get; set; }
    }

    public sealed class HikvisionClient
    {
        private readonly HttpClient _http;

        public HikvisionClient(HttpClient http) => _http = http;

        public async Task<DeviceListResponse?> GetDeviceListAsync(CancellationToken ct = default)
        {
            var url = "/ISAPI/ContentMgmt/DeviceMgmt/deviceList?format=json";

            // Same body as your Postman request
            var payload = new
            {
                SearchDescription = new
                {
                    position = 0,
                    maxResult = 100,
                    Filter = new
                    {
                        key = "",
                        devType = "",
                        protocolType = new[] { "ISAPI" },
                        // Put whatever you used in Postman. If unsure, remove devStatus or keep empty array.
                        devStatus = new[] { "online", "offline" }
                    }
                }
            };

            var jsonBody = JsonSerializer.Serialize(payload);

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var res = await _http.SendAsync(req, ct);
            var json = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
                throw new Exception($"Hikvision API failed: {(int)res.StatusCode} {res.ReasonPhrase}\n{json}");

            return JsonSerializer.Deserialize<DeviceListResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task<DeviceCredentialCheckResult> CheckDeviceCredentialsAsync(
    string ipAddress, int port, string username, string password,
    string? devIndex = null,
    CancellationToken ct = default)
        {
            var baseUri = new Uri($"http://{ipAddress}:{port}/");

            using var handler = new HttpClientHandler
            {
                PreAuthenticate = true,
                UseCookies = false,
                Credentials = new CredentialCache
        {
            { baseUri, "Digest", new NetworkCredential(username, password) },
            { baseUri, "Basic",  new NetworkCredential(username, password) }
        }
            };

            using var http = new HttpClient(handler) { BaseAddress = baseUri };
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var path = string.IsNullOrWhiteSpace(devIndex)
        ? "ISAPI/System/deviceInfo?format=json"
        : $"ISAPI/System/deviceInfo?format=json&devIndex={Uri.EscapeDataString(devIndex)}";

            var response = await http.GetAsync(path, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            var wwwAuth = response.Headers.WwwAuthenticate != null
                ? string.Join(" | ", response.Headers.WwwAuthenticate.Select(h => h.ToString()))
                : null;

            var unauthorized = response.StatusCode == HttpStatusCode.Unauthorized
                || (body?.Contains("<statusValue>401</statusValue>", StringComparison.OrdinalIgnoreCase) == true);

            return new DeviceCredentialCheckResult
            {
                IsAuthorized = !unauthorized,
                StatusCode = (int)response.StatusCode,
                ResponseBody = body,
                RequestUrl = new Uri(baseUri, path).ToString(),
                WwwAuthenticate = wwwAuth
            };
        }
        private static (string BeginTime, string EndTime) BuildValidPeriod10Years()
        {
            var beginDt = DateTime.Now;
            var endDt = beginDt.AddYears(10);

            var maxEnd = new DateTime(2037, 12, 31, 23, 59, 59);
            if (endDt > maxEnd) endDt = maxEnd;

            return (
                beginDt.ToString("yyyy-MM-ddTHH:mm:ss"),
                endDt.ToString("yyyy-MM-ddTHH:mm:ss")
            );
        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task AddPersonToDeviceAsync(
    string ipAddress,
    int port,
    string username,
    string password,
    HikvisionPersonInfo person,
    string? devIndex = null,
    string userType = "normal",
    CancellationToken ct = default)
        {
            var baseUri = new Uri($"http://{ipAddress}:{port}/");

            using var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password),
                PreAuthenticate = false,
                UseCookies = false,
                AllowAutoRedirect = false
            };

            using var http = new HttpClient(handler) { BaseAddress = baseUri };
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var (begin, end) = BuildValidPeriod10Years();

            var payload = new HikvisionAddPersonRequest_Device
            {
                UserInfo = new HikvisionUserInfo
                {
                    EmployeeNo = person.EmployeeNo,
                    Name = person.Name,
                    UserType = userType,

                    Valid = new HikvisionValid
                    {
                        Enable = true,
                        BeginTime = begin,
                        EndTime = end
                    },

                    // ✅ added static door permission fields
                    DoorRight = "1",
                    RightPlan = new List<HikvisionRightPlan>
            {
                new HikvisionRightPlan { DoorNo = 1, PlanTemplateNo = "1" }
            },
                    Password = "123456",
                    Gender = "female",
                    LocalUIRight = false
                }
            };
            Console.WriteLine("payload-----" + payload);
            var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);

            var url = string.IsNullOrWhiteSpace(devIndex)
                ? "/ISAPI/AccessControl/UserInfo/Record?format=json"
                : $"/ISAPI/AccessControl/UserInfo/Record?format=json&devIndex={Uri.EscapeDataString(devIndex)}";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            var response = await http.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                var wwwAuth = response.Headers.WwwAuthenticate != null
                    ? string.Join(" | ", response.Headers.WwwAuthenticate.Select(h => h.ToString()))
                    : null;

                throw new Exception(
                    $"Hikvision Add Person failed: {(int)response.StatusCode} {response.ReasonPhrase}\n" +
                    $"URL: {new Uri(baseUri, url)}\n" +
                    $"WWW-Authenticate: {wwwAuth}\n" +
                    $"{responseBody}\nREQUEST:\n{jsonBody}"
                );
            }
        }

        public async Task UpdatePersonInDeviceAsync(
            string ipAddress,
            int port,
            string username,
            string password,
            HikvisionPersonInfo person,
            string? devIndex = null,
            string userType = "normal",
            CancellationToken ct = default)
        {
            var baseUri = new Uri($"http://{ipAddress}:{port}/");

            using var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password),
                PreAuthenticate = false,
                UseCookies = false,
                AllowAutoRedirect = false
            };

            using var http = new HttpClient(handler) { BaseAddress = baseUri };
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var (begin, end) = BuildValidPeriod10Years();

            var payload = new HikvisionAddPersonRequest_Device
            {
                UserInfo = new HikvisionUserInfo
                {
                    EmployeeNo = person.EmployeeNo,
                    Name = person.Name,
                    UserType = userType,

                    Valid = new HikvisionValid
                    {
                        Enable = true,
                        BeginTime = begin,
                        EndTime = end
                    },

                    DoorRight = "1",
                    RightPlan = new List<HikvisionRightPlan>
        {
            new HikvisionRightPlan { DoorNo = 1, PlanTemplateNo = "1" }
        },
                    Password = "123456",
                    Gender = "female",
                    LocalUIRight = false
                }
            };


            var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);

            var url = string.IsNullOrWhiteSpace(devIndex)
                ? "/ISAPI/AccessControl/UserInfo/Modify?format=json"
                : $"/ISAPI/AccessControl/UserInfo/Modify?format=json&devIndex={Uri.EscapeDataString(devIndex)}";

            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            var response = await http.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                var wwwAuth = response.Headers.WwwAuthenticate != null
                    ? string.Join(" | ", response.Headers.WwwAuthenticate.Select(h => h.ToString()))
                    : null;

                throw new Exception(
                    $"Hikvision Update Person failed: {(int)response.StatusCode} {response.ReasonPhrase}\n" +
                    $"URL: {new Uri(baseUri, url)}\n" +
                    $"WWW-Authenticate: {wwwAuth}\n" +
                    $"{responseBody}\nREQUEST:\n{jsonBody}"
                );
            }
        }
        //public async Task AddCardAsync(
        //   string ipAddress,
        //   int port,
        //   string username,
        //   string password,
        //   HikvisionCardInfo card,
        //   CancellationToken ct = default)
        //{
        //    var baseUri = new Uri($"http://{ipAddress}:{port}/");

        //    using var handler = new HttpClientHandler
        //    {
        //        PreAuthenticate = true,
        //        UseCookies = false,
        //        Credentials = new CredentialCache
        //        {
        //            { baseUri, "Digest", new NetworkCredential(username, password) }
        //        }
        //    };

        //    using var http = new HttpClient(handler)
        //    {
        //        BaseAddress = baseUri
        //    };

        //    http.DefaultRequestHeaders.Accept.Add(
        //        new MediaTypeWithQualityHeaderValue("application/json"));

        //    var payload = new HikvisionAddCardRequest
        //    {
        //        CardInfo = new HikvisionCardInfo
        //        {
        //            EmployeeNo = card.EmployeeNo,
        //            CardNo = card.CardNo
        //        }
        //    };

        //    var jsonBody = JsonSerializer.Serialize(payload);
        //    var request = new HttpRequestMessage(HttpMethod.Post, "/ISAPI/AccessControl/CardInfo/Record?format=json");
        //    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        //    var response = await http.SendAsync(request, ct);
        //    var responseBody = await response.Content.ReadAsStringAsync(ct);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new Exception($"Hikvision Add Card failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{responseBody}");
        //    }
        //}

        public async Task AddCardToDeviceAsync(
            string ipAddress,
            int port,
            string username,
            string password,
            HikvisionCardInfo card,
            string? devIndex = null,
            CancellationToken ct = default)
        {
            var baseUri = new Uri($"http://{ipAddress}:{port}/");

            using var handler = new HttpClientHandler
            {
                PreAuthenticate = true,
                UseCookies = false,
                Credentials = new CredentialCache
        {
            { baseUri, "Digest", new NetworkCredential(username, password) },
            { baseUri, "Basic",  new NetworkCredential(username, password) } // safe to include
        }
            };

            using var http = new HttpClient(handler) { BaseAddress = baseUri };
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payload = new HikvisionAddCardRequest
            {
                CardInfo = new HikvisionCardInfo
                {
                    EmployeeNo = card.EmployeeNo,
                    CardNo = card.CardNo,
                    CardType = string.IsNullOrWhiteSpace(card.CardType) ? "normalCard" : card.CardType
                }
            };

            var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);

            var url = string.IsNullOrWhiteSpace(devIndex)
                ? "/ISAPI/AccessControl/CardInfo/Record?format=json"
                : $"/ISAPI/AccessControl/CardInfo/Record?format=json&devIndex={Uri.EscapeDataString(devIndex)}";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            var response = await http.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                var wwwAuth = response.Headers.WwwAuthenticate != null
                    ? string.Join(" | ", response.Headers.WwwAuthenticate.Select(h => h.ToString()))
                    : null;

                throw new Exception(
                    $"Hikvision Add Person failed: {(int)response.StatusCode} {response.ReasonPhrase}\n" +
                    $"URL: {new Uri(baseUri, url)}\n" +
                    $"WWW-Authenticate: {wwwAuth}\n" +
                    $"{responseBody}\nREQUEST:\n{jsonBody}"
                );
            }
        }

        public async Task<HikvisionBiometricStatus?> GetUserBiometricStatusAsync(
            string ipAddress,
            int port,
            string username,
            string password,
            string employeeNo,
            string? devIndex = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(employeeNo))
                return null;

            using var http = CreateAuthHttpClient(ipAddress, port, username, password);

            var searchId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var payload = new
            {
                UserInfoSearchCond = new
                {
                    searchID = searchId,
                    searchResultPosition = 0,
                    maxResults = 1,
                    employeeNo = employeeNo
                }
            };

            var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);

            var url = string.IsNullOrWhiteSpace(devIndex)
                ? "/ISAPI/AccessControl/UserInfo/Search?format=json"
                : $"/ISAPI/AccessControl/UserInfo/Search?format=json&devIndex={Uri.EscapeDataString(devIndex)}";

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            var response = await http.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(responseBody))
                return null;

            var status = new HikvisionBiometricStatus();
            using var doc = JsonDocument.Parse(responseBody);

            if (doc.RootElement.TryGetProperty("UserInfoSearch", out var userInfoSearch)
                && userInfoSearch.TryGetProperty("UserInfo", out var userInfo))
            {
                JsonElement? record = null;

                if (userInfo.ValueKind == JsonValueKind.Array && userInfo.GetArrayLength() > 0)
                    record = userInfo[0];
                else if (userInfo.ValueKind == JsonValueKind.Object)
                    record = userInfo;

                if (record.HasValue)
                {
                    status.HasFace = (TryGetFirstInt(record.Value, "numOfFace", "faceNum") ?? 0) > 0;
                    status.HasFingerprint = (TryGetFirstInt(record.Value, "numOfFP", "numOfFingerprint", "fingerprintNum") ?? 0) > 0;
                    status.HasCard = (TryGetFirstInt(record.Value, "numOfCard", "cardNum") ?? 0) > 0;

                    status.FaceId = TryGetFirstString(record.Value, "faceId");
                    status.FingerprintId = TryGetFirstString(record.Value, "fingerPrintID", "fingerId");
                    status.CardNo = TryGetFirstString(record.Value, "cardNo");
                }
            }

            if (status.HasCard && string.IsNullOrWhiteSpace(status.CardNo))
            {
                status.CardNo = await TryFetchCardNoAsync(http, devIndex, employeeNo, ct);
            }

            return status;
        }

        private static HttpClient CreateAuthHttpClient(string ipAddress, int port, string username, string password)
        {
            var baseUri = new Uri($"http://{ipAddress}:{port}/");

            var handler = new HttpClientHandler
            {
                PreAuthenticate = true,
                UseCookies = false,
                Credentials = new CredentialCache
                {
                    { baseUri, "Digest", new NetworkCredential(username, password) },
                    { baseUri, "Basic", new NetworkCredential(username, password) }
                }
            };

            var http = new HttpClient(handler) { BaseAddress = baseUri };
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return http;
        }

        private static int? TryGetFirstInt(JsonElement element, params string[] names)
        {
            foreach (var name in names)
            {
                if (!element.TryGetProperty(name, out var value))
                    continue;

                if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var num))
                    return num;

                if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out num))
                    return num;
            }

            return null;
        }

        private static string? TryGetFirstString(JsonElement element, params string[] names)
        {
            foreach (var name in names)
            {
                if (!element.TryGetProperty(name, out var value))
                    continue;

                if (value.ValueKind == JsonValueKind.String)
                    return value.GetString();
            }

            return null;
        }

        private static async Task<string?> TryFetchCardNoAsync(
            HttpClient http,
            string? devIndex,
            string employeeNo,
            CancellationToken ct)
        {
            var searchId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var payload = new
            {
                CardInfoSearchCond = new
                {
                    searchID = searchId,
                    searchResultPosition = 0,
                    maxResults = 1,
                    employeeNo = employeeNo
                }
            };

            var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);
            var url = string.IsNullOrWhiteSpace(devIndex)
                ? "/ISAPI/AccessControl/CardInfo/Search?format=json"
                : $"/ISAPI/AccessControl/CardInfo/Search?format=json&devIndex={Uri.EscapeDataString(devIndex)}";

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            var response = await http.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(responseBody))
                return null;

            using var doc = JsonDocument.Parse(responseBody);
            if (!doc.RootElement.TryGetProperty("CardInfoSearch", out var cardInfoSearch))
                return null;

            if (!cardInfoSearch.TryGetProperty("CardInfo", out var cardInfo))
                return null;

            JsonElement? record = null;
            if (cardInfo.ValueKind == JsonValueKind.Array && cardInfo.GetArrayLength() > 0)
                record = cardInfo[0];
            else if (cardInfo.ValueKind == JsonValueKind.Object)
                record = cardInfo;

            return record.HasValue ? TryGetFirstString(record.Value, "cardNo") : null;
        }
    }
}
