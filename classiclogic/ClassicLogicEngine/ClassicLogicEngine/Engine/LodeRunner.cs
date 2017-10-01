using System;

namespace LoderRunnerGame
{
    public class LodeRunner
    {
        public int curAiVersion = AI_VERSION;
        public int maxGuard = MAX_NEW_GUARD; 

        public void setSpeedByAiVersion()
        {
            var idx = (curAiVersion > spriteSpeed.length)?(spriteSpeed.length-1):(curAiVersion-1); //array index don't overflow
            var speedObj = spriteSpeed[idx];
            
            RUNNER_SPEED = speedObj.runnerSpeed;
            GUARD_SPEED = speedObj.guardSpeed;
            DIG_SPEED = speedObj.digSpeed;
            FILL_SPEED = speedObj.fillSpeed;
            
            xMove = speedObj.xMoveBase; 
            yMove = speedObj.yMoveBase;
            
            //------------------------------------------------------------------------------------
            // Change move policy for support LR FAN BOOK with one guard 
            // Original policy for one guard is [0, 1, 1] ==> 2/3 speed of runner 
            // while AI_VERSION >= 3 change policy to [ 0, 1, 0, 1, 0, 1 ] ==> 1/2 speed of runner
            //------------------------------------------------------------------------------------
            if(curAiVersion < 3) {
                movePolicy[1] = [0, 1, 1, 0, 1, 1];
                maxGuard = MAX_OLD_GUARD;
            } else {
                movePolicy[1] = [0, 1, 0, 1, 0, 1]; //slow down the guard when only one guard
                maxGuard = MAX_NEW_GUARD;           //change max guard 
            }
            
            themeDataReset(1); //4/16/2015
            createHoleObj();  //6/27/2016
        }

        	
        public void createHoleObj()
        {
            holeObj = new HoleObj();
            holeObj.sprite = new createjs.Sprite(holeData, Shape.digHoleLeft);
            
            if(curAiVersion < 3) {
                holeObj.digLimit = 6; //for check guard is close to runner when digging
            } else {
                holeObj.digLimit = 8; //for check guard is close to runner when digging
            }
            
            holeObj.action = Action.ACT_STOP; //no digging 
        }

        
        var tileW, tileH; //tile width & tile height
        var tileWScale, tileHScale; //tile width/height with scale
        var W2, W4;       //W2: 1/2 tile-width,  W4: 1/4 tile width
        var H2, H4;       //H2: 1/2 tile-height, H4: 1/4 tile height

        public GameState gameState;
        public GameState lastGameState;
        public double tileScale, xMove, yMove;

        public int[] speedMode = {14, 18, 23, 29, 35}; //slow   normal  fast , slow down all speed 6/2/2016

        public int speed = 2; //normal 
        public int demoSpeed = 35;

        object levelData = defaultLevelData(); //Classic Lode Runner

        public int curLevel = 1, maxLevel = 1, passedLevel = 0;
        public PlayMode playMode = PlayMode.PLAY_CLASSIC;
        public int playData = 1; //classic lode runner
        public int curTime = 0; //count from 0 to MAX_TIME_COUNT

        Theme curTheme = Theme.APPLE; //support 2 themes: apple2 & C64

        public void init()
        {
            initAutoDemoRnd(); //init auto demo random levels
            
            loadStoreVariable(); //load data from localStorage
            
            getLastPlayInfo();
            initDemoData(); //get demo data from server
            
            ////genUserLevel(MAX_EDIT_LEVEL); //for debug only
            getEditLevelInfo(); //load edit levels
        }

        public void loadStoreVariable()
        {
            playerName = getPlayerName();
            if( ((playerUid = getUid()) == "" || playerUid.length != 32) && typeof(uId) != "undefined" ) {
                playerUid = uId; //uId variable in lodeRunner.wData.jss
                setUid(playerUid);
            }
            curTheme = getThemeMode();
            getThemeColor();
            getRepeatAction();
            if(getGamepadMode()) gamepadEnable();
        }

        public void stopDemoAndPlay()
        {
            var showStartMsg = 1;
            if(changingLevel) return false;
            clearIdleDemoTimer();
            disableStageClickEvent();

            soundStop(Sound.soundFall);		
            stopAllSpriteObj();
            
            if(playMode == PlayMode.PLAY_DEMO || playMode == PlayMode.PLAY_DEMO_ONCE) selectIconObj.disable(1); 
            if(playMode == PlayMode.PLAY_DEMO_ONCE) showStartMsg = 0;
            ////genUserLevel(MAX_EDIT_LEVEL); //for debug only
            ////getEditLevelInfo(); //load edit levels
            selectGame(showStartMsg);
        }

        
        public void checkIdleTime(maxIdleTime)
        {
            var idleTime = (new Date() - startIdleTime);
                
            if(idleTime > maxIdleTime){ //start demo
                clearIdleDemoTimer();
                playMode = PlayMode.PLAY_AUTO;
                anyKeyStopDemo(); 
                startGame();
            }
        }

        //==========================
        // Get playMode & playData
        //==========================
        public void getLastPlayInfo()
        {
            var infoJSON = getStorage(STORAGE_LASTPLAY_MODE);
            playMode = PLAY_NONE;

            if(infoJSON) {
                var infoObj = JSON.parse(infoJSON);
                playMode = infoObj.m; //mode= 1: classic, 2:time 
                playData = infoObj.d; //1: classic lode runner, 2: professional lode runner, 3: lode runner 3 ....
            }
            
            if( (playMode != PlayMode.PLAY_CLASSIC && playMode != PlayMode.PLAY_MODERN) || 
                (playData < 1 || (playData > maxPlayId && playData != PlayMode.PLAY_DATA_USERDEF) )
            ){
                playMode = PlayMode.PLAY_CLASSIC;
                playData = 1; //classic lode runner
            }
        }

        object gameTicker = null;
        public int changingLevel = 0; 
        public void startPlayTicker()
        {
            stopPlayTicker();
            //createjs.Ticker.timingMode = createjs.Ticker.RAF;
            if(playMode == PlayMode.PLAY_AUTO || playMode == PlayMode.PLAY_DEMO || playMode == PlayMode.PLAY_DEMO_ONCE) {
                createjs.Ticker.setFPS(demoSpeed); //06/12/2014
            } else {
                createjs.Ticker.setFPS(speedMode[speed]);
            }
            gameTicker = createjs.Ticker.on("tick", mainTick);	
        }
            
        public void stopPlayTicker()
        {
            if(gameTicker) {
                createjs.Ticker.off("tick", gameTicker);	
                gameTicker = null;
            }
        }

        public void startGame(noCycle)
        {
            var levelMap;
            gameState = GameState.GAME_WAITING;
            startPlayTicker();
            changingLevel = 1;
            
            curAiVersion = AI_VERSION; //07/04/2014
            initHotKeyVariable();      //07/09/2014
            
            switch(playMode) {
            case PlayMode.PLAY_CLASSIC:
                getClassicInfo();	
                levelMap = levelData[curLevel-1];	
                if(curLevel >= levelData.length && (passedLevel+1) >= levelData.length) {
                    loadEndingMusic(); //6/15/2015, music prepare for winner
                }
                break;
            case PlayMode.PLAY_MODERN:
                getModernInfo();	
                levelMap = levelData[curLevel-1];
                break;	
            case PlayMode.PLAY_TEST:
                levelMap = getTestLevelMap();
                break;
            case PlayMode.PLAY_DEMO:
                getDemoInfo();	
                levelMap = levelData[curLevel-1];
                break;	
            case PlayMode.PLAY_DEMO_ONCE:
                getDemoOnceInfo();	
                levelMap = levelData[curLevel-1];
                break;	
            case PlayMode.PLAY_AUTO:
                getAutoDemoLevel(1);
                levelMap = levelData[curLevel-1];	
                break;
            }
            showLevel(levelMap);
            if(noCycle) {
                beginPlay();
            } else {
                addCycScreen();
                setTimeout(function() { openingScreen(cycDiff*2);}, 5);
            }
        }

        int maxTileX = NO_OF_TILES_X - 1, maxTileY = NO_OF_TILES_Y - 1;

        var runner = null,  guard= [];
        var map; //[x][y] = { base: base map, act : active map, state:, bitmap: }
        var guardCount, goldCount, goldComplete;

        public void initVariable()
        {
            guard = [];
            keyAction = holeObj.action = Action.ACT_STOP; 
            goldCount = guardCount = goldComplete = 0;
            runner = null;
            dspTrapTile = 0;
            
            initRnd(); 
            initModernVariable();
            initGuardVariable();
            initInfoVariable();
            initCycVariable();
            
            initStillFrameVariable(); //05/01/2015 replace sprite with still frame image 
            setSpeedByAiVersion(); //07/04/2014
            
            debug("curAiVersion = " + curAiVersion);
        }

        


        public int runnerLife = RUNNER_LIFE;
        public int curScore = 0;
        public int curGetGold = 0, curGuardDeadNo = 0; //for modern mode 
        public int sometimePlayInGodMode = 0; //if sometime play in god mode then don't save to hi-scoe, 12/23/2014


        //=============================
        // initial modern mode variable
        //=============================
        public void initModernVariable()
        {
            //curTime = MAX_TIME_COUNT;
            curTime = curGetGold = curGuardDeadNo = 0;
        }


        // draw score number 
        public void drawScore(int addScore)
        {
            curScore += addScore;
        }

        public void drawGold(int addGold)
        {
            curGetGold += addGold;
        }

        public void drawGuard(int addGuard)
        {
            curGuardDeadNo += addGuard;
            if(curGuardDeadNo > 100) curGuardDeadNo = 100;
        }


        public void countTime(int addTime)
        {
            if(curTime >= MAX_TIME_COUNT) return;
            if(addTime) curTime++;
            if(curTime > MAX_TIME_COUNT) { curTime = MAX_TIME_COUNT;}
        }


        public int playTickTimer = 0;
        public void playGame(int deltaS)
        {
            if(goldComplete && runner.pos.y == 0 && runner.pos.yOffset == 0) {
                gameState = GameState.GAME_FINISH;
                return;
            }
            
            if(++playTickTimer >= TICK_COUNT_PER_TIME) {
                if(playMode != PlayMode.PLAY_CLASSIC && playMode != PlayMode.PLAY_AUTO && playMode != PlayMode.PLAY_DEMO) drawTime(1);
                else countTime(1);
                playTickTimer = 0;
            }
            
            if(playMode == PlayMode.PLAY_AUTO || playMode == PlayMode.PLAY_DEMO || playMode == PlayMode.PLAY_DEMO_ONCE) playDemo();
            if(recordMode) processRecordKey();
            if(!isDigging()) moveRunner();
            else processDigHole();
            if(gameState != GameState.GAME_RUNNER_DEAD) moveGuard();
            
            if(curAiVersion >= 3) {
                processGuardShake();
                processFillHole();
                processReborn();
            }
        }

        //***********************
        // BEGIN show new level *
        //***********************
        function showLevel(levelMap)
        {
            mainStage.removeAllChildren();
            
            loadingTxt.text = "";
            //loadingTxt.text = tileScale;  //for debug
            mainStage.addChild(loadingTxt); //for debug

            initVariable();	
            setBackground();
            
            buildLevelMap(levelMap);
            
            buildGroundInfo();
        }

        public int dspTrapTile = 0;
        public void toggleTrapTile()
        {
            dspTrapTile ^= 1;
            
            for(var y = 0; y < NO_OF_TILES_Y; y++) {
                for(var x = 0; x < NO_OF_TILES_X; x++) {
                    if( map[x][y].base == Type.TRAP_T) {
                        if(dspTrapTile) {
                            map[x][y].bitmap.setAlpha(0.5); //show trap tile
                        } else {
                            map[x][y].bitmap.setAlpha(1); //hide trap tile
                        }
                    }
                }
            }
        }

        public void startAllSpriteObj()
        {
            //(1) runner stop
            if(runner && !runner.paused) runner.sprite.play();
            
            //(2) guard stop
            for(var i = 0; i < guardCount; i++) {
                if(!guard[i].paused) guard[i].sprite.play();
            }
            
            if(curAiVersion < 3) { //for sprite only
                //(3) fill hole stop
                for(var i = 0; i < fillHoleObj.length; i++)
                    fillHoleObj[i].play();
            
                //(4) hole digging
                if(holeObj.action == ACT_DIGGING) holeObj.sprite.play();
            }
        }

        public void stopAllSpriteObj()
        {
            //(1) runner stop
            if(runner) {
                runner.paused = runner.sprite.paused;
                runner.sprite.stop();
            }
            
            //(2) guard stop
            for(var i = 0; i < guardCount; i++) {
                guard[i].paused = guard[i].sprite.paused;
                guard[i].sprite.stop();
            }
            
            if(curAiVersion < 3) { //for sprite only
                //(3) fill hole stop
                for(var i = 0; i < fillHoleObj.length; i++)
                    fillHoleObj[i].stop();
            
                //(4) hole digging
                if(holeObj.action == ACT_DIGGING) holeObj.sprite.stop();
            }
            
        }



        public void beginPlay()
        {
            gameState = GAME_START;
            keyAction = ACT_STOP;
            runner.sprite.gotoAndPlay();
            changingLevel = 0;
                
            if(recordMode) initRecordVariable();
            if(playMode == PLAY_AUTO || playMode == PLAY_DEMO || playMode == PLAY_DEMO_ONCE) {
                initPlayDemo();
                if(playMode == PLAY_DEMO || playMode == PLAY_DEMO_ONCE) initForPlay();
            } else { 

                if(playerName == "" || playerName.length <= 1) {
                    inputPlayerName(mainStage, showHelpMenu);
                } else {
                    showHelpMenu();
                }
                    
                
                if(playMode != PLAY_TEST && playMode != PLAY_EDIT) {
                    enableAutoDemoTimer(); //while start game and idle too long will into demo mode
                }
            }
        }

        public void incLevel(int incValue, int passed)
        {
            var wrap = 0;
            curLevel += incValue;
            while (curLevel > levelData.length) { curLevel-= levelData.length; wrap = 1; }
            if(playMode == PlayMode.PLAY_CLASSIC) setClassicInfo(passed);
            if(playMode == PlayMode.PLAY_MODERN) setModernInfo();
            
            return wrap;
        }

        public void decLevel(decValue)
        {
            curLevel -= decValue
            while (curLevel <= 0) curLevel += levelData.length;
            if(playMode == PLAY_CLASSIC) setClassicInfo(0);
            if(playMode == PLAY_MODERN) setModernInfo();
        }


        public void updateModernScoreInfo()
        {
            var lastHiScore = modernScoreInfo[curLevel-1];
            var levelScore = ((MAX_TIME_COUNT - curTime) + curGetGold + curGuardDeadNo) * SCORE_VALUE_PER_POINT;
            
            if(lastHiScore < levelScore) {
                modernScoreInfo[curLevel-1] = levelScore;
                setModernScoreInfo();
            }
            
            if(lastHiScore < 0) lastHiScore = 0;
            
            return lastHiScore;
        }

        public void gameFinishActiveNew(int level)
        {
            curLevel = level;
            setModernInfo();
            startGame();
        }

        public void gameFinishCloseIcon()
        {
            startGame();
        }

        public void gameFinishCallback(selectMode)
        {
            switch(selectMode) {
            case 0: //return (same level)
                gameState = GAME_NEW_LEVEL;
                break;
            case 1: //menu selection
                //incLevel(1);	
                activeSelectMenu(gameFinishActiveNew, gameFinishCloseIcon, null)	
                break;
            case 2: //new level
                incLevel(1,0);	
                gameState = GAME_NEW_LEVEL;
                break;
            default:
                debug("design error !");	
                break;	
            }
        }

        var lastScoreTime, scoreDuration;
        var scoreIncValue, finalScore;

        public void mainTick(int deltaTick)
        { 
            var scoreInfo;
            
            switch(gameState) {
            case GAME_START:
                countAutoDemoTimer();	
                if(keyAction != ACT_STOP && keyAction != ACT_UNKNOWN) {
                    disableAutoDemoTimer();	
                    gamepadClearId();	
                    gameState = GAME_RUNNING;
                    if(playMode == PLAY_MODERN) demoIconObj.disable(1);
                    playTickTimer = 0; //modern mode time counter
                    if(goldCount <= 0) showHideLaddr();
                }
                break;	
            case GAME_RUNNING:
                playGame(deltaTick);
                break;
            case GAME_RUNNER_DEAD:
                //console.log("Time=" + curTime + ", Tick= " + playTickTimer);
                //if(recordMode) recordModeToggle(GAME_RUNNER_DEAD); //for debug only (if enable it must disable below statement)
                if(recordMode == RECORD_KEY) recordModeDump(GAME_RUNNER_DEAD);	
                    
                soundStop(soundFall);
                stopAllSpriteObj();	
                themeSoundPlay("dead");
                switch(playMode) {
                case PLAY_CLASSIC:
                case PLAY_AUTO:
                    --runnerLife;
                    drawLife();	
                    if(runnerLife <= 0) {
                        gameOverAnimation();
                        menuIconDisable(1);
                        if(playMode == PLAY_CLASSIC) clearClassicInfo();
                        gameState = GAME_OVER_ANIMATION;
                    } else {
                        setTimeout(function() {gameState = GAME_NEW_LEVEL; }, 500);
                        gameState = GAME_WAITING;	
                        if(playMode == PLAY_CLASSIC) setClassicInfo(0);
                    }
                    break;
                case PLAY_DEMO:	
                    error(arguments.callee.name, "DEMO dead level=" + curLevel);
                        
                    setTimeout(function() {incLevel(1,0); gameState = GAME_NEW_LEVEL; }, 500);
                    gameState = GAME_WAITING;	
                    break;	
                case PLAY_DEMO_ONCE:
                    error(arguments.callee.name, "DEMO dead level=" + curLevel);
                        
                    disableStageClickEvent();
                    document.onkeydown = handleKeyDown;
                    setTimeout(function() {playMode = PLAY_MODERN; startGame(); }, 500);
                    gameState = GAME_WAITING;	
                    break;	
                case PLAY_MODERN:		
                    setTimeout(function() {gameState = GAME_NEW_LEVEL; }, 500);
                    gameState = GAME_WAITING;	
                    break;
                case PLAY_TEST:		
                    setTimeout(function() { back2EditMode(0); }, 500);
                    gameState = GAME_WAITING;	
                    break;
                default:
                    debug("GAME_RUNNER_DEAD: desgin error !");	
                    break;	
                }
                break;	
            case GAME_OVER_ANIMATION:
                break;	
            case GAME_OVER:
                scoreInfo = null;	
                if(playMode == PLAY_CLASSIC && !sometimePlayInGodMode) {	
                    scoreInfo = {s:curScore, l: passedLevel+1 };
                }	
                    
                showScoreTable(playData, scoreInfo , function() { showCoverPage();});	
                gameState = GAME_WAITING;	
                return;
            case GAME_FINISH: 
                stopAllSpriteObj();
                //console.log("Time=" + curTime + ", Tick= " + playTickTimer);
                    
                switch(playMode) {
                case PLAY_CLASSIC:
                case PLAY_AUTO:		
                case PLAY_DEMO:		
                    soundPlay(soundPass);
                    finalScore = curScore + SCORE_COMPLETE_LEVEL;
                    scoreDuration = ((soundPass.getDuration()) /(SCORE_COUNTER+1))| 0;
                    lastScoreTime = event.time;
                    scoreIncValue = SCORE_COMPLETE_LEVEL/SCORE_COUNTER|0;
                    drawScore(scoreIncValue);
                    gameState = GAME_FINISH_SCORE_COUNT;	
                    break;
                case PLAY_DEMO_ONCE:
                    soundPlay(soundEnding);
                    disableStageClickEvent();
                    document.onkeydown = handleKeyDown;
                    setTimeout(function() {playMode = PLAY_MODERN; startGame(); }, 500);
                    gameState = GAME_WAITING;
                    break;
                case PLAY_MODERN:
                    soundPlay(soundEnding);
                    var lastHiScore = lastHiScore = updateModernScoreInfo();
                    levelPassDialog(curLevel, curGetGold, curGuardDeadNo, curTime, lastHiScore, 
                                returnBitmap, select1Bitmap, nextBitmap,
                                mainStage, tileScale, gameFinishCallback);	
                    gameState = GAME_WAITING;
                    break;
                case PLAY_TEST:
                    soundPlay(soundEnding);
                    setTimeout(function() { back2EditMode(1);},500);	
                    gameState = GAME_WAITING;
                    break;
                default:
                    error(arguments.callee.name, "design error, playMode =" + playMode);
                    break;	
                }

                //if(recordMode) recordModeToggle(GAME_FINISH); //for debug only (if enable it must comment below if statement)
                if(recordMode == RECORD_KEY) {
                    recordModeDump(GAME_FINISH);	
                    
                    if( (playMode == PLAY_CLASSIC || playMode == PLAY_MODERN) && playData <= maxPlayId ) {
                        updatePlayerDemoData(playData, curDemoData); //update current player demo data
                    }
                }
                break;	
            case GAME_FINISH_SCORE_COUNT:		
                if(event.time > lastScoreTime+ scoreDuration) {
                    lastScoreTime += scoreDuration;
                    if(curScore + scoreIncValue >= finalScore) {
                        curScore = finalScore;
                        drawScore(0);
                        gameState = GAME_NEW_LEVEL;
                        
                        switch(playMode) {
                        case PLAY_TEST:		
                            setTimeout(function() { back2EditMode(1);},500);	
                            break;
                        case PLAY_CLASSIC:
                            if(++runnerLife > RUNNER_MAX_LIFE) runnerLife = RUNNER_MAX_LIFE;	
                            break;	
                        case PLAY_AUTO:
                            if(demoCount >= demoMaxCount) {
                                setTimeout(function(){ showCoverPage();}, 500);	
                                gameState = GAME_WAITING;
                            }
                            break;	
                        }

                        if(recordMode != RECORD_PLAY) {
                            if(incLevel(1, 1) && playMode == PLAY_CLASSIC && passedLevel >= levelData.length) 
                                gameState = GAME_WIN;
                        }
                        
                    } else {
                        drawScore(scoreIncValue);
                    }
                }
                break;
            case GAME_NEXT_LEVEL:
                soundStop(soundFall);		
                stopAllSpriteObj();
                incLevel(shiftLevelNum, 0);
                gameState = GAME_NEW_LEVEL; 
                return;
            case GAME_PREV_LEVEL:
                soundStop(soundFall);		
                stopAllSpriteObj();
                decLevel(shiftLevelNum);	
                gameState = GAME_NEW_LEVEL; 
                return;
            case GAME_NEW_LEVEL:
                gameState = GAME_WAITING;	
                newLevel();	
                break;
            case GAME_WIN:		
                scoreInfo = {s:curScore, l: levelData.length, w:1 }; //winner	
                menuIconDisable(1);
                clearClassicInfo();
                showScoreTable(playData, scoreInfo , function() { showCoverPage();});	
                gameState = GAME_WAITING;	
                return;			
            case GAME_LOADING:
                break;	
            case GAME_PAUSE:
            default:
                return;	
            }
            
            mainStage.update();
        }
    }
}