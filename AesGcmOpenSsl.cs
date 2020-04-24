using System;
using System.Runtime.InteropServices;

namespace Ropu.AesGcm
{
    public class AesGcmOpenSsl : IDisposable
    {
        readonly byte[] _key;
        readonly IntPtr _cipherContext;
        readonly IntPtr _cipher;
        
        public AesGcmOpenSsl(byte[] key)
        {
            _key = key;
            _cipherContext = NativeMethods.EVP_CIPHER_CTX_new();
            _cipher = NativeMethods.EVP_aes_256_gcm();
            int result = NativeMethods.EVP_EncryptInit(_cipherContext, _cipher, IntPtr.Zero, IntPtr.Zero);
            CheckResult(result, "EVP_EncryptInit_ex");
        }

        public void Encrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> plaintext, Span<byte> ciphertext, Span<byte> tag)
        {
            int result = NativeMethods.EVP_EncryptInit_ex(_cipherContext, IntPtr.Zero, IntPtr.Zero, _key, ref MemoryMarshal.GetReference(nonce));
            CheckResult(result, "EVP_EncryptInit_ex");

            result = NativeMethods.EVP_EncryptUpdate(_cipherContext, ref MemoryMarshal.GetReference(ciphertext), out int outLength, ref MemoryMarshal.GetReference(plaintext), plaintext.Length);
            CheckResult(result, "EVP_EncryptUpdate");

            result = NativeMethods.EVP_EncryptFinal_ex(_cipherContext, ref MemoryMarshal.GetReference(ciphertext), out int length);
            CheckResult(result, "EVP_EncryptFinal_ex");

            result = NativeMethods.EVP_CIPHER_CTX_ctrl(_cipherContext, NativeMethods.EVP_CTRL_AEAD_GET_TAG, tag.Length, ref MemoryMarshal.GetReference(tag));
            CheckResult(result, "EVP_CIPHER_CTX_ctrl");
        }

        void CheckResult(int result, string methodName)
        {
            if(result != 1)
            {
                throw new Exception($"OpenSSL {methodName} returned error {result}");
            }
        }

        public void Decrypt(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> tag, Span<byte> plaintext)
        {
            int result = NativeMethods.EVP_DecryptInit_ex(_cipherContext, IntPtr.Zero, IntPtr.Zero, _key, ref MemoryMarshal.GetReference(nonce));
            CheckResult(result, "EVP_DecryptInit_ex");

            result = NativeMethods.EVP_DecryptUpdate(_cipherContext, ref MemoryMarshal.GetReference(plaintext), out int outLength, ref MemoryMarshal.GetReference(ciphertext), ciphertext.Length);
            CheckResult(result, "EVP_DecryptUpdate");


            NativeMethods.EVP_CIPHER_CTX_ctrl(_cipherContext, NativeMethods.EVP_CTRL_AEAD_SET_TAG, tag.Length, ref MemoryMarshal.GetReference(tag));
            CheckResult(result, "EVP_CIPHER_CTX_ctrl");

            result = NativeMethods.EVP_DecryptFinal_ex(_cipherContext, ref MemoryMarshal.GetReference(plaintext), out int length);
            if(result != 1)
            {
                throw new Exception("Decrypt failed because Tag didn't match");
            }
        }

        #region IDisposable Support
        bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                NativeMethods.EVP_CIPHER_CTX_free(_cipherContext);
                disposedValue = true;
            }
        }

        ~AesGcmOpenSsl()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}