# -*- coding: utf-8 -*-

# import tensorflow as tf
import tensorflow.compat.v1 as tf
import numpy as np
import cv2
import find_finger as ff
from PIL import Image
import base64
import numpy as np
import json


class NailDetection:
    min_confidence = 0.6
    def __init__(self):
        arg_model = "./nailtracking/model/export_model_008/frozen_inference_graph.pb"

        self.model = tf.Graph()
        with self.model.as_default():
            graphDef = tf.GraphDef()

            with tf.gfile.GFile(arg_model, "rb") as f:
                serializedGraph = f.read()
                graphDef.ParseFromString(serializedGraph)
                tf.import_graph_def(graphDef, name="")

    def nail_detect(self, img):
        with self.model.as_default():
            with tf.Session(graph=self.model) as sess:
                imageTensor = self.model.get_tensor_by_name("image_tensor:0")
                boxesTensor = self.model.get_tensor_by_name("detection_boxes:0")

                scoresTensor = self.model.get_tensor_by_name("detection_scores:0")
                classesTensor = self.model.get_tensor_by_name("detection_classes:0")
                numDetections = self.model.get_tensor_by_name("num_detections:0")

                image = img
                (H, W) = image.shape[:2]
                output = image.copy()
                img_ff, bin_mask, res = ff.find_hand_old(image.copy())
                image = cv2.cvtColor(res, cv2.COLOR_BGR2RGB)
                image = np.expand_dims(image, axis=0)

                (boxes, scores, labels, N) = sess.run(
                    [boxesTensor, scoresTensor, classesTensor, numDetections],
                    feed_dict={imageTensor: image})
                boxes, scores, labels  = np.squeeze(boxes), np.squeeze(scores), np.squeeze(labels)
                boxnum = 0
                result = []

                for (box, score, label) in zip(boxes, scores, labels):
                    if score < self.min_confidence:
                        continue
                    boxnum += 1
                    (startY, startX, endY, endX) = box
                    startX, endX, startY, endY = int(startX * W), int(endX * W), int(startY * H), int(endY * H)
                    result.append({"startX": startX, "endX": endX, "startY": startY, "endY": endY})
        return {"boxnum": boxnum, "result": result}

if __name__ == '__main__':
    model = NailDetection()
    # str = input()
    # print("HelloWorld")

    while True:
        str = input()
        
        png_original = base64.b64decode(str)
        png = np.frombuffer(png_original, dtype=np.uint8)
        img = cv2.imdecode(png, flags=1)

        result = model.nail_detect(img)
        s = json.dumps(result, ensure_ascii=False)
        print(s)
        # print("HelloWorld")

        # with open('/Users/yoshidaairi/Documents/OpenHack/nailtracking/result.txt', mode='a') as f:
        #     f.write(s + "\n")
