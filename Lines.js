	    	var auto = true;
    	    	var LINES = 4000;
        	var BOUNDS = 50;
        	var FACTOR = 7.0;
        	var leader, viewer, viewing = 2, autocount = 0;
        	var centreX = 0, centreY = 0, centreZ = 0;
        	var centreOX, centreOY, centreOZ;
        	var time = new Date();

        	var random = new Rand();
        	var line = new Array(LINES);
			var lineP;
			
	    	var milliSeconds = ((time.getHours() * 3600) + (time.getMinutes() * 60) + time.getSeconds() * 1000) + time.getMilliseconds();

        	for (j = 0; j < milliSeconds; j++)
        	{
            	random.Get();
        	}

            // Initialize the lines
        	for (var j = 0; j < LINES; j++)
        	{
            		line[j] = new Line();
            		line[j].positionX = (BOUNDS * random.Get());
            		line[j].positionY = (BOUNDS * random.Get());
            		line[j].positionZ = (BOUNDS * random.Get());
            		line[j].positionOX = line[j].positionX;
            		line[j].positionOY = line[j].positionY;
            		line[j].positionOZ = line[j].positionZ;
            		line[j].attraction = ((0.2 / (FACTOR * 5)) + ((0.1 / (FACTOR * 5)) * random.Get()));
            		line[j].velocity = ((1 / FACTOR) + ((0.5 / FACTOR) * random.Get()));
            		line[j].speedX = (random.Get() * line[j].velocity);
            		line[j].speedY = (random.Get() * line[j].velocity);
            		line[j].speedZ = (random.Get() * line[j].velocity);
            		line[j].red = Math.floor(128 + ((random.Get() * 128)));
            		line[j].green = Math.floor(128 + ((random.Get() * 128)));
            		line[j].blue = Math.floor(128 + ((random.Get() * 128)));
            		line[j].NormalizeVelocity();
            		centreX += (line[j].positionOX / LINES);
            		centreY += (line[j].positionOY / LINES);
            		centreZ += (line[j].positionOZ / LINES);
        	}	

        	leader = Math.round((LINES / 2) + (random.Get() * (LINES / 2)));
        	viewer = Math.round((LINES / 2) + (random.Get() * (LINES / 2)));
        	
        	var SCREEN_WIDTH = window.innerWidth,
			SCREEN_HEIGHT = window.innerHeight,

			mouseX = 0, mouseY = 0,

			windowHalfX = window.innerWidth / 2,
			windowHalfY = window.innerHeight / 2,

			camera, scene, renderer;

			init();
			setInterval( render, 1000 / (2 * FACTOR));
			
		function RGB2Hex(red, green, blue)
		{
    			var hexRed = red.toString(16), hexGreen = green.toString(16), hexBlue = blue.toString(16);
    			
    			if (hexRed.length < 2)
    			{
    				hexRed = "0" + hexRed;
    			}
    			
    			if (hexGreen.length < 2)
    			{
    				hexGreen = "0" + hexGreen;
    			}
    			
    			if (hexBlue.length < 2)
    			{
    				hexBlue = "0" + hexBlue;
    			}
    			
    			var hexColor = hexRed + hexGreen + hexBlue;
    				
    			return "0x" + hexColor;
		}

		function init() {

			var container, separation = 100, amountX = 50, amountY = 50;
				
				container = document.createElement('div');
				document.body.appendChild(container);

				camera = new THREE.Camera( 45, SCREEN_WIDTH / SCREEN_HEIGHT, 0.01, 1000 );
				camera.position.z = BOUNDS;

				scene = new THREE.Scene();

				renderer = new THREE.CanvasRenderer();
				renderer.setSize(SCREEN_WIDTH, SCREEN_HEIGHT);
				container.appendChild(renderer.domElement);

			// lines

			for (var i = 0; i < LINES; i++) {

				var geo = new THREE.Geometry();
				var vector = new THREE.Vector3(line[i].positionOX, line[i].positionOY, line[i].positionOZ);

				geo.vertices.push( new THREE.Vertex( vector ) );

				var vector2 = vector.clone();
				vector2.multiplyScalar( Math.random() * 0.3 + 1 );

				geo.vertices.push( new THREE.Vertex( vector2 ) );
				lineP = new THREE.Line( geo, new THREE.LineBasicMaterial( { color: RGB2Hex(line[i].red, line[i].green, line[i].blue), opacity: 1.0 } ) );
				lineP.dynamic = true;
				scene.addObject( lineP );
			}

		}


		function render() {
	            	var xDiff, yDiff, zDiff, hypotenuse, change;

	            	change = 50.0 + (random.Get() * 50.0);

	            	if (change < 0.03)
        	    	{
		   		autocount += 1;
	
        	        	for (var j = 0; j < 5; j++)
                		{
                		    random.Get();
                		}
                	
            	    		leader = Math.floor((1.0 + random.Get()) * (LINES / 2.0));
            		}

	       		centreOX = centreX;
        	    	centreOY = centreY;
            		centreOZ = centreZ;
	            	centreX = 0;
        	    	centreY = 0;
            		centreZ = 0;
	
        	    	for (var j = 0; j < LINES; j++)
            		{
            	    		line[j].positionOX = line[j].positionX - centreOX;
            	    		line[j].positionOY = line[j].positionY - centreOY;
            	    		line[j].positionOZ = line[j].positionZ - centreOZ;

 	           		if (j != leader)
            	    		{
	           			xDiff = line[leader].positionX - line[j].positionX;
            	        		yDiff = line[leader].positionY - line[j].positionY;
            	        		zDiff = line[leader].positionZ - line[j].positionZ;
	
    	                		hypotenuse = (xDiff * xDiff) + (yDiff * yDiff) + (zDiff * zDiff);
    	                		hypotenuse = Math.sqrt(hypotenuse);

    	                		line[j].speedX += ((xDiff / hypotenuse) * line[j].attraction);
    	                		line[j].speedY += ((yDiff / hypotenuse) * line[j].attraction);
    	                		line[j].speedZ += ((zDiff / hypotenuse) * line[j].attraction);
	
    	                		line[j].NormalizeVelocity();
    	            		}

    	            		line[j].positionX = line[j].positionOX + line[j].speedX;
    	            		line[j].positionY = line[j].positionOY + line[j].speedY;
    	            		line[j].positionZ = line[j].positionOZ + line[j].speedZ;
	
	    	            	centreX += (line[j].positionX / LINES);
    	            		centreY += (line[j].positionY / LINES);
    	            		centreZ += (line[j].positionZ / LINES);

				scene.objects[j].geometry.vertices[0].position.x = line[j].positionOX;
				scene.objects[j].geometry.vertices[0].position.y = line[j].positionOY;
				scene.objects[j].geometry.vertices[0].position.z = line[j].positionOZ;
				scene.objects[j].geometry.vertices[1].position.x = line[j].positionX;
				scene.objects[j].geometry.vertices[1].position.y = line[j].positionY;
				scene.objects[j].geometry.vertices[1].position.z = line[j].positionZ;
				scene.objects[j].geometry.__dirtyVertices = true;
			}

			camera.position.x = line[viewer].positionX * 2;
			camera.position.y = line[viewer].positionY * 2;
			camera.position.z = line[viewer].positionZ * 2;

			renderer.render( scene, camera );
		}