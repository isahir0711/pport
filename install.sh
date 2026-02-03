#!/bin/bash


APP_NAME="pport"
INSTALL_DIR="${INSTALL_DIR:-$HOME/.local/bin}"
REPO_URL="https://github.com/isahir0711/pport"
VERSION="${VERSION:-latest}"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

detect_platform() {
    OS="$(uname -s)"
    ARCH="$(uname -m)"

    case "$OS" in
        Linux*)
            OS="linux"
            ;;
        Darwin*)
            OS="macos"
            ;;
        MINGW*|MSYS*|CYGWIN*)
            print_error "Windows is not supported by this installer. Please download the Windows version manually."
            exit 1
            ;;
        *)
            print_error "Unsupported operating system: $OS"
            exit 1
            ;;
    esac

    case "$ARCH" in
        x86_64|amd64)
            ARCH="x64"
            ;;
        aarch64|arm64)
            ARCH="arm64"
            ;;
        *)
            print_error "Unsupported architecture: $ARCH"
            exit 1
            ;;
    esac

    print_info "Detected platform: $OS-$ARCH"
}

check_dependencies() {
    print_info "Checking dependencies..."

    if [! command -v curl &> /dev/null]; then
        print_error "cURL not found. Please install it."
        exit 1
    fi

    if ! command -v unzip &> /dev/null && ! command -v tar &> /dev/null; then
        print_error "Neither unzip nor tar found. Please install one of them."
        exit 1
    fi
}

download_file() {
    local url="$1"
    local output="$2"

    if command -v curl &> /dev/null; then
        curl -fsSL "$url" -o "$output"
    else
        wget -q "$url" -O "$output"
    fi
}

get_latest_version() {
    if [ "$VERSION" = "latest" ]; then
        print_info "Fetching latest version..."
        VERSION=$(curl -fsSL "https://api.github.com/repos/isahir0711/pport/releases/latest" | grep '"tag_name":' | sed -E 's/.*"([^"]+)".*/\1/')

        if [ -z "$VERSION" ]; then
            print_error "Could not determine latest version"
            exit 1
        fi
        print_info "Latest version: $VERSION"
    fi
}

install_app() {
    print_info "Installing $APP_NAME..."

    TMP_DIR=$(mktemp -d)

    cd "$TMP_DIR"

    DOWNLOAD_URL="$REPO_URL/releases/download/$VERSION/$APP_NAME-$OS-$ARCH.tar.gz"

    print_info "Downloading from: $DOWNLOAD_URL"
    download_file "$DOWNLOAD_URL" "$APP_NAME.tar.gz"

    print_info "Extracting..."
    tar -xzf "$APP_NAME.tar.gz"

    mkdir -p "$INSTALL_DIR"

    chmod +x "$APP_NAME"
    mv "$APP_NAME" "$INSTALL_DIR/"

    cd - > /dev/null
    rm -rf "$TMP_DIR"

    print_info "Installation complete!"
}

update_path() {
    if [[ ":$PATH:" != *":$INSTALL_DIR:"* ]]; then
        print_warning "$INSTALL_DIR is not in your PATH"

        SHELL_NAME=$(basename "$SHELL")

        case "$SHELL_NAME" in
            bash)
                RC_FILE="$HOME/.bashrc"
                ;;
            zsh)
                RC_FILE="$HOME/.zshrc"
                ;;
            fish)
                RC_FILE="$HOME/.config/fish/config.fish"
                ;;
            *)
                RC_FILE="$HOME/.profile"
                ;;
        esac

        echo ""
        print_info "Add this to your $RC_FILE:"
        echo -e "  ${GREEN}export PATH=\"\$PATH:$INSTALL_DIR\"${NC}"
        echo ""
        print_info "Then run: source $RC_FILE"
    else
        print_info "You can now run: $APP_NAME"
    fi
}
main() {
    echo ""
    print_info "Installing $APP_NAME..."
    echo ""

    detect_platform
    check_dependencies
    get_latest_version
    install_app
    update_path

    echo ""
    print_info "✓ Installation successful!"
}

main