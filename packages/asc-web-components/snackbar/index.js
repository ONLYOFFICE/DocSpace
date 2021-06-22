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
    Snackbar.snackbar.className = `snackbar-container ${
      options.parentElementId && "inline"
    } ${options.customClass}`;
    Snackbar.snackbar.style.width = options.width;

    if (options.logoImg) {
      let logoImg = document.createElement("div");
      logoImg.src = options.logoImg;
      logoImg.className = "logo";
      logoImg.alt = "logo";
      logoImg.innerHTML = `<svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path fill-rule="evenodd" clip-rule="evenodd" d="M8 15C11.866 15 15 11.866 15 8C15 4.13401 11.866 1 8 1C4.13401 1 1 4.13401 1 8C1 11.866 4.13401 15 8 15ZM7 6V4H9V6H7ZM7 12V7H9V12H7Z" fill="black"/>
      </svg>`;
      Snackbar.snackbar.appendChild(logoImg);
    }

    let textContainer = document.createElement("div");
    textContainer.className = "text-container";
    Snackbar.snackbar.appendChild(textContainer);

    if (options.textHeader) {
      let header = document.createElement("p");
      header.style.margin = 0;
      header.style.padding = 0;
      header.style.color = options.textColor;
      header.style.fontSize = "12px";
      header.style.fontWeight = 600;
      header.style.lineHeight = "16px";
      header.innerHTML = options.textHeader;
      textContainer.appendChild(header);
    }

    let body = document.createElement("p");
    body.style.margin = 0;
    body.style.padding = 0;
    body.style.color = options.textColor;
    body.style.fontSize = "12px";
    body.style.lineHeight = "16px";
    body.innerHTML = options.textBody;
    textContainer.appendChild(body);

    Snackbar.snackbar.appendChild(textContainer);
    Snackbar.snackbar.style.background = options.backgroundColor;

    if (options.showSecondButton) {
      let secondButton = document.createElement("button");
      secondButton.className = "action";
      secondButton.innerHTML = options.secondButtonText;
      secondButton.style.color = options.secondButtonTextColor;
      secondButton.addEventListener("click", () => {
        options.onSecondButtonClick(Snackbar.snackbar);
      });
      Snackbar.snackbar.appendChild(secondButton);
    }

    if (options.showAction) {
      let actionButton = document.createElement("button");
      actionButton.className = "action";

      if (options.actionIcon) {
        actionButton.innerHTML = `<svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path fill-rule="evenodd" clip-rule="evenodd" d="M7.76288 6.35869C7.56764 6.16343 7.56764 5.84687 7.76288 5.65161L10.9493 2.46498C11.1445 2.26973 11.1445 1.95316 10.9493 1.75791L10.2422 1.05077C10.0469 0.855489 9.73031 0.855489 9.53504 1.05077L6.34878 4.23729C6.15352 4.43257 5.83691 4.43257 5.64165 4.23729L2.46017 1.05556C2.26491 0.860275 1.9483 0.860275 1.75304 1.05556L1.04596 1.76269C0.850716 1.95795 0.850716 2.27451 1.04596 2.46977L4.22755 5.65161C4.42279 5.84687 4.42279 6.16343 4.22755 6.35869L1.0501 9.53639C0.854858 9.73165 0.854858 10.0482 1.0501 10.2435L1.75718 10.9506C1.95245 11.1459 2.26905 11.1459 2.46432 10.9506L5.64165 7.77302C5.83691 7.57774 6.15352 7.57774 6.34878 7.77302L9.5309 10.9554C9.72616 11.1507 10.0428 11.1507 10.238 10.9554L10.9451 10.2483C11.1404 10.053 11.1404 9.73644 10.9451 9.54118L7.76288 6.35869Z" fill="#999976"/>
        </svg>`; //`<img src="${options.actionIcon}" alt="${options.actionText}" class="action-icon">`;
      } else {
        actionButton.innerHTML = options.actionText;
        actionButton.style.color = options.actionTextColor;
      }

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
      function (event) {
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

    var parentElm =
      options.parentElementId &&
      document.getElementById(options.parentElementId);

    if (parentElm) {
      parentElm.appendChild(Snackbar.snackbar);
    } else {
      document.body.appendChild(Snackbar.snackbar);
    }

    //let bottom = getComputedStyle(Snackbar.snackbar).bottom;
    //let top = getComputedStyle(Snackbar.snackbar).top;
    Snackbar.snackbar.style.opacity = 1;

    Snackbar.snackbar.className = `snackbar-container ${
      options.parentElementId && "inline"
    } ${options.customClass} ${
      !options.parentElementId && ` snackbar-pos ${options.position}`
    }`;
  },
  close() {
    if (Snackbar.current) {
      Snackbar.current.style.opacity = 0;
    }
  },
};

const defaultConfig = {
  parentElementId: null,
  backgroundColor: "#F8F7BF",
  textColor: "#000",
  width: "auto",
  textHeader: null,
  textBody: "Default Body text",
  logoImg: "/static/images/info.react.svg",

  showAction: false,
  actionIcon: "/static/images/cross.react.svg",
  actionText: "Close",
  actionTextColor: "#0F4071",

  showSecondButton: false,
  secondButtonText: "Reload",
  secondButtonTextColor: "#0F4071",

  position: "bottom-center",
  duration: null,
  customClass: "",

  onActionClick: (element) => {
    element.style.opacity = 0;
  },
  onSecondButtonClick: () => {
    location.reload();
  },
  onClose: () => {},
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
