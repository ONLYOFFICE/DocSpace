import React from "react";
import ContentLoader from "react-content-loader";

const RowsLoader = (props) => (
  <div style={{ width: "100%", height: "100%" }}>
    <ContentLoader
      speed={2}
      width={"100%"}
      height={300}
      backgroundColor="#f3f3f3"
      foregroundColor="#ecebeb"
      {...props}
    >
      <rect x="0" y="24" rx="3" ry="3" width="16" height="16" />
      <rect x="26" y="20" rx="5" ry="5" width="24" height="24" />
      <rect x="65" y="15" rx="5" ry="5" width="100vw" height="35" />

      <rect x="0" y="82" rx="3" ry="3" width="16" height="16" />
      <rect x="26" y="78" rx="5" ry="5" width="24" height="24" />
      <rect x="65" y="72" rx="5" ry="5" width="100vw" height="35" />

      <rect x="0" y="140" rx="3" ry="3" width="16" height="16" />
      <rect x="26" y="136" rx="5" ry="5" width="24" height="24" />
      <rect x="65" y="129" rx="5" ry="5" width="100vw" height="35" />

      <rect x="0" y="198" rx="3" ry="3" width="16" height="16" />
      <rect x="26" y="194" rx="5" ry="5" width="24" height="24" />
      <rect x="65" y="186" rx="5" ry="5" width="100vw" height="35" />

      <rect x="0" y="255" rx="3" ry="3" width="16" height="16" />
      <rect x="26" y="252" rx="5" ry="5" width="24" height="24" />
      <rect x="65" y="243" rx="5" ry="5" width="100vw" height="35" />
    </ContentLoader>
  </div>
);

export default RowsLoader;
