using UnityEngine;
using UnityEngine.UI;

namespace Framework.Components
{
	[AddComponentMenu("Framework/UI/UI Regular Poly")]
	[RequireComponent(typeof(CanvasRenderer))]
	public class UIRegularPoly : MaskableGraphic
	{
		[SerializeField] Color outerColor = Color.white;
		public Color OuterColor
		{
			get => this.outerColor;
			set
			{
				if (this.outerColor == value)
					return;
				this.outerColor = value;
				SetVerticesDirty();
			}
		}
		
		[SerializeField] Rect uvRect1 = new Rect(0f, 0f, 1f, 1f);
		public Rect UVRect
		{
			get => this.uvRect1;
			set
			{
				if (this.uvRect1 == value)
					return;
				this.uvRect1 = value;
				SetVerticesDirty();
			}
		}
		
		[SerializeField] bool radialUV = true;
		public bool RadialUV
		{
			get => this.radialUV;
			set
			{
				if (this.radialUV == value)
					return;
				this.radialUV = value;
				SetVerticesDirty();
			}
		}

		private const int MIN_SEGMENTS = 3;
		[SerializeField] int segments = 20;
		public int Segments
		{
			get
			{
				if (this.segments < MIN_SEGMENTS)
					this.segments = MIN_SEGMENTS;
				
				return this.segments;
			}
			set
			{
				int segments = (value < MIN_SEGMENTS) ? MIN_SEGMENTS : value;
				
				if (this.segments == segments)
					return;
				
				this.segments = value;
				SetVerticesDirty();
			}
		}
		
		[SerializeField] int angle = 0;
		public int Angle
		{
			get => this.angle;
			set
			{
				if (this.angle == value)
					return;
				
				this.angle = value;
				SetVerticesDirty();
			}
		}
		
		
		public override Texture mainTexture
		{
			get
			{
				if (material != null && material.mainTexture != null)
					return material.mainTexture;

				return s_WhiteTexture;
			}
		}

		protected UIRegularPoly()
		{
			useLegacyMeshGeneration = false;
		}
		
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			var radialUV = this.radialUV;

			var uvRect1 = this.uvRect1;
			
			float uvXmid = (uvRect1.xMax + uvRect1.xMin)*0.5f;
			float uvXext = (uvRect1.xMax - uvRect1.xMin)*0.5f;
			
			float uvYmid = (uvRect1.yMax + uvRect1.yMin)*0.5f;
			float uvYext = (uvRect1.yMax - uvRect1.yMin)*0.5f;
			
			Color32 innerColor = this.color;
			Color32 outerColor = this.outerColor;

			var rect = rectTransform.rect;
			float rx = rect.width * 0.5f;
			float ry = rect.height * 0.5f;
			float cx = rect.xMin + rx;
			float cy = rect.yMin + ry;
			
			int segCount = this.Segments;
			
			float alfa = Mathf.PI * 2f / (float)segCount;
			
			var sinA = Mathf.Sin(alfa);
			var cosA = Mathf.Cos(alfa);

			int angle = this.angle;
			float vx = 1f;
			float vy = 0f;

			if (angle != 0)
			{
				float beta = Mathf.PI * ((float)angle) / 180f;
				vx = Mathf.Cos(beta);
				vy = Mathf.Sin(beta);
			}
			
			var innerUV = radialUV ? new Vector2(uvXmid - uvXext, uvYmid) : new Vector2(uvXmid, uvYmid);
			var outerUV = radialUV ? new Vector2(uvXmid + uvXext, uvYmid) : new Vector2(vx * uvXext + uvXmid, vy * uvYext + uvYmid);
			
			vh.AddVert(new Vector3(cx, cy, 0f), innerColor, innerUV);
			vh.AddVert(new Vector3(cx + (vx * rx), cy  + (vy * ry), 0f), outerColor, outerUV);

			for (int k = 2; k <= segCount; ++k)
			{
				float x = (vx * cosA) - (vy * sinA);
				float y = (vx * sinA) + (vy * cosA);

				vx = x;
				vy = y;

				if (!radialUV)
				{
					outerUV.x = vx * uvXext + uvXmid;
					outerUV.y = vy * uvYext + uvYmid;
				}

				vh.AddVert(new Vector3(cx + (vx * rx), cy + (vy * ry), 0f), outerColor, outerUV);
				vh.AddTriangle(0, k - 1, k);
			}
			
			vh.AddTriangle(0, segCount, 1);

		}

		public override bool Raycast(Vector2 sp, Camera eventCamera)
		{
			if (base.Raycast(sp, eventCamera))
			{
				Vector2 localPoint;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
					rectTransform, sp, eventCamera,
					out localPoint))
				{
					var rect = rectTransform.rect;
					float rx = rect.width * 0.5f;
					float ry = rect.height * 0.5f;
					float cx = rect.xMin + rx;
					float cy = rect.yMin + ry;

					float dx = (localPoint.x - cx)/rx;
					float dy = (localPoint.y - cy)/ry;

					return ((dx * dx + dy * dy) <= 1f);
				}
			}

			return false;
		}

	}
}
