using UnityEngine;
using UnityEngine.UI;

namespace UnitySide
{
    public class MessageBus : MonoBehaviour
    {
        public static MessageBus Instance { get; private set; }

        [SerializeField] Text text_result;
        [SerializeField] Text text_message;
        [SerializeField] Text text_mousePosition;
        [SerializeField] Text text_mouseButton;

        public int value = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                InputMessage("plus");
            }
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                InputMessage("reset");
            }

            text_mousePosition.text = Input.mousePosition.ToString();
            text_mouseButton.text = Input.GetMouseButton(0) ? "left"
                : Input.GetMouseButton(1) ? "right"
                : Input.GetMouseButton(2) ? "center"
                : "no mouse button";
        }

        public void InputMessage(string msg) => InputMessage(msg, out _);

        public void InputMessage(string msg, out string result)
        {
            switch (msg)
            {
                case "plus":
                    value++;
                    break;
                case "reset":
                    value = 0;
                    break;
            }
            result = $"{msg} => {value}";
            text_result.text = value.ToString();
            text_message.text = msg;
        }
    }
}
