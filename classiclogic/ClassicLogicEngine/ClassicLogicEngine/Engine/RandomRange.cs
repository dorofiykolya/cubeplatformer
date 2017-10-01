namespace LodeRunnerGame
{
    public class RandomRange
    {
      private int[] rndList;
      private int idx, items;
      private int min, max;
      private int reset;
      private int seed = 0;

      public RandomRange(int minValue, int maxValue, int seedValue)
      {
          //---------
          // initial 
          //---------
          reset = 0;
          min = minValue;
          max = maxValue;
          if(min > max) { //swap
            var tmp;
            tmp = min;
            min = max;
            max = tmp;
          }
          items = max - min + 1;
          
          rndStart();
      }

      public int rndReset()
      {
        return reset;
      }

      private int seedRandom()
      {
          var x = Math.Sin(seed++) * 10000;
          return x - Math.Floor(x);
      }

      private void rndStart()
      {
        int swapId, tmp;
      
        rndList = new int[items];
        for(var i = 0; i < items; i++) rndList[i] = min + i;
        for(var i = 0; i < items; i++) {
          if(seedValue > 0) {
            seed = seedValue;	
            swapId = (seedRandom() * items) | 0;
          } else {
            swapId = (Math.random() * items) | 0;
          }
          tmp = rndList[i];
          rndList[i] = rndList[swapId];
          rndList[swapId] = tmp;
        }
        idx = 0;
        //debug(rndList);
      }

      public int get()
      {
        if( idx >= items) {
          rndStart();
          reset = 1;
        } else {
          reset = 0;
        }
        return rndList[idx++];
      }
    }
}