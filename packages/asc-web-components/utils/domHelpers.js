export default class DomHelpers {
  static getViewport() {
    let win = window,
      d = document,
      e = d.documentElement,
      g = d.getElementsByTagName("body")[0],
      w = win.innerWidth || e.clientWidth || g.clientWidth,
      h = win.innerHeight || e.clientHeight || g.clientHeight;

    return { width: w, height: h };
  }

  static getOffset(el) {
    if (el) {
      let rect = el.getBoundingClientRect();

      return {
        top:
          rect.top +
          (window.pageYOffset ||
            document.documentElement.scrollTop ||
            document.body.scrollTop ||
            0),
        left:
          rect.left +
          (window.pageXOffset ||
            document.documentElement.scrollLeft ||
            document.body.scrollLeft ||
            0),
      };
    }

    return {
      top: "auto",
      left: "auto",
    };
  }

  static getOuterWidth(el, margin) {
    if (el) {
      let width = el.offsetWidth;

      if (margin) {
        let style = getComputedStyle(el);
        width += parseFloat(style.marginLeft) + parseFloat(style.marginRight);
      }

      return width;
    }
    return 0;
  }

  static getHiddenElementOuterWidth(element) {
    if (element) {
      element.style.visibility = "hidden";
      element.style.display = "block";
      let elementWidth = element.offsetWidth;
      element.style.display = "none";
      element.style.visibility = "visible";

      return elementWidth;
    }
    return 0;
  }

  static getHiddenElementOuterHeight(element) {
    if (element) {
      element.style.visibility = "hidden";
      element.style.display = "block";
      let elementHeight = element.offsetHeight;
      element.style.display = "none";
      element.style.visibility = "visible";

      return elementHeight;
    }
    return 0;
  }

  static calculateScrollbarWidth(el) {
    if (el) {
      let style = getComputedStyle(el);
      return (
        el.offsetWidth -
        el.clientWidth -
        parseFloat(style.borderLeftWidth) -
        parseFloat(style.borderRightWidth)
      );
    } else {
      if (this.calculatedScrollbarWidth != null)
        return this.calculatedScrollbarWidth;

      let scrollDiv = document.createElement("div");
      scrollDiv.className = "p-scrollbar-measure";
      document.body.appendChild(scrollDiv);

      let scrollbarWidth = scrollDiv.offsetWidth - scrollDiv.clientWidth;
      document.body.removeChild(scrollDiv);

      this.calculatedScrollbarWidth = scrollbarWidth;

      return scrollbarWidth;
    }
  }

  static generateZIndex() {
    this.zIndex = this.zIndex || 1000;
    return ++this.zIndex;
  }

  static revertZIndex() {
    this.zIndex = 1000 < this.zIndex ? --this.zIndex : 1000;
  }

  static getCurrentZIndex() {
    return this.zIndex;
  }
}
