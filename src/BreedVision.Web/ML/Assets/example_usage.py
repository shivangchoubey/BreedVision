"""
Hugging Face Model Hub Integration Example
==========================================

This script demonstrates how to use the cattle breed classification model
from Hugging Face Model Hub.
"""

import onnxruntime as ort
import numpy as np
import json
from PIL import Image
from torchvision import transforms
import requests
from huggingface_hub import hf_hub_download
import os

class CattleBreedClassifier:
    def __init__(self, model_name="your-username/cattle-breed-classifier"):
        """
        Initialize the classifier by downloading model files from Hugging Face
        
        Args:
            model_name: HuggingFace model repository name
        """
        self.model_name = model_name
        self.session = None
        self.prototypes = None
        self.metadata = None
        
        # Download and load model files
        self._download_model_files()
        self._load_model()
        self._load_prototypes()
        
    def _download_model_files(self):
        """Download model files from Hugging Face Hub"""
        print("📥 Downloading model files from Hugging Face...")
        
        # Download ONNX model
        self.model_path = hf_hub_download(
            repo_id=self.model_name,
            filename="model.onnx"
        )
        
        # Download prototypes
        self.prototypes_path = hf_hub_download(
            repo_id=self.model_name,
            filename="prototypes.json"
        )
        
        # Download metadata
        self.metadata_path = hf_hub_download(
            repo_id=self.model_name,
            filename="metadata.json"
        )
        
        print("✅ Model files downloaded successfully!")
        
    def _load_model(self):
        """Load the ONNX model"""
        self.session = ort.InferenceSession(self.model_path)
        print("✅ ONNX model loaded")
        
    def _load_prototypes(self):
        """Load breed prototypes"""
        with open(self.prototypes_path, 'r') as f:
            self.prototypes = json.load(f)
        
        with open(self.metadata_path, 'r') as f:
            self.metadata = json.load(f)
            
        print(f"✅ Loaded prototypes for {len(self.prototypes['prototypes'])} breeds")
        
    def preprocess_image(self, image_input):
        """
        Preprocess image for model inference
        
        Args:
            image_input: PIL Image, numpy array, or file path
            
        Returns:
            numpy.ndarray: Preprocessed image tensor
        """
        # Handle different input types
        if isinstance(image_input, str):
            image = Image.open(image_input).convert('RGB')
        elif isinstance(image_input, np.ndarray):
            image = Image.fromarray(image_input).convert('RGB')
        else:
            image = image_input.convert('RGB')
            
        # Apply preprocessing
        transform = transforms.Compose([
            transforms.Resize(256),
            transforms.CenterCrop(224),
            transforms.ToTensor(),
            transforms.Normalize(
                mean=[0.485, 0.456, 0.406],
                std=[0.229, 0.224, 0.225]
            )
        ])
        
        tensor = transform(image).unsqueeze(0)
        return tensor.numpy()
    
    def predict(self, image_input, return_all_scores=False):
        """
        Predict cattle/buffalo breed from image
        
        Args:
            image_input: Image input (PIL Image, numpy array, or file path)
            return_all_scores: Whether to return scores for all breeds
            
        Returns:
            dict: Prediction results
        """
        # Preprocess image
        input_data = self.preprocess_image(image_input)
        
        # Run inference
        features = self.session.run(None, {'input': input_data})[0][0]
        
        # Calculate similarities with all breed prototypes
        similarities = {}
        for breed, prototype in self.prototypes['prototypes'].items():
            similarity = np.dot(features, np.array(prototype))
            similarities[breed] = float(similarity)
        
        # Get top prediction
        predicted_breed = max(similarities, key=similarities.get)
        confidence = similarities[predicted_breed]
        
        # Determine animal type
        buffalo_breeds = ['Bhadawari', 'Jaffarbadi', 'Mehsana', 'Murrah', 'Surti']
        animal_type = 'Buffalo' if predicted_breed in buffalo_breeds else 'Cattle'
        
        result = {
            'predicted_breed': predicted_breed,
            'confidence': confidence,
            'animal_type': animal_type
        }
        
        if return_all_scores:
            result['all_scores'] = similarities
            
        return result

# Example usage
def main():
    # Initialize classifier (will download model from Hugging Face)
    classifier = CattleBreedClassifier("your-username/cattle-breed-classifier")
    
    # Example 1: Predict from local image
    image_path = "path/to/your/image.jpg"
    if os.path.exists(image_path):
        result = classifier.predict(image_path, return_all_scores=True)
        
        print(f"\n🐄 Prediction Results:")
        print(f"Animal Type: {result['animal_type']}")
        print(f"Predicted Breed: {result['predicted_breed']}")
        print(f"Confidence: {result['confidence']:.4f}")
        
        print(f"\n📊 All Breed Scores:")
        for breed, score in sorted(result['all_scores'].items(), 
                                 key=lambda x: x[1], reverse=True):
            print(f"  {breed}: {score:.4f}")
    
    # Example 2: Predict from PIL Image
    from PIL import Image
    image = Image.open(image_path)
    result = classifier.predict(image)
    print(f"\nDirect PIL prediction: {result['predicted_breed']} ({result['confidence']:.4f})")

if __name__ == "__main__":
    main()
