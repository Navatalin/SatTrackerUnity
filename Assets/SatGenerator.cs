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

public class satObjData : MonoBehaviour {
    [SerializeField] public ToolTip toolTip;
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;
    public Tle tle;
    public string satName;

    private double Altitude;
    private double Latitude;
    private double Longitude;
    private Vector3 Velocity;

    private void Update() {
        updatePos();
    }

    private void FixedUpdate() {
        
    }
    public satObjData(string satName,Tle tle, Vector3 scale, Quaternion rot){
        this.satName = satName;
        this.tle = tle;
        this.scale = scale;
        this.rot = rot;
        updatePos();
    }

    public void updatePos(){
        if(tle != null){
            var sat = new Satellite(tle);
            var eci = sat.Predict();
            this.Altitude = eci.ToGeodetic().Altitude;
            this.Latitude = eci.ToGeodetic().Latitude.Degrees;
            this.Longitude = eci.ToGeodetic().Longitude.Degrees;
            this.Velocity = new Vector3((float)eci.Velocity.X,(float)eci.Velocity.Y,(float)eci.Velocity.Z);
            pos = new Vector3((float)eci.Position.X/100, (float)eci.Position.Z/100, (float)eci.Position.Y/100);
            this.transform.position = pos;
        }
    }

    public string GetPositionData(){
        return $"Latitude: {this.Latitude.ToString("#.##")}°\nLongitude: {this.Longitude.ToString("#.##")}°\nX: {this.pos.x.ToString("#.##")} Y: {this.pos.y.ToString("#.##")} Z: {this.pos.z.ToString("#.##")}\n";
    }
    public string GetAltitudeData(){
        return $"Altitude: {this.Altitude.ToString("#.##")} km\n";
    }
    public string GetVelocityData(){
        return $"Velocity: {Math.Abs(this.Velocity.magnitude).ToString("#.##")} km\\s\n";
    }
    private void OnMouseEnter() {
        toolTip.DisplayInfo(this);
    }
    private void OnMouseExit() {
        toolTip.HideInfo();    
    }
}
public class SatGenerator : MonoBehaviour
{
    [SerializeField] ToolTip toolTip;
    public TextAsset tleFile;
    private List<GameObject> sats;
    private List<Tle> tleData;
    private Thread branchUpdateThread;
    public GameObject Prefab;

    // Start is called before the first frame update
    void Start()
    {      
        Debug.Log("Loading Sats");
        tleData = GenerateTles();
        GetSats();

    }
   
    public void GetSats(){
        foreach(var tle in tleData){
            try{
                var satGameObject = Instantiate(Prefab,new Vector3(0,0,0),Quaternion.identity);
                var satGameObjectData = satGameObject.AddComponent<satObjData>();
                satGameObjectData.name = tle.Name;
                satGameObjectData.tle = tle;
                satGameObjectData.pos =  new Vector3(0.5f, 0.5f,0.5f);
                satGameObjectData.rot = Quaternion.identity;
                satGameObjectData.toolTip = toolTip;
                sats.Add(satGameObject);
            }
            catch(Exception ex){
                //print($"Problem generating satellite position for {tle.Name} error: {ex.Message}");
            }
        }
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

    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate() {
    }

}
