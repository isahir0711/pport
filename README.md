<div align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://github.com/isahir0711/pport/raw/master/assets/images/pport-logo-white.png">
    <source media="(prefers-color-scheme: light)" srcset="https://github.com/isahir0711/pport/raw/master/assets/images/pport-logo-black.png">
    <img alt="logo" src="https://github.com/isahir0711/pport/raw/master/assets/images/pport-logo-black.png" width="50%">
  </picture>
</div>

PPORT is a lightweight CLI tool for Linux that lists **currently listening network ports** and the **processes that own them**, so you can close them.

## Features

- List listening ports
- Resolve ports to:
  - PID
  - Process name
  - Command line
  - Protocol Version
- Kill a process

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

- [x] `--watch 1s`  
       Continuously refresh port and process information at a specified interval

- [ ] `--port 8080`  
       Filter results by a specific port

- [x] `--state <state>`  
       Include ports in states other than `LISTEN` (e.g. `ESTABLISHED`)

- [x] `--csv`, `--json`  
       Export results in machine-readable formats

- [x] `--kill processname`  
       Try to kill the process

### Networking Support

- [x] IPv6 support
- [ ] UDP port listing
