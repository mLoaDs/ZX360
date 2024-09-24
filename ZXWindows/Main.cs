
#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Core;
#endregion

namespace ZXWindows
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager   graphics;
        ContentManager          content;

        Texture2D               targetTexture;
        SpriteBatch             spriteBatch;

        Spectrum                spectrum = new Spectrum();

        private bool            running;
        private string          currentRom;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);
        }

        public void LoadRom(string filename)
        {
            if (running)
            {
                running = false;
                //myEngine.SaveRam();
            }

            spectrum.LoadROM("roms/spectrum.rom");

#if XBOX
            using (StorageContainer container = saveDevice.OpenContainer("ZX360"))
            {
                //myEngine.LoadRam();
                SaveRomName(filename, container.Path);
            }
#else
            //myEngine.LoadRam();
            SaveRomName(filename, StorageContainer.TitleLocation);
#endif

            spectrum.reset();
            //myEngine.StartCart();

            //spectrum.LoadSNA(filename);
            spectrum.LoadZ80("roms/manic.z80");

            currentRom = Path.GetFileName(filename);
            running = true;
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

            base.Initialize();
            Sound.Initialize();

            
        }

        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                // TODO: Load any ResourceManagementMode.Automatic content
            }

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            targetTexture = new Texture2D(graphics.GraphicsDevice, 256, 192, 1, ResourceUsage.None, 
                SurfaceFormat.Bgr32, ResourceManagementMode.Manual);
        }

        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent == true)
            {
                content.Unload();
            }
        }

        protected void SaveRomName(string rom, string savePath)
        {
            using (StreamWriter writer = File.CreateText(Path.Combine(savePath, "LastRom.txt")))
            {
                writer.Write(rom);
            }
        }

        protected string LoadRomName(string savePath)
        {
            string filename = Path.Combine(savePath, "LastRom.txt");
            string result = String.Empty;

            if (File.Exists(filename))
            {
                using (StreamReader reader = File.OpenText(filename))
                {
                    result = reader.ReadLine();
                }
            }

            if (result.Length == 0 || !File.Exists(result))
            {
                string[] roms = Directory.GetFiles(StorageContainer.TitleLocation + @"\Roms", "*.sna");
                if (roms.Length == 0)
                    throw new Exception("No roms found.");
                Array.Sort(roms);
                result = roms[0];
            }

            return result;
        }

        void Quit()
        {
#if XBOX
            using (StorageContainer container = saveDevice.OpenContainer("ZX360"))
            {
                //myEngine.SaveRamDirectory = container.Path;
                //myEngine.StopCart(); // Writes SaveRam
            }
#else
            //myEngine.SaveRamDirectory = StorageContainer.TitleLocation;
            //myEngine.StopCart(); // Writes SaveRam
#endif
            this.Exit();
        }

        void LoadNextRom()
        {
            string[] roms = Directory.GetFiles(StorageContainer.TitleLocation + @"\Roms", "*.sna");
            if (roms.Length == 0)
                throw new Exception("No roms found.");
            Array.Sort(roms);
            int count;
            for (count = 0; count < roms.Length; count++)
            {
                if (Path.GetFileName(roms[count]) == currentRom)
                {
                    if (count == roms.Length - 1)
                        count = -1;
                    break;
                }
            }
            LoadRom(roms[count + 1]);
        }

        void LoadPreviousRom()
        {
            string[] roms = Directory.GetFiles(StorageContainer.TitleLocation + @"\Roms", "*.sna");
            if (roms.Length == 0)
                throw new Exception("No roms found.");
            Array.Sort(roms);
            int count;
            for (count = 0; count < roms.Length; count++)
            {
                if (Path.GetFileName(roms[count]) == currentRom)
                {
                    if (count == 0)
                        count = roms.Length;
                    break;
                }
            }
            LoadRom(roms[count - 1]);
        }

#if XBOX
        StorageDevice saveDevice;
        bool storageDeviceRequested;
        IAsyncResult storageDeviceRequestResult;
#endif

        const float thumbstickThreshold = .5F;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GamePadState pad = GamePad.GetState(PlayerIndex.One);

            if (pad.Buttons.RightShoulder == ButtonState.Pressed)
            {
                if (pad.Buttons.Back == ButtonState.Pressed)
                    Quit();

                if (pad.DPad.Left == ButtonState.Pressed || pad.DPad.Up == ButtonState.Pressed)
                    LoadPreviousRom();
                if (pad.DPad.Right == ButtonState.Pressed || pad.DPad.Down == ButtonState.Pressed)
                    LoadNextRom();
            }

            if (pad.Buttons.A == ButtonState.Pressed)
            {
                spectrum.KSYMB(true);
                spectrum.KCAPS(true);
            }
            else
            {
                spectrum.KSYMB(false);
                spectrum.KCAPS(false);
            }

            if (pad.Buttons.X == ButtonState.Pressed)
            {
                spectrum.KSYMB(true);
                spectrum.KZ(true);
            }
            else
            {
                spectrum.KZ(false);
            }

            if (pad.Buttons.B == ButtonState.Pressed)
                spectrum.KENT(true);
            else
                spectrum.KENT(false);

            if (pad.Triggers.Right > thumbstickThreshold)
                spectrum.Fire(true);
            else
                spectrum.Fire(false);

            if (pad.ThumbSticks.Left.Y > thumbstickThreshold)
                spectrum.Up(true);
            else
                spectrum.Up(false);

            if (pad.ThumbSticks.Left.Y < -thumbstickThreshold)
                spectrum.Down(true);
            else
                spectrum.Down(false);

            if (pad.ThumbSticks.Left.X < -thumbstickThreshold)
                spectrum.Left(true);
            else
                spectrum.Left(false);

            if (pad.ThumbSticks.Left.X > thumbstickThreshold)
                spectrum.Right(true);
            else
                spectrum.Right(false);

            if (pad.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                if (pad.Buttons.A == ButtonState.Pressed)
                    spectrum.K0(true);
                else
                    spectrum.K0(false);

                if (pad.Buttons.X == ButtonState.Pressed)
                    spectrum.K1(true);
                else
                    spectrum.K1(false);

                if (pad.Buttons.Y == ButtonState.Pressed)
                    spectrum.K2(true);
                else
                    spectrum.K2(false);

                if (pad.Buttons.B == ButtonState.Pressed)
                    spectrum.K3(true);
                else
                    spectrum.K3(false);

                if (pad.DPad.Down == ButtonState.Pressed)
                    spectrum.K4(true);
                else
                    spectrum.K4(false);

                if (pad.DPad.Left == ButtonState.Pressed)
                    spectrum.K5(true);
                else
                    spectrum.K5(false);

                if (pad.DPad.Up == ButtonState.Pressed)
                    spectrum.K6(true);
                else
                    spectrum.K6(false);

                if (pad.DPad.Right == ButtonState.Pressed)
                    spectrum.K7(true);
                else
                    spectrum.K7(false);
            }

            if (running)
            {
                spectrum.execute();
            }
            else
            {
#if XBOX
                if (saveDevice == null)
                {
                    if (!storageDeviceRequested)
                    {
                        storageDeviceRequested = true;
                        storageDeviceRequestResult = StorageDevice.BeginShowStorageDeviceGuide(PlayerIndex.One, null, null);
                    }

                    if ((storageDeviceRequested) && (storageDeviceRequestResult.IsCompleted))
                    {
                        saveDevice = StorageDevice.EndShowStorageDeviceGuide(storageDeviceRequestResult);

                        string filename;
                        using (StorageContainer container = saveDevice.OpenContainer("ZX360"))
                        {
                            filename = LoadRomName(container.Path);
                        }

                        LoadCart(filename);
                    }
                }
#else
                LoadRom(LoadRomName(StorageContainer.TitleLocation));
#endif
            }

            base.Update(gameTime);

            if (spectrum.Sound > 0)
            {
                //System.Diagnostics.Debug.WriteLine(spectrum.Sound.ToString());

                //float pitch = ((float)spectrum.Sound / 256F) * 4F;

                Sound.Play("SpectrumBeep", 1);
            }

            Sound.Update();
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            if (running)
            {
                targetTexture.SetData<uint>(spectrum.ScreenBuffer, 0,
                  targetTexture.Width * targetTexture.Height, SetDataOptions.None);

                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                spriteBatch.Draw(targetTexture, new Rectangle(0, 0, 
                    graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                spriteBatch.End();

            }

            base.Draw(gameTime);
        }
    }
}