import difference from "lodash/difference";
import { LANGUAGE } from "@docspace/common/constants";
import { getLanguage, getCookie } from "@docspace/common/utils";
export const getBannerAttribute = () => {
  const bar = document.getElementById("bar-banner");
  const mainBar = document.getElementById("main-bar");
  const rects = mainBar ? mainBar.getBoundingClientRect() : null;

  const headerHeight = bar ? 108 + 50 : mainBar ? rects.height + 40 : 48 + 50;
  const sectionHeaderTop = bar
    ? "108px"
    : rects
    ? rects.height + 40 + "px"
    : "48px";
  const sectionHeaderMarginTop = bar
    ? "106px"
    : rects
    ? rects.height + 36 + "px"
    : "46px";

  const loadLanguagePath = async () => {
    if (!window.firebaseHelper) return;

    const lng = getCookie(LANGUAGE) || "en";
    const language = getLanguage(lng instanceof Array ? lng[0] : lng);

    const bar = (localStorage.getItem("bar") || "")
      .split(",")
      .filter((bar) => bar.length > 0);

    const closed = JSON.parse(localStorage.getItem("barClose"));

    const banner = difference(bar, closed);

    let index = Number(localStorage.getItem("barIndex") || 0);
    if (index >= banner.length) {
      index -= 1;
    }
    const currentBar = banner[index];

    let htmlUrl =
      currentBar && window.firebaseHelper.config.authDomain
        ? `https://${window.firebaseHelper.config.authDomain}/${language}/${currentBar}/index.html`
        : null;

    if (htmlUrl) {
      await fetch(htmlUrl).then((data) => {
        if (data.ok) return;
        htmlUrl = null;
      });
    }

    return [htmlUrl, currentBar];
  };

  return {
    headerHeight,
    sectionHeaderTop,
    sectionHeaderMarginTop,
    loadLanguagePath,
  };
};
