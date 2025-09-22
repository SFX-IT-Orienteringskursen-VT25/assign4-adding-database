using Docker.DotNet;
using Docker.DotNet.Models;
namespace AdditionApi;

public class DockerStarter
{
    public static async Task StartDockerContainerAsync(CancellationToken ct = default)
    {
        // Connect to Docker Desktop on macOS
        var dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();

        // 1) Pull image for AMD64 (required on Apple silicon)
        await dockerClient.Images.CreateImageAsync(
            new ImagesCreateParameters
            {
                FromImage = "mcr.microsoft.com/mssql/server",
                Tag = "2022-latest",
                Platform = "linux/amd64"
            },
            authConfig: null,
            progress: new Progress<JSONMessage>(m =>
            {
                if (!string.IsNullOrWhiteSpace(m.Status))
                    Console.WriteLine($"{m.Status} {m.ProgressMessage}");
            }),
            cancellationToken: ct
        );

        // 2) Start existing container if present
        if (await StartContainerIfItExists(dockerClient, ct)) return;

        // 3) Create container (note: MSSQL_SA_PASSWORD + Platform)
        var container = await CreateContainer(dockerClient, ct);

        await dockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters(), ct);
    }

    private static async Task<bool> StartContainerIfItExists(DockerClient dockerClient, CancellationToken ct)
    {
        var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true }, ct);
        var existing = containers.FirstOrDefault(c => c.Names.Any(n => n.TrimStart('/') == "sqlserver"));

        if (existing != null)
        {
            if (existing.State != "running")
            {
                await dockerClient.Containers.StartContainerAsync(existing.ID, new ContainerStartParameters(), ct);
            }
            return true;
        }
        return false;
    }

    private static async Task<CreateContainerResponse> CreateContainer(DockerClient dockerClient, CancellationToken ct)
    {
        var resp = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "mcr.microsoft.com/mssql/server:2022-latest",
            Name = "sqlserver",
            Platform = "linux/amd64", // <- important on Apple silicon
            Env = new List<string>
            {
                "ACCEPT_EULA=Y",
                // Use the correct env var name:
                "MSSQL_SA_PASSWORD=" + SqlCredentials.Password   // not SA_PASSWORD
            },
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                { "1433/tcp", default }
            },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "1433/tcp", new List<PortBinding> { new PortBinding { HostPort = "1433" } } }
                }
            }
        }, ct);

        return resp;
    }
}
