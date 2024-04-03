using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UILineRenderer : Graphic
{
    int vertexCounter = 0;
    UIVertex vertex;
    VertexHelper vh;
    List<float> data;
    float height, width;
    public string entityType;
    public string dataType;
    float padding = 10f;
    protected override void OnPopulateMesh(VertexHelper v) {
        vh = v;
        vertexCounter = 0;
        vh.Clear(); // clears vertex cache
        width = rectTransform.rect.width-padding*2;
        height = rectTransform.rect.height-padding*2;
        renderData(StaticScript.getData(entityType, dataType));
        
    }

    public float largest = -1f, smallest = -1f;
    void renderData(List<float> sentData) {
        // if(entityType == "Fox") {
        //     yield return new WaitForSeconds(0.2f);
        // }
        data = sentData;
        vertex = new UIVertex();
        // vertex.color = material.color;
        vertex.color = new Color(0f, 1f, 0f, 0.8f);
        try {
            if(largest == -1f || smallest == -1f) {  
                for(int index = 0; index < data.Count; index++) {
                    if(data[index] > largest) {
                        largest = data[index];
                    } else if (data[index] < smallest) {
                        smallest = data[index];
                    }
                }
            }
            
            // Debug.Log("Largest: "+largest+" Smallest: "+smallest);
            float range = (largest-smallest);
            if(largest == smallest) {
                range = 1f;
            }

            int numLines = data.Count - 1;
            float xPos = -width/2;
            float incrementAmount = width/numLines;
            

            float nextPoint = 0f, currentPoint = 0f;
            for(int index = 0; index < numLines; index++) {
                if(float.IsNaN(data[index])) {
                    continue;
                    // currentPoint = smallest;
                    // Debug.Log("NaN");
                } else {
                    currentPoint = height*((data[index]-smallest)/range)-height/2;
                }

                if(float.IsNaN(data[index+1])) {
                    continue;
                    // nextPoint = smallest;
                    // Debug.Log("NaN");
                } else {
                    nextPoint = height*((data[index+1]-smallest)/range)-height/2;
                }
                plotPoint(vh, xPos, xPos + incrementAmount, currentPoint, nextPoint);
                xPos += incrementAmount;
            }
            // SetVerticesDirty();
            // UpdateGeometry();
            // SetAllDirty();
        } catch {
            // Debug.Log(data);
            // Debug.Log("Failed To Render");
        }

        // string outs = entityType+ " Lowest: "+smallest+" Highest: "+largest;
        // foreach(float dat in data) {
        //     outs += " "+dat;
            
        // }
        // Debug.Log(outs);
    }

    public float lineWidth = 10f;
    void plotPoint(VertexHelper vh, float x1, float x2, float y1, float y2) {
        try {
            float halfLineWidth = lineWidth/2f;
            vertex.position = new Vector3(x1, y1-halfLineWidth);
            vh.AddVert(vertex);

            vertex.position = new Vector3(x1, y1+halfLineWidth);
            vh.AddVert(vertex);

            vertex.position = new Vector3(x2, y2-halfLineWidth);
            vh.AddVert(vertex);

            vertex.position = new Vector3(x2, y2+halfLineWidth);
            vh.AddVert(vertex);

            vh.AddTriangle(vertexCounter, vertexCounter+1, vertexCounter+2);
            vh.AddTriangle(vertexCounter+1, vertexCounter+2, vertexCounter+3);
            vertexCounter += 4;
        } catch {}
    }
}
