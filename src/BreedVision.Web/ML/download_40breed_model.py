"""
Download ujjwal75/indian-bovine-breeds-model from HuggingFace.
This model classifies 40 Indian cattle/buffalo breeds including
Red Sindhi, Rathi, Hariana, Hallikar, Deoni, Naguri, etc.

Run this script from your terminal with network access:
  python src/BreedVision.Web/ML/download_40breed_model.py
"""
import urllib.request
import os
import json
import shutil

TARGET_DIR = os.path.join(os.path.dirname(__file__), "Assets40")
os.makedirs(TARGET_DIR, exist_ok=True)

REPO_ID = "ujjwal75/indian-bovine-breeds-model"
BASE_URL = f"https://huggingface.co/{REPO_ID}/resolve/main"

FILES = [
    "Indian_bovine_finetuned_model.onnx",
    "classes.json",
]

HEADERS = {
    'User-Agent': 'Mozilla/5.0 BreedVision/1.0'
}

def download_file(filename):
    dest = os.path.join(TARGET_DIR, filename)
    url = f"{BASE_URL}/{filename}"
    print(f"Downloading {filename} from {url} ...")
    try:
        req = urllib.request.Request(url, headers=HEADERS)
        with urllib.request.urlopen(req, timeout=300) as resp, open(dest, 'wb') as out:
            total = resp.getheader('Content-Length')
            if total:
                print(f"  Size: {int(total)/1048576:.1f} MB")
            shutil.copyfileobj(resp, out)
        size_mb = os.path.getsize(dest) / 1048576
        print(f"  Saved {filename} ({size_mb:.1f} MB)")
        return True
    except Exception as e:
        print(f"  FAILED: {e}")
        return False

def try_alternate_pth():
    """Try .pth if .onnx not found, then we export it."""
    filename = "Indian_bovine_finetuned_model.pth"
    dest = os.path.join(TARGET_DIR, filename)
    url = f"{BASE_URL}/{filename}"
    print(f"Trying alternate: {filename}")
    try:
        req = urllib.request.Request(url, headers=HEADERS)
        with urllib.request.urlopen(req, timeout=300) as resp, open(dest, 'wb') as out:
            total = resp.getheader('Content-Length')
            if total:
                print(f"  Size: {int(total)/1048576:.1f} MB")
            shutil.copyfileobj(resp, out)
        size_mb = os.path.getsize(dest) / 1048576
        print(f"  Saved {filename} ({size_mb:.1f} MB)")
        return True
    except Exception as e:
        print(f"  FAILED: {e}")
        return False

if __name__ == "__main__":
    print("=" * 60)
    print(" BreedVision - 40-Breed Indian Bovine Model Downloader")
    print("=" * 60)

    success = []
    for f in FILES:
        if download_file(f):
            success.append(f)
        else:
            if f == "Indian_bovine_finetuned_model.onnx":
                # Repo might have only .pth - fall back
                if try_alternate_pth():
                    success.append("Indian_bovine_finetuned_model.pth")

    # Verify classes.json
    classes_path = os.path.join(TARGET_DIR, "classes.json")
    if os.path.exists(classes_path):
        with open(classes_path) as f:
            data = json.load(f)
        print(f"\n classes.json: {len(data)} breeds found")
        for i, name in enumerate(data if isinstance(data, list) else data.values()):
            print(f"  {i:2d}. {name}")
    
    print(f"\nDownloaded: {len(success)}/{len(FILES)} files to {TARGET_DIR}")
    print("Next step: Run export_to_onnx.py if only .pth was downloaded.")
