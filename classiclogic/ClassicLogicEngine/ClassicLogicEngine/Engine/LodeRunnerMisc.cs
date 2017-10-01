using System;

namespace LoadeRunnerGame
{
    public partial class LoadeRunner
    {
        

        public class Sound
        {
          public const string soundFall = "soundFall";
          public const string down = "down";
        }

        public enum Theme
        {
          C64,
          Apple
        }

        //============
        // MISC
        //============
        public int DEBUG = 0;
        public int demoSoundOff = 1;

        public RecordMode recordMode = RecordMode.RECORD_KEY; 

        public void debug(string @string) 
        {
          if(DEBUG != 0) console.log(@string);
        }

        public void assert(bool expression, string msg)
        {
          console.assert(expression, msg);
        }

        public void error(string funName, string @string) 
        {
          console.log("Error In " + funName + "( ): " + @string);
        }

        //==========================
        // BEGIN for Sound function
        //==========================
        public int soundOff = 0;

        public void themeSoundPlay(string name) 
        {
          soundPlay(name + curTheme);
        }

        public int soundDisable()
        {
          if(playMode == PLAY_AUTO) return 1;
          if(playMode == PLAY_DEMO || playMode == PLAY_DEMO_ONCE) return demoSoundOff;
          else return soundOff;
        }

        public int soundPlay(string name)
        {
          if(soundDisable()) return;
          
          if(name != null) {
            return createjs.Sound.play(name);
          } else {
            name.stop(); //12/21/2014 , for support soundJS 0.6.0
            name.play();
          }
        }

        public void soundStop(string name)
        {
          if(name != null) {
            return createjs.Sound.stop(name);
          } else {
            name.stop();
          }
        }

        public void soundPause(string name)
        {
          if(soundDisable()) return;
          
          if(name != null) {
            return createjs.Sound.pause(name);
          } else {
            name.paused=true; //SoundJS 0.6.2 API Changed, 8/28/2016 
          }
        }

        public void soundResume(string name)
        {
          if(soundDisable()) return;
          
          if(name != null) {
            return createjs.Sound.resume(name);
          } else {
            name.paused=false; //SoundJS 0.6.2 API Changed, 8/28/2016
          }
        }

        //==============================
        // get time zone for php format
        //==============================
        public string phpTimeZone()
        {
          var d = new Date()
          var n = d.getTimezoneOffset();
          var n1 = Math.Abs(n);

          //------------------------------------------------------------------------------------
          // AJAX POST and Plus Sign ( + ) — How to Encode:
          // Use encodeURIComponent() in JS and in PHP you should receive the correct values.
          // http://stackoverflow.com/questions/1373414/ajax-post-and-plus-sign-how-to-encode
          //------------------------------------------------------------------------------------
          
          //+0:00, +1:00, +8:00, -8:00 ....
          return ((n <=0)?encodeURIComponent("+"):"-")+ (n1/60|0) + ":" + ("0"+n1%60).slice(-2);
        }

        //========================================
        // get local time (YYYY-MM-DD HH:MM:SS)
        //========================================
        public string getLocalTime()
        {
          var d = new Date();
          
          return (("000"+d.getFullYear()).slice(-4)+ "-" +("0"+(d.getMonth() + 1)).slice(-2)+ "-" +("0"+d.getDate()).slice(-2)+ " " +	
                ("0"+d.getHours()).slice(-2) + ":" + ("0"+d.getMinutes()).slice(-2) + ":"  + ("0"+d.getSeconds()).slice(-2));
        }

        //===============
        // Random Object
        //===============
        public RandomRange rangeRandom(int minValue, int maxValue, int seedValue)
        {
          return new RandomRange(minValue, maxValue, seedValue);
        }

        //======================================
        // get demo data by playData (wData.js)
        //======================================
        public void getDemoData(int playData) 
        {
          wDemoData = [];	
          switch(playData) {
          case 1:
            if(	typeof wfastDemoData1 !== "undefined" ) wDemoData = wfastDemoData1;
            break;
          case 2:
            if(	typeof wfastDemoData2 !== "undefined" ) wDemoData = wfastDemoData2;
            break;
          case 3:
            if(	typeof wfastDemoData3 !== "undefined" ) wDemoData = wfastDemoData3;
            break;
          case 4:
            if(	typeof wfastDemoData4 !== "undefined" ) wDemoData = wfastDemoData4;
            break;
          case 5:
            if(	typeof wfastDemoData5 !== "undefined" ) wDemoData = wfastDemoData5;
            break;
          }
          for(var i = 0; i < wDemoData.length; i++) { //temp
            playerDemoData[wDemoData[i].level-1] = wDemoData[i]; 
          }
        }

    }   
}