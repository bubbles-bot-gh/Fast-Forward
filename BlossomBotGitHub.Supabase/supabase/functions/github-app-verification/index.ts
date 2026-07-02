import { withSupabase } from "npm:@supabase/server@^1";
import { createAppAuth } from "npm:@octokit/auth-app@^7";
import { Octokit } from "npm:@octokit/rest@^21";
import * as jose from "npm:jose@^5";

interface ReqPayload {
    installationId: string;
}

const GH_OIDC_ISSUER: string = "https://token.actions.githubusercontent.com";

// Cache JWKs
const JWKS = jose.createRemoteJWKSet(
    new URL(`${GH_OIDC_ISSUER}/.well-known/jwks`)
);

// Handler
Deno.serve(async (req) => {
    // Verify JWT
    const { token } = await req.json();
    let claims;
    try {
        claims = await verifyJWT(token, "blossom-bot-github-audience")
    } catch (err) {
        return new Response("Invalid token", { status: 401 });
    }
    
    // Get repo info
    const repo = claims.repository as string
    const [owner, repoName] = repo.split('/');
    
    // Get installation ID
    const appOctokit = new Octokit({
        authStrategy: createAppAuth,
        auth: {
            appId: Deno.env.get("GH_APP_ID"),
            privateKey: Deno.env.get("GH_APP_PRIVATE_KEY")
        }
    });
    
    const { data: installation } = await appOctokit.request(
        "GET /repos/{owner}/{repo}/installation",
        { owner, repoName }
    );
    
    // Get installation access token
    const installationAuth = createAppAuth({
        appId: Deno.env.get("GH_APP_ID"),
        privateKey: Deno.env.get("GH_APP_PRIVATE_KEY"),
    });
    
    const { token } = await installationAuth({
        type: "installation",
        installationId: installation.id,
        repositoryNames: [ repoName ],
        permissions: { contents: "read" }
    });
    
    // Return new access token
    return new Response(JSON.stringify({ token }), {
        headers: { "Content-Type": "application/json" }
    });
})

// Helpers
async function verifyJWT(token: string, expectedAud: string) {
    const { payload } = await jose.jwtVerify(token, JWKS, {
        issuer: GH_OIDC_ISSUER,
        audience: expectedAud
    });
    
    return payload;
}