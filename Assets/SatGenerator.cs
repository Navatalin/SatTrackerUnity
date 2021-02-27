using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SGPdotNET.Observation;
using SGPdotNET.TLE;
using System.Threading;
using System.Threading.Tasks;

public class satObjData {
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;
    public Tle tle;
    public string satName;
    public Matrix4x4 matrix{
        get{
            return Matrix4x4.TRS(pos, rot, scale);
        }
    }

    public satObjData(string satName,Tle tle, Vector3 scale, Quaternion rot){
        this.satName = satName;
        this.tle = tle;
        this.scale = scale;
        this.rot = rot;
        updatePos();
    }

    public void updatePos(){
        var sat = new Satellite(tle);
        var eci = sat.Predict();
        pos = new Vector3((float)eci.Position.X/100, (float)eci.Position.Z/100, (float)eci.Position.Y/100);
    }

}
public class SatGenerator : MonoBehaviour
{
    public TextAsset tleFile;
    public Mesh objectMesh;
    public Material mat;
    private List<List<satObjData>> batches;
    private List<satObjData> sats;
    private List<Tle> tleData;
    private Thread branchUpdateThread;

    // Start is called before the first frame update
    void Start()
    {      
        Debug.Log("Loading Sats");
        tleData = GenerateTles();
        GetSats();
        Debug.Log("Starting Batches");
        GenerateBatches();
        Debug.Log("Batches finished");
        Debug.Log($"Batch size: {batches.Count}");

        branchUpdateThread = new Thread (UpdateBatches);
        branchUpdateThread.Start();
    }
    public void GenerateBatches(){
        int batchIndexNum = 0;
        List<satObjData> currBatch = new List<satObjData>();
        batches = new List<List<satObjData>>();
        foreach(var sat in sats)
        {
            currBatch.Add(sat);
            batchIndexNum++;
            if(batchIndexNum >= 1000)
            {
                batches.Add(currBatch);
                currBatch = new List<satObjData>();
                batchIndexNum = 0;
            }
        }
        batches.Add(currBatch);
    }
    public void GetSats(){
        var satObjDataList = new List<satObjData>();

        foreach(var tle in tleData){
            try{
                var satData = new satObjData(tle.Name, tle, new Vector3(0.5f, 0.5f,0.5f), Quaternion.identity);
                satObjDataList.Add(satData);
            }
            catch(Exception ex){
                print($"Problem generating satellite position for {tle.Name} error: {ex.Message}");
            }
        }

        sats = satObjDataList;
    }
    private List<Tle> GenerateTles(){
        List<Tle> tles = new List<Tle>();
        var tleData = tleFile.text;
        var tleSplit = tleData.Split('\n');
        for(int i = 0; i < tleSplit.Length; i = i + 3){
            try{
                var name = tleSplit[i].Remove('\r').Trim();
                var lineOne = tleSplit[i+1];
                var lineTwo = tleSplit[i+2];
                var tle = new Tle(name, lineOne.Substring(0,lineOne.Length-1) , lineTwo.Substring(0, lineTwo.Length-1));
                tles.Add(tle);
            }
            catch(Exception ex){
                print($"unable to process line: {ex.Message}");
            }
        }
        return tles;
    }

    private void UpdateBatches()
    {
        
        foreach(var batch in batches)
        {
            foreach(var satobj in batch)
            {
                satobj.updatePos();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!branchUpdateThread.IsAlive){
            branchUpdateThread = new Thread ( UpdateBatches );
            branchUpdateThread.Start();
        }
        foreach(var batch in batches)
        {
            Graphics.DrawMeshInstanced(objectMesh,0, mat,batch.Select((a) => a.matrix).ToList());
        }
    }
    private void FixedUpdate() {
    }

}
