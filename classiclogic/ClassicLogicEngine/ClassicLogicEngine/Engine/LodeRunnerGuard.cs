using System;

namespace LodeRunnerGame
{
    public partial class LodeRunner
    {
      //=============================================================================
      // AI reference book: "玩 Lode Runner 學 C 語言"
      // http://www.kingstone.com.tw/book/book_page.asp?kmcode=2014710650538
      //=============================================================================

      public static int[][] movePolicy = new int[][]{ 
                        new int[]{0, 0, 0, 0, 0, 0}, //* move_map is used to find *//
                        new int[]{0, 1, 1, 0, 1, 1}, //* wheather to move a enm   *//
                        new int[]{1, 1, 1, 1, 1, 1}, //* by indexing into it with *//
                        new int[]{1, 2, 1, 1, 2, 1}, //* enm_byte + num_enm +     *//
                        new int[]{1, 2, 2, 1, 2, 2}, //* set_num to get a byte.   *//
                        new int[]{2, 2, 2, 2, 2, 2}, //* then that byte is checked*//
                        new int[]{2, 2, 3, 2, 2, 3}, //* for !=0 and then decrmnt *//
                        new int[]{2, 3, 3, 2, 3, 3}, //* for next test until = 0  *// 
                        new int[]{3, 3, 3, 3, 3, 3}, 
                        new int[]{3, 3, 4, 3, 3, 4},
                        new int[]{3, 4, 4, 3, 4, 4},
                        new int[]{4, 4, 4, 4, 4, 4}
      };	

      public int moveOffset = 0;
      public int moveId = 0;    //current guard id

      public int numOfMoveItems = movePolicy[0].length;

      //********************************
      //initial guard start move value 
      //********************************
      public void initGuardVariable()
      {
        moveOffset = moveId = 0;
      }

      public void moveGuard()
      {
        int moves;
        int curGuard;
        int x, y;
        double yOffset;
        
        if(guardCount == 0) return; //no guard
        
        if( ++moveOffset >= numOfMoveItems ) moveOffset = 0;
        moves = movePolicy[guardCount][moveOffset];  // get next moves 
      
        while ( moves-- > 0) {                       // slows guard relative to runner
          if(++moveId >= guardCount) moveId = 0; 
          curGuard = guard[moveId];
          
          if(curGuard.action == Action.ACT_IN_HOLE || curGuard.action == Action.ACT_REBORN) {
            continue;
          }
          
          guardMoveStep(moveId, bestMove(moveId));
        }
      }	

      public void guardMoveStep(int id,Action action)
      {
        var curGuard = guard[id];
        var x = curGuard.pos.x;
        var xOffset = curGuard.pos.xOffset;
        var y = curGuard.pos.y;
        var yOffset = curGuard.pos.yOffset;

        Type curToken, nextToken;
        int centerX, centerY;
        Shape curShape, newShape;
        int stayCurrPos;
        
        centerX = centerY = Action.ACT_STOP;
        curShape = newShape = curGuard.shape;

        if(curGuard.action == Action.ACT_CLIMB_OUT && action == Action.ACT_STOP) 
          curGuard.action = Action.ACT_STOP; //for level 16, 30, guard will stock in hole
        
        switch(action) {
        case Action.ACT_UP:
        case Action.ACT_DOWN:
        case Action.ACT_FALL:
          if ( action == Action.ACT_UP ) {	
            stayCurrPos = ( y <= 0 ||
                            (nextToken = map[x][y-1].act) == Type.BLOCK_T ||
                            nextToken == Type.SOLID_T || nextToken == Type.TRAP_T || 
                          nextToken == Type.GUARD_T);
            
            if( yOffset <= 0 && stayCurrPos)
              action = Action.ACT_STOP;
          } else { //ACT_DOWN || ACT_FALL
            stayCurrPos = ( y >= maxTileY ||
                            (nextToken = map[x][y+1].act) == Type.BLOCK_T ||
                            nextToken == Type.SOLID_T || nextToken == Type.GUARD_T);	
            
            if( action == Action.ACT_FALL && yOffset < 0 && map[x][y].base == Type.BLOCK_T) {
              action = Action.ACT_IN_HOLE;
              stayCurrPos = 1;
            } else {
              if ( yOffset >= 0 && stayCurrPos) 
                action = Action.ACT_STOP;
            }
          }
          
          if ( action != Action.ACT_STOP ) {
            if ( xOffset > 0) 
              centerX = Action.ACT_LEFT;
            else if ( xOffset < 0)
              centerX = Action.ACT_RIGHT;
          }
          break;
        case Action.ACT_LEFT:
        case Action.ACT_RIGHT:		
          if ( action == Action.ACT_LEFT ) {
            /* original source code from book
            stayCurrPos = ( x <= 0 ||
                            (nextToken = map[x-1][y].act) == BLOCK_T ||
                            nextToken == SOLID_T || nextToken == TRAP_T || 
                              nextToken == GUARD_T); 
            */					
            // change check TRAP_T from base, 
            // for support level 41==> runner in trap will cause guard move
            stayCurrPos = ( x <= 0 ||
                            (nextToken = map[x-1][y].act) == Type.BLOCK_T ||
                            nextToken == Type.SOLID_T || nextToken == Type.GUARD_T ||
                      map[x-1][y].base == Type.TRAP_T); 
          
            if (xOffset <= 0 && stayCurrPos)
              action = Action.ACT_STOP;
          } else { //ACT_RIGHT
            /* original source code from book
            stayCurrPos = ( x >= maxTileX ||
                            (nextToken = map[x+1][y].act) == BLOCK_T ||
                            nextToken == SOLID_T || nextToken == TRAP_T || 
                              nextToken == GUARD_T); 
            */
            // change check TRAP_T from base, 
            // for support level 41==> runner in trap will cause guard move
            stayCurrPos = ( x >= maxTileX ||
                            (nextToken = map[x+1][y].act) == Type.BLOCK_T ||
                            nextToken == Type.SOLID_T || nextToken == Type.GUARD_T || 
                              map[x+1][y].base == Type.TRAP_T); 
            
            if (xOffset >= 0 && stayCurrPos)
              action = Action.ACT_STOP;
          }
            
          if ( action != Action.ACT_STOP ) {
            if ( yOffset > 0 ) 
              centerY = Action.ACT_UP;
            else if ( yOffset < 0) 
              centerY = Action.ACT_DOWN;
          }
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
            if(map[x][y].act == Type.RUNNER_T) setRunnerDead(); //collision
            //map[x][y].act = GUARD_T;
          }
          
          if( yOffset <= 0 && yOffset > -yMove) {
            dropGold(id); //decrease count
          }
          newShape = Shape.runUpDn;
        }
        
        if ( centerY == Action.ACT_UP ) {
          yOffset -= yMove;
          if( yOffset < 0) yOffset = 0; //move to center Y	
        }
        
        if ( action == Action.ACT_DOWN || action == Action.ACT_FALL || action == Action.ACT_IN_HOLE) {
          var holdOnBar = 0;
          if(curToken == Type.BAR_T) {
            if( yOffset < 0) holdOnBar = 1;
            else if(action == Action.ACT_DOWN && y < maxTileY && map[x][y+1].act != Type.LADDR_T) {
              action = Action.ACT_FALL; //shape fixed: 2014/03/27
            }
          }
          
          yOffset += yMove;
          
          if(holdOnBar == 1 && yOffset >= 0) {
            yOffset = 0; //fall and hold on bar
            action = Action.ACT_FALL_BAR;
          }
          if(stayCurrPos && yOffset > 0 ) yOffset = 0; //stay on current position
          else if(yOffset > H2) { //move to y+1 position
            if(curToken == Type.BLOCK_T || curToken == Type.HLADR_T) curToken = Type.EMPTY_T; //in hole or hide laddr
            map[x][y].act = curToken; //runner move to [x][y+1], so set [x][y].act to previous state
            y++;
            yOffset = yOffset - tileH;
            if(map[x][y].act == Type.RUNNER_T) setRunnerDead(); //collision
            //map[x][y].act = GUARD_T;
          }
          
          //add condition: AI version >= 3 will decrease drop count while guard fall
          if( ((curAiVersion >= 3 && action == Action.ACT_FALL) || action == Action.ACT_DOWN) && 
              yOffset >= 0 && yOffset < yMove) 
          { 	//try drop gold
            dropGold(id); //decrease count
          }
          
          if(action == Action.ACT_IN_HOLE) { //check in hole or still falling
            if (yOffset < 0) {
              action = Action.ACT_FALL; //still falling
              
              //----------------------------------------------------------------------
              //For AI version >= 4, drop gold before guard failing into hole totally
              if(curAiVersion >= 4 && curGuard.hasGold > 0) {
                if(map[x][y-1].base == Type.EMPTY_T) {
                  //drop gold above
                  addGold(x, y-1);
                } else 
                  decGold(); //gold disappear 
                curGuard.hasGold = 0;
                guardRemoveRedhat(curGuard); //9/4/2016
              }
              //----------------------------------------------------------------------
              
            } else { //fall into hole (yOffset MUST = 0)

              //----------------------------------------------------------------------
              //For AI version < 4, drop gold after guard failing into hole totally
              if( curGuard.hasGold > 0 ) {
                if(map[x][y-1].base == Type.EMPTY_T) {
                  //drop gold above
                  addGold(x, y-1);
                } else 
                  decGold(); //gold disappear 
              }
              curGuard.hasGold = 0;
              guardRemoveRedhat(curGuard); //9/4/2016
              //----------------------------------------------------------------------
              
              if( curShape == Shape.fallRight) newShape = Shape.shakeRight;
              else newShape = Shape.shakeLeft;
              themeSoundPlay(Sound.trap);
              shakeTimeStart = recordCount; //for debug
              if(curAiVersion < 3) {
                curGuard.sprite.on("animationend", () =>  climbOut(id));
              } else {
                add2GuardShakeQueue(id, newShape);
              }
              
              if(playMode == PlayMode.PLAY_CLASSIC || playMode == PlayMode.PLAY_AUTO || playMode == PlayMode.PLAY_DEMO) {
                drawScore(SCORE_IN_HOLE);
              } else {
                //for modem mode & edit mode
                //drawGuard(1); //only guard dead need add count
              }
            }
          }
          
          if(action == Action.ACT_DOWN) {
            newShape = Shape.runUpDn;
          } else { //ACT_FALL or ACT_FALL_BAR
            if (action == Action.ACT_FALL_BAR) {
              if(curGuard.lastLeftRight == Action.ACT_LEFT) newShape = Shape.barLeft;
              else newShape = Shape.barRight;
            } else {
              if(action == Action.ACT_FALL && curShape != Shape.fallLeft && curShape != Shape.fallRight) {
                if (curGuard.lastLeftRight == Action.ACT_LEFT) newShape = Shape.fallLeft;
                else newShape = Shape.fallRight;
              }
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
            if(map[x][y].act == Type.RUNNER_T) setRunnerDead(); //collision
            //map[x][y].act = GUARD_T;
          }
          if( xOffset <= 0 && xOffset > -xMove) {
            dropGold(id); //try to drop gold
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
            if(map[x][y].act == Type.RUNNER_T) setRunnerDead(); //collision
            //map[x][y].act = GUARD_T;
          }
          if( xOffset >= 0 && xOffset < xMove) {
            dropGold(id);
          }
          if(curToken == Type.BAR_T) newShape = Shape.barRight;
          else newShape = Shape.runRight;
        }
        
        if ( centerX == Action.ACT_RIGHT ) {
          xOffset += xMove;
          if ( xOffset > 0) xOffset = 0; //move to center X
        }
      
        //if(curGuard == ACT_CLIMB_OUT) action == ACT_CLIMB_OUT;
        
        if(action == Action.ACT_STOP) {
          if(curGuard.action != Action.ACT_STOP){
            curGuard.sprite.stop();
            if(curGuard.action != Action.ACT_CLIMB_OUT) curGuard.action = Action.ACT_STOP;
          }
        } else {
          if(curGuard.action == Action.ACT_CLIMB_OUT) action = Action.ACT_CLIMB_OUT;
          curGuard.sprite.x = (x * tileW + xOffset) * tileScale | 0;
          curGuard.sprite.y = (y * tileH + yOffset) * tileScale | 0;
          curGuard.pos = new Position{ x = x, y = y, xOffset = xOffset, yOffset = yOffset};	
          if(curShape != newShape) {
            curGuard.sprite.gotoAndPlay(newShape);
            curGuard.shape = newShape;
          }
          if(action != curGuard.action){
            curGuard.sprite.play();
          }
          curGuard.action = action;
          if(action == Action.ACT_LEFT || action == Action.ACT_RIGHT) curGuard.lastLeftRight = action;
        }
        map[x][y].act = Type.GUARD_T;

        // Check to get gold and carry 
        if(map[x][y].base == Type.GOLD_T && curGuard.hasGold == 0 &&
          ((!xOffset && yOffset >= 0 && yOffset < H4) || 
          (!yOffset && xOffset >= 0 && xOffset < W4) || 
          (y < maxTileY && map[x][y+1].base == Type.LADDR_T && yOffset < H4) // gold above laddr
          )
          )  
        {
          //curGuard.hasGold = ((Math.random()*26)+14)|0; //14 - 39 
          curGuard.hasGold = ((Math.random()*26)+12)|0; //12 - 37 change gold drop steps
          guardWearRedhat(curGuard); //9/4/2016
          if(playMode == PlayMode.PLAY_AUTO || playMode == PlayMode.PLAY_DEMO || playMode == PlayMode.PLAY_DEMO_ONCE) 	getDemoGold(curGuard);
          if(recordMode) processRecordGold(curGuard);
          removeGold(x, y);
          //debug ("get, (x,y) = " + x + "," + y + ", offset = " + xOffset); 
        }

        //check collision again 
        //checkCollision(runner.pos.x, runner.pos.y);
      }

      //meanings: guard with gold
      public void guardWearRedhat(Guard guard)
      {
        if(redhatMode) guard.sprite.spriteSheet = redhatData;
      }

      //meanings: guard without gold
      public void guardRemoveRedhat(Guard guard) 
      {
        if(redhatMode) guard.sprite.spriteSheet = guardData;
      }

      public void dropGold(int id) 
      {
        var curGuard = guard[id];
        var nextToken;
        var drop = 0;
        
        switch (true) {
        case (curGuard.hasGold > 1):
          curGuard.hasGold--; // count > 1,  don't drop it only decrease count 
          //loadingTxt.text = "(" + id + ") = " + curGuard.hasGold;
          break;	
        case (curGuard.hasGold == 1): //drop gold
          var x = curGuard.pos.x, y = curGuard.pos.y;
          
          if(map[x][y].base == Type.EMPTY_T && ( y >= maxTileY ||
            ((nextToken = map[x][y+1].base) == Type.BLOCK_T || 
            nextToken == Type.SOLID_T || nextToken == Type.LADDR_T) )
          ) {
            addGold(x,y);
            curGuard.hasGold = -1; //for record play action always use value = -1
            //curGuard.hasGold =  -(((Math.random()*10)+1)|0); //-1 ~ -10; //waiting time for get gold
            guardRemoveRedhat(curGuard); //9/4/2016	
            drop = 1;
          }
          break;	
        case (curGuard.hasGold < 0):
          curGuard.hasGold++; //wait, don't get gold till count = 0
          //loadingTxt.text = "(" + id + ") = " + curGuard.hasGold;
          break;	
        }
        return drop;
      }


      //=============================================
      // BEGIN NEW SHAKE METHOD for AI version >= 3
      //=============================================
      public int DEBUG_TIME = 0;
      public int[] shakeRight  = new int[]{  8,  9, 10,  9, 10,  8 };
      public int[] shakeLeft   = new int[]{ 30, 31, 32, 31, 32, 30 };
      public int[] shakeTime   = new int[]{ 36,  3,  3,  3,  3,  3 };

      public List<int> shakingGuardList = new Listm<>(int);

      public void initStillFrameVariable()
      {
        initGuardShakeVariable();
        initFillHoleVariable();
        initRebornVariable();
      }

      public void initGuardShakeVariable()
      {
        shakingGuardList.Clear();
        
        //-------------------------------------------------------------------------
        // Shake time extension when guard in hole,
        // so the runner can dig three hold for three guards and pass through them. 
        // The behavior almost same as original lode runner (APPLE-II version).
        // 2016/06/04
        //-------------------------------------------------------------------------
        if(curAiVersion <= 3) shakeTime = new int[]{ 36,  3,  3,  3,  3,  3 }; //for AI VERSION = 3
        else                  shakeTime = new int[]{ 51,  3,  3,  3,  3,  3 }; //for AI VERSION > 3
      }

      public void add2GuardShakeQueue(int id, Shape shape)
      {
        var curGuard = guard[id];
        
        if(shape == Shape.shakeRight) {
          curGuard.shapeFrame = shakeRight;	
        } else { 
          curGuard.shapeFrame = shakeLeft;	
        }
        
        curGuard.curFrameIdx  =  0;
        curGuard.curFrameTime = -1; //for init
          
        shakingGuardList.Add(id);
        //error(arguments.callee.name, "push id =" + id + "(" + shakingGuardList + ")" );
        
      }

      public void processGuardShake()
      {
        Guard curGuard;
        int curIdx;
        for(var i = 0; i < shakingGuardList.length;) {
          curGuard = guard[shakingGuardList[i]];
          curIdx = curGuard.curFrameIdx;
          
          if( curGuard.curFrameTime < 0) { //start shake => set still frame
            curGuard.curFrameTime = 0;
            curGuard.sprite.gotoAndStop(curGuard.shapeFrame[curIdx]);
          } else {
            if(++curGuard.curFrameTime >= shakeTime[curIdx]) {
              if(++curGuard.curFrameIdx < curGuard.shapeFrame.length) {
                //change frame
                curGuard.curFrameTime = 0;
                curGuard.sprite.gotoAndStop(curGuard.shapeFrame[curGuard.curFrameIdx]);
              } else {
                //shake time out 
              
                var id = shakingGuardList[i];
                shakingGuardList.splice(i, 1); //remove from list
                //error(arguments.callee.name, "remove id =" + id + "(" + shakingGuardList + ")" );
                climbOut(id); //climb out
                continue;
              }
              
            }
          }
          i++;
        }
      }

      public void removeFromShake(id)
      {
        for(var i = 0; i < shakingGuardList.length;i++) {
          if(shakingGuardList[i] == id) {
            shakingGuardList.RemoveAt(i); //remove from list
            //error(arguments.callee.name, "remove id =" + id + "(" + shakingGuardList + ")" );
            return;
          }
        }
        error(arguments.callee.name, "design error id =" + id + "(" + shakingGuardList + ")" );
      }

      //======================
      // END NEW SHAKE METHOD 
      //======================

      public void climbOut(int id)
      {
        var curGuard = guard[id]
        
        curGuard.action = Action.ACT_CLIMB_OUT;
        curGuard.sprite.removeAllEventListeners ("animationend");
        curGuard.sprite.gotoAndPlay(Shape.runUpDn);
        curGuard.shape = Shape.runUpDn;
        curGuard.holePos = new Position{x = curGuard.pos.x, y = curGuard.pos.y };
        
        if(DEBUG_TIME) loadingTxt.text = "ShakeTime = " + (recordCount - shakeTimeStart); //for debug
      }

      public Action bestMove(int id)
      {
        var guarder = guard[id];
        var x = guarder.pos.x;
        var xOffset = guarder.pos.xOffset;
        var y = guarder.pos.y;
        var yOffset = guarder.pos.yOffset;
        
        Type curToken, nextBelow, nextMove;
        var checkSameLevelOnly = 0;
        
        curToken = map[x][y].base;
        
        if(guarder.action == Action.ACT_CLIMB_OUT)  { //clib from hole
          if(guarder.pos.y == guarder.holePos.y) {
            return (Action.ACT_UP);
          }else {
            checkSameLevelOnly = 1;
            if(guarder.pos.x != guarder.holePos.x) { //out of hole
              guarder.action = Action.ACT_LEFT;
            }
          }
        }
        
        if( !checkSameLevelOnly) {
        
          /****** next check to see if enm must fall. if so ***********/
          /****** return e_fall and skip the rest.          ***********/
        
          if ( curToken == Type.LADDR_T || (curToken == Type.BAR_T && yOffset === 0) ) { //ladder & bar
            /* no guard fall */ 
          } else if ( yOffset < 0) //no laddr & yOffset < 0 ==> falling
            return (Action.ACT_FALL);
          else if ( y < maxTileY )
          {
            nextBelow = map[x][y+1].act;
            
            if ( (nextBelow == Type.EMPTY_T || nextBelow == Type.RUNNER_T )) {
              return (Action.ACT_FALL);
            } else if ( nextBelow == Type.BLOCK_T || nextBelow == Type.SOLID_T || 
                  nextBelow == Type.GUARD_T || nextBelow == Type.LADDR_T ) {
              /* no guard fall */
            } else {
              return ( Action.ACT_FALL );		
            }
          }	
        }

        /******* next check to see if palyer on same floor *********/
        /******* and whether enm can get him. Ignore walls *********/
        var runnerX = runner.pos.x;
        var runnerY = runner.pos.y;	
        
      //	if ( y == runnerY ) { // same floor with runner
        if ( y == runnerY && runner.action != Action.ACT_FALL) { //case : guard on laddr and falling => don't catch it 
          while ( x != runnerX ) {
            if ( y < maxTileY )
              nextBelow = map[x][y+1].base;
            else nextBelow = Type.SOLID_T;
            
            curToken = map[x][y].base;
            
            if ( curToken == Type.LADDR_T || curToken == Type.BAR_T ||  // go through	
              nextBelow == Type.SOLID_T || nextBelow == Type.LADDR_T ||
              nextBelow == Type.BLOCK_T || map[x][y+1].act == Type.GUARD_T || //fixed: must check map[].act with guard_t (for support champLevel:43)
                nextBelow == Type.BAR_T || nextBelow == Type.GOLD_T) //add BAR_T & GOLD_T for support level 92 
            {
              if ( x < runnerX)  // guard left to runner
                ++x;	
              else if ( x > runnerX )
                --x;      // guard right to runner
            } else break;     // exit loop with closest x if no path to runner
          }
          
          if ( x == runnerX )  // scan for a path ignoring walls is a success
          {
            if (guarder.pos.x < runnerX ) {  //if left of man go right else left 
              nextMove = Action.ACT_RIGHT;
            } else if ( guarder.pos.x > runnerX ) {
              nextMove = Action.ACT_LEFT;
            } else { // guard X = runner X
              if ( guarder.pos.xOffset < runner.pos.xOffset )
                nextMove = Action.ACT_RIGHT;
              else
                nextMove = Action.ACT_LEFT;
            }
            return (nextMove); 
          }
        } // if ( y == runnerY ) { ... 
        
        /********** If can't reach man on current level, then scan floor *********/
        /********** (ignoring walls) and look up and down for best move  *********/

        return scanFloor(id);
      }

      Action bestPath, bestRating, curRating;		
      int leftEnd, rightEnd;	
      int startX, startY;

      public Action scanFloor(int id)
      {
        int x, y;
        Type curToken, nextBelow;
        Action curPath;
        
        x = startX = guard[id].pos.x;
        y = startY = guard[id].pos.y;
        
        bestRating = 255;   // start with worst rating
        curRating = 255;
        bestPath = Action.ACT_STOP;
        
        /****** get ends for search along floor ******/
        
        while ( x > 0 ) {                                    //get left end first
          curToken = map[x-1][y].act;
          if ( curToken == Type.BLOCK_T || curToken == Type.SOLID_T )
            break;
          if ( curToken == Type.LADDR_T || curToken == Type.BAR_T || y >= maxTileY ||
              y < maxTileY && ( ( nextBelow = map[x-1][y+1].base) == Type.BLOCK_T ||
              nextBelow == Type.SOLID_T || nextBelow == Type.LADDR_T ) )
            --x;
          else {
            --x;                                        // go on left anyway 
            break;
          }		
        }
        
        leftEnd = x;
        x = startX;
        while ( x < maxTileX ) {                           // get right end next
          curToken = map[x+1][y].act;
          if ( curToken  == Type.BLOCK_T || curToken == Type.SOLID_T )
            break;
          
          if ( curToken == Type.LADDR_T || curToken == Type.BAR_T || y >= maxTileY ||
              y < maxTileY && ( ( nextBelow = map[x+1][y+1].base) == Type.BLOCK_T ||
              nextBelow == Type.SOLID_T || nextBelow == Type.LADDR_T ) )
            ++x;
          else {                                         // go on right anyway
            ++x;                                        
            break;
          }		
        }
        
        rightEnd = x;

        /******* Do middle scan first for best rating and direction *******/
        
        x = startX;
        if ( y < maxTileY && 
          (nextBelow = map[x][y+1].base) != Type.BLOCK_T && nextBelow != Type.SOLID_T )
          scanDown( x, Action.ACT_DOWN ); 

        if( map[x][y].base == Type.LADDR_T )
          scanUp( x, Action.ACT_UP );

        /******* next scan both sides of floor for best rating *******/

        curPath = Action.ACT_LEFT;
        x = leftEnd; 
        
        while ( true ) {
          if ( x == startX ) {
            if ( curPath == Action.ACT_LEFT && rightEnd != startX ) {
              curPath = Action.ACT_RIGHT;
              x = rightEnd;
            }
            else break;
          }
        
          if( y < maxTileY && 
            (nextBelow = map [x][y+1].base) != Type.BLOCK_T && nextBelow != Type.SOLID_T )
            scanDown (x, curPath );

          if( map[x][y].base == Type.LADDR_T )
            scanUp ( x, curPath );
          
          if ( curPath == Action.ACT_LEFT )
            x++;
          else x--;	
        }	

        
        return ( bestPath );	
      }                           // end scan floor for best direction to go  
        
      public Action scanDown(int x, Action curPath ) 
      {
        int y;
        Type nextBelow; //curRating;
        var runnerX = runner.pos.x;
        var runnerY = runner.pos.y;
        
        y = startY;
        
        while( y < maxTileY && ( nextBelow = map [x][y+1].base) != Type.BLOCK_T &&
              nextBelow != Type.SOLID_T )                  // while no floor below ==> can move down
        {
          if ( map[x][y].base != Type.EMPTY_T && map[x][y].base != Type.HLADR_T) { // if not falling ==> try move left or right 
            //************************************************************************************
            // 2014/04/14 Add check  "map[x][y].base != HLADR_T" for support 
            // champLevel 19: a guard in hole with h-ladder can run left after dig the left hole
            //************************************************************************************
            if ( x > 0 ) {                          // if not at left edge check left side
              if ( (nextBelow = map[x-1][y+1].base) == Type.BLOCK_T ||
                nextBelow == Type.LADDR_T || nextBelow == Type.SOLID_T ||
                map[x-1][y].base == Type.BAR_T )     // can move left       
              {
                if ( y >= runnerY )             // no need to go on
                  break;                      // already below runner
              }
            }	
            
            if ( x < maxTileX )                     // if not at right edge check right side
            {
              if ( (nextBelow = map[x+1][y+1].base) == Type.BLOCK_T ||
                nextBelow == Type.LADDR_T || nextBelow == Type.SOLID_T ||
                map[x+1][y].base == Type.BAR_T )     // can move right
              {
                if ( y >= runnerY )
                  break;
              }
            }
          }                                           // end if not falling
          ++y;                                        // go to next level down
        }                                               // end while ( y < maxTileY ... ) scan down
        
        if( y == runnerY ) {                            // update best rating and direct.
          curRating = Math.Abs(startX - x); 
      //		if ( (curRating = runnerX - x) < 0) //BUG from original book ? (changed by Simon)
      //			curRating = -curRating; //ABS
        } else if ( y > runnerY )
          curRating = y - runnerY + 200;               // position below runner
        else curRating = runnerY - y + 100;              // position above runner
        
        if( curRating < bestRating )
        {
          bestRating = curRating;
          bestPath = curPath;
        }
        
      }                                                   // end Scan Down

      public Action scanUp(int x, Action curPath )
      {
        int y;
        Type nextBelow; //curRating;
        var runnerX = runner.pos.x;
        var runnerY = runner.pos.y;
        
        y = startY;
        
        while ( y > 0 && map[x][y].base == Type.LADDR_T ) {  // while can go up
          --y;
          if ( x > 0 ) {                              // if not at left edge check left side
            if ( (nextBelow = map[x-1][y+1].base) == Type.BLOCK_T ||
              nextBelow == Type.SOLID_T || nextBelow == Type.LADDR_T ||
              map[x-1][y].base == Type.BAR_T )         // can move left
            {
              if ( y <= runnerY )                 // no need to go on 
                break;                          // already above runner
            }
          }

          if ( x < maxTileX ) {                       // if not at right edge check right side
            if ( (nextBelow = map[x+1][y+1].base) == Type.BLOCK_T ||
              nextBelow == Type.SOLID_T || nextBelow == Type.LADDR_T ||
              map[x+1][y].base == Type.BAR_T )         // can move right
            {
              if ( y <= runnerY )
                break;
            }
          }  
          //--y;
        }                                               // end while ( y > 0 && laddr up) scan up 
        
        if ( y == runnerY ) {                           // update best rating and direct.
          curRating = Math.Abs(startX - x);
          //if ( (curRating = runnerX - x) < 0) // BUG from original book ? (changed by Simon)
          //	curRating = -curRating; //ABS
        } else if ( y > runnerY )
          curRating = y - runnerY + 200;              // position below runner   
        else curRating = runnerY - y + 100;             // position above runner    
        
        if ( curRating < bestRating )
        {
          bestRating = curRating;
          bestPath = curPath;
        }
        
      }                                                   // end scan Up

      RandomRange bornRndX; //range random 0..maxTileX

      public void initRnd()
      {
        //bornRndX = new rangeRandom(0, maxTileX, curLevel); //set fixed seed for demo mode
        bornRndX = new rangeRandom(0, maxTileX, 0); //random range 0 .. maxTileX
      }

      public int getGuardId(int x, int y)
      {
        int id;
        
        for(id = 0; id < guardCount; id++) {
          if ( guard[id].pos.x == x && guard[id].pos.y == y) break;
        }
        assert(id < guardCount, "Error: can not get guard position!");
        
        return id;
      }

      public void guardReborn(int x, int y)
      {
        int id;
        
        //get guard id  by current in hole position
        id = getGuardId(x, y);

        var bornY = 1; //start on line 2
        var bornX = bornRndX.get();
        var rndStart = bornX;
        
        
        while(map[bornX][bornY].act != Type.EMPTY_T || map[bornX][bornY].base == Type.GOLD_T || map[bornX][bornY].base == Type.BLOCK_T ) { 
          //BUG FIXED for level 115 (can not reborn at bornX=27)
          //don't born at gold position & diged position, 2/24/2015
          if( (bornX = bornRndX.get()) == rndStart) {                               
            bornY++;
          }
          assert(bornY <= maxTileY, "Error: Born Y too large !");
        }
        //debug("bornX = " + bornX);
        if(playMode == PlayMode.PLAY_AUTO || playMode == PlayMode.PLAY_DEMO || playMode == PlayMode.PLAY_DEMO_ONCE) {
          var bornPos = getDemoBornPos();
          bornX = bornPos.x;
          bornY = bornPos.y;
        }
        
        if(recordMode == RecordMode.RECORD_KEY) saveRecordBornPos(bornX, bornY);
        else if(recordMode == RecordMode.RECORD_PLAY) {
          var bornPos = getRecordBornPos();
          bornX = bornPos.x;
          bornY = bornPos.y;
        }
        
        map[bornX][bornY].act = Type.GUARD_T;
        //debug("born (x,y) = (" + bornX + "," + bornY + ")");
        
        var curGuard = guard[id];
        
        curGuard.pos = new Position{ x = bornX, y = bornY, xOffset = 0, yOffset = 0 };
        curGuard.sprite.x = bornX * tileWScale | 0;
        curGuard.sprite.y = bornY * tileHScale | 0;
        
        rebornTimeStart = recordCount;
        if(curAiVersion < 3) {
          curGuard.sprite.on("animationend", () =>  rebornComplete(id));
          curGuard.sprite.gotoAndPlay(Shape.reborn);
        } else {
          add2RebornQueue(id);
        }

        curGuard.shape = Shape.reborn;
        curGuard.action = Action.ACT_REBORN;
        
      }

      public void rebornComplete(int id)
      {
        var x = guard[id].pos.x;
        var y = guard[id].pos.y;

        if( map[x][y].act == Type.RUNNER_T) setRunnerDead(); //collision
        map[x][y].act  = Type.GUARD_T; 
        guard[id].sprite.removeAllEventListeners ("animationend");
        guard[id].action = Action.ACT_FALL;
        guard[id].shape = Shape.fallRight;
        //guard[id].hasGold = 0;
        guard[id].sprite.gotoAndPlay(Shape.fallRight);
        themeSoundPlay(Shape.reborn);
        
        if(DEBUG_TIME) loadingTxt.text = "rebornTime = " + (recordCount - rebornTimeStart); //for debug
      }

      public void setRunnerDead()
      {
        if(!godMode) gameState = GameState.GAME_RUNNER_DEAD; 
      }

      //===============================================
      // BEGIN NEW FOR REBORN (ai version >= 3)
      //===============================================
      public int[] rebornFrame = new int[]{ 28, 29 };
      public int[] rebornTime  = new int[]{  6,  2 };

      public List<int> rebornGuardList = new List<int>();

      public void initRebornVariable()
      {
        rebornGuardList.Clear();
      }

      public void add2RebornQueue(int id)
      {
        var curGuard = guard[id];
        
        curGuard.sprite.gotoAndStop(Shape.reborn);
        curGuard.curFrameIdx  =   0;
        curGuard.curFrameTime =  -1;
          
        rebornGuardList.Add(id);
      }

      public void processReborn()
      {
        Guard curGuard; 
        int curIdx;
        
        for(var i = 0; i < rebornGuardList.length;) {
          curGuard = guard[rebornGuardList[i]];
          curIdx = curGuard.curFrameIdx;
          
          if(++curGuard.curFrameTime >= rebornTime[curIdx]) {
            if(++curGuard.curFrameIdx < rebornFrame.length) {
              //change frame
              curGuard.curFrameTime = 0;
              curGuard.sprite.gotoAndStop(rebornFrame[curGuard.curFrameIdx]);
            } else {
              //reborn 
              var id = rebornGuardList[i];
              rebornGuardList.splice(i, 1); //remove from list
              rebornComplete(id);
              continue;
            }
          }
          i++;
        }
      }
      //====================
      // END NEW FOR REBORN 
      //====================

    }
}