from fastapi import FastAPI, HTTPException, Request
import jwt
import datetime
import bcrypt
import logging
import os
from cryptography.fernet import Fernet

app = FastAPI()

# Secret keys
SECRET_KEY = "your_secret_key"
ENCRYPTION_KEY = Fernet.generate_key()
cipher_suite = Fernet(ENCRYPTION_KEY)

# Setup logging for Intrusion Detection
logging.basicConfig(filename="security_logs.txt", level=logging.INFO, format="%(asctime)s - %(message)s")

# Dummy user database (Replace with real DB)
users = {"admin": bcrypt.hashpw("password123".encode(), bcrypt.gensalt())}

# Track failed login attempts
failed_attempts = {}

# ✅ 1. Authentication (JWT)
@app.post("/authenticate")
def authenticate(user: dict):
    username = user.get("username")
    password = user.get("password")

    if username in users and bcrypt.checkpw(password.encode(), users[username]):
        token = jwt.encode({"user": username, "exp": datetime.datetime.utcnow() + datetime.timedelta(hours=1)}, SECRET_KEY, algorithm="HS256")
        return {"token": token}
    
    # Intrusion Detection: Log failed attempts
    ip = user.get("ip") or "unknown"
    failed_attempts[ip] = failed_attempts.get(ip, 0) + 1
    if failed_attempts[ip] > 3:
        logging.warning(f"Potential Brute Force Attack from {ip}")
    
    raise HTTPException(status_code=401, detail="Invalid credentials")

# ✅ 2. Token Verification
@app.get("/verify")
def verify_token(token: str):
    try:
        decoded = jwt.decode(token, SECRET_KEY, algorithms=["HS256"])
        return {"user": decoded["user"], "status": "valid"}
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")

# ✅ 3. Data Encryption
@app.post("/encrypt")
def encrypt_data(data: dict):
    plaintext = data.get("text")
    encrypted_text = cipher_suite.encrypt(plaintext.encode()).decode()
    return {"encrypted_text": encrypted_text}

@app.post("/decrypt")
def decrypt_data(data: dict):
    encrypted_text = data.get("encrypted_text")
    decrypted_text = cipher_suite.decrypt(encrypted_text.encode()).decode()
    return {"decrypted_text": decrypted_text}

# ✅ 4. Intrusion Detection & Logging
@app.get("/logs")
def get_logs():
    if os.path.exists("security_logs.txt"):
        with open("security_logs.txt", "r") as f:
            logs = f.readlines()
        return {"logs": logs}
    return {"logs": "No logs available"}
