if __name__ == "__main__":
    import uvicorn
    import os

    dir_path = os.path.dirname(os.path.realpath(__file__))
    print("path: ", dir_path)
    uvicorn.run("main:app", host="10.0.0.12", port=8000, ssl_keyfile='private-key.pem', ssl_certfile='cert.pem',
                app_dir=dir_path)
