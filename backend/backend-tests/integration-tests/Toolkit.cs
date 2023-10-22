namespace ExampleWebApp.Backend.Tests.Integration;

public static class Toolkit
{

    public static JwtSecurityToken? DecodeToJwtSecurityToken(string jwtEncoded)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadToken(jwtEncoded) as JwtSecurityToken;
    }

    public static SymmetricSecurityKey RandomJwtEncryptionKey()
    {
        var rsaKey = RSA.Create();
        var privateKey = rsaKey.ExportRSAPrivateKey();
        return new SymmetricSecurityKey(privateKey);
    }

}