import React from "react";
import ContentLoader from "react-content-loader";

const FilterLoader = props => (
  <ContentLoader
    speed={2}
    width={"100%"}
    height={32}
    backgroundColor="#f3f3f3"
    foregroundColor="#ecebeb"
    {...props}
  >
    <rect x="0" y="0" rx="3" ry="3" width="100vw" height="32" />
  </ContentLoader>
);

export default FilterLoader;
