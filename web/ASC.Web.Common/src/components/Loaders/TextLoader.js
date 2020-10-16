import React from "react";
import ContentLoader from "react-content-loader";

const TextLoader = (props) => (
  <ContentLoader
    speed={2}
    width={174}
    height={23}
    viewBox="0 0 174 23"
    backgroundColor="#f3f3f3"
    foregroundColor="#ecebeb"
    {...props}
  >
    <rect x="0" y="0" rx="0" ry="0" width="174" height="23" />
  </ContentLoader>
);

export default TextLoader;
