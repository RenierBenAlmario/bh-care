from fastapi import FastAPI, HTTPException
import jwt  
import datetime

SECRET_KEY = "your_secret_key"

app = FastAPI()

@app.post("/authenticate")
def authenticate(user: dict):
    username = user.get("username")
    password = user.get("password")
    
    # Dummy check (replace with database validation)
    if username == "admin" and password == "password123":
        token = jwt.encode({"user": username, "exp": datetime.datetime.utcnow() + datetime.timedelta(hours=1)}, SECRET_KEY, algorithm="HS256")
        return {"token": token}
    else:
        raise HTTPException(status_code=401, detail="Invalid credentials")

@app.get("/verify")
def verify_token(token: str):
    try:
        decoded = jwt.decode(token, SECRET_KEY, algorithms=["HS256"])
        return {"user": decoded["user"], "status": "valid"}
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")
