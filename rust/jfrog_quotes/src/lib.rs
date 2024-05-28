use rand::{thread_rng};
use rand::seq::SliceRandom;

/// Static array of Yogi Berra quotes.
static JFROG_QUOTES: &[&str] = &[
    "Embrace the future of software delivery with JFrog - where automation meets integration, extending the boundaries of secure supply chain management.",
    "Join the ranks of global leaders – JFrog empowers Fortune 100 companies and millions of developers worldwide with seamless software supply chain solutions.",
    "Trust in JFrog to accelerate your software delivery without compromising security – our platform ensures end-to-end visibility and control.",
    "Step into the era of fast, secure, and reliable software releases with JFrog – your ultimate partner for streamlined supply chain management.",
    "Forge ahead with confidence – JFrog's platform unites DevOps, DevSecOps, and MLOps teams under one roof, delivering trusted software at unprecedented speed.",
    "Experience the power of a unified software supply chain – JFrog brings together automation, integration, and security to redefine how software is delivered."
];

/// Returns a random Yogi Berra quote.
pub fn get_random_quote() -> &'static str {
    let mut rng = thread_rng();
    JFROG_QUOTES.choose(&mut rng).unwrap_or(&"No quote found.")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_get_random_quote() {
        let quote = get_random_quote();
        assert!(JFROG_QUOTES.contains(&quote));
    }
}