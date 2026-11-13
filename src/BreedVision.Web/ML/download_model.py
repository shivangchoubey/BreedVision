import urllib.request
import os
import json

TARGET_DIR = os.path.join(os.path.dirname(__file__), "Assets")
os.makedirs(TARGET_DIR, exist_ok=True)

BASE_URL = "https://huggingface.co/vishnuamar/cattle-breed-classifier/resolve/main"
FILES_TO_DOWNLOAD = [
    "metadata.json",
    "config.json",
    "model.onnx",
    "Gir_sample.jpg",
    "Bhadawari_sample.jpg",
    "Jaffrabadi_sample.png"
]

def download():
    for filename in FILES_TO_DOWNLOAD:
        dest_path = os.path.join(TARGET_DIR, filename)
        url = f"{BASE_URL}/{filename}"
        print(f"Downloading {filename} from {url}...")
        try:
            req = urllib.request.Request(
                url, 
                headers={'User-Agent': 'Mozilla/5.0'}
            )
            with urllib.request.urlopen(req, timeout=120) as response, open(dest_path, 'wb') as out_file:
                total_size = response.getheader('Content-Length')
                if total_size:
                    print(f"File size: {int(total_size)/(1024*1024):.2f} MB")
                out_file.write(response.read())
            print(f"Successfully downloaded {filename} ({os.path.getsize(dest_path)} bytes)")
        except Exception as e:
            print(f"Failed to download {filename}: {e}")

if __name__ == "__main__":
    download()
