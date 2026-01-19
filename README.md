# PPORT

PPORT is a lightweight CLI tool for Linux that lists **currently listening network ports** and the **processes that own them**, including process names and command-line arguments.

## Features

- List listening ports
- Resolve ports to:
  - PID
  - Process name
  - Command line

## Supported Platforms

- Linux (requires `/proc` filesystem)

## Usage

```bash
pport
```

# Example output

```bash
┌─────────┬──────────────────────────┬──────────────────────────┐
│ PORT    │ PROCESS NAME             │ COMMANDLINE              │
├─────────┼──────────────────────────┼──────────────────────────┤
│ 80      │ nginx                    │ /usr/sbin/nginx          │
│ 3000    │ node                     │ /usr/bin/server.js       │
└─────────┴──────────────────────────┴──────────────────────────┘
```

## Roadmap

### CLI Arguments

- `--watch 1s`  
  Continuously refresh port and process information at a specified interval

- `--port 8080`  
  Filter results by a specific port

- `--state <state>`  
  Include ports in states other than `LISTEN` (e.g. `ESTABLISHED`)

- `--csv`, `--json`  
  Export results in machine-readable formats

- `--kill 80`  
  Try to kill the process for the specified port

### Networking Support

- IPv6 support
- UDP port listing
