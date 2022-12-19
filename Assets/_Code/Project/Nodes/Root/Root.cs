using Framework;
using UnityEngine;

namespace Project
{
	public class Root : MonoBehaviour
	{
		[SerializeField]
		private RootCfg rootCfg;
		
		[SerializeField]
		private RootView rootView;

		private NodeUpdater nodeUpdater;
		private SystemUpdater systemUpdater;

		private RootNode rootNode;
		
		private void Awake()
		{
			this.nodeUpdater = new NodeUpdater();
			this.systemUpdater = new SystemUpdater();

			this.rootNode = new RootNode(nodeUpdater, systemUpdater, rootCfg, rootView);
		}

		private void OnDestroy()
		{
			this.rootNode.Dispose();
			
			this.systemUpdater.Dispose();
			this.nodeUpdater.Dispose();
		}

		private void Update()
		{
			this.systemUpdater.Update(Time.deltaTime);
		}
		
		private void LateUpdate()
		{
			this.nodeUpdater.Update();
		}
		
	}
	
}
