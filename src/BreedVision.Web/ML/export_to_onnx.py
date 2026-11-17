"""
Export Indian Bovine 41-breed PyTorch model (ConvNeXt-Tiny via timm) to ONNX.

Requirements:
  pip install torch torchvision timm onnx

Run:
  python src/BreedVision.Web/ML/export_to_onnx.py

Converts Assets40/Indian_bovine_finetuned_model.pth -> Assets/model_41breed.onnx
"""
import os, json, sys
import torch
import torch.nn as nn
import timm

ASSETS40  = os.path.join(os.path.dirname(__file__), "Assets40")
ASSETS_OUT = os.path.join(os.path.dirname(__file__), "Assets")
PTH_PATH  = os.path.join(ASSETS40, "Indian_bovine_finetuned_model.pth")
CLS_PATH  = os.path.join(ASSETS40, "classes.json")
ONNX_OUT  = os.path.join(ASSETS_OUT, "model_41breed.onnx")

def main():
    print("=" * 60)
    print(" BreedVision — Export 41-Breed ConvNeXt-Tiny to ONNX")
    print("=" * 60)

    # 1. Load class list
    with open(CLS_PATH) as f:
        classes = json.load(f)
    num_classes = len(classes)
    print(f"Classes: {num_classes}")

    # 2. Build ConvNeXt-Tiny with 41 classes
    print(f"\nBuilding timm ConvNeXt-Tiny (num_classes={num_classes})...")
    model = timm.create_model("convnext_tiny", pretrained=False, num_classes=num_classes)

    # 3. Load weights
    print(f"Loading weights from {PTH_PATH}...")
    state = torch.load(PTH_PATH, map_location="cpu")

    if isinstance(state, dict):
        if "model_state_dict" in state:
            state = state["model_state_dict"]
        elif "state_dict" in state:
            state = state["state_dict"]
        new_state = {}
        for k, v in state.items():
            new_k = k.replace("module.", "")
            new_state[new_k] = v
        state = new_state

    model.load_state_dict(state, strict=True)
    model.eval()
    print("Weights loaded successfully.")

    # 4. Run dummy forward pass
    dummy = torch.randn(1, 3, 224, 224)
    with torch.no_grad():
        out = model(dummy)
    print(f"Dummy forward pass: input {list(dummy.shape)} -> output {list(out.shape)}")
    if out.shape[1] != num_classes:
        print(f"ERROR: output classes {out.shape[1]} != {num_classes}")
        sys.exit(1)

    # 5. Export to ONNX
    os.makedirs(ASSETS_OUT, exist_ok=True)
    print(f"\nExporting to {ONNX_OUT}...")
    torch.onnx.export(
        model,
        dummy,
        ONNX_OUT,
        export_params=True,
        opset_version=18,
        dynamo=False,
        do_constant_folding=True,
        input_names=["input"],
        output_names=["output"],
        dynamic_axes={"input": {0: "batch_size"}, "output": {0: "batch_size"}},
    )

    size_mb = os.path.getsize(ONNX_OUT) / 1048576
    print(f"Exported: {ONNX_OUT} ({size_mb:.1f} MB)")

    # 6. Quick ONNX Runtime verification
    try:
        import onnxruntime as ort
        sess = ort.InferenceSession(ONNX_OUT)
        import numpy as np
        inp = np.random.randn(1, 3, 224, 224).astype(np.float32)
        res = sess.run(None, {"input": inp})
        pred_idx = int(np.argmax(res[0]))
        print(f"ONNX Runtime verification: predicted class index {pred_idx} = '{classes[pred_idx]}'")
        print("\n SUCCESS — model_41breed.onnx is ready!")
    except ImportError:
        print("\n SUCCESS — model_41breed.onnx exported (onnxruntime not installed for python, C# onnxruntime will use it).")

if __name__ == "__main__":
    main()
