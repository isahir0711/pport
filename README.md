<h1 align="center">
  pport
</h3>

<h3 align="center">
  CLI Tool for port management
  <br/>
  Show listening ports and kill the owning processes
</h3>

## Installation

### Run

```bash
curl -fsSl https://github.com/isahir0711/pport/raw/master/install.sh | bash
```

### Download Release

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

- Linux

## Contributing

If you want to: expand, change or fix anything please feel free to do it.

1. Fork the repo
2. Do your changes
3. Create a PR
