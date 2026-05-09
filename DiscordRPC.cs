using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FakeExe
{
    public class DiscordRPC
    {
        private NamedPipeClientStream _pipe;
        private bool _connected = false;
        private string _clientId;
        private Thread _readThread;
        private bool _running = false;

        // Default Client ID - works for basic presence
        // Users can use their own from Discord Developer Portal
        public DiscordRPC(string clientId = "1234567890123456789")
        {
            _clientId = clientId;
        }

        public bool Connect()
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    _pipe = new NamedPipeClientStream(".", $"discord-ipc-{i}", PipeDirection.InOut, PipeOptions.Asynchronous);
                    _pipe.Connect(1000);
                    _connected = true;
                    _running = true;

                    // Send handshake
                    SendHandshake();

                    // Start reading thread
                    _readThread = new Thread(ReadLoop) { IsBackground = true };
                    _readThread.Start();

                    return true;
                }
                catch
                {
                    _pipe?.Dispose();
                }
            }
            return false;
        }

        private void ReadLoop()
        {
            byte[] buffer = new byte[4096];
            while (_running && _connected)
            {
                try
                {
                    if (_pipe.IsConnected)
                    {
                        int bytesRead = _pipe.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;
                    }
                    else break;
                }
                catch { break; }
                Thread.Sleep(100);
            }
        }

        private void SendHandshake()
        {
            string handshake = $"{{\"v\":1,\"client_id\":\"{_clientId}\"}}";
            WritePacket(0, handshake);

            // Read handshake response
            byte[] header = new byte[8];
            _pipe.Read(header, 0, 8);
            int len = BitConverter.ToInt32(header, 4);
            byte[] data = new byte[len];
            _pipe.Read(data, 0, len);
        }

        private void WritePacket(int opcode, string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] header = new byte[8];
            BitConverter.GetBytes(opcode).CopyTo(header, 0);
            BitConverter.GetBytes(dataBytes.Length).CopyTo(header, 4);

            _pipe.Write(header, 0, 8);
            _pipe.Write(dataBytes, 0, dataBytes.Length);
            _pipe.Flush();
        }

        public void SetPresence(string gameName, string details, string state, string largeImageKey = "game", string largeImageText = "")
        {
            if (!_connected) return;

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            string largeText = string.IsNullOrEmpty(largeImageText) ? gameName : largeImageText;

            // Show game name as details (line 1), user details as state (line 2)
            // This makes the game name appear prominently like CustomRP does
            string line1 = gameName;
            string line2 = string.IsNullOrEmpty(details) ? state : details;
            string line3 = string.IsNullOrEmpty(details) ? "" : state;

            string payload = $@"{{
                ""cmd"": ""SET_ACTIVITY"",
                ""args"": {{
                    ""pid"": {System.Diagnostics.Process.GetCurrentProcess().Id},
                    ""activity"": {{
                        ""details"": ""{EscapeJson(line1)}"",
                        ""state"": ""{EscapeJson(line2)}"",
                        ""timestamps"": {{
                            ""start"": {timestamp}
                        }},
                        ""assets"": {{
                            ""large_image"": ""{EscapeJson(largeImageKey)}"",
                            ""large_text"": ""{EscapeJson(largeText)}""
                        }}
                    }}
                }},
                ""nonce"": ""{Guid.NewGuid()}""
            }}";

            try
            {
                WritePacket(1, payload);
            }
            catch
            {
                _connected = false;
            }
        }

        public void ClearPresence()
        {
            if (!_connected) return;

            string payload = $@"{{
                ""cmd"": ""SET_ACTIVITY"",
                ""args"": {{
                    ""pid"": {System.Diagnostics.Process.GetCurrentProcess().Id},
                    ""activity"": null
                }},
                ""nonce"": ""{Guid.NewGuid()}""
            }}";

            try
            {
                WritePacket(1, payload);
            }
            catch { }
        }

        public void Disconnect()
        {
            _running = false;
            _connected = false;
            try { ClearPresence(); } catch { }
            try { _pipe?.Close(); _pipe?.Dispose(); } catch { }
        }

        public bool IsConnected => _connected;

        private string EscapeJson(string s)
        {
            return s?.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r") ?? "";
        }
    }
}
