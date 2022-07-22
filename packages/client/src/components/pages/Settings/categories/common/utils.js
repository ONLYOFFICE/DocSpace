const checkScrollSettingsBlock = () => {
  let initHeight = 0;
  let initHeightScroll = 0;

  const settingsDiv = document.getElementsByClassName("settings-block")?.[0];
  const scrollBody = settingsDiv?.closest(".scroll-body");

  const height = parseInt(
    !!settingsDiv ? getComputedStyle(settingsDiv).height.slice(0, -2) : 0,
    0
  );

  const heightScroll = parseInt(
    !!scrollBody ? getComputedStyle(scrollBody).height.slice(0, -2) : 0,
    0
  );

  return () => {
    if (height === initHeight && heightScroll !== initHeightScroll) {
      return;
    }

    if (height !== initHeight) {
      initHeight = height;
    }
    if (heightScroll !== initHeightScroll) {
      initHeightScroll = heightScroll;
    }

    if (height > heightScroll) {
      return true;
    } else {
      return false;
    }
  };
};

export default checkScrollSettingsBlock;
