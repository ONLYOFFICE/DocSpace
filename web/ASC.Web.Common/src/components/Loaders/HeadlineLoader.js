import React from "react";
import ContentLoader from "react-content-loader";

const HeadlineLoader = () => (
  <ContentLoader
    speed={2}
    width={264}
    height={56}
    viewBox="0 0 264 56"
    backgroundColor="#f3f3f3"
    foregroundColor="#ecebeb"
  >
    <rect x="0" y="21" rx="0" ry="0" width="216" height="23" />
  </ContentLoader>
);

export default HeadlineLoader;
