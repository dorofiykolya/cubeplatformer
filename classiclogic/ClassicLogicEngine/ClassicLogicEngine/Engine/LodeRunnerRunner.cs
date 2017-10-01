using System;

namespace LodeRunnerGame
{
    public partial class LodeRunner
    {
      //=============================================================================
      // AI reference book: "玩 Lode Runner 學 C 語言"
      // http://www.kingstone.com.tw/book/book_page.asp?kmcode=2014710650538
      //=============================================================================

      public enum MoveState
      { 
        STATE_NONE = 0,
        STATE_OK_TO_MOVE = 1, 
        STATE_FALLING = 2
      }
      
      public enum Shape
      {
        runUpDn,
        runUpDn,
        barLeft,
        barRight,
        runLeft,
        fallLeft,
        fallRight
      }

      public void moveRunner()
      {
        var x = runner.pos.x;
        var xOffset = runner.pos.xOffset;
        var y = runner.pos.y;
        var yOffset = runner.pos.yOffset;
        var stayCurrPos = 0;
        MoveState curState;
        Type curToken, nextToken;
        
        curToken = map[x][y].base;
        
        if ( curToken == Type.LADDR_T || (curToken == Type.BAR_T && yOffset == 0) ) { //ladder & bar
          curState = MoveState.STATE_OK_TO_MOVE; //ok to move (on ladder or bar)
        } else if (yOffset < 0) {  //no ladder && yOffset < 0 ==> falling 
          curState = MoveState.STATE_FALLING;
        } else if (y < maxTileY) { //no laddr && y < maxTileY && yOffset >= 0

          nextToken = map[x][y+1].act; 
          
          switch (true) {
          case (nextToken == Type.EMPTY_T):
            curState = MoveState.STATE_FALLING;
            break;
          case ( nextToken == Type.BLOCK_T || nextToken == Type.LADDR_T || nextToken == Type.SOLID_T):
            curState = MoveState.STATE_OK_TO_MOVE;
            break;	
          case ( nextToken == Type.GUARD_T):
            curState = MoveState.STATE_OK_TO_MOVE;
            break;
          default:
            curState = MoveState.STATE_FALLING;
            break;	
          }
        } else { // no laddr && y == maxTileY 
          curState = MoveState.STATE_OK_TO_MOVE;
        }
          
        if ( curState == MoveState.STATE_FALLING ) {
          stayCurrPos = ( y >= maxTileY ||
            (nextToken = map[x][y+1].act) == Type.BLOCK_T ||
            nextToken == Type.SOLID_T || nextToken == Type.GUARD_T);
          
          runnerMoveStep(Action.ACT_FALL, stayCurrPos);
          return;
        }
        
        /****** Check Key Action ******/
        
        Action moveStep = Action.ACT_STOP;
        stayCurrPos = 1;
        
        switch(keyAction) {
        case Action.ACT_UP:
          stayCurrPos = ( y <= 0 ||
            ( nextToken = map[x][y-1].act) == Type.BLOCK_T ||
              nextToken == Type.SOLID_T || nextToken == Type.TRAP_T );
            
          if (y > 0 && map[x][y].base != Type.LADDR_T && yOffset < H4 && yOffset > 0 && map[x][y+1].base == Type.LADDR_T)
          {
            stayCurrPos = 1;
            moveStep = Action.ACT_UP;
          } else
          if (!( map[x][y].base != Type.LADDR_T &&
            (yOffset <= 0 || map[x][y+1].base != Type.LADDR_T) ||
            (yOffset <= 0 &&  stayCurrPos ) ) 
          ) {
            moveStep = Action.ACT_UP;
          } 
            
          break;	
        case Action.ACT_DOWN:
          stayCurrPos = ( y >= maxTileY ||
            (nextToken = map[x][y+1].act) == Type.BLOCK_T ||
            nextToken == Type.SOLID_T);	
            
          if (!(yOffset >= 0 && stayCurrPos))
            moveStep = Action.ACT_DOWN;	
          break;
        case Action.ACT_LEFT:
          stayCurrPos = ( x <= 0 ||
            (nextToken = map[x-1][y].act) == Type.BLOCK_T ||
            nextToken == Type.SOLID_T || nextToken == Type.TRAP_T); 
          
          if (!(xOffset <= 0 && stayCurrPos))
            moveStep = Action.ACT_LEFT;
          break;
        case Action.ACT_RIGHT:
          stayCurrPos = ( x >= maxTileX	||
            (nextToken = map[x+1][y].act) == Type.BLOCK_T ||
            nextToken == Type.SOLID_T || nextToken == Type.TRAP_T);
            
          if (!(xOffset >= 0 && stayCurrPos))
            moveStep = Action.ACT_RIGHT;
          break;
        case Action.ACT_DIG_LEFT:
        case Action.ACT_DIG_RIGHT:
          if(ok2Dig(keyAction)) {
            runnerMoveStep(keyAction, stayCurrPos);
            digHole(keyAction);
          } else {
            runnerMoveStep(Action.ACT_STOP, stayCurrPos);	
          }
          keyAction = Action.ACT_STOP;
          return;	
        }		
        runnerMoveStep(moveStep, stayCurrPos);	
      }

      public void runnerMoveStep(Action action, int stayCurrPos)
      {
        var x = runner.pos.x;
        var xOffset = runner.pos.xOffset;
        var y = runner.pos.y;
        var yOffset = runner.pos.yOffset;

        Type curToken;
        Type nextToken;
        Shape curShape, newShape;
        
        curShape = newShape = runner.shape;
        
        Action centerX = Action.ACT_STOP;
        Action centerY = Action.ACT_STOP;
        
        switch(action)	{
        case Action.ACT_DIG_LEFT:		
        case Action.ACT_DIG_RIGHT:	
          xOffset = 0;
          yOffset = 0;	
          break;	
        case Action.ACT_UP:
        case Action.ACT_DOWN:
        case Action.ACT_FALL:	
          if ( xOffset > 0 ) centerX = Action.ACT_LEFT;
          else if (xOffset < 0) centerX = Action.ACT_RIGHT;
          break;
        case Action.ACT_LEFT:
        case Action.ACT_RIGHT:
          if( yOffset > 0 ) centerY = Action.ACT_UP;
          else if (yOffset < 0) centerY = Action.ACT_DOWN;
          break;
        }
        
        curToken = map[x][y].base;
        
        if ( action == Action.ACT_UP ) {
          yOffset -= yMove;
          
          if(stayCurrPos && yOffset < 0) yOffset = 0; //stay on current position
          else if(yOffset < -H2) { //move to y-1 position 
            if ( curToken == Type.BLOCK_T || curToken == Type.HLADR_T ) curToken = Type.EMPTY_T; //in hole or hide laddr
            map[x][y].act = curToken; //runner move to [x][y-1], so set [x][y].act to previous state
            y--;
            yOffset = tileH + yOffset;
            if(map[x][y].act == Type.GUARD_T && guardAlive(x,y)) setRunnerDead(); //collision
          }
          newShape = Shape.runUpDn;
        }
        
        if ( centerY == Action.ACT_UP ) {
          yOffset -= yMove;
          if( yOffset < 0) yOffset = 0; //move to center Y	
        }
        
        if ( action == Action.ACT_DOWN || action == Action.ACT_FALL) {
          var holdOnBar = 0;
          if(curToken == Type.BAR_T) {
            if( yOffset < 0) holdOnBar = 1;
            else {
              //when runner with bar and press down will into falling state 
              // except "laddr" or "guard" at below, 11/25/2016
              if(action == Action.ACT_DOWN && y < maxTileY && 
                map[x][y+1].act != Type.LADDR_T && map[x][y+1].act != Type.GUARD_T) 
              {
                action = Action.ACT_FALL;
              }
            }
          }
          
          yOffset += yMove;
          
          if(holdOnBar == 1 && yOffset >= 0) {
            yOffset = 0; //fall and hold on bar
            action = Action.ACT_FALL_BAR;
          }
          if(stayCurrPos && yOffset > 0) yOffset = 0; //stay on current position
          else if(yOffset > H2) { //move to y+1 position
            if(curToken == Type.BLOCK_T || curToken == Type.HLADR_T) curToken = Type.EMPTY_T; //in hole or hide laddr
            map[x][y].act = curToken; //runner move to [x][y+1], so set [x][y].act to previous state
            y++;
            yOffset = yOffset - tileH;
            if(map[x][y].act == Type.GUARD_T && guardAlive(x,y)) setRunnerDead(); //collision
          }
          
          if(action == Action.ACT_DOWN) { 
            newShape = Shape.runUpDn;
          } else { //ACT_FALL or ACT_FALL_BAR
            
            if (y < maxTileY && map[x][y+1].act == Type.GUARD_T) { //over guard
              //don't collision
              var id = getGuardId(x, y+1);
              if(yOffset > guard[id].pos.yOffset)	yOffset = guard[id].pos.yOffset;
            }

            if (action == Action.ACT_FALL_BAR) {
              if(runner.lastLeftRight == Action.ACT_LEFT) newShape = Shape.barLeft;
              else newShape = Shape.barRight;
            } else {
              if(runner.lastLeftRight == Action.ACT_LEFT) newShape = Shape.fallLeft;
              else newShape = Shape.fallRight;
              
            }
          }
        }
        
        if ( centerY == Action.ACT_DOWN ) {
          yOffset += yMove;
          if ( yOffset > 0 ) yOffset = 0; //move to center Y
        }
        
        if ( action == Action.ACT_LEFT) {
          xOffset -= xMove;

          if(stayCurrPos && xOffset < 0) xOffset = 0; //stay on current position
          else if ( xOffset < -W2) { //move to x-1 position 
            if(curToken == Type.BLOCK_T || curToken == Type.HLADR_T) curToken = Type.EMPTY_T; //in hole or hide laddr
            map[x][y].act = curToken; //runner move to [x-1][y], so set [x][y].act to previous state
            x--;
            xOffset = tileW + xOffset;
            if(map[x][y].act == Type.GUARD_T && guardAlive(x,y)) setRunnerDead(); //collision
          }
          if(curToken == Type.BAR_T) newShape = Shape.barLeft;
          else newShape = Shape.runLeft;
        }
        
        if ( centerX == Action.ACT_LEFT ) {
          xOffset -= xMove;
          if ( xOffset < 0) xOffset = 0; //move to center X
        }
        
        if ( action == Action.ACT_RIGHT ) {
          xOffset += xMove;

          if(stayCurrPos && xOffset > 0) xOffset = 0; //stay on current position
          else if ( xOffset > W2) { //move to x+1 position 
            if(curToken == Type.BLOCK_T || curToken == Type.HLADR_T) curToken = Type.EMPTY_T; //in hole or hide laddr
            map[x][y].act = curToken; //runner move to [x+1][y], so set [x][y].act to previous state
            x++;
            xOffset = xOffset - tileW;
            if(map[x][y].act == Type.GUARD_T && guardAlive(x,y)) setRunnerDead(); //collision
          }
          if(curToken == Type.BAR_T) newShape = Shape.barRight;
          else newShape = Shape.runRight;
        }
        
        if ( centerX == Action.ACT_RIGHT ) {
          xOffset += xMove;
          if ( xOffset > 0) xOffset = 0; //move to center X
        }
        
        if(action == Action.ACT_STOP ) {
          if(runner.action == Action.ACT_FALL) {
            soundStop(Sound.soundFall);
            themeSoundPlay(Sound.down);
          }
          if(runner.action != Action.ACT_STOP){
            runner.sprite.stop();
            runner.action = Action.ACT_STOP;
          }
        } else {
          runner.sprite.x = (x * tileW + xOffset) * tileScale | 0;
          runner.sprite.y = (y * tileH + yOffset) * tileScale | 0;
          runner.pos = new Position { x = x, y = y, xOffset = xOffset, yOffset = yOffset };	
          if(curShape != newShape) {
            runner.sprite.gotoAndPlay(newShape);
            runner.shape = newShape;
          }
          if(action != runner.action){
            if(runner.action == Action.ACT_FALL) {
              soundStop(Sound.soundFall);
              themeSoundPlay(Sound.down);
            } else if ( action == Action.ACT_FALL) {
              soundPlay(Sound.soundFall);
            }
            runner.sprite.play();
          }
          if(action == Action.ACT_LEFT || action == Action.ACT_RIGHT) runner.lastLeftRight = action;
          runner.action = action;
        }
        map[x][y].act = Type.RUNNER_T;
        
        //show trap tile if runner fall into the tile, 9/12/2015
        if(map[x][y].base == Type.TRAP_T) map[x][y].bitmap.setAlpha(0.5); //show trap tile
        
        // Check runner to get gold (MAX MOVE MUST < H4 & W4) 
        if( map[x][y].base == Type.GOLD_T &&
          ((!xOffset && yOffset >= 0 && yOffset < H4) || 
          (!yOffset && xOffset >= 0 && xOffset < W4) || 
          (y < maxTileY && map[x][y+1].base == Type.LADDR_T && yOffset < H4) // gold above laddr
          )
          )  
        {
          removeGold(x,y);
          themeSoundPlay(Sound.getGold);
          decGold();
          //debug("gold = " + goldCount);
          if(playMode == PlayMode.PLAY_CLASSIC || playMode == PlayMode.PLAY_AUTO || playMode == PlayMode.PLAY_DEMO) {
            drawScore(SCORE_GET_GOLD);
          } else {	
            //for modern mode , edit mode
            drawGold(1); //get gold 
          }
        }
        //if(!goldCount && !goldComplete) showHideLaddr();
        
        //check collision with guard !
        checkCollision(x, y);
      }

      //dec gold count
      function decGold()
      {
        if(--goldCount <= 0) {
          showHideLaddr();
          if(runner.pos.y > 0) {
            if(curTheme == Theme.C64)  soundPlay(Sound.goldFinish + ((curLevel-1)%6+1)); //six sounds
            else soundPlay(Sound.goldFinish); //for all apple2 mode, 9/12/2015
          }
        }
      }

      public void removeGold(int x, inty)
      {
        map[x][y].base = Type.EMPTY_T;
        mainStage.removeChild(map[x][y].bitmap);
        map[x][y].bitmap = null;
      }

      public void addGold(int x, int y)
      {
        Bitmap tile;
        
        map[x][y].base = Type.GOLD_T;
        tile = map[x][y].bitmap = getThemeBitmap("gold");
        tile.setTransform(x * tileWScale, y * tileHScale,tileScale, tileScale); //x,y, scaleX, scaleY 
        mainStage.addChild(tile); 
        
        moveSprite2Top(); //reset runner, guard & fill hole object order
      }

      public void showHideLaddr()
      {
        var haveHLadder = 0;
        for(var y = 0; y < NO_OF_TILES_Y; y++) {
          for(var x = 0; x < NO_OF_TILES_X; x++) {
            if( map[x][y].base == HLADR_T) {
              haveHLadder = 1;
              map[x][y].base = Type.LADDR_T;
              map[x][y].act  = Type.LADDR_T;
              map[x][y].bitmap.setAlpha(1); //display laddr
            }
          }
        }
        goldComplete = 1;
        return haveHLadder;
      }

      public void checkCollision(int runnerX, int runnerY)
      {
        var x = -1, y = -1;
        //var dbg = "NO";

        switch(true) {
        case ( runnerY > 0 && map[runnerX][runnerY-1].act == Type.GUARD_T):
          x = runnerX; y = runnerY-1;	
          //dbg = "UP";	
          break;	
        case ( runnerY < maxTileY && map[runnerX][runnerY+1].act == Type.GUARD_T):
          x = runnerX; y = runnerY+1;	
          //dbg = "DN";	
          break;	
        case ( runnerX > 0 && map[runnerX-1][runnerY].act == Type.GUARD_T):
          x = runnerX-1; y = runnerY;	
          //dbg = "LF";	
          break;
        case ( runnerX < maxTileX && map[runnerX+1][runnerY].act== Type.GUARD_T):
          x = runnerX+1; y = runnerY;	
          //dbg = "RT";	
          break;	
        }
        //if( dbg != "NO") debug(dbg);
        if( x >= 0) {
          for(var i = 0; i < guardCount; i++) {
            if( guard[i].pos.x == x && guard[i].pos.y == y) break;
          }
          assert( (i < guardCount), "checkCollision design error !");
          if(guard[i].action != Action.ACT_REBORN) { //only guard alive need check collection
            //var dw = Math.abs(runner.sprite.x - guard[i].sprite.x);
            //var dh = Math.abs(runner.sprite.y - guard[i].sprite.y);
            
            //change detect method ==> don't depend on scale 
            var runnerPosX = runner.pos.x*tileW+runner.pos.xOffset;
            var runnerPosY = runner.pos.y*tileH+runner.pos.yOffset;
            var guardPosX = guard[i].pos.x*tileW+guard[i].pos.xOffset;
            var guardPosY = guard[i].pos.y*tileH+guard[i].pos.yOffset;

            var dw = Math.Abs(runnerPosX - guardPosX);
            var dh = Math.Abs(runnerPosY - guardPosY);
            
            if( dw <= W4*3 && dh <= H4*3 ) {
              setRunnerDead(); //07/04/2014
              //debug("runner dead!");
            }
          }
        }
      }

      //Page 276 misc.c (book)
      public bool ok2Dig(Action nextMove)
      {
        var x = runner.pos.x;
        var y = runner.pos.y;
        var token, rc = 0;
        
        switch(nextMove) {
        case Action.ACT_DIG_LEFT:
      //		debug("[x-1][y+1] = " + map[x-1][y+1].act + " [x-1][y] = " + map[x-1][y].act + 
      //			  "[x-1][y].base = " + map[x-1][y].base );
        
          if( y < maxTileY && x > 0 && map[x-1][y+1].act == Type.BLOCK_T &&
              map[x-1][y].act == Type.EMPTY_T && map[x-1][y].base != Type.GOLD_T)
            rc = 1;
          break;
        case Action.ACT_DIG_RIGHT:
      //		debug("[x+1][y+1] = " + map[x+1][y+1].act + " [x+1][y] = " + map[x+1][y].act + 
      //			  "[x+1][y].base = " + map[x+1][y].base );
        
          if( y < maxTileY && x < maxTileX && map[x+1][y+1].act == Type.BLOCK_T && 
              map[x+1][y].act == Type.EMPTY_T && map[x+1][y].base != Type.GOLD_T)
            rc = 1;
          break;		
        }
        
        return rc;
      }

      //=======================
      // BEGIN NEW DIG METHOD
      //=======================
      public int[] digHoleLeft =	{ 0, 1,  2,  2,  3,  4,  4,  5,  6,  6,  7 };
      public int[] digHoleRight =	{ 8, 9, 10, 10, 11, 12, 12, 13, 14, 14, 15 };

      public void processDigHole()
      {
        if(curAiVersion < 3) return;
        
        if(++holeObj.curFrameIdx < holeObj.shapeFrame.length) {
          // change frame
          holeObj.sprite.gotoAndStop(holeObj.shapeFrame[holeObj.curFrameIdx]);
          holeObj.sprite.currentAnimationFrame = holeObj.curFrameIdx;
        } else { //dig complete
          digComplete();
        }
      }
      //========================
      // END NEW DIG METHOD 
      //========================

      int digTimeStart, shakeTimeStart;       //for debug       
      int fillHoleTimeStart, rebornTimeStart; //for debug
      public void digHole(Action action)
      {
        var x,y, holeShape;
        
        if(action == Action.ACT_DIG_LEFT) {
          x = runner.pos.x-1;
          y = runner.pos.y;
          
          runner.shape = Shape.digLeft;
          holeShape = Shape.digHoleLeft;

        } else { //DIG RIGHT
          
          x = runner.pos.x+1;
          y = runner.pos.y;
          
          runner.shape = Shape.digRight;
          holeShape = Shape.digHoleRight;
        }
        
        soundPlay(Sound.soundDig);
        map[x][y+1].bitmap.setAlpha(0); //hide block (replace with digging image)
        runner.sprite.gotoAndPlay(runner.shape);
          
        holeObj.action = Action.ACT_DIGGING;
        holeObj.pos = new Position{ x = x, y = y };
        holeObj.sprite.setTransform(x * tileWScale, y * tileHScale,tileScale, tileScale);
        
        digTimeStart = recordCount; //for debug
        
        if(curAiVersion < 3) {
          holeObj.sprite.gotoAndPlay(holeShape);
          holeObj.sprite.on("animationend", digComplete);
        } else {
          if(action == Action.ACT_DIG_LEFT) holeShape = digHoleLeft;
          else holeShape = digHoleRight; 
            
          holeObj.sprite.gotoAndStop(holeShape[0]);
          holeObj.shapeFrame = holeShape;
          holeObj.curFrameIdx = 0;
        }

        mainStage.addChild(holeObj.sprite);
      }

      var DEBUG_DIG=0;
      public int isDigging()
      {
        var rc = 0;
        
        if(holeObj.action == Action.ACT_DIGGING) {
          var x = holeObj.pos.x, y = holeObj.pos.y;
          if(map[x][y].act == Type.GUARD_T) { //guard come close to the digging hole !
            var id = getGuardId(x, y);
            if(holeObj.sprite.currentAnimationFrame < holeObj.digLimit && guard[id].pos.yOffset > -H4) {
              if(DEBUG_DIG) loadingTxt.text = "dig : " + holeObj.sprite.currentAnimationFrame + " (X)";

              stopDigging(x,y);
            } else {
              if(DEBUG_DIG) loadingTxt.text = "dig : " + holeObj.sprite.currentAnimationFrame + " (O)";
              if(curAiVersion >= 3) { //This is a bug while AI VERSION < 3
                map[x][y+1].act = Type.EMPTY_T; //assume hole complete
                rc = 1;
              }
            }
          } else {
            switch( runner.shape ) {
            case Shape.digLeft:
              if(holeObj.sprite.currentAnimationFrame > 2 ) {
                runner.sprite.gotoAndStop(Shape.runLeft); //change shape
                runner.shape = Shape.runLeft;
                runner.action = Action.ACT_STOP;
              }
              break;
            case Shape.digRight:
              if(holeObj.sprite.currentAnimationFrame > 2) {
                runner.sprite.gotoAndStop(Shape.runRight); //change shape
                runner.shape = Shape.runRight;
                runner.action = Action.ACT_STOP;
              }
              break;
            }
            rc = 1;
          }
        }
        return rc;
      }

      public void stopDigging(int x, int y)
      {
        //(1) remove holeObj
        holeObj.sprite.removeAllEventListeners ("animationend");
        holeObj.action = Action.ACT_STOP; //no digging
        mainStage.removeChild(holeObj.sprite); 

        //(2) fill hole
        y++;
        map[x][y].act = map[x][y].base; //BLOCK_T
        assert(map[x][y].base == Type.BLOCK_T, "fill hole != BLOCK_T");
        map[x][y].bitmap.setAlpha(1); //display block
        
        //(3) change runner shape
        switch( runner.shape ) {
        case Shape.digLeft:
          runner.sprite.gotoAndStop(Shape.runLeft);
          runner.shape = Shape.runLeft;
          runner.action = Action.ACT_STOP;
          break;
        case Shape.digRight:
          runner.sprite.gotoAndStop(Shape.runRight);
          runner.shape = Shape.runRight;
          runner.action = Action.ACT_STOP;
          break;
        }
        
        soundStop(Sound.soundDig); //stop sound of digging
      }

      public void digComplete()
      {
        var x = holeObj.pos.x;
        var y = holeObj.pos.y + 1;
        
        map[x][y].act = Type.EMPTY_T;
        holeObj.sprite.removeAllEventListeners ("animationend");
        holeObj.action = Action.ACT_STOP; //no digging
        mainStage.removeChild(holeObj.sprite); 
        
        if(DEBUG_TIME) loadingTxt.text = "DigTime = " + (recordCount - digTimeStart);
        
        fillHole(x, y);
      }

      public List<Sprite> fillHoleObj = new List<Sprite>();
      public void fillHole(int x, int y)
      {
        var fillSprite = new createjs.Sprite(holeData, "fillHole");
        
        fillSprite.pos = new Position{ x = x, y = y }; //save position 11/18/2014
        fillSprite.setTransform(x * tileWScale, y * tileHScale, tileScale, tileScale);
        
        if(curAiVersion < 3) {
          fillSprite.on("animationend", () => fillComplete(fillSprite));
          fillSprite.play();
        } else {
          fillSprite.curFrameIdx  =   0;
          fillSprite.curFrameTime =  -1;
          fillSprite.gotoAndStop(fillHoleFrame[0]);
        }
        mainStage.addChild(fillSprite); 
        fillHoleObj.Add(fillSprite);
        
        fillHoleTimeStart = recordCount; //for debug
      }

      public void moveFillHoleObj2Top()
      {
        for(var i = 0; i < fillHoleObj.length; i++) {
          moveChild2Top(mainStage, fillHoleObj[i]);
        }
      }

      public void fillComplete(Sprite data)
      {
        //don't use "divide command", it will cause loss of accuracy while scale changed (ex: tileScale = 0.6...)
        //var x = this.x / tileWScale | 0; //this : scope default to the dispatcher
        //var y = this.y / tileHScale | 0;
        
        var fillObj = data.obj;
        var x = fillObj.pos.x, y = fillObj.pos.y; //get position 

        map[x][y].bitmap.setAlpha(1); //display block
        fillObj.removeAllEventListeners ("animationend");
        mainStage.removeChild(fillObj);
        removeFillHoleObj(fillObj);
        
        switch(map[x][y].act) {
        case Type.RUNNER_T : // runner dead
          //loadingTxt.text = "RUNNER DEAD"; 
          gameState = GameState.GAME_RUNNER_DEAD;
          runner.sprite.setAalpha(0); //hidden runner --> dead
          break;
        case Type.GUARD_T: //guard dead
          var id = getGuardId(x,y);
          if(curAiVersion >= 3 && guard[id].action == Action.ACT_IN_HOLE) removeFromShake(id);	
          if(guard[id].hasGold > 0) { //guard has gold and not fall into the hole
            decGold(); 
            guard[id].hasGold = 0;
            guardRemoveRedhat(guard[id]); //9/4/2016	
          }
          guardReborn(x,y);
          if(playMode == PlayMode.PLAY_CLASSIC || playMode == PlayMode.PLAY_AUTO || playMode == PlayMode.PLAY_DEMO) {	
            drawScore(SCORE_GUARD_DEAD);
          } else {
            //for modern mode & edit mode
            drawGuard(1); //guard dead, add count
          }
          break;
        }
        map[x][y].act = Type.BLOCK_T;
        
        if(DEBUG_TIME) loadingTxt.text = "FillHoleTime = " + (recordCount - fillHoleTimeStart); //for debug
      }

      public void removeFillHoleObj(Sprite spriteObj)
      {
        for(var i = 0; i < fillHoleObj.length; i++) {
          if(fillHoleObj[i] == spriteObj) {
            fillHoleObj.splice(i,1);
            return;
          }
        }
        error(arguments.callee.name, "design error !");
      }

      //=======================================
      // BEGIN NEW fill Hold (Ai version >= 3)
      //=======================================

      public int[] fillHoleFrame = {  16, 17, 18, 19 };
      public int[] fillHoleTime  = { 166,  8,  8,  4 };

      public void initFillHoleVariable()
      {
        fillHoleObj.Clear();
      }

      public void processFillHole()
      {
        int curIdx; 
        Sprite curFillObj;
        
        for(var i = 0; i < fillHoleObj.length;) {
          curFillObj = fillHoleObj[i];
          curIdx = curFillObj.curFrameIdx;
          
          if(++curFillObj.curFrameTime >= fillHoleTime[curIdx]) {
            if(++curFillObj.curFrameIdx < fillHoleFrame.length) {
              //change frame
              curFillObj.curFrameTime = 0;
              curFillObj.gotoAndStop(fillHoleFrame[curFillObj.curFrameIdx]);
            } else {
              //fill hole complete 
              fillComplete(curFillObj);
              continue;
            }
              
          }
          i++;
        }
      }
      //====================
      // END NEW fill Hold 
      //====================

      //==================================
      // Check guard is alive or not 
      // 2014/10/31
      //==================================
      function guardAlive(int x, int y)
      {
        for(var i = 0; i < guardCount; i++) {
          if( guard[i].pos.x == x && guard[i].pos.y == y) break;
        }
        assert( (i < guardCount), "guardAlive() design error !");
        
        if(guard[i].action != Action.ACT_REBORN) return 1; //alive
        
        return 0; //reborn
      }
    }
}