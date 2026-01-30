<div align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://github.com/isahir0711/pport/raw/master/assets/images/pport-logo-white.png">
    <source media="(prefers-color-scheme: light)" srcset="https://github.com/isahir0711/pport/raw/master/assets/images/pport-logo-black.png">
    <img alt="logo" src="https://github.com/isahir0711/pport/raw/master/assets/images/pport-logo-black.png" width="50%">
  </picture>
</div>

<h3 align="center">
  CLI Tool for port management
  <br/>
  Show listening ports and kill the owning processes
</h3>

## Instalation (WIP)

### Download Release (Recommended)

Download the latest release from [GitHub Releases](https://github.com/isahir0711/pport/releases):

## Usage

### Basic Usage

List all listening ports:

```bash
# Prints currently LISTEN ports:
pport
# Kill process running on a specific port:
pport --kill 3000
# Auto-refresh every second:
pport --watch 1s
# Include connections in states other than LISTEN (e.g., ESTABLISHED):
pport --state ESTABLISHED
# Show only a specific port:
pport --port 8080
# Export to JSON:
pport --json
# Export to CSV:
pport --csv
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

## Supported Platforms

- Linux (requires `/proc` filesystem)

## Roadmap

### CLI Arguments

- [x] `--watch 1s`  
       Continuously refresh port and process information at a specified interval

- [x] `--port 8080`  
       Filter results by a specific port

- [x] `--state <state>`  
       Include ports in states other than `LISTEN` (e.g. `ESTABLISHED`)

- [x] `--csv`, `--json`  
       Export results in machine-readable formats

- [x] `--kill processname`  
       Try to kill the process

- [x] IPv6 support
