# Rust and Cargo with Integration With Artifactory

# Cargo Package Manager Integration with Artifactory
A repo to demonstrate Rust's package manager, Cargo, and how it integrates with Artifactory.

# Project Goal
The two Rust projects in this folder will demonstrate the basics of integrating Rust's package manager, Cargo, with
Jfrog's Artifactory.

# Project Structure
This repo will hold two projects: an [Inner Source](https://en.wikipedia.org/wiki/Inner_source) library, to be built and published with Artifactory's Cargo
integration, and a "Production" application that will incorporate the library.

## Inner Source Library
A simple library that will return a randomly-selected quote about JFrog, The Secure Software Supply Chain experts.

## Production Application
A basic Rust application that will incorporate the Inner Source library.

## Pre Reqs (Local Build)
1. Install Cargo
2. Configure Cargo
3. Setup `~/.cargo/credentials`.  Note, if you use a credentials file, DO NOT check this into Version Control
```toml
 [registries.artifactory]                                                  
 token = "Bearer <access token>"
                  
 [registry]         
 token = "<access token>"   
```
Alternatively, you can set your various registries' authentication tokens with environment variables, or other credentials providers. (TODO: Add links)
This example will rely on the most simple: an environment variable.  This is done by following this pattern: `export CARGO_REGISTRIES_<NAME>_TOKEN="Bearer <secret>"`
For example, this will set the token for the registry set in `.cargo/config.toml` named "foo" with the value of "Bearer bar"
```bash
export CARGO_REGISTRIES_FOO_TOKEN="Bearer bar"
```
## Cargo configuration
TODO: Provide details on the library's `Cargo.toml`, esp around where to resolve external dependencies
## Steps to build and publish the innersource library, `jfrog_quotes`
### Build the Library
```bash
cd rust/jfrog_quotes
cargo build --release
```
### Publish the Library
```bash
cargo publish --registry innersource
```

