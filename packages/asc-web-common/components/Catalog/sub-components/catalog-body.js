import React from "react";

const CatalogBody = ({ children }) => {
  return <> {children}</>;
};

CatalogBody.displayName = "Body";

export default React.memo(CatalogBody);
