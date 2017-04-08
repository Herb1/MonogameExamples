namespace Effects
{
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region member vars

        private SpriteBatch _spriteBatch;

        private Vector3 _lookAt;

        private Model _model;
        private Matrix _view;
        private Matrix _projection;

        private Texture2D _texturePandaOriginal;
        private Texture2D _texturePandaZombie;

        private float _angle;
        private float _scanHeight;

        private Matrix _world;

        private Effect _effect;
        private GraphicsDeviceManager _graphics;

        #endregion

        #region constructors and destructors

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.Title = "Effects";
        }

        #endregion

        #region methods

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /*foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = _world * mesh.ParentBone.Transform * Matrix.CreateTranslation(new Vector3(0, -1.5f, 0));
                    effect.View = _view;
                    effect.Projection = _projection;
                }
                mesh.Draw();
            }*/

            DrawModelWithEffect(_model, _world, _view, _projection, _effect);
            base.Draw(gameTime);
        }

        protected override void Initialize()
        {
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);

            _view = Matrix.CreateLookAt(new Vector3(0, 1, 10), new Vector3(0, -100, 0), new Vector3(0, 1, 0));

            _world = Matrix.CreateScale(0.02f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _model = Content.Load<Model>("panda");
            _effect = Content.Load<Effect>("Effects/cheddar");

            _texturePandaOriginal = Content.Load<Texture2D>("Textures/panda_original");
            _texturePandaZombie = Content.Load<Texture2D>("Textures/panda_zombie");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _scanHeight = ((float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / 1000) + 1) / 2f;

            _angle += 0.001f * gameTime.ElapsedGameTime.Milliseconds;
            _lookAt = new Vector3((float)Math.Sin(_angle), 0, (float)Math.Cos(_angle));

            _view = Matrix.CreateLookAt(_lookAt * 10, new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            base.Update(gameTime);
        }

        private void DrawModelWithEffect(Model model, Matrix world, Matrix view, Matrix projection, Effect myEffect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = myEffect;
                    myEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform * Matrix.CreateTranslation(new Vector3(0, -1.5f, 0)));
                    myEffect.Parameters["View"].SetValue(view);
                    myEffect.Parameters["Projection"].SetValue(projection);

                    //myEffect.Parameters["AmbientColor"].SetValue(new Vector4(1, 1, 0, 1));
                    myEffect.Parameters["ModelTextureA"].SetValue(_texturePandaOriginal);
                    myEffect.Parameters["ModelTextureB"].SetValue(_texturePandaZombie);

                    myEffect.Parameters["scanHeight"].SetValue(_scanHeight);
                }
                mesh.Draw();
            }
        }

        #endregion
    }
}