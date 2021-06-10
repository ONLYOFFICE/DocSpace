import "./snackbar.css";

export const Snackbar = {
  current: null,
  snackbar: null,
  show(params) {
    let options = extend(true, defaultConfig, params);

    if (Snackbar.current) {
      Snackbar.current.style.opacity = 0;
      setTimeout(
        function () {
          let parent = this.parentElement;
          if (parent)
            // possible null if too many/fast snackbars
            parent.removeChild(this);
        }.bind(Snackbar.current),
        500
      );
    }

    Snackbar.snackbar = document.createElement("div");
    Snackbar.snackbar.className = "snackbar-container " + options.customClass;
    Snackbar.snackbar.style.width = options.width;

    let textContainer = document.createElement("div");
    textContainer.className = "text-container";
    Snackbar.snackbar.appendChild(textContainer);

    let header = document.createElement("p");
    header.style.margin = 0;
    header.style.padding = 0;
    header.style.color = options.textColor;
    header.style.fontSize = "13px";
    header.style.fontWeight = 800;
    header.style.lineHeight = "1em";
    header.innerHTML = options.textHeader;
    textContainer.appendChild(header);

    let body = document.createElement("p");
    body.style.margin = 0;
    body.style.padding = 0;
    body.style.color = options.textColor;
    body.style.fontSize = "12px";
    body.style.lineHeight = "1em";
    body.innerHTML = options.textBody;
    textContainer.appendChild(body);

    Snackbar.snackbar.appendChild(textContainer);
    Snackbar.snackbar.style.background = options.backgroundColor;

    if (options.showSecondButton) {
      let secondButton = document.createElement("button");
      secondButton.className = "action";
      secondButton.innerHTML = options.secondButtonText;
      secondButton.style.color = options.secondButtonTextColor;
      secondButton.addEventListener("click", function () {
        options.onSecondButtonClick(Snackbar.snackbar);
      });
      Snackbar.snackbar.appendChild(secondButton);
    }

    if (options.showAction) {
      let actionButton = document.createElement("button");
      actionButton.className = "action";
      actionButton.innerHTML = options.actionText;
      actionButton.style.color = options.actionTextColor;
      actionButton.addEventListener("click", function () {
        options.onActionClick(Snackbar.snackbar);
      });
      Snackbar.snackbar.appendChild(actionButton);
    }

    if (options.duration) {
      setTimeout(
        function () {
          if (Snackbar.current === this) {
            Snackbar.current.style.opacity = 0;
            // When natural remove event occurs let's move the snackbar to its origins
            Snackbar.current.style.top = "-100px";
            Snackbar.current.style.bottom = "-100px";
          }
        }.bind(Snackbar.snackbar),
        options.duration
      );
    }

    Snackbar.snackbar.addEventListener(
      "transitionend",
      function (event, elapsed) {
        if (event.propertyName === "opacity" && this.style.opacity === "0") {
          if (typeof options.onClose === "function") options.onClose(this);

          this.parentElement.removeChild(this);
          if (Snackbar.current === this) {
            Snackbar.current = null;
          }
        }
      }.bind(Snackbar.snackbar)
    );

    Snackbar.current = Snackbar.snackbar;

    document.body.appendChild(Snackbar.snackbar);
    let bottom = getComputedStyle(Snackbar.snackbar).bottom;
    let top = getComputedStyle(Snackbar.snackbar).top;
    Snackbar.snackbar.style.opacity = 1;
    Snackbar.snackbar.className =
      "snackbar-container " +
      options.customClass +
      " snackbar-pos " +
      options.position;
  },
  close() {
    if (Snackbar.current) {
      Snackbar.current.style.opacity = 0;
    }
  },
};

const defaultConfig = {
  backgroundColor: "#F1DA92",
  textColor: "#000",
  width: "auto",
  textHeader: "Default Header text",
  textBody: "Default Body text",

  showAction: false,
  actionText: "Close",
  actionTextColor: "#0F4071",

  showSecondButton: true,
  secondButtonText: "Reload",
  secondButtonTextColor: "#0F4071",

  position: "bottom-center",
  duration: null,
  customClass: "",

  onActionClick: function (element) {
    element.style.opacity = 0;
  },
  onSecondButtonClick: function (element) {
    location.reload();
  },
  onClose: function (element) {},
};

const extend = function () {
  let extended = {};
  let deep = false;
  let i = 0;
  let length = arguments.length;

  if (Object.prototype.toString.call(arguments[0]) === "[object Boolean]") {
    deep = arguments[0];
    i++;
  }

  const merge = function (obj) {
    for (let prop in obj) {
      if (Object.prototype.hasOwnProperty.call(obj, prop)) {
        if (
          deep &&
          Object.prototype.toString.call(obj[prop]) === "[object Object]"
        ) {
          extended[prop] = extend(true, extended[prop], obj[prop]);
        } else {
          extended[prop] = obj[prop];
        }
      }
    }
  };

  for (; i < length; i++) {
    let obj = arguments[i];
    merge(obj);
  }

  return extended;
};
