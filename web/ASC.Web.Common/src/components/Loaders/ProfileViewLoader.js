import React from "react";
import ContentLoader from "react-content-loader";
import { LoaderStyle } from "../../constants/index";

const ProfileViewLoader = () => (
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
      <circle cx="80" cy="80" r="80" />
      <rect x="0" y="176" rx="5" ry="5" width="160" height="36" />

      <rect x="193" y="8" rx="3" ry="3" width="380" height="15" />
      <rect x="193" y="35" rx="3" ry="3" width="380" height="15" />
      <rect x="193" y="62" rx="3" ry="3" width="380" height="15" />
      <rect x="193" y="89" rx="3" ry="3" width="380" height="15" />
    </ContentLoader>
  </div>
);

export default ProfileViewLoader;
