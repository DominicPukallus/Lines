using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Lines2_1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        bool auto = true;
        const int LINES = 80000;
        const float BOUNDS = 100;
        const float FACTOR = 10.0f;
        int leader, viewer, viewing = 1, autocount = 0;
        float centreX, centreY, centreZ;
        float centreOX, centreOY, centreOZ;
        DateTime time = DateTime.Now;

        Rand random;
        Line[] line;

        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        int screenWidth;
        int screenHeight;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        Matrix projection;
        Matrix view;

        Vector3 cameraPosition;

        // a basic effect, which contains the shaders that we will use to draw our
        // primitives.
        BasicEffect basicEffect;

        // a block of vertices that calling AddVertex will fill. Flush will draw using
        // this array, and will determine how many primitives to draw from
        // positionInBuffer.
        VertexPositionColor[] vertices = new VertexPositionColor[LINES * 2];
        short[] lineListIndices = new short[LINES * 2];

        // the vertex declaration that will be set on the device for drawing. this is 
        // created automatically using VertexPositionColor's vertex elements.
        VertexDeclaration vertexDeclaration;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds((1000.0f / 2f) / FACTOR);
            graphics.SynchronizeWithVerticalRetrace = true;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            random = new Rand();
            line = new Line[LINES];

            int hour = time.Hour;
            int minute = time.Minute;
            int second = time.Second;
            int milliSeconds = ((hour * 3600) + (minute * 60) + second) * 1000;

            for (int j = 0; j < milliSeconds; j++)
            {
                random.Get();
            }

            // Initialize the lines
            for (int j = 0; j < LINES; j++)
            {
                line[j] = new Line();
                line[j].positionX = (float)(BOUNDS * random.Get());
                line[j].positionY = (float)(BOUNDS * random.Get());
                line[j].positionZ = (float)(BOUNDS * random.Get());
                line[j].positionOX = line[j].positionX;
                line[j].positionOY = line[j].positionY;
                line[j].positionOZ = line[j].positionZ;
                line[j].attraction = (float)((0.2 / (FACTOR * 5)) + ((0.1 / (FACTOR * 5)) * random.Get()));
                line[j].velocity = (float)((1 / FACTOR) + ((0.5 / FACTOR) * random.Get()));
                line[j].speedX = (float)(random.Get() * line[j].velocity);
                line[j].speedY = (float)(random.Get() * line[j].velocity);
                line[j].speedZ = (float)(random.Get() * line[j].velocity);
                line[j].red = (int)(128 + (float)((random.Get() * 128)));
                line[j].green = (int)(128 + (float)((random.Get() * 128)));
                line[j].blue = (int)(128 + (float)((random.Get() * 128)));
                line[j].normalizeVelocity();

                centreX += (line[j].positionOX / LINES);
                centreY += (line[j].positionOY / LINES);
                centreZ += (line[j].positionOZ / LINES);
            }

            leader = (int)((LINES / 2) + (random.Get() * (LINES / 2)));
            viewer = (int)((LINES / 2) + (random.Get() * (LINES / 2)));

            // Populate the array with references to indices in the vertex buffer
            for (int i = 0; i < LINES; i++)
            {
                lineListIndices[i * 2] = (short)(i * 2);
                lineListIndices[(i * 2) + 1] = (short)((i * 2) + 1);
            }

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            device = graphics.GraphicsDevice;
            
            // how big is the screen?
            screenWidth = device.Viewport.Width;
            screenHeight = device.Viewport.Height;

            aspectRatio = device.Viewport.AspectRatio;

            // Set the position of the camera in world space, for our view matrix.
            cameraPosition = new Vector3(0.0f, 0.0f, (float)(BOUNDS / 4));

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 100000.0f);
            view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

            // create a vertex declaration, which tells the graphics card what kind of
            // data to expect during a draw call. We're drawing using
            // VertexPositionColors, so we'll use those vertex elements.
            vertexDeclaration = new VertexDeclaration(device,
                VertexPositionColor.VertexElements);

            // set up a new basic effect, and enable vertex colors.
            basicEffect = new BasicEffect(device, null);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = projection;
            basicEffect.View = view;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                viewing = 1;
                auto = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                viewing = 2;
                auto = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                viewing = 3;
                auto = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F4))
            {
                auto = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

                // TODO: Add your update logic here
            byte red, green, blue;
            float xDiff, yDiff, zDiff;
            float hypotenuse;
            float change;

            change = 50 + ((float)random.Get() * 50);
            if (change < 0.03)
            {
                leader = (int)((LINES / 2) + (random.Get() * (LINES / 2)));
                autocount += 1;

                for (int j = 0; j < 5; j++)
                {
                    random.Get();
                }
            }

            centreOX = centreX;
            centreOY = centreY;
            centreOZ = centreZ;
            centreX = 0;
            centreY = 0;
            centreZ = 0;

            for (int j = 0; j < LINES; j++)
            {
                line[j].positionOX = line[j].positionX;
                line[j].positionOY = line[j].positionY;
                line[j].positionOZ = line[j].positionZ;

                if (j != leader)
                {
                    xDiff = line[leader].positionX - line[j].positionX;
                    yDiff = line[leader].positionY - line[j].positionY;
                    zDiff = line[leader].positionZ - line[j].positionZ;

                    hypotenuse = (xDiff * xDiff) + (yDiff * yDiff) + (zDiff * zDiff);
                    hypotenuse = (float)System.Math.Sqrt(hypotenuse);

                    line[j].speedX += ((xDiff / hypotenuse) * line[j].attraction);
                    line[j].speedY += ((yDiff / hypotenuse) * line[j].attraction);
                    line[j].speedZ += ((zDiff / hypotenuse) * line[j].attraction);

                    line[j].normalizeVelocity();
                }

                line[j].positionX += (line[j].speedX - centreOX);
                line[j].positionY += (line[j].speedY - centreOY);
                line[j].positionZ += (line[j].speedZ - centreOZ);

                centreX += (line[j].positionX / LINES);
                centreY += (line[j].positionY / LINES);
                centreZ += (line[j].positionZ / LINES);

                red = (byte)line[j].red;
                green = (byte)line[j].green;
                blue = (byte)line[j].blue;

                Color col = new Color(red, green, blue);

                vertices[j * 2].Position = new Vector3(line[j].positionOX, line[j].positionOY, line[j].positionOZ);
                vertices[(j * 2) + 1].Position = new Vector3(line[j].positionX, line[j].positionY, line[j].positionZ);

                vertices[j * 2].Color = col;
                vertices[(j * 2) + 1].Color = col;
            }
          
            // Set the position of the camera in world space, for our view matrix.
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.001f, 10000.0f);

            // Viewer is static outside and looking at centre
            if (viewing == 1)
            {
                cameraPosition = new Vector3(0.0f, 0.0f, (float)(BOUNDS / 4));
                view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

                if (auto && (autocount > 1))
                {
                    if (random.Get() > 0)
                        viewing = 2;
                    else
                        viewing = 3;

                    autocount = 0;
                }
            }
            
            // Viewer is moving inside and looking at centre
            if (viewing == 2)
            {
                cameraPosition = new Vector3((line[viewer].positionX * 2), (line[viewer].positionY * 2), (line[viewer].positionZ * 2));
                view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

                if (auto && (autocount > 4))
                {
                    if (random.Get() > 0)
                        viewing = 3;
                    else
                        viewing = 1;

                    autocount = 0;
                }
            }

            // Viewer is moving inside and looking ahead
            if (viewing == 3)
            {
                cameraPosition = new Vector3(line[viewer].positionX, line[viewer].positionY, line[viewer].positionZ);
                view = Matrix.CreateLookAt(cameraPosition, new Vector3(line[viewer].positionX + line[viewer].speedX - centreOX, line[viewer].positionY + line[viewer].speedY - centreOY, line[viewer].positionZ + line[viewer].speedZ - centreOZ), Vector3.Up);

                if (auto && (autocount > 2))
                {
                    if (random.Get() > 0)
                        viewing = 1;
                    else
                        viewing = 2;

                    autocount = 0;
                }
            }

            basicEffect.Projection = projection;
            basicEffect.View = view;


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            device.Clear(Color.Black);

            // TODO: Add your drawing code here

            // prepare the graphics device for drawing by setting the vertex declaration
            // and telling our basic effect to begin.
            device.VertexDeclaration = vertexDeclaration;
            basicEffect.Begin();
            basicEffect.CurrentTechnique.Passes[0].Begin();

            // submit the draw call to the graphics card
            device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0,
                LINES * 2, lineListIndices, 0, LINES);

            // and then tell basic effect that we're done.
            basicEffect.CurrentTechnique.Passes[0].End();
            basicEffect.End();


            base.Draw(gameTime);
        }
    }
}
