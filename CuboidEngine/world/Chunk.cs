﻿using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace CuboidEngine
{
	internal class Chunk
	{
		public const int ChunkLength = 32;
		public const int ChunkLengthShift = 5;
		public const int DistMapLength = ChunkLength >> 1;
		public const int ChunkSize = ChunkLength * ChunkLength * ChunkLength;

		private Voxel[] _voxels;
		private byte[] _distMap0;
		private byte[] _distMap1;
		private byte[] _distMap2;
		private byte[] _distMap3;
		private byte[] _distMap4;

		public Chunk() {
			_voxels = new Voxel[ChunkSize]; //32x32x32
			_distMap0 = new byte[1]; //1x1x1
			_distMap1 = new byte[1]; //2x2x2
			_distMap2 = new byte[8]; //4x4x4
			_distMap3 = new byte[64]; //8x8x8
			_distMap4 = new byte[512]; //16x16x16
		}

		public Voxel this[ int x, int y, int z ] {
			get => _voxels[x + ChunkLength * y + ChunkLength * ChunkLength * z];
			set {
				if ( value.color != 0 ) {
					_distMap0[0] =  1;
					_distMap1[0] |= ( byte ) ( 1 << ( ( ( x >> 4 ) & 1 ) + ( ( ( y >> 4 ) & 1 ) << 1 ) + ( ( ( z >> 4 ) & 1 ) << 2 ) ) );
					_distMap2[( ( ( x >> 4 ) & 1 ) + ( ( y >> 3 ) & 2 ) + ( ( z >> 2 ) & 4 ) )] |= ( byte ) ( 1 << ( ( ( x >> 3 ) & 1 ) + ( ( ( y >> 3 ) & 1 ) << 1 ) + ( ( ( z >> 3 ) & 1 ) << 2 ) ) );
					_distMap3[( ( ( x >> 3 ) & 3 ) + ( ( y >> 1 ) & 12 ) + ( ( z << 1 ) & 48 ) )] |= ( byte ) ( 1 << ( ( ( x >> 2 ) & 1 ) + ( ( ( y >> 2 ) & 1 ) << 1 ) + ( ( ( z >> 2 ) & 1 ) << 2 ) ) );
					_distMap4[( ( ( x >> 2 ) & 7 ) + ( ( y << 1 ) & 56 ) + ( ( z << 4 ) & 448 ) )] |= ( byte ) ( 1 << ( ( ( x >> 1 ) & 1 ) + ( ( ( y >> 1 ) & 1 ) << 1 ) + ( ( ( z >> 1 ) & 1 ) << 2 ) ) );
				}

				/*
				if ( value.color == 0 ) {
					_distMap0[0] = 0;
					_distMap1[0] &= ( byte ) ~( 1 << ( ( ( x >> 4 ) & 1 ) + ( ( ( y >> 4 ) & 1 ) << 1 ) + ( ( ( z >> 4 ) & 1 ) << 2 ) ) );
					_distMap2[( ( ( x >> 4 ) & 1 ) + ( ( y >> 3 ) & 2 ) + ( ( z >> 2 ) & 4 ) )] &= ( byte ) ~( 1 << ( ( ( x >> 3 ) & 1 ) + ( ( ( y >> 3 ) & 1 ) << 1 ) + ( ( ( z >> 3 ) & 1 ) << 2 ) ) );
					_distMap3[( ( ( x >> 3 ) & 2 ) + ( ( y >> 1 ) & 8 ) + ( ( z << 1 ) & 32 ) )] &= ( byte ) ~( 1 << ( ( ( x >> 2 ) & 1 ) + ( ( ( y >> 2 ) & 1 ) << 1 ) + ( ( ( z >> 2 ) & 1 ) << 2 ) ) );
					_distMap4[( ( ( x >> 2 ) & 4 ) + ( ( y << 1 ) & 32 ) + ( ( z << 4 ) & 128 ) )] &= ( byte ) ~( 1 << ( ( ( x >> 1 ) & 1 ) + ( ( ( y >> 1 ) & 1 ) << 1 ) + ( ( ( z >> 1 ) & 1 ) << 2 ) ) );
				}*/

				_voxels[x + ChunkLength * y + ChunkLength * ChunkLength * z] = value;
			}
		}

		public bool IsEmpty( int x, int y, int z, int lvl ) {
			return lvl switch {
				0 => ( _distMap0[0] & 1 ) == 0,
				1 => ( _distMap1[0] & ( 1 << ( ( ( x >> 4 ) & 1 ) + ( ( ( y >> 4 ) & 1 ) << 1 ) + ( ( ( z >> 4 ) & 1 ) << 2 ) ) ) ) == 0,
				2 => ( _distMap2[( ( ( x >> 4 ) & 1 ) + ( ( y >> 3 ) & 2 ) + ( ( z >> 2 ) & 4 ) )] & ( byte ) ( 1 << ( ( ( x >> 3 ) & 1 ) + ( ( ( y >> 3 ) & 1 ) << 1 ) + ( ( ( z >> 3 ) & 1 ) << 2 ) ) ) ) == 0,
				3 => ( _distMap3[( ( ( x >> 3 ) & 3 ) + ( ( y >> 1 ) & 12 ) + ( ( z << 1 ) & 48 ) )] & ( byte ) ( 1 << ( ( ( x >> 2 ) & 1 ) + ( ( ( y >> 2 ) & 1 ) << 1 ) + ( ( ( z >> 2 ) & 1 ) << 2 ) ) ) ) == 0,
				4 => ( _distMap4[( ( ( x >> 2 ) & 7 ) + ( ( y << 1 ) & 56 ) + ( ( z << 4 ) & 448 ) )] & ( byte ) ( 1 << ( ( ( x >> 1 ) & 1 ) + ( ( ( y >> 1 ) & 1 ) << 1 ) + ( ( ( z >> 1 ) & 1 ) << 2 ) ) ) ) == 0,
				5 => _voxels[x + ( y << ChunkLengthShift ) + ( z << ( ChunkLengthShift + ChunkLengthShift ) )].color == 0,
				_ => false
			};
		}

		public Voxel GetVoxel( int x, int y, int z ) { //inline
			return _voxels[x + Chunk.ChunkLength * y + Chunk.ChunkLength * Chunk.ChunkLength * z];
		}

		public void SetVoxel( int x, int y, int z, byte color ) { //inline
			this[x, y, z] = new Voxel( color );
		}
	}
}