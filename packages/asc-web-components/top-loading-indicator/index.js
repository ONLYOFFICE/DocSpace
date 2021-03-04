const intervalTimeout = 10;

const MAX = 100;
let timerId;
let width = 0;
let percentage = 0;
let increasePercentage = 0.75;
let moreIncreasePercentage = 3;

const elem = document.getElementById("ipl-progress-indicator");

const startInterval = () => {
  if (!timerId) {
    timerId = setInterval(animatingWidth, intervalTimeout);
  }
};

const animatingWidth = () => {
  if (width >= MAX) {
    clearTimeout(timerId);
    timerId = null;
    elem.style.width = 0;
    width = 0;
    return;
  }

  width += percentage !== MAX ? increasePercentage : moreIncreasePercentage;
  elem.style.width = width + "%";
};

export default class TopLoaderService {
  static start() {
    percentage = 0;
    startInterval();
  }

  static end() {
    percentage = MAX;
  }
}
