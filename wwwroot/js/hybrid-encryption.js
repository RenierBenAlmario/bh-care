/**
 * Hybrid Encryption Utilities for Client-Side Operations
 * Uses Web Crypto API for AES-256-GCM and RSA-2048 encryption
 */

class HybridEncryptionClient {
    constructor() {
        this.algorithm = {
            aes: { name: 'AES-GCM', length: 256 },
            rsa: { name: 'RSA-OAEP', hash: 'SHA-256' }
        };
    }

    /**
     * Generates a new RSA key pair
     * @returns {Promise<CryptoKeyPair>} RSA key pair
     */
    async generateRSAKeyPair() {
        try {
            return await window.crypto.subtle.generateKey(
                {
                    name: this.algorithm.rsa.name,
                    modulusLength: 2048,
                    publicExponent: new Uint8Array([1, 0, 1]),
                    hash: this.algorithm.rsa.hash
                },
                true, // extractable
                ['encrypt', 'decrypt']
            );
        } catch (error) {
            console.error('Error generating RSA key pair:', error);
            throw new Error('Failed to generate RSA key pair');
        }
    }

    /**
     * Exports RSA key to Base64 string
     * @param {CryptoKey} key - RSA key to export
     * @param {string} type - 'public' or 'private'
     * @returns {Promise<string>} Base64 encoded key
     */
    async exportRSAKey(key, type) {
        try {
            const format = type === 'public' ? 'spki' : 'pkcs8';
            const exported = await window.crypto.subtle.exportKey(format, key);
            return this.arrayBufferToBase64(exported);
        } catch (error) {
            console.error(`Error exporting ${type} key:`, error);
            throw new Error(`Failed to export ${type} key`);
        }
    }

    /**
     * Imports RSA key from Base64 string
     * @param {string} keyData - Base64 encoded key
     * @param {string} type - 'public' or 'private'
     * @returns {Promise<CryptoKey>} Imported RSA key
     */
    async importRSAKey(keyData, type) {
        try {
            const format = type === 'public' ? 'spki' : 'pkcs8';
            const keyBuffer = this.base64ToArrayBuffer(keyData);
            
            const keyUsage = type === 'public' ? ['encrypt'] : ['decrypt'];
            
            return await window.crypto.subtle.importKey(
                format,
                keyBuffer,
                {
                    name: this.algorithm.rsa.name,
                    hash: this.algorithm.rsa.hash
                },
                false,
                keyUsage
            );
        } catch (error) {
            console.error(`Error importing ${type} key:`, error);
            throw new Error(`Failed to import ${type} key`);
        }
    }

    /**
     * Encrypts data using hybrid encryption (AES + RSA)
     * @param {string} plaintext - Data to encrypt
     * @param {string} publicKeyData - RSA public key (Base64 encoded)
     * @returns {Promise<Object>} Encrypted data structure
     */
    async encrypt(plaintext, publicKeyData) {
        if (!plaintext) {
            return {
                encryptedData: '',
                encryptedKey: '',
                iv: '',
                algorithm: 'AES-256-GCM+RSA-2048'
            };
        }

        try {
            // Generate random AES key and IV
            const aesKey = await window.crypto.subtle.generateKey(
                this.algorithm.aes,
                true,
                ['encrypt', 'decrypt']
            );

            const iv = window.crypto.getRandomValues(new Uint8Array(12)); // 12 bytes for GCM

            // Encrypt data with AES-GCM
            const plaintextBuffer = new TextEncoder().encode(plaintext);
            const encryptedData = await window.crypto.subtle.encrypt(
                {
                    name: this.algorithm.aes.name,
                    iv: iv
                },
                aesKey,
                plaintextBuffer
            );

            // Export AES key
            const aesKeyBuffer = await window.crypto.subtle.exportKey('raw', aesKey);

            // Import RSA public key
            const publicKey = await this.importRSAKey(publicKeyData, 'public');

            // Encrypt AES key with RSA
            const encryptedKey = await window.crypto.subtle.encrypt(
                this.algorithm.rsa.name,
                publicKey,
                aesKeyBuffer
            );

            return {
                encryptedData: this.arrayBufferToBase64(encryptedData),
                encryptedKey: this.arrayBufferToBase64(encryptedKey),
                iv: this.arrayBufferToBase64(iv),
                algorithm: 'AES-256-GCM+RSA-2048'
            };
        } catch (error) {
            console.error('Error during hybrid encryption:', error);
            throw new Error('Encryption failed');
        }
    }

    /**
     * Decrypts data using hybrid decryption (RSA + AES)
     * @param {Object} encryptedData - Encrypted data structure
     * @param {string} privateKeyData - RSA private key (Base64 encoded)
     * @returns {Promise<string>} Decrypted plaintext
     */
    async decrypt(encryptedData, privateKeyData) {
        if (!encryptedData || !encryptedData.encryptedData) {
            return '';
        }

        try {
            // Import RSA private key
            const privateKey = await this.importRSAKey(privateKeyData, 'private');

            // Decrypt AES key with RSA
            const encryptedKeyBuffer = this.base64ToArrayBuffer(encryptedData.encryptedKey);
            const aesKeyBuffer = await window.crypto.subtle.decrypt(
                this.algorithm.rsa.name,
                privateKey,
                encryptedKeyBuffer
            );

            // Import AES key
            const aesKey = await window.crypto.subtle.importKey(
                'raw',
                aesKeyBuffer,
                this.algorithm.aes,
                false,
                ['encrypt', 'decrypt']
            );

            // Decrypt data with AES-GCM
            const iv = this.base64ToArrayBuffer(encryptedData.iv);
            const cipherData = this.base64ToArrayBuffer(encryptedData.encryptedData);

            const decryptedBuffer = await window.crypto.subtle.decrypt(
                {
                    name: this.algorithm.aes.name,
                    iv: iv
                },
                aesKey,
                cipherData
            );

            return new TextDecoder().decode(decryptedBuffer);
        } catch (error) {
            console.error('Error during hybrid decryption:', error);
            throw new Error('Decryption failed. Invalid key or corrupted data.');
        }
    }

    /**
     * Validates if a string is a valid RSA private key
     * @param {string} privateKey - Base64 encoded private key
     * @returns {Promise<boolean>} True if valid
     */
    async isValidRSAPrivateKey(privateKey) {
        try {
            if (!privateKey) return false;
            await this.importRSAKey(privateKey, 'private');
            return true;
        } catch {
            return false;
        }
    }

    /**
     * Validates if a string is a valid RSA public key
     * @param {string} publicKey - Base64 encoded public key
     * @returns {Promise<boolean>} True if valid
     */
    async isValidRSAPublicKey(publicKey) {
        try {
            if (!publicKey) return false;
            await this.importRSAKey(publicKey, 'public');
            return true;
        } catch {
            return false;
        }
    }

    /**
     * Converts ArrayBuffer to Base64 string
     * @param {ArrayBuffer} buffer - ArrayBuffer to convert
     * @returns {string} Base64 encoded string
     */
    arrayBufferToBase64(buffer) {
        const bytes = new Uint8Array(buffer);
        let binary = '';
        for (let i = 0; i < bytes.byteLength; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return btoa(binary);
    }

    /**
     * Converts Base64 string to ArrayBuffer
     * @param {string} base64 - Base64 encoded string
     * @returns {ArrayBuffer} ArrayBuffer
     */
    base64ToArrayBuffer(base64) {
        const binary = atob(base64);
        const bytes = new Uint8Array(binary.length);
        for (let i = 0; i < binary.length; i++) {
            bytes[i] = binary.charCodeAt(i);
        }
        return bytes.buffer;
    }
}

// Global instance
window.hybridEncryption = new HybridEncryptionClient();

/**
 * Legacy compatibility functions for existing code
 */

// Enhanced decryption function that handles hybrid encryption
async function decryptValueHybrid(encryptedValue, privateKey) {
    try {
        if (!encryptedValue || !privateKey) return encryptedValue;

        // Check if it's hybrid encrypted format
        if (typeof encryptedValue === 'object' && encryptedValue.encryptedData) {
            return await window.hybridEncryption.decrypt(encryptedValue, privateKey);
        }

        // Handle legacy AES256_ENCRYPTED_ format
        if (encryptedValue.startsWith('AES256_ENCRYPTED_')) {
            const base64Data = encryptedValue.replace('AES256_ENCRYPTED_', '');
            try {
                return atob(base64Data);
            } catch (e) {
                return encryptedValue;
            }
        }

        // Handle legacy ASYMMETRIC_ENCRYPTED_ format
        if (encryptedValue.startsWith('ASYMMETRIC_ENCRYPTED_')) {
            const base64Data = encryptedValue.replace('ASYMMETRIC_ENCRYPTED_', '');
            try {
                return atob(base64Data);
            } catch (e) {
                return encryptedValue;
            }
        }

        return encryptedValue;
    } catch (error) {
        console.error('Decryption error:', error);
        return encryptedValue;
    }
}

// Enhanced encryption function for hybrid encryption
async function encryptValueHybrid(plaintext, publicKey) {
    try {
        if (!plaintext) return plaintext;
        return await window.hybridEncryption.encrypt(plaintext, publicKey);
    } catch (error) {
        console.error('Encryption error:', error);
        return plaintext;
    }
}

// Export for use in other scripts
window.decryptValueHybrid = decryptValueHybrid;
window.encryptValueHybrid = encryptValueHybrid;


