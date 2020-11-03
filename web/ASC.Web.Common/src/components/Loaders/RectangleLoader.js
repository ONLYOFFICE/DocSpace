import React from "react";
import ContentLoader from "react-content-loader";
import { LoaderStyle } from "../../constants/index";
const RectangleLoader = (props) => (
  <ContentLoader
    speed={2}
    width={"100%"}
    height={32}
    style={LoaderStyle}
    {...props}
  >
    <rect x="0" y="0" width={props.width} height={props.height} />
  </ContentLoader>
);

export default RectangleLoader;
