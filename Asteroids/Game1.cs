using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Asteroids
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        enum CollisionType { None, Boundary, Target, bigTarget, power, bullet}

        struct Bullet
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 direction;
        }

        struct Rock
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 direction;
            public BoundingSphere bound;
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        private Model shipModel;
        private Model skyModel;

        Vector3 skyPosition = new Vector3(8, 1, -10);
        Quaternion skyRotation = Quaternion.Identity;
        Vector3 shipPosition = new Vector3(8, 1, -3);
        Quaternion shipRotation = Quaternion.Identity;
        float gameSpeed = 1.0f;
        
        Model bulletModel;

        List<Bullet> bulletList = new List<Bullet>();
        double lastBulletTime = 0;
        private SoundEffect laser;

        Quaternion cameraRotation = Quaternion.Identity;

        const int maxTargets = 35;
        const int maxBigTargets = 15;
        const int maxSpikes = 25;
        Model targetModel;
        Model bigTargetModel;
        Model spikeModel;
        List<BoundingSphere> targetList = new List<BoundingSphere>();
        List<BoundingSphere> bigTargetList = new List<BoundingSphere>();
        List<BoundingSphere> spikeList = new List<BoundingSphere>();
        private SoundEffect boom;



        const int maxShieldPower = 5;
        Model shieldPowerModel;
        List<BoundingSphere> shieldPowerList = new List<BoundingSphere>();

        Matrix viewMatrix;
        Matrix projectionMatrix;

        private SpriteFont font;
        private SpriteFont shieldFont;
        private SpriteFont shipCountFont;
        private int score = 0;
        private int shield = 3;
        private int shipCount = 3;
        private SoundEffect shieldHit;
        private SoundEffect respawn;

        bool gameOver = false;
        private SpriteFont gameOverFont;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 700;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Asteroids";

            base.Initialize();
        }

        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            shipModel = Content.Load<Model>("spaceship");

            skyModel = Content.Load<Model>("skybox");

            targetModel = Content.Load<Model>("rock");

            bigTargetModel = Content.Load<Model>("bigRock2");

            spikeModel = Content.Load<Model>("Spikes");

            bulletModel = Content.Load<Model>("bullet");

            shieldPowerModel = Content.Load<Model>("shieldPower");

            font = Content.Load<SpriteFont>("Score");

            shieldFont = Content.Load<SpriteFont>("Shield");

            shipCountFont = Content.Load<SpriteFont>("Shield");

            gameOverFont = Content.Load<SpriteFont>("gameOver");

            laser = Content.Load<SoundEffect>("laserSound");

            boom = Content.Load<SoundEffect>("boom");

            shieldHit = Content.Load<SoundEffect>("shieldHit");

            respawn = Content.Load<SoundEffect>("respawn");

            AddBigTargets();
            AddTargets();
            AddShieldPower();
            AddSpikes();
            // TODO: use this.Content to load your game content here
        }



        private void AddTargets()
        {
            int Width = 10;
            int Length = 10;
            Random random = new Random();

            while (targetList.Count < maxTargets)
            {
                int x = random.Next(Width);
                int z = -random.Next(Length);
                float y = (float)random.Next(9000) / 1000f + 1;
                //float radius = (float)random.Next(1000) / 1000f * 0.2f + 0.01f;
                float radius = 0.2f;
                BoundingSphere newTarget = new BoundingSphere(new Vector3(x, y, z), radius);
                if (CheckCollision(newTarget) == CollisionType.None)
                {
                    targetList.Add(newTarget);

                }
            }
        }

        private void AddBigTargets()
        {
            int Width = 10;
            int Length = 10;
            Random random = new Random();

            while (bigTargetList.Count < maxBigTargets)
            {
                int x = random.Next(Width);
                int z = -random.Next(Length);
                float y = (float)random.Next(6000) / 1000f + 1;
                //float radius = (float)random.Next(3000) / 1000f * 0.2f + 0.01f;
                float radius = 0.5f;
                BoundingSphere newBigTarget = new BoundingSphere(new Vector3(x, y, z), radius);
                if (CheckCollision(newBigTarget) == CollisionType.None)
                {
                    bigTargetList.Add(newBigTarget);

                }
            }
        }


        private void AddSpikes()
        {
            int Width = 15;
            int Length = 15;
            Random random = new Random();

            while (spikeList.Count < maxSpikes)
            {
                int x = random.Next(Width);
                int z = -random.Next(Length);
                float y = (float)random.Next(7000) / 1000f + 1;
                //float radius = (float)random.Next(1000) / 1000f * 0.2f + 0.01f;
                float radius = 0.2f;
                BoundingSphere newSpikes = new BoundingSphere(new Vector3(x, y, z), radius);
                if (CheckCollision(newSpikes) == CollisionType.None)
                {
                    spikeList.Add(newSpikes);

                }
            }
        }

        private void AddShieldPower()
        {
            int Width = 15;
            int Length = 15;
            Random random = new Random();

            while (shieldPowerList.Count < maxShieldPower)
            {
                int x = random.Next(Width);
                int z = -random.Next(Length);
                float y = (float)random.Next(2000) / 1000f + 1;
                //float radius = (float)random.Next(1000) / 1000f * 0.2f + 0.01f;
                float radius = 0.3f;
                BoundingSphere newShieldPower = new BoundingSphere(new Vector3(x, y, z), radius);
                if (CheckCollision(newShieldPower) == CollisionType.None)
                {
                    shieldPowerList.Add(newShieldPower);

                }
            }
        }

        private void SetUpCamera()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 30), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.2f, 500.0f);
        }

        

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            ProcessKeyboard(gameTime);
            //UpdateTimer(gameTime);
            float moveSpeed = gameTime.ElapsedGameTime.Milliseconds / 1500.0f * gameSpeed;
            MoveShipForward(ref shipPosition, shipRotation, moveSpeed);

            BoundingSphere shipSphere = new BoundingSphere(shipPosition, 0.04f);
            UpdateBulletPositions(moveSpeed);


            if (CheckCollision(shipSphere) == CollisionType.Target)
            {
                if (shield > 0)
                {
                    shield -= 1;
                    shieldHit.Play();
                }
                else
                {
                    shipPosition = new Vector3(8, 1, -3);
                    shipRotation = Quaternion.Identity;
                    gameSpeed /= 1.1f;
                    //score = 0;
                    shield = 3;
                    shipCount -= 1;
                    if (shipCount < 0)
                    {
                        gameOver = true;
                    }
                    respawn.Play();
                }
            }

            if (CheckCollision(shipSphere) == CollisionType.power)
            {
                shield += 1;
                if (shield > 3)
                {
                    shield = 3;
                }
            }



                UpdateCamera();

            base.Update(gameTime);
        }

        private void UpdateCamera()
        {


            cameraRotation = Quaternion.Lerp(cameraRotation, shipRotation, 0.1f);

            Vector3 campos = new Vector3(0, 0.1f, 0.6f);
            campos = Vector3.Transform(campos, Matrix.CreateFromQuaternion(cameraRotation));
            campos += shipPosition;

            Vector3 camup = new Vector3(0, 1, 0);
            camup = Vector3.Transform(camup, Matrix.CreateFromQuaternion(cameraRotation));

            viewMatrix = Matrix.CreateLookAt(campos, shipPosition, camup);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.2f, 500.0f);
        }



        private void ProcessKeyboard(GameTime gameTime)
        {
            float leftRightRot = 0;

            float turningSpeed = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            turningSpeed *= 1.6f * gameSpeed;
            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.Right))
                leftRightRot += turningSpeed;
            if (keys.IsKeyDown(Keys.Left))
                leftRightRot -= turningSpeed;

            float upDownRot = 0;
            if (keys.IsKeyDown(Keys.Down))
                upDownRot += turningSpeed;
            if (keys.IsKeyDown(Keys.Up))
                upDownRot -= turningSpeed;

            if (keys.IsKeyDown(Keys.Space))
            {
                double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
                if (currentTime - lastBulletTime > 500)
                {
                    laser.Play();
                    Bullet newBullet = new Bullet();
                    newBullet.position = shipPosition;
                    newBullet.rotation = shipRotation;
                    bulletList.Add(newBullet);

                    lastBulletTime = currentTime;
                }
            }

            Quaternion additionalRot = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), leftRightRot) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), upDownRot);
            shipRotation *= additionalRot;
        }

        private void UpdateBulletPositions(float moveSpeed)
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                Bullet currentBullet = bulletList[i];
                MoveForward(ref currentBullet.position, currentBullet.rotation, moveSpeed * 10.0f);
                bulletList[i] = currentBullet;

                BoundingSphere bulletSphere = new BoundingSphere(currentBullet.position, 0.05f);
                CollisionType colType = CheckCollision(bulletSphere);
                if (colType == CollisionType.Target)
                {
                    boom.Play();
                    bulletList.RemoveAt(i);
                    i--;
                    score += 1;
                    if (colType == CollisionType.Target)
                        gameSpeed *= 1.05f;
                }
            }
        }

            

        private void MoveTarget(ref Vector3 position, Quaternion rotationQuat, Vector3 direction, float speed, Bullet current)
        {
            if (direction == new Vector3(0, 0, 0))
            {
                Random r = new Random();
                int x = r.Next(-10, 10);
                int y = r.Next(-10, 10);
                int z = r.Next(-10, 10);
                current.direction = Vector3.Transform(new Vector3(x, y, z), rotationQuat);
                position += direction * speed;
            }
            else
            {
                position += current.direction * speed;
            }
        }

        private void MoveForward(ref Vector3 position, Quaternion rotationQuat, float speed)
        {
            Vector3 addVector = Vector3.Transform(new Vector3(0, 0, -1), rotationQuat);
            position += addVector * speed;
        }

        private void MoveShipForward(ref Vector3 position, Quaternion rotationQuat, float speed)
        {
            Vector3 addVector = Vector3.Transform(new Vector3(0, 0, -1), rotationQuat);
            position += addVector * speed;
            Vector3 diff = position - skyPosition;
            float dist = (float)Math.Sqrt(Vector3.Dot(diff, diff));
            if (dist > 16)
            {
                position -= addVector * speed * 60;
                if (shield > 0)
                {
                    shield -= 1;
                    shieldHit.Play();
                }
                else
                {
                    shipPosition = new Vector3(8, 1, -3);
                    shipRotation = Quaternion.Identity;
                    gameSpeed /= 1.1f;
                    //score = 0;
                    shield = 3;
                    shipCount -= 1;
                    if (shipCount < 0)
                    {
                        gameOver = true;
                    }
                    respawn.Play();
                }
            }

        }

        private CollisionType CheckCollision(BoundingSphere sphere)
        {



            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] != null)
                {
                    if (targetList[i].Contains(sphere) != ContainmentType.Disjoint )
                    {
                        targetList.RemoveAt(i);
                        i--;
                        
                        return CollisionType.Target;
                    }
                }
                
            }

            for (int i = 0; i < bigTargetList.Count; i++)
            {
                int num = bigTargetList.Count;
                if (bigTargetList[i] != null)
                {
                    if (bigTargetList[i].Contains(sphere) != ContainmentType.Disjoint)
                    {
                        bigTargetList.RemoveAt(i);
                        i--;

                        return CollisionType.Target;
                    }
                }

            }

            for (int i = 0; i < spikeList.Count; i++)
            {
                if (spikeList[i] != null)
                {
                    if (spikeList[i].Contains(sphere) != ContainmentType.Disjoint)
                    {
                        spikeList.RemoveAt(i);
                        i--;

                        return CollisionType.Target;
                    }
                }

            }

            for (int i = 0; i < shieldPowerList.Count; i++)
            {
                if (shieldPowerList[i] != null)
                {
                    if (shieldPowerList[i].Contains(sphere) != ContainmentType.Disjoint)
                    {
                        shieldPowerList.RemoveAt(i);
                        i--;
                        

                        return CollisionType.power;
                    }
                }

            }

            return CollisionType.None;
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            
            // TODO: Add your drawing code here
            if (!gameOver)
            {
                DrawModel(shipModel, viewMatrix, projectionMatrix);
                Drawsky(skyModel, viewMatrix, projectionMatrix);
                DrawTargets(targetModel, viewMatrix, projectionMatrix);
                DrawBigTargets(bigTargetModel, viewMatrix, projectionMatrix);
                DrawSpikes(spikeModel, viewMatrix, projectionMatrix);
                DrawShieldPower(shieldPowerModel, viewMatrix, projectionMatrix);
                DrawBullets(bulletModel, viewMatrix, projectionMatrix);

                
                spriteBatch.DrawString(font, "Score: " + score, new Vector2(100, 100), Color.Gold);
                spriteBatch.DrawString(shieldFont, "Shield: " + shield, new Vector2(100, 125), Color.CornflowerBlue);
                spriteBatch.DrawString(shipCountFont, "Ships left: " + shipCount, new Vector2(100, 150), Color.DarkSeaGreen);
            }
            else
            {
                spriteBatch.DrawString(gameOverFont, "GAME OVER!!", new Vector2(250, 300), Color.Crimson);
            }
            spriteBatch.End();
            base.Draw(gameTime);
            
        }



        private void DrawModel(Model model, Matrix view, Matrix projection)
        {
            Matrix worldMatrix = Matrix.CreateScale(0.01f, 0.01f, 0.01f) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion(shipRotation) * Matrix.CreateTranslation(shipPosition);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        private void Drawsky(Model model, Matrix view, Matrix projection)
        {
            Matrix worldMatrix = Matrix.CreateScale(20f, 20f, 20f) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion(skyRotation) * Matrix.CreateTranslation(skyPosition);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        private void DrawTargets(Model model, Matrix view, Matrix projection)
        {
            for (int i = 0; i < targetList.Count; i++)
            {
                Matrix worldMatrix = Matrix.CreateScale(targetList[i].Radius) * Matrix.CreateTranslation(targetList[i].Center);

                Matrix[] targetTransforms = new Matrix[targetModel.Bones.Count];
                targetModel.CopyAbsoluteBoneTransformsTo(targetTransforms);
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = worldMatrix;
                        effect.View = view;
                        effect.Projection = projection;
                    }

                    mesh.Draw();
                }
            }
        }

        private void DrawBigTargets(Model model, Matrix view, Matrix projection)
        {
            int number = bigTargetList.Count;
            for (int i = 0; i < bigTargetList.Count; i++)
            {
                Matrix worldMatrix = Matrix.CreateScale(bigTargetList[i].Radius) * Matrix.CreateTranslation(bigTargetList[i].Center);

                Matrix[] bigTargetTransforms = new Matrix[bigTargetModel.Bones.Count];
                bigTargetModel.CopyAbsoluteBoneTransformsTo(bigTargetTransforms);
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = worldMatrix;
                        effect.View = view;
                        effect.Projection = projection;
                    }

                    mesh.Draw();
                }
            }
        }

        private void DrawSpikes(Model model, Matrix view, Matrix projection)
        {
            for (int i = 0; i < spikeList.Count; i++)
            {
                Matrix worldMatrix = Matrix.CreateScale(spikeList[i].Radius) * Matrix.CreateTranslation(spikeList[i].Center);

                Matrix[] spikeTransforms = new Matrix[spikeModel.Bones.Count];
                spikeModel.CopyAbsoluteBoneTransformsTo(spikeTransforms);
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = worldMatrix;
                        effect.View = view;
                        effect.Projection = projection;
                    }

                    mesh.Draw();
                }
            }
        }

        private void DrawShieldPower(Model model, Matrix view, Matrix projection)
        {
            for (int i = 0; i < shieldPowerList.Count; i++)
            {
                Matrix worldMatrix = Matrix.CreateScale(shieldPowerList[i].Radius) * Matrix.CreateTranslation(shieldPowerList[i].Center);

                Matrix[] shieldPowerTransforms = new Matrix[shieldPowerModel.Bones.Count];
                shieldPowerModel.CopyAbsoluteBoneTransformsTo(shieldPowerTransforms);
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = worldMatrix;
                        effect.View = view;
                        effect.Projection = projection;
                    }

                    mesh.Draw();
                }
            }

        }

        private void DrawBullets(Model model, Matrix view, Matrix projection)
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                Matrix worldMatrix = Matrix.CreateScale(0.01f) * Matrix.CreateTranslation(bulletList[i].position);

                Matrix[] targetTransforms = new Matrix[targetModel.Bones.Count];
                targetModel.CopyAbsoluteBoneTransformsTo(targetTransforms);
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = worldMatrix;
                        effect.View = view;
                        effect.Projection = projection;
                    }

                    mesh.Draw();
                }
            }
        }
    }
}
