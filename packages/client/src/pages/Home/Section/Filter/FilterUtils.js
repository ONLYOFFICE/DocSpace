let timer = null;

const startInterval = () => {
  const elem = document.getElementById("filter-loader");
  if (elem) elem.style.display = "grid";
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

  const elem = document.getElementById("filter-loader");
  if (elem) elem.style.display = "none";
}
