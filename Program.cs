using System.Text;

HttpClient cliente = new HttpClient();

// TU CLAVE PUBLICABLE DE SUPABASE
string apiKey = "sb_publishable_lZAgCTxQ_5lbJJCOYxWWxg_ZSFXDgw9";

// Cabeceras requeridas por Supabase
cliente.DefaultRequestHeaders.Add("apikey", apiKey);
cliente.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

// Registro de prueba
string json = """
{
    "temperatura": 25.5,
    "humedad": 60.2,
    "estado": "ESTABLE"
}
""";

var contenido = new StringContent(
    json,
    Encoding.UTF8,
    "application/json"
);

// URL de tu tabla registros
string url =
    "https://ricukmhudmkwetdfrdpe.supabase.co/rest/v1/registros";

var respuesta = await cliente.PostAsync(url, contenido);

Console.WriteLine($"Código HTTP: {(int)respuesta.StatusCode}");

string respuestaTexto =
    await respuesta.Content.ReadAsStringAsync();

Console.WriteLine("Respuesta:");
Console.WriteLine(respuestaTexto);

Console.WriteLine("\nPresiona una tecla para salir...");
Console.ReadKey();