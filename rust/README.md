# Rust and Cargo with Integration With Artifactory

# Cargo Package Manager Integration with Artifactory
This is a small repo to demonstrate Rust's package manager, Cargo, and how it integrates with Artifactory.

# Project Goal
The two Rust projects in this folder will demonstrate the basics of integrating Rust's package manager, Cargo, with
Jfrog's Artifactory.

# Project Structure
This repo will hold two projects: an [Inner Source](https://en.wikipedia.org/wiki/Inner_source) library, to be built and published with Artifactory's Cargo integration, and a "Production" application that will incorporate the library.

## Inner Source Library
This is a simple library that will return a randomly-selected quote about JFrog, The Secure Software Supply Chain 
experts.

## Production Application
A basic Rust application that will incorporate the Inner Source library.  Upon execution, it will print out a 
randomly selected quote to the console. 

## Pre Reqs (Local Build)
1. Install Cargo
2. Configure Cargo
3. Create three repositories in an instance of Artifactory: One remote repository that proxies `crates.io`, one 
   local repository to host the "innersource" library and one local repository to host the actual application binary.
4. Using these repositories edit the two Cargo config files as follows:
`jfrog_app/.cargo/config.toml`: This file will take the "innersource" library local repo and 
```toml
[registry]
default = "jfrogquotes"

[registries.jfrogquotes]
index = "sparse+https://tomjfrog.jfrog.io/artifactory/api/cargo/jfrogquotes-cargo-local/index/"

[registries.innersource]
index = "sparse+https://tomjfrog.jfrog.io/artifactory/api/cargo/innersource-cargo-local/index/"
```
`jfrog_quotes/.cargo/config.toml`
```toml
[registry]
default = "innersource"

[registries.innersource]
index = "sparse+https://BASE_URL/artifactory/api/cargo/innersource-cargo-local/index/"

[registries.crates-remote]
index = "sparse+https://BASE_URL/artifactory/api/cargo/crates-remote/index/"
```
5. Setup `~/.cargo/credentials`.  Note, if you use a credentials file, DO NOT check this into Version Control
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
To build the library, first run a local build.  Enter the root of the `rust` example and execute `cargo build 
--release`.  

> Note:  The first time this is run, a file called `Cargo.lock` will be generated.  It's important to commit this
> to version control.  The version of the library in the remote Cargo remote, hosted in Artifactory, will be pinned 
> to a specific SHA associated with the version.  By checking in the Cargo.lock, it will ensure that the correct SHA 
> will always be used.

```bash
cd rust/jfrog_quotes
cargo build --release
```
### Publish the Library
```bash
cargo publish --registry innersource
```

## Advanced Use-Case: Execute build and deploy in Github Actions
Github Actions, one of the most popular CI tools on the market today, has native support for Cargo in it's hosted 
runners.  The example here has two Github Actions specs: building the library and building the application.  Follow 
these steps to get this use-case implemented.

### Add Repository Variable for JFrog URL


