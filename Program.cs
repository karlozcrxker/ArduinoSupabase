using System.IO.Ports;
using System.Text;
using System.Text.Json;
using System.Globalization;

string puerto = "COM3"; // Cambia si tu Arduino usa otro COM

string apiKey = "sb_publishable_lZAgCTxQ_5lbJJCOYxWWxg_ZSFXDgw9";

string url =
    "https://ricukmhudmkwetdfrdpe.supabase.co/rest/v1/registros";

SerialPort serial = new SerialPort(puerto, 9600);

serial.ReadTimeout = 5000;

try
{
    serial.Open();

    Console.WriteLine($"Conectado a {puerto}");
    Console.WriteLine("Esperando reinicio del Arduino...");

    Thread.Sleep(3000);
}
catch (Exception ex)
{
    Console.WriteLine($"Error al abrir puerto: {ex.Message}");
    Console.ReadKey();
    return;
}

HttpClient cliente = new HttpClient();

cliente.DefaultRequestHeaders.Add("apikey", apiKey);
cliente.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

Console.WriteLine("Escuchando Arduino...");

while (true)
{
    try
    {
        string linea = serial.ReadLine().Trim();

        Console.WriteLine($"Recibido: {linea}");

        string[] datos = linea.Split(',');

        if (datos.Length != 2)
        {
            Console.WriteLine("Línea ignorada");
            continue;
        }

        float temperatura = float.Parse(
            datos[0],
            CultureInfo.InvariantCulture
        );

        float humedad = float.Parse(
            datos[1],
            CultureInfo.InvariantCulture
        );

        var registro = new
        {
            temperatura = temperatura,
            humedad = humedad,
            estado = "PRUEBA"
        };

        string json = JsonSerializer.Serialize(registro);

        Console.WriteLine("JSON enviado:");
        Console.WriteLine(json);

        var contenido = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        var respuesta = await cliente.PostAsync(
            url,
            contenido
        );

        Console.WriteLine($"HTTP: {(int)respuesta.StatusCode}");

        string respuestaTexto =
            await respuesta.Content.ReadAsStringAsync();

        Console.WriteLine("Respuesta Supabase:");
        Console.WriteLine(respuestaTexto);

        Console.WriteLine("--------------------------------");
    }
    catch (TimeoutException)
    {
        Console.WriteLine("Esperando datos...");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        break;
    }
}

serial.Close();