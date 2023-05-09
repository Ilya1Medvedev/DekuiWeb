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
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_terminalControllerOptions.Value.BaseUrl}/api/addproduct", addProductRequest);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> TryAddReservation(AddReseravationRequest addReseravationRequest)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_terminalControllerOptions.Value.BaseUrl}/api/addreservation", addReseravationRequest);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {

            return false;
        }

    }
}
