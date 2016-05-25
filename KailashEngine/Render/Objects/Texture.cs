﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KailashEngine.Render.Objects
{
    class Texture
    {


        private int _id;
        public int id
        {
            get { return _id; }

        }


        private TextureTarget _target;
        public TextureTarget target
        {
            get { return _target; }
        }

        //------------------------------------------------------
        // Options
        //------------------------------------------------------
        private bool _enable_mipmap;
        public bool enable_mipmap
        {
            get { return _enable_mipmap; }
            set { _enable_mipmap = value; }
        }

        private int _max_mipmap_levels;

        private bool _enable_aniso;
        public bool enable_aniso
        {
            get { return _enable_aniso; }
            set { _enable_aniso = value; }
        }

        private float _max_anisotropy;


        //------------------------------------------------------
        // Pixel Settings
        //------------------------------------------------------

        private PixelInternalFormat _pif;
        public PixelInternalFormat pif
        {
            get { return _pif; }
        }

        private PixelFormat _pf;
        public PixelFormat pf
        {
            get { return _pf; }
        }

        private PixelType _pt;
        public PixelType pt
        {
            get { return _pt; }
        }


        //------------------------------------------------------
        // Sizing
        //------------------------------------------------------

        private int _width;
        public int width
        {
            get { return _width; }
        }

        private int _height;
        public int height
        {
            get { return _height; }
        }

        private int _depth;
        public int depth
        {
            get { return _depth; }
        }

        //------------------------------------------------------
        // Texture Parameters
        //------------------------------------------------------

        private TextureMinFilter _min_filter;
        public TextureMinFilter min_filter
        {
            get { return _min_filter; }
        }

        private TextureMagFilter _mag_filter;
        public TextureMagFilter mag_filter
        {
            get { return _mag_filter; }
        }

        private TextureWrapMode _wrap_mode;
        public TextureWrapMode wrap_mode
        {
            get { return _wrap_mode; }
        }


        //------------------------------------------------------
        // Constructor
        //------------------------------------------------------

        public Texture(TextureTarget target, 
            int width, int height, int depth,
            bool enable_mipmap, bool enable_aniso,
            PixelInternalFormat pif, PixelFormat pf, PixelType pt,
            TextureMinFilter min_filter, TextureMagFilter mag_filter, TextureWrapMode wrap_mode)
        {
            // Set texture configuration
            _target = target;

            _width = width;
            _height = height;
            _depth = depth;

            _enable_mipmap = enable_mipmap;
            _enable_aniso = enable_aniso;

            _pif = pif;
            _pf = pf;
            _pt = pt;

            _min_filter = min_filter;
            _mag_filter = mag_filter;
            _wrap_mode = wrap_mode;

            if(_enable_mipmap)
            {
                _max_mipmap_levels = getMaxMipMap(_width, _height);
                _min_filter = TextureMinFilter.LinearMipmapLinear;
            }

            if(_enable_aniso)
            {
                _max_anisotropy = GL.GetFloat((GetPName)All.MaxTextureMaxAnisotropyExt);
            }

            // Finally, Generate a texture object
            GL.GenTextures(1, out _id);

        }


        //------------------------------------------------------
        // Helpers
        //------------------------------------------------------

        public static int getMaxMipMap(int width, int height)
        {
            int largest_dimension = Math.Max(width, height);

            return (int)Math.Log(largest_dimension, 2) - 1;
        }

        public void generateMipMap()
        {
            GL.GenerateTextureMipmap(_id);
        }


        //------------------------------------------------------
        // Main Methods
        //------------------------------------------------------

        public void load(IntPtr data)
        {
            Debug.DebugHelper.logGLError();

            GL.BindTexture(_target, _id);
            GL.TexImage2D(
                _target,
                0,
                _pif,
                _width,
                _height,
                0,
                _pf,
                _pt,
                data);

            //GL.TexEnv(
            //    TextureEnvTarget.TextureEnv,
            //    TextureEnvParameter.TextureEnvMode,
            //    (float)TextureEnvMode.Modulate);
            GL.TexParameter(
                _target,
                TextureParameterName.TextureMinFilter,
                (float)_min_filter);
            GL.TexParameter(
                _target,
                TextureParameterName.TextureMagFilter,
                (float)_mag_filter);
            GL.TexParameter(
                _target,
                TextureParameterName.TextureWrapS,
                (float)_wrap_mode);
            GL.TexParameter(
                _target,
                TextureParameterName.TextureWrapT,
                (float)_wrap_mode);


            if (_enable_mipmap)
            {
                GL.TexParameter(_target, TextureParameterName.TextureMaxLevel, _max_mipmap_levels);
                generateMipMap();
            }

            if (_enable_aniso)
            {
                GL.TexParameter(_target, (TextureParameterName)All.TextureMaxAnisotropyExt, _max_anisotropy);
            }
        }

        public void bind(int texture_uniform, int index)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + index);
            GL.BindTexture(_target, _id);
            GL.Uniform1(texture_uniform, index);
        }

    }
}