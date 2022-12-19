using UnityEngine.EventSystems;

namespace Framework.Components.Handlers
{
	public interface IDragEventsHandler
	{
		void OnBeginDrag(PointerEventData eventData);
		void OnEndDrag(PointerEventData eventData);
		void OnDrag(PointerEventData eventData);
	}

	public class DragHandlerComp : HandlerComp<IDragEventsHandler>,
		IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			foreach (var handler in this.handlers)
				handler.OnBeginDrag(eventData);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			foreach (var handler in this.handlers)
				handler.OnEndDrag(eventData);
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			foreach (var handler in this.handlers)
				handler.OnDrag(eventData);
		}
	}
}
