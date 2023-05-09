using BakWeb.TerminalControllerClient.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BakWeb.TerminalControllerClient;

public class TerminalClient
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<TerminalControllerOptions> _terminalControllerOptions;

    public TerminalClient(HttpClient httpClient, IOptions<TerminalControllerOptions> terminalControllerOptions)
    {
        _httpClient = httpClient;
        _terminalControllerOptions = terminalControllerOptions;
    }

    public async Task<bool> TryAddProduct(AddProductRequest addProductRequest)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_terminalControllerOptions.Value.BaseUrl}/api/addproduct", addProductRequest);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> TryAddReservation(AddReseravationRequest addReseravationRequest)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_terminalControllerOptions.Value.BaseUrl}/api/addreservation", addReseravationRequest);
        return response.IsSuccessStatusCode;
    }

    public async Task<HttpResponseMessage> AddProduct(AddProductRequest addProductRequest) =>
        await _httpClient.PostAsJsonAsync($"{_terminalControllerOptions.Value.BaseUrl}/api/addproduct", addProductRequest);

    public async Task<HttpResponseMessage> AddReservation(AddReseravationRequest addReseravationRequest) =>
       await _httpClient.PostAsJsonAsync($"{_terminalControllerOptions.Value.BaseUrl}/api/addreservation", addReseravationRequest);

    public async Task<HttpResponseMessage> Ping() => 
        await _httpClient.PostAsJsonAsync($"{_terminalControllerOptions.Value.BaseUrl}", new { });

}
