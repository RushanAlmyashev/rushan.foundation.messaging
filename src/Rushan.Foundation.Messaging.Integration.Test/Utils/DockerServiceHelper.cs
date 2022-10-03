using Docker.DotNet;
using Docker.DotNet.Models;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Rushan.Foundation.Messaging.Integration.Tests.Utils
{
    internal class DockerServiceHelper
    {
        public const int AssertionBrokerDelayMs = 50;
        public const int RestartDelayMs = 5000;

        private const string ImageName = @"rabbitmq";
        private const string ImageTag = @"management";

        private string FullImageName => $"{ImageName}:{ImageTag}";

        private readonly DockerClient _dockerClient;

        private DockerServiceHelper(DockerClient dockerClient)
        {
            _dockerClient = dockerClient;
        }

        public static async Task StopContainerAsync()
        {
            using (var client = CreateClient())
            {
                var helper = new DockerServiceHelper(client);
                var containerId = await helper.GetContainerId();

                if (!string.IsNullOrEmpty(containerId))
                    await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
            }
        }

        public static async Task StartContainerAsync()
        {
            using (var client = CreateClient())
            {
                var helper = new DockerServiceHelper(client);

                if (!await helper.IsImageExist())
                {
                    await helper.PullImage();
                }

                var containerId = await helper.GetContainerId();

                if (string.IsNullOrEmpty(containerId))
                {
                    containerId = await helper.CreateContainer();
                }

                if (!await helper.IsContainerRunning(containerId))
                {
                    await helper.StartContainer(containerId);
                    BrokerHelper.AwaitingBrokerStart();
                }
            }
        }

        public static async Task PauseContainerAsync()
        {
            using (var client = CreateClient())
            {
                var helper = new DockerServiceHelper(client);

                if (!await helper.IsImageExist())
                {
                    await helper.PullImage();
                }

                var containerId = await helper.GetContainerId();

                if (string.IsNullOrEmpty(containerId))
                {
                    containerId = await helper.CreateContainer();
                }

                if (await helper.IsContainerRunning(containerId))
                {
                    await client.Containers.PauseContainerAsync(containerId);                    
                }
            }
        }

        private static DockerClient CreateClient()
        {
            return new DockerClientConfiguration(new Uri(GetDockerApiUri()))
                .CreateClient();
        }

        private static string GetDockerApiUri()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
            {
                return "npipe://./pipe/docker_engine";
            }

            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            if (isLinux)
            {
                return "unix:/var/run/docker.sock";
            }

            throw new Exception("Support only Windows & Linux");
        }

        private async Task<bool> IsImageExist()
        {
            //Dictionary<string, Dictionary<string, bool>> filters = new Dictionary<string, Dictionary<string, bool>>
            //    {
            //        { ImageName, new Dictionary<string, bool>()  { { ImageName, true } } }
            //    };
            var parms = new ImagesListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["reference"] = new Dictionary<string, bool>
                    {
                        [ImageName] = true
                    }
                },
                All = true
            };

            var list = await _dockerClient.Images.ListImagesAsync(parms);

            return list.Count > 0;
        }

        private async Task PullImage()
        {
            await _dockerClient.Images
                .CreateImageAsync(new ImagesCreateParameters
                {
                    FromImage = ImageName,
                    Tag = ImageTag
                },
                new AuthConfig(),
                new Progress<JSONMessage>());
        }

        private async Task<bool> IsContainerRunning(string containerId)
        {
            if (string.IsNullOrEmpty(containerId))
                return false;

            var parms = new ContainersListParameters
            {
                All = true
            };

            var list = await _dockerClient.Containers.ListContainersAsync(parms);

            return list.Any(item => item.ID == containerId && item.State == "running");
        }

        private async Task<string> GetContainerId()
        {
            var parms = new ContainersListParameters
            {
                All = true
            };
            var list = await _dockerClient.Containers.ListContainersAsync(parms);
            var listResponse = list.FirstOrDefault(item => item.Image == FullImageName);

            return listResponse == null ? null : listResponse.ID;
        }

        private async Task<string> CreateContainer()
        {
            var port5672 = BrokerConstants.Port5672.ToString();
            var port15672 = BrokerConstants.Port15672.ToString();

            var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = FullImageName,

                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    {
                        port5672, default
                    },
                    {
                        port15672, default
                    }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { port5672, new List<PortBinding> {new PortBinding {HostPort = "5672" } }},
                    { port15672, new List<PortBinding> {new PortBinding {HostPort = "15672" } }}
                },
                    PublishAllPorts = true
                }
            });

            return response.ID;
        }

        private async Task StartContainer(string containerId)
        {
            const int Attempts = 30;
            const int DelayMs = 1000;

            await _dockerClient.Containers.StartContainerAsync(containerId, null);

            int i = 1;

            while (!await IsContainerRunning(containerId) && i < Attempts)
            {
                await Task.Delay(DelayMs);
                i++;
            }
        }
    }
}
