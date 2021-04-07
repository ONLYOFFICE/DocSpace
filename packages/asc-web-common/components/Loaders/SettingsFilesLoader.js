import React from "react";
import ContentLoader from "react-content-loader";
import { LoaderStyle } from "../../constants";

const SettingsFilesLoader = () => (
  <div style={{ width: "100%", height: "100%" }}>
    <ContentLoader
      speed={2}
      width={"100%"}
      height={235}
      backgroundColor={LoaderStyle.backgroundColor}
      foregroundColor={LoaderStyle.foregroundColor}
      backgroundOpacity={LoaderStyle.backgroundOpacity}
      foregroundOpacity={LoaderStyle.foregroundOpacity}
    >
      <rect x="0" y="5" rx="6" ry="6" width="24" height="14" />
      <rect x="33" y="5" rx="3" ry="3" width="100%" height="14" />

      <rect x="0" y="39" rx="6" ry="6" width="24" height="14" />
      <rect x="33" y="39" rx="3" ry="3" width="100%" height="14" />

      <rect x="0" y="73" rx="6" ry="6" width="24" height="14" />
      <rect x="33" y="73" rx="3" ry="3" width="100%" height="14" />

      <rect x="0" y="107" rx="6" ry="6" width="24" height="14" />
      <rect x="33" y="107" rx="3" ry="3" width="100%" height="14" />

      <rect x="0" y="141" rx="6" ry="6" width="24" height="14" />
      <rect x="33" y="141" rx="3" ry="3" width="100%" height="14" />
    </ContentLoader>
  </div>
);

export default SettingsFilesLoader;
