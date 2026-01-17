job "blazorapp1" {
  datacenters = ["dc1"]
  type = "service"

  group "web" {

    network {
      port "http" {}
    }

    task "static" {
      driver = "raw_exec"

      config {
        command = "local\\StaticHost.exe"
        args = [
          "--urls=http://0.0.0.0:${NOMAD_PORT_http}"
        ]
      }

      artifact {
        # This is your local dev HTTP server hosting the ZIP
        source      = "http://localhost/BlazorApp1.zip"
        destination = "local/"
      }

      service {
          name = "blazorapp1"
          port = "http"

          check {
            name     = "health"
            type     = "http"
            path     = "/health"
            interval = "10s"
            timeout  = "2s"
          }
        }

      resources {
        cpu    = 200
        memory = 256
      }
    }
  }
}