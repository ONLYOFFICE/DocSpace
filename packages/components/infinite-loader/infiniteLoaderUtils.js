let timer = null;

const startInterval = () => {
  const elem = document.getElementById("infinite-page-loader");
  elem.style.display = "block";
};

export function showLoader() {
  hideLoader();
  timer = setTimeout(() => startInterval(), 300);
}

export function hideLoader() {
  if (timer) {
    clearTimeout(timer);
    timer = null;
  }

  const elem = document.getElementById("infinite-page-loader");
  if (elem) elem.style.display = "none";
}
