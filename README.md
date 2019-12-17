# Dexih.Utils.Crypto

[build]:    https://ci.appveyor.com/project/dataexperts/dexih-utils-crypto 
[build-img]: https://ci.appveyor.com/api/projects/status/wy7esdbc8ska4xna?svg=true
[nuget]:     https://www.nuget.org/packages/dexih.utils.crypto
[nuget-img]: https://badge.fury.io/nu/dexih.utils.crypto.svg
[nuget-name]: dexih.utils.crypto
[dex-img]: https://dataexpertsgroup.com/assets/img/dex_web_logo.png
[dex]: https://dataexpertsgroup.com

[![][dex-img]][dex]

[![Build status][build-img]][build] [![Nuget][nuget-img]][nuget]

The `Crypto` library provides best practice encryption and hashing methods.  These functions can be used to safely encrypt data and hash/salt passwords.  

## Installation

Add the [latest version][nuget] of the package "dexih.utils.crypto" to a .net core/.net project.  This requires .net standard framework 2.0 or newer, or the .net framework 4.6 or newer.

## Acknowledgements

This library is based a number of publicly shared algorithms.

* Hashing - https://github.com/defuse/password-hashing/blob/master/PasswordStorage.cs
* Encryption - http://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp

## Unique Hashing

Unique hashing is used to generate a unique fixed size key from a large string of data.

This implementation uses SHA256 algorithm to generate the key.

```csharp
var hash = UniqueHash.CreateHash(data);

// to test if data changed
var compareHash = UniqueHash.CreateHash(newData);

if(hash == compareHash)
{
    // data is the same.
}
```

## Secure Hashing

Cryptographic (secure) hashing is used for storing data which does not need to be decrypted.  The common use-case scenario for this is when storing passwords, where the password is hashed, and is then used to compare to subsequent hashed versions of the password.

This implementation uses [PBKDF2](https://en.wikipedia.org/wiki/PBKDF2) with a SHA256 salt added.  This algorithm is slow by design to prevent brute-force style attacks.  

The following example shows how to hash a value:

```csharp
var hashedPassword = SecureHash.CreateHash(password);
```

Unlike `UniqueHash`, every hash is that created is different, so a value cannot be hashed and compared directly with a previously hashed value.

```csharp
var hashedPassword = SecureHash.CreateHash(password);
var comparePassword = SecureHash.CreateHash(password);

if(hashedPassword == comparePassword)
{
    // this will never happen !!!!
}
```

In order to validate a value matches an existing has the `ValidateHash` function should be used.

```csharp
var hashedPassword = SecureHash.CreateHash(password);

if(HashString.ValidateHash(password, hashedPassword))
{
    // the password is correct.
}
```

## Encrypting Data

Encryption is use to store or transport data securely, by using a common key for both encrypt/decrypt functions.

The encryption function uses an AES (Advanced Encryption Standard) algorithm to perform encryption, and generates random salt values for the encryption key.

The encrypt function can use any key, however it is generally a good idea to use a cryptographically random key to reduce the possibility of common keys being used for a brute-force attack.

Generate a random key as follows:

```csharp
// generate a key 50 characters long.
var key = EncryptString.GenerateRandomKey(50);
```

The `Encrypt` method is used to encrypt a string as follows.  Increase the derivations parameter to make the encryption slower but more secure.

```csharp
// encrypt with 1000 derivations.  
var encrypted = EncryptString.Encrypt(value, key, 1000);
```

The `Decrypt` method is used to decrypt a string as follows.

```csharp
var originalValue = EncryptString.Decrypt(encrypted, key, 1000);
```

## Asymmetric Encryption

Asymmetric encryption is used in scenarios where many public keys can be used to encrypt data, however only a secure private key can be used to decrypt the data.  A common use-case for this is when encrypting between a client/server where the server should be able to encrypt, but only the client can decrypt.

The asymmetric encryption uses the [Microsoft RSA Provider](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider?view=netframework-4.8).  The RSA provider cannot be used for larger strings, so for strings larger than 50 characters, a random key is generated RSA encrypted, and use to encrypt the data using the (symmetric) AES algorithm.

Public/Private keys must be generated as follows.  User-defined keys cannot be used for this method.  The public key can be generated multiple times, however the private key must be kept unique.
```csharp
var privateKey = AsymmetricEncrypt.GeneratePrivateKey();
var publicKey = AsymmetricEncrypt.GeneratePublicKey(privateKey);
```

The `Encrypt` method is used to encrypt a string as follows:

```csharp
var encrypted = AsymmetricEncrypt.Encrypt(value, publicKey);
```

The `Decrypt` method is used to decrypt a string as follows.

```csharp
var originalValue = AsymmetricEncrypt.Decrypt(encrypted, privateKey);
```

## Issues and Feedback

This library is provided free of charge under the MIT licence and is actively maintained by the [Data Experts Group](https://dataexpertsgroup.com)

Raise issues or bugs through the issues section of the git hub repository ([here](https://github.com/DataExperts/Dexih.Utils.Crypto/issues)).  

Pull requests are welcomed.